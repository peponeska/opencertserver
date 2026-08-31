# opencertserver.acme.aspnetclient.tests

This test project validates the ASP.NET Core ACME client integration for OpenCertServer. It verifies how the client behaves when registered in dependency injection and when it is exercised via ASP.NET Core test infrastructure.

## Functionality
- Tests ASP.NET Core integration patterns for ACME client consumption
- Verifies dependency injection wiring and client behavior under test-host hosting
- Provides regression coverage for ACME client configuration and service usage

## Dependencies
- `../../src/opencertserver.acme.aspnetclient/opencertserver.acme.aspnetclient.csproj`
- `Microsoft.AspNetCore.TestHost`
- `Microsoft.Extensions.DependencyInjection`
- `NSubstitute`
- `xunit.v3.mtp-v2`

This suite is dedicated to the ASP.NET Core integration layer rather than the lower-level ACME protocol logic.
