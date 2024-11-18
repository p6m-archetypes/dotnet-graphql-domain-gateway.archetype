{% import "macros/dotnet" as dotnet %}
using Microsoft.Extensions.DependencyInjection;
using {{ ProjectName }}.Server;
{%- for service_key in services -%}
{% set service = services[service_key] %}
using {{ service['ProjectName'] }}.API;
{%- endfor %}

namespace {{ ProjectName }}.IntegrationTests;

public class GprcServiceMockStartup({{ dotnet.core_gateway_constructor_args() }}): IAppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        {%- for service_key in services -%}
        {% set service = services[service_key] %}
        services.AddSingleton({{ service['ProjectName'] | camel_case}});
        {%- endfor %}
    }
}