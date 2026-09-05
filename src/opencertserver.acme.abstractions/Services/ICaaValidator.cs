using CertesSlim.Acme;
using CertesSlim.Acme.Resource;

namespace OpenCertServer.Acme.Abstractions.Services;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Defines a service that validates whether the CA is permitted to issue
/// a certificate for a given identifier based on the CAA records published
/// in DNS, as specified by RFC 8659.
/// </summary>
public interface ICaaValidator
{
    /// <summary>
    /// Determines whether this CA is permitted to issue a certificate for the
    /// identifier, according to the CAA records published in DNS.
    /// </summary>
    /// <param name="identifier">The identifier to validate.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>
    /// An error when the CA is not permitted to issue for the identifier,
    /// or <c>null</c> when issuance is not restricted.
    /// </returns>
    Task<AcmeError?> ValidateAsync(
        Identifier identifier,
        CancellationToken cancellationToken);
}
