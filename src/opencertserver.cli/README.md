# opencertserver.cli

This project contains the `opencert` command-line tool for OpenCertServer. It exposes the certificate operations most commonly used during development and administration, including generating CSRs, printing certificate details, signing requests, and enrolling with EST endpoints.

## Functionality
- Provides the console entry point named `opencert`
- Supports certificate inspection and formatting via CA utilities
- Creates and signs CSRs for certificate issuance workflows
- Enrolls and re-enrolls certificates through the EST client

## Dependencies
- `System.CommandLine` for the CLI command model and parsing
- `opencertserver.ca.utils` for X.509 / PKI helper logic
- `opencertserver.ca` for the core CA logic
- `opencertserver.est.client` for secure enrollment flows

This project acts as the public-facing command-line surface over the lower-level CA and EST libraries.
