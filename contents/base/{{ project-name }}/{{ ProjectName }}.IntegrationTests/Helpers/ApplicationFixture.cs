using Moq;
using {{ ProjectName }}.Server;
{%- for service_key in services -%}
{% set service = services[service_key] %}
using {{ service['ProjectName']}}.API;
{%- endfor %}


namespace {{ ProjectName }}.IntegrationTests;

public class ApplicationFixture: IDisposable
{
    private readonly {{ ProjectName }}Server _server;
    private readonly HttpClient _client;

    {%- for service_key in services -%}
    {% set service = services[service_key] %}
    private readonly Mock<I{{ service['ProjectName']}}> _{{ service['ProjectName'] | camel_case}}Mock;
    {%- endfor %}
    
    public ApplicationFixture()
    {
        {%- for service_key in services -%}
        {% set service = services[service_key] %}
        _{{ service['ProjectName'] | camel_case}}Mock = new Mock<I{{ service['ProjectName']}}>();
        {%- endfor %}    
        
        var gprcServiceMockStartup = new GprcServiceMockStartup(
            {%- for service_key in services -%}
            {% set service = services[service_key] %}
            _{{ service['ProjectName'] | camel_case}}Mock.Object
            {%- endfor %}
            );
        _server = new {{ ProjectName }}Server()
            .WithRandomPorts()
            .WithStartup(gprcServiceMockStartup)
            .Start();
        
        _client = new HttpClient{
            BaseAddress = new Uri(_server.GetGraphQlUrl() ?? "http://localhost:8080")
        };
    }


    public {{ ProjectName }}Server GetServer() => _server;
    public HttpClient GetClient() => _client;

    {%- for service_key in services -%}
    {% set service = services[service_key] %}
    public Mock<I{{ service['ProjectName']}}> GetMock{{ service['ProjectName']}}() => _{{ service['ProjectName'] | camel_case}}Mock;
    {%- endfor %}           
    

    public void Dispose()
    {
        _server.Stop();
        GC.SuppressFinalize(this);
    }
}

[CollectionDefinition("ApplicationCollection")]
public class ApplicationCollection : ICollectionFixture<ApplicationFixture>
{
    // This class has no code; it's just a marker for the test collection
}