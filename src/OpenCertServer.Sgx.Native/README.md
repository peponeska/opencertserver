# OpenCertServer.Sgx.Native

This project packages the Intel SGX DCAP library as a native NuGet asset for OpenCertServer attestation and enclave-related scenarios. The package contains no managed code; it is designed to deliver the correct Linux runtime library to consumers automatically.

## Functionality
- Packages `libsgx_dcap_ql.so` for Linux x64 and ARM64 runtimes
- Keeps native binaries under the .NET `runtimes/{rid}/native` layout
- Lets downstream projects consume the right SGX library without manual file copying

## Dependencies
- No managed library dependencies
- Consumed by `opencertserver.attestation` through a non-managed project reference
- Relies on Intel DCAP binaries obtained from the Intel SGX ecosystem and placed in `runtimes/.../native/`
- Targets `.NET 10` and is intended for packaging and deployment support rather than direct execution
