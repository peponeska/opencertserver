using CertesSlim.Acme;

namespace OpenCertServer.CertServer.Tests.StepDefinitions;

using Acme.Abstractions.Model;
using Acme.Abstractions.Services;
using Acme.Server.Services;

internal sealed class TestAcmeHttp01ChallengeValidator : TokenChallengeValidator, IValidateHttp01Challenges
{
    private readonly TestAcmeChallengeValidationState _state;

    public TestAcmeHttp01ChallengeValidator(TestAcmeChallengeValidationState state)
    {
        _state = state;
    }

    public override Task<(bool IsValid, AcmeError? error)> ValidateChallenge(
        Challenge challenge,
        Account account,
        CancellationToken cancellationToken)
    {
        _state.LastValidatedChallengeType = challenge.Type;
        if (_state.CaaRejected)
        {
            return Task.FromResult<(bool, AcmeError?)>((false,
                                                        new AcmeError
                                                        {
                                                            Type = "caa",
                                                            Detail =
                                                                "CAA record does not authorize this CA to issue certificates for the identifier.",
                                                            Identifier = challenge.Authorization.Identifier
                                                        }));
        }

        if (_state.HttpShouldSucceed)
        {
            return Task.FromResult((true, (AcmeError?)null));
        }

        return Task.FromResult<(bool, AcmeError?)>((false,
                                                    new AcmeError
                                                    {
                                                        Type = _state.FailureType,
                                                        Detail = _state.FailureDetail,
                                                        Identifier = challenge.Authorization.Identifier
                                                    }));
    }

    protected override Task<(List<string>? Contents, AcmeError? Error)> LoadChallengeResponse(
        Challenge challenge,
        CancellationToken cancellationToken)
        => throw new NotImplementedException();

    protected override string GetExpectedContent(Challenge challenge, Account account)
        => throw new NotImplementedException();
}
