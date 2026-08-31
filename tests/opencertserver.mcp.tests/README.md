# opencertserver.mcp.tests

This project validates the OpenCertServer Model Context Protocol server. It uses Reqnroll and xUnit to verify that the MCP tool surface and server behavior remain consistent as the certificate-management capabilities evolve.

## Functionality
- Tests Model Context Protocol server registration and tool execution
- Verifies certificate operations exposed through the MCP interface
- Exercises core CA and CA-server integration points used by the MCP surface

## Dependencies
- `../../src/opencertserver.mcp/opencertserver.mcp.csproj`
- `../../src/opencertserver.ca/opencertserver.ca.csproj`
- `../../src/opencertserver.ca.utils/opencertserver.ca.utils.csproj`
- `../../src/opencertserver.ca.server/opencertserver.ca.server.csproj`
- `Reqnroll.xUnit.v3` and `xunit.v3.mtp-v2`

This project ensures the MCP server remains aligned with the underlying certificate authority components.
