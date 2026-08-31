# OpenCertServer.Attestation

This library provides the attestation layer for OpenCertServer. It abstracts the discovery and use of platform-specific hardware security features such as AMD SEV-SNP and Intel SGX so that the certificate authority logic can integrate with trusted execution and hardware-backed attestation flows.

## Functionality
- Loads and selects the appropriate native attestation provider for the current platform
- Exposes the attestation APIs used by the CA and TPM-related integrations
- Coordinates with RID-specific native packages for AMD and SGX support
- Gives the solution a common interface for hardware trust and attestation checks

## Dependencies
- `opencertserver.ca.utils` for shared PKI and crypto helpers
- `OpenCertServer.Sgx.Native` and `OpenCertServer.Amd.Native` for Linux runtime-native libraries
- `Microsoft.Extensions.Configuration`, `Microsoft.Extensions.DependencyInjection`, `Microsoft.Extensions.Http`, and logging abstractions

The project is not a standalone application; it is a supporting library for enabling attestation-aware certificate and key workflows.
