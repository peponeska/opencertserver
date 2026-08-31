# OpenCertServer.TSS.Net.Managed

This project vendors and adapts the TSS.Net TPM 2.0 library for OpenCertServer. It provides a managed .NET layer for interacting with TPM 2.0 devices without the Windows-specific BCrypt dependency that the original package has, which makes it usable across Linux, macOS, and Windows builds.

## Functionality
- Wraps TPM 2.0 functionality in a managed .NET assembly
- Provides the underlying TPM-related primitives used by OpenCertServer’s TPM and attestation features
- Avoids the platform-specific Windows-only path in the upstream library by using managed crypto code instead

## Dependencies
- No project references in the current project file
- Uses the vendored TSS.Net source copied from the upstream `TSS.MSR` project
- Includes crypto support from BouncyCastle-style managed code patterns used by the vendored library
- Targets .NET 8/AnyCPU-style compatibility and is intended as a reusable TPM library rather than an executable app

This assembly is a compatibility layer used to keep TPM support portable across development and deployment environments.
