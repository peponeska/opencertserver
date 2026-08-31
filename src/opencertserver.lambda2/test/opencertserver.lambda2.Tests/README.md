# opencertserver.lambda2.Tests

This test project validates the AWS Lambda host for OpenCertServer. It exercises the server in a Lambda-friendly environment and checks that request handling, bootstrapping, and integration with AWS event types behave correctly.

## Functionality
- Verifies the Lambda entry point and request processing paths
- Uses AWS Lambda test utilities to emulate API Gateway and Lambda inputs
- Confirms the OpenCertServer ASP.NET Core application works correctly when hosted in AWS Lambda

## Dependencies
- `opencertserver.lambda2` project under test
- `Amazon.Lambda.Core`, `Amazon.Lambda.TestUtilities`, and `Amazon.Lambda.APIGatewayEvents`
- `AWSSDK.Extensions.NETCore.Setup` for AWS configuration integration
- `xUnit` and the .NET test SDK for the test harness

This project is focused on verifying the serverless hosting layer rather than the certificate logic itself.
