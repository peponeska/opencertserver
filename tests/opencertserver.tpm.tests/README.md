# opencertserver.tpm.tests

This project validates the TPM-backed key-storage and certificate integration used by OpenCertServer. It covers the components that provision and manage TPM-backed private keys while also exercising the required containerized TPM simulator setup.

## Functionality
- Verifies TPM-backed certificate and key workflows
- Starts the IBM TPM2 simulator container used by the tests
- Confirms integrations with the CA and certificate utilities remain correct

## Dependencies
- `../../src/opencertserver.tpm/opencertserver.tpm.csproj`
- `../../src/opencertserver.ca/opencertserver.ca.csproj`
- `../../src/opencertserver.ca.utils/opencertserver.ca.utils.csproj`
- `Reqnroll.xUnit.v3`, `xunit.v3.mtp-v2`, and `Testcontainers`

This project is focused on hardware-backed key management and its compatibility with the core CA logic.
