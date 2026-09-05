namespace OpenCertServer.CertServer.Tests.StepDefinitions;

internal sealed class TestAcmeChallengeValidationState
{
    public bool HttpShouldSucceed { get; set; } = true;

    public bool DnsShouldSucceed { get; set; } = true;

    public string FailureType { get; set; } = "incorrectResponse";

    public string FailureDetail { get; set; } = "Simulated challenge validation failure.";

    public string? LastValidatedChallengeType { get; set; }

    /// <summary>
    /// When true, the simulated challenge validators reject validation because the
    /// CAA lookup denies issuance for the identifier (RFC 8659).
    /// </summary>
    public bool CaaRejected { get; set; }

    public void Reset()
    {
        HttpShouldSucceed = true;
        DnsShouldSucceed = true;
        FailureType = "incorrectResponse";
        FailureDetail = "Simulated challenge validation failure.";
        LastValidatedChallengeType = null;
        CaaRejected = false;
    }
}

