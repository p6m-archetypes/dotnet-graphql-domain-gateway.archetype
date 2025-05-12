# {{ project-title }}

**// TODO:** Add description of your project's business function.

Generated from the [.NET gRPC Service Archetype](https://github.com/p6m-archetypes/dotnet-grpc-service-archetype).

[[_TOC_]]

## Prereqs
Running this service requires .NET 9+ and NuGet to be configured with an Artifactory encrypted key. 
For development, be sure to have Docker installed and running locally.

## Overview

## Build System
This project uses [dotnet](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet#general) as its build system. Common goals include

| Goal    | Description                                        |
|---------|----------------------------------------------------|
| clean   | Clean build outputs.                               |
| build   | Builds a .NET application.                         |
| restore | Restores the dependencies for a given application. |
| run     | Runs the application from source                   |
| test    | Runs tests using a test runner                     |

## Build 
```bash
dotnet build
```

## Run the Server
From the project root, run the server:
```bash
dotnet run --project {{ ProjectName }}.Server
```
This server accepts connections on the following ports:
- {{ service-port }}: used for application gRPC Service traffic
- {{ management-port }}: used to monitor the application over HTTP

### Nitro GraphQL Playground
[Nitro GraphQL Playground](http://localhost:{{ service-port }}/graphql)
Chocolate GraphQL Server provides Nitro as GraphQL Playground to execute query/mutation to server.

## Tests
```bash
dotnet test
```

## Management Server
### Health Checks
You can verify things are up and running by looking at the [/health](http://localhost:{{ management-port }}/health) endpoint:
```bash
curl localhost:{{ management-port }}/health
```

### Metrics
Prometheus - [Prometheus](https://github.com/prometheus-net/prometheus-net)

You can verify things are up and running by looking at the [/metrics](http://localhost:{{ management-port }}/metrics) endpoint:
```bash
curl localhost:{{ management-port }}/metrics
```

## Modules

| Directory                                                                 | Description                                                                                |
|---------------------------------------------------------------------------|--------------------------------------------------------------------------------------------|
| [{{ ProjectName }}.GraphQL]({{ ProjectName }}.GraphQL/README.md)                        | GraphQL schema files.                                                           |
| [{{ ProjectName }}.Core]({{ ProjectName }}.Core/README.md)                            | Business Logic. Abstracts Persistence, defines Transaction Boundaries. Implements the API. |
| [{{ ProjectName }}.IntegrationTests]({{ ProjectName }}.IntegrationTests/README.md)    | Leverages the Client to test the Server and it's dependencies.                             |
| [{{ ProjectName }}.Server]({{ ProjectName }}.Server/README.md)                        | Transport/Protocol Host.  Wraps Core.                                                      |

## Contributions
**// TODO:** Add description of how you would like issues to be reported and people to reach out.