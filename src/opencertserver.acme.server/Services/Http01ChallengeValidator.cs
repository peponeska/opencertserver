namespace OpenCertServer.Acme.Server.Services;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Model;
using Abstractions.Services;
using CertesSlim.Acme;
using Microsoft.IdentityModel.Tokens;

public sealed class ValidateHttp01Challenges : TokenChallengeValidator, IValidateHttp01Challenges
{
    private readonly List<IPNetwork> _prohibitedRanges =
    [
        IPNetwork.Parse("127.0.0.0/8"),
        IPNetwork.Parse("::1/128"),
        IPNetwork.Parse("169.254.0.0/16"),
        IPNetwork.Parse("fe80::/10"),
        IPNetwork.Parse("10.0.0.0/8"),
        IPNetwork.Parse("172.16.0.0/12"),
        IPNetwork.Parse("192.168.0.0/16"),
        IPNetwork.Parse("169.254.169.254/32") // AWS metadata
    ];

    private readonly HttpClient _httpClient;
    private readonly ICaaValidator _caaValidator;

    public ValidateHttp01Challenges(HttpClient httpClient, ICaaValidator caaValidator)
    {
        _httpClient = httpClient;
        _caaValidator = caaValidator;
    }

    protected override string GetExpectedContent(Challenge challenge, Account account)
    {
        var thumbprintBytes = account.Jwk.ComputeJwkThumbprint();
        var thumbprint = Base64UrlEncoder.Encode(thumbprintBytes);

        var expectedContent = $"{challenge.Token}.{thumbprint}";
        return expectedContent;
    }

    protected override async Task<(List<string>? Contents, AcmeError? Error)> LoadChallengeResponse(
        Challenge challenge,
        CancellationToken cancellationToken)
    {
        var caaError = await _caaValidator
            .ValidateAsync(challenge.Authorization.Identifier, cancellationToken).ConfigureAwait(false);
        if (caaError != null)
        {
            return (null, caaError);
        }

        var resolvedIps = await Task.WhenAll(
                Dns.GetHostAddressesAsync(challenge.Authorization.Identifier.Value, cancellationToken))
            .ConfigureAwait(false);
        if (resolvedIps.SelectMany(i => i).Any(ip => _prohibitedRanges.Any(r => r.Contains(ip))))
        {
            return (
                null,
                new AcmeError
                {
                    Type = "rejectedIdentifier",
                    Detail = "Challenge target resolves to prohibited address range.",
                    Identifier = challenge.Authorization.Identifier
                });
        }

        var challengeUrl =
            $"http://{challenge.Authorization.Identifier.Value}/.well-known/acme-challenge/{challenge.Token}";

        try
        {
            var response = await _httpClient.GetAsync(new Uri(challengeUrl), cancellationToken)
                .ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var error = new AcmeError
                {
                    Type = "incorrectResponse",
                    Detail = $"Got non 200 status code: {response.StatusCode}",
                    Identifier = challenge.Authorization.Identifier
                };
                return (null, error);
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return ([content], null);
        }
        catch (HttpRequestException ex)
        {
            var error = new AcmeError
            {
                Type = "connection",
                Detail = ex.Message,
                Identifier = challenge.Authorization.Identifier
            };
            return (null, error);
        }
    }
}
