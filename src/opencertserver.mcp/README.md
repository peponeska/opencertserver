# OpenCertServer MCP

This project hosts the OpenCertServer Model Context Protocol (MCP) server. It exposes the certificate and CA capabilities of the platform to MCP clients over stdio, making it possible to query, issue, and revoke certificates through a tool-based interface.

## Functionality
- Starts a stdio-based MCP server using the official `ModelContextProtocol` .NET SDK
- Discovers and registers tools with `[McpServerToolType]` and `[McpServerTool]`
- Exposes certificate-management operations backed by the CA and CA-server layers
- Provides a single protocol surface for MCP-driven certificate administration workflows

## Dependencies
- `ModelContextProtocol` package for the MCP server runtime
- `opencertserver.ca` for certificate authority logic
- `opencertserver.ca.utils` for shared PKI helpers
- `opencertserver.ca.server` for the server-side CA implementation

This project is an integration layer that exposes existing OpenCertServer functionality through the MCP protocol rather than implementing certificate logic on its own.
