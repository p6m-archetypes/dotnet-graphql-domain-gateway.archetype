{% if use-default-service == false %}
{%- for service_key in services -%}
{% set service = services[service_key] %}
using {{ service['ProjectName']}}.API;
{%- endfor %}
{% endif %}
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit.Abstractions;

namespace {{ ProjectName }}.IntegrationTests;

public class BaseIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient Client;
    {% if use-default-service == false %}
    {%- for service_key in services -%}
    {% set service = services[service_key] %}
    protected readonly Mock<I{{ service['ProjectName']}}> {{ service['ProjectName'] }}Mock;
    {%- endfor %}
    {% endif %}
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly ITestOutputHelper TestOutputHelper;

    public BaseIntegrationTest(ITestOutputHelper testOutputHelper, WebApplicationFactory<Program> factory)
    {
        {% if use-default-service == false %}
        {%- for service_key in services -%}
        {% set service = services[service_key] %}
        {{ service['ProjectName'] }}Mock = new Mock<I{{ service['ProjectName'] }}>();
        {%- endfor %} 
        {% endif %}
        TestOutputHelper = testOutputHelper;
        Factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                
                {% if use-default-service == false %}
                {%- for service_key in services -%}
                {% set service = services[service_key] %}
                // Remove the real gRPC service
                var descriptor{{ service['ProjectName']}} = services.SingleOrDefault(
                    d => d.ServiceType == typeof(I{{ service['ProjectName']}}));
                if (descriptor{{ service['ProjectName']}} != null) services.Remove(descriptor{{ service['ProjectName']}});

                // Add the mocked gRPC service
                services.AddSingleton<I{{ service['ProjectName']}}>({{ service['ProjectName']}}Mock.Object);
                {%- endfor %}
                {% endif %}
            });
        });

        Client = Factory.CreateClient();
    }

    protected async Task<string> ExecuteQuery(object? queryObject)
    {
        var content = new StringContent(JsonConvert.SerializeObject(queryObject), Encoding.UTF8, "application/json");
        var response = await Client.PostAsync("/graphql", content);
        return await response.Content.ReadAsStringAsync();
    }
}