# OpenCertServer.Amd.Native

This project packages the AMD SEV-SNP user-space driver as a native NuGet asset for OpenCertServer attestation scenarios. It does not ship managed .NET code; instead it distributes the platform-specific shared library under the standard .NET runtime layout.

## Functionality
- Packages `amd_snp_driver.so` for Linux x64 and ARM64 runtimes
- Exposes the native library to .NET consumers via RID-based NuGet conventions
- Supports attestation workflows that need access to AMD SEV-SNP hardware features

## Dependencies
- No managed library dependencies
- Consumed by `opencertserver.attestation` through a `ProjectReference` with `ReferenceOutputAssembly="false"`
- Relies on native binaries supplied by AMD and copied into `runtimes/{rid}/native/`
- Targets `.NET 10` and is intended to be used as a packaging project, not as an application entry point
