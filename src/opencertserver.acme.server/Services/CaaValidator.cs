namespace OpenCertServer.Acme.Server.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Services;
using CertesSlim.Acme;
using CertesSlim.Acme.Resource;
using Configuration;
using DnsClient;
using DnsClient.Protocol;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Validates whether this CA is permitted to issue a certificate for a given
/// identifier based on the CAA records published in DNS, as specified by RFC 8659.
/// </summary>
/// <remarks>
/// The processing follows RFC 8659 "Relevant Resource Record Set":
/// <list type="bullet">
/// <item>The Relevant RRset is located by climbing the DNS name tree from the
/// identifier up to (but not including) the DNS root until a CAA RRset is found.</item>
/// <item>A critical Property with an unknown or unsupported tag forbids issuance.</item>
/// <item>If the Relevant RRset contains no <c>issue</c> or <c>issuewild</c> tags,
/// CAA does not restrict issuance.</item>
/// <item><c>issue</c> restricts issuance to the listed issuer domain names;
/// an empty issuer domain name forbids issuance.</item>
/// <item><c>issuewild</c> applies only to Wildcard Domain Names and takes precedence
/// over <c>issue</c> when present.</item>
/// </list>
/// </remarks>
public sealed partial class CaaValidator : ICaaValidator
{
    private const string IssueTag = "issue";
    private const string IssueWildTag = "issuewild";

    private const int IssuerCriticalFlag = 128;

    private readonly ILogger<CaaValidator> _logger;
    private readonly ILookupClient _client;
    private readonly IOptions<AcmeServerOptions> _options;

    public CaaValidator(
        ILogger<CaaValidator> logger,
        ILookupClient client,
        IOptions<AcmeServerOptions> options)
    {
        _logger = logger;
        _client = client;
        _options = options;
    }

    /// <inheritdoc />
    public async Task<AcmeError?> ValidateAsync(
        Identifier identifier,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(identifier);

        var caaIdentities = _options.Value.CAAIdentities;
        if (caaIdentities == null || caaIdentities.Length == 0)
        {
            return null;
        }

        var fqdn = identifier.Value.Replace("*.", "", StringComparison.OrdinalIgnoreCase);
        var isWildcard = identifier.IsWildcard;
        IReadOnlyList<CaaRecord>? relevantSet;
        try
        {
            relevantSet = await LoadRelevantRecordSetAsync(fqdn, cancellationToken).ConfigureAwait(false);
        }
        catch (DnsResponseException ex)
        {
            LogCaaLookupFailed(fqdn, ex.Message);
            return new AcmeError { Type = "caa", Detail = $"Could not read CAA records from DNS: {ex.Message}" };
        }

        if (relevantSet == null || relevantSet.Count == 0)
        {
            return null;
        }

        if (HasUnsupportedCriticalTag(relevantSet))
        {
            return new AcmeError
            {
                Type = "caa",
                Detail = "CAA record contains a critical property with an unsupported tag."
            };
        }

        var applicable = GetApplicableProperties(relevantSet, isWildcard);
        if (applicable.Count == 0)
        {
            return null;
        }

        if (IsAuthorized(applicable))
        {
            return null;
        }

        return new AcmeError
        {
            Type = "caa",
            Detail = "CAA record does not authorize this CA to issue certificates for the identifier."
        };
    }

    /// <summary>
    /// Locates the Relevant RRset by climbing the DNS name tree from the specified
    /// FQDN until a CAA RRset is found, or until the DNS root is reached.
    /// </summary>
    private async Task<IReadOnlyList<CaaRecord>?> LoadRelevantRecordSetAsync(
        string fqdn,
        CancellationToken cancellationToken)
    {
        var domain = fqdn.TrimEnd('.');
        while (domain.Length > 0)
        {
            LogQueryingCaa(domain);
            var response = await _client
                .QueryAsync(domain, QueryType.CAA, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var records = response.Answers.OfType<CaaRecord>().ToList();
            if (records.Count > 0)
            {
                return records;
            }

            var separator = domain.IndexOf('.');
            if (separator < 0)
            {
                break;
            }

            domain = domain[(separator + 1)..];
        }

        return null;
    }

    /// <summary>
    /// Determines whether the Relevant RRset contains a critical Property for an
    /// unknown or unsupported tag, in which case per RFC 8659 issuance must not happen.
    /// </summary>
    private static bool HasUnsupportedCriticalTag(IEnumerable<CaaRecord> relevantSet)
    {
        foreach (var record in relevantSet)
        {
            if ((record.Flags & IssuerCriticalFlag) != IssuerCriticalFlag)
            {
                continue;
            }

            var tag = record.Tag;
            if (!string.Equals(tag, IssueTag, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(tag, IssueWildTag, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Selects the properties that apply to the request, honoring the precedence
    /// rules between <c>issue</c> and <c>issuewild</c>.
    /// </summary>
    private static List<CaaRecord> GetApplicableProperties(
        IEnumerable<CaaRecord> relevantSet,
        bool isWildcard)
    {
        var hasIssueWild = relevantSet.Any(
            r => string.Equals(r.Tag, IssueWildTag, StringComparison.OrdinalIgnoreCase));

        if (isWildcard && hasIssueWild)
        {
            return relevantSet
                .Where(r => string.Equals(r.Tag, IssueWildTag, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return relevantSet
            .Where(r => string.Equals(r.Tag, IssueTag, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Determines whether any applicable property authorizes this CA to issue,
    /// based on the configured <see cref="AcmeServerOptions.CAAIdentities"/>.
    /// </summary>
    private bool IsAuthorized(IEnumerable<CaaRecord> applicable)
    {
        var identities = _options.Value.CAAIdentities ?? [];
        var normalizedIdentities = identities
            .Where(i => !string.IsNullOrWhiteSpace(i))
            .Select(i => NormalizeDomain(i!))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var record in applicable)
        {
            var issuerDomainName = ExtractIssuerDomainName(record.Value);
            if (issuerDomainName.Length == 0)
            {
                continue;
            }

            if (normalizedIdentities.Contains(issuerDomainName))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Extracts the issuer domain name from an <c>issue</c>/<c>issuewild</c> value,
    /// discarding any parameters that follow the first semicolon. An empty or
    /// malformed value yields an empty domain name, which forbids issuance.
    /// </summary>
    private static string ExtractIssuerDomainName(string value)
    {
        var issuer = value;
        var separator = issuer.IndexOf(';');
        if (separator >= 0)
        {
            issuer = issuer[..separator];
        }

        return NormalizeDomain(issuer.Trim());
    }

    private static string NormalizeDomain(string domain)
        => domain.Trim().TrimEnd('.').ToLowerInvariant();

    [LoggerMessage(LogLevel.Debug, "Querying CAA records for {domain}")]
    partial void LogQueryingCaa(string domain);

    [LoggerMessage(LogLevel.Warning, "CAA lookup failed for {domain}: {message}")]
    partial void LogCaaLookupFailed(string domain, string message);
}
