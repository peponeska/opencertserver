# CertesSlim.Tests

This test project verifies the behavior of the lightweight `CertesSlim` ACME client library. It covers the protocol primitives used to interact with ACME servers and validates certificate-related flows against the library’s public API.

## Functionality
- Executes xUnit tests for ACME client behavior
- Validates certificate, account, and challenge workflow logic
- Uses a data folder containing fixtures needed by the library tests

## Dependencies
- `../../src/CertesSlim/CertesSlim.csproj` as the library under test
- `xunit.v3.mtp-v2` for the test runner
- `NSubstitute` for test doubles and dependency isolation

This project is the direct regression suite for the ACME client implementation.
