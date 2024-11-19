{% import "macros/dotnet" as dotnet %}
using System.Text;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using {{ ProjectName }}.GraphQL.Types;
{%- for service_key in services -%}
{% set service = services[service_key] %}
using {{ service['ProjectName']}}.API;
{%- endfor %}
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;

namespace {{ ProjectName }}.IntegrationTests;

public class {{ ProjectName }}Test : BaseIntegrationTest
{
    public {{ ProjectName }}Test(ITestOutputHelper testOutputHelper, WebApplicationFactory<Program> factory) : base(testOutputHelper, factory)
    {
    }
    private readonly string _id = Guid.NewGuid().ToString();
    private readonly string _name = "name_" + Guid.NewGuid();

    {%- for service_key in services -%}
    {% set service = services[service_key] %}
    {%- for entity_key in service.model.entities -%}
    {%- set entity = service.model.entities[entity_key] %}
    {{ dotnet.integration_test_methods(entity_key, service.model.entities[entity_key], service.model) }}
    {% endfor %}
    {% endfor %}
}