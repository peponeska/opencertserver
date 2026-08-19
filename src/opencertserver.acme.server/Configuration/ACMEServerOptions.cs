namespace OpenCertServer.Acme.Server.Configuration;

public sealed class AcmeServerOptions
{
    public BackgroundServiceOptions HostedWorkers { get; set; } = new();

    public string? WebsiteUrl { get; set; }

    public bool ExternalAccountRequired { get; set; }

    public TOSOptions TOS { get; set; } = new();

    /// <summary>
    /// Gets or sets the CAA identities that are served in the ACME directory.
    /// </summary>
    public string[]? CAAIdentities { get; set; }
}
