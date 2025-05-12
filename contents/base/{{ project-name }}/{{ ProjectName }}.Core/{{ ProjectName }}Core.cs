{% import "macros/dotnet" as dotnet %}
using {{ ProjectName }}.GraphQL.Types;
{% if use-default-service == false %}
{%- for service_key in services -%}
{% set service = services[service_key] %}
using {{ service['ProjectName'] }}.API;
{%- endfor %}
{% endif %}
using Serilog;

namespace {{ ProjectName }}.Core;

public class {{ ProjectName }}Core({{ dotnet.core_gateway_constructor_args() }})
{

    {%- for service_key in services -%}
    {% set service = services[service_key] %}
    {%- for entity_key in service.model.entities -%}
    {%- set entity = service.model.entities[entity_key] %}
    {% if use-default-service == false %}
    {{ dotnet.core_implementation_methods(entity_key, service.model.entities[entity_key], service.model) }}
    {% else %}
    {{ dotnet.core_implementation_methods_defaults(entity_key, service.model.entities[entity_key], service.model) }}
    {% endif %}
    {% endfor %}
    {% endfor %}

}
