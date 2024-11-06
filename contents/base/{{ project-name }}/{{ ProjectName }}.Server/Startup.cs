using System.Text.Json;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using {{ ProjectName }}.Core;

using {{ ProjectName }}.GraphQL.Types;
using {{ ProjectName }}.Server.GraphQL;
using {{ ProjectName }}.Server.GraphQL.Resolvers;
{%- for service_key in services -%}
{% set service = services[service_key] %}
using {{ service['ProjectName'] }}.Client;
{%- endfor %}
using Path = System.IO.Path;


namespace {{ ProjectName }}.Server;

public class Startup
{
    public Startup(IConfigurationRoot configuration)
    {
        Configuration = configuration;
    }
    public IConfigurationRoot Configuration { get; }
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddGraphQLServer()
            .AddApolloFederation(FederationVersion.Federation22)
            .AddDocumentFromFile(Path.Combine(AppContext.BaseDirectory, "{{ project-name }}.graphqls"))
            {%- for service_key in services -%}
            {% set service = services[service_key] %}
            {%- for entity_key in service.model.entities -%}
            {%- set entity = service.model.entities[entity_key] %}
            .BindRuntimeType<{{ entity_key | pascal_case }}>()
            .AddResolver<QueryResolver>()
            .AddResolver<MutationResolver>()
            {%- endfor %}
            {%- endfor %}
            
            .AddErrorFilter<GraphQlErrorFilter>()
            ;

        {%- for service_key in services -%}
        {% set service = services[service_key] %}
        services.AddSingleton({{ service['ProjectName'] }}Client.Of(Configuration["CoreServices:{{ service['ProjectName'] }}:Url"]));
        {% endfor %}
        services.AddSingleton<{{ ProjectName }}Core>();

        services.AddHealthChecks();
        
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("{{ project-name}}"))
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
                metrics.AddOtlpExporter();
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter();
            })
            ;
    }
        
        
    public void Configure(WebApplication app)
    {
        app.MapGet("/", () => "{{ ProjectName }}");
        app.MapGraphQL();

        app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description ?? "No description",
                        duration = e.Value.Duration.ToString()
                    })
                });
                await context.Response.WriteAsync(result);
            }
        });
    }
}