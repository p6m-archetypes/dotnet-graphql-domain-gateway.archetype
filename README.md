# .NET GraphQL Domain Gateway Archetype

## Usage

To get started, [install archetect](https://github.com/p6m-dev/development-handbook)
and render this template to your current working directory:

```sh
archetect render git@github.com:p6m-dev/dotnet-graphql-domain-gateway.archetype.git
```

For information about interacting with the domain gateway, refer to the README at the generated
project's root.

## Prompts

When rendering the archetype, you'll be prompted for the following values:

| Property          | Description                                                                                                         | Example                       |
| ----------------- | ------------------------------------------------------------------------------------------------------------------- | ----------------------------- |
| `org-name`        | Organization Name                                                                                                   | afi, cpd, a1p                 |
| `solution-name`   | Solution Name                                                                                                       | apps, xyz                     |
| `prefix`          | General name that represents the service domain that is used to set the entity, service, and RPC stub names         | invoice, order, booking       |

## What's Inside

Features include:
- Simple CRUD over GraphQL [HotChocolate Server](https://chillicream.com/docs/hotchocolate/v13)
- Authentication and Authorization setup
- Integration with gRPC service 
- Docker image publication to Artifactory
- Integration tests
- GitHub Actions SDLC pipelines
- Kubernetes manifests
- Open Telemetry Configuration
- Serilog
- Heath Checks
