{% import "macros/dotnet" as dotnet %}
using {{ ProjectName }}.Core;
using {{ ProjectName }}.GraphQL.Types;

namespace {{ ProjectName }}.Server.GraphQL.Resolvers;

[GraphQLName("Query")]
public class QueryResolver({{ ProjectName }}Core {{ projectName }})
{
    {%- for service_key in services -%}
    {% set service = services[service_key] %}
    {%- for entity_key in service.model.entities -%}
    {%- set entity = service.model.entities[entity_key] %}
    {{ dotnet.query_methods(entity_key, service.model.entities[entity_key], service.model) }}
    {% endfor %}
    {% endfor %}
}