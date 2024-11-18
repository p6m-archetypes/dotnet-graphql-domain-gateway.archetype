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
using Xunit.Abstractions;

namespace {{ ProjectName }}.IntegrationTests;

[Collection("ApplicationCollection")]
public class {{ ProjectName }}IT(ITestOutputHelper testOutputHelper, ApplicationFixture applicationFixture)
{
    private readonly HttpClient _httpClient = applicationFixture.GetClient();
    
    {%- for service_key in services -%}
    {% set service = services[service_key] %}
    private readonly Mock<I{{ service['ProjectName']}}> _{{ service['ProjectName'] | camel_case}}Mock = applicationFixture.GetMock{{ service['ProjectName']}}();
    {%- endfor %}

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