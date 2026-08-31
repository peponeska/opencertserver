# opencertserver.cli.tests

This project exercises the OpenCertServer command-line tooling through end-to-end BDD-style scenarios. It validates the CLI behavior using Reqnroll and xUnit, with PEM fixtures representing CA keys, certificates, and CSR inputs.

## Functionality
- Runs CLI feature tests for certificate creation, inspection, and EST flows
- Uses prepared fixtures such as `ca.key`, `ca.crt`, `test.crt`, and `test.csr`
- Verifies real command execution and expected outputs from the `opencert` application

## Dependencies
- `../../src/opencertserver.cli/opencertserver.cli.csproj`
- `../../src/opencertserver.ca/opencertserver.ca.csproj`
- `../../src/opencertserver.ca.server/opencertserver.ca.server.csproj`
- `../../src/opencertserver.ca.utils/opencertserver.ca.utils.csproj`
- `../../src/opencertserver.est.server/opencertserver.est.server.csproj`
- `Microsoft.AspNetCore.TestHost`, `Reqnroll.xUnit.v3`, and `xunit.v3.mtp-v2`

This suite is the primary verification layer for the CLI’s certificate workflows.
