using System.Text.Json;
using {{ ProjectName }}.Core;
using {{ ProjectName }}.GraphQL.Types;
using {{ ProjectName }}.Server.GraphQL;
using {{ ProjectName }}.Server.GraphQL.Resolvers;
{% if use-default-service == false %}
{%- for service_key in services -%}
{% set service = services[service_key] %}
using {{ service['ProjectName'] }}.API;
using {{ service['ProjectName'] }}.Client;
{% endfor %}
{% endif %}
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Path = System.IO.Path;

var builder = WebApplication.CreateBuilder(args);

//Serilog configuration
builder.Host.UseSerilog((content, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(builder.Configuration)
);

builder.Logging.AddOpenTelemetry(logging => logging.AddOtlpExporter());

builder.Services
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

{% if use-default-service == false %}
{%- for service_key in services -%}
{% set service = services[service_key] %}
builder.Services.AddSingleton<I{{ service['ProjectName'] }}>({{ service['ProjectName'] }}Client.Of(builder.Configuration["CoreServices:{{ service['ProjectName'] }}:Url"]));
{% endfor %}
{% endif %}
builder.Services.AddSingleton<{{ ProjectName }}Core>();

builder.Services.AddHealthChecks();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("{{ project-name}}"))
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
        metrics.AddOtlpExporter();
        metrics.AddPrometheusExporter();
    })
    .WithTracing(tracing =>
    {
        tracing
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddOtlpExporter();
    })
    ;

var app = builder.Build();

app.MapGraphQL();
app.MapGet("/", () => "{{ ProjectName }}");
app.MapPrometheusScrapingEndpoint("/metrics");
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

app.Run();

public partial class Program { }