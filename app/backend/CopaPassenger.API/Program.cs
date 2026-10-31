using CopaPassenger.API.Middleware;
using CopaPassenger.Domain.Interfaces;
using CopaPassenger.Infrastructure.Persistence;
using CopaPassenger.Infrastructure.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Application Insights: el paquete 3.x falla al arrancar si no hay cadena, por eso
// solo se registra cuando existe. Acepta la clave propia o la variable estandar de Azure.
var aiConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
if (string.IsNullOrWhiteSpace(aiConnectionString))
    aiConnectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");

if (!string.IsNullOrWhiteSpace(aiConnectionString))
    builder.Services.AddApplicationInsightsTelemetry(opts => opts.ConnectionString = aiConnectionString);

// Serilog. writeToProviders permite que los logs lleguen tambien al proveedor
// de Application Insights, sin depender de un sink propio.
builder.Host.UseSerilog(
    (ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration)
                   .Enrich.FromLogContext()
                   .WriteTo.Console(),
    writeToProviders: true);

// CORS
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:4200"];
builder.Services.AddCors(options =>
{
    options.AddPolicy("CopaPolicy", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());

    // Politica exclusiva para el probe de conectividad: siempre permite cualquier origen
    options.AddPolicy("ProbePolicy", policy =>
        policy.AllowAnyOrigin().WithMethods("GET").AllowAnyHeader());
});

// EF Core InMemory
builder.Services.AddDbContext<CopaDbContext>(opts =>
    opts.UseInMemoryDatabase("CopaDb"));

// Repositorios
builder.Services.AddScoped<IFlightRepository, FlightRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<CopaPassenger.Application.Queries.GetFlightByNumberQuery>());

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CopaPassenger.Application.Validators.GetFlightByNumberQueryValidator>();


// Health checks
builder.Services.AddHealthChecks()
    .AddCheck<DeepHealthCheck>("deep");
builder.Services.AddScoped<DeepHealthCheck>();

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
        opts.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Copa Passenger API",
        Version = "v1",
        Description = "Servicios de estado de vuelo y gestion de reservas"
    });

    options.DocumentFilter<CopaPassenger.API.Swagger.HealthChecksDocumentFilter>();
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Copa Passenger API v1");
    options.DocumentTitle = "Copa Passenger API";
});

// Middleware pipeline
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseSerilogRequestLogging();

app.UseRouting();
app.UseCors("CopaPolicy");

app.UseAuthorization();
app.MapControllers();

// Probe de conectividad: CORS abierto intencionalmente
app.MapGet("/api/diagnostics/probe", () => Results.Ok(new { probe = true }))
    .RequireCors("ProbePolicy")
    .ExcludeFromDescription();

// Health endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = async (ctx, _) =>
    {
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new { status = "Healthy" }));
    }
});

app.MapHealthChecks("/health/deep", new HealthCheckOptions
{
    Predicate = hc => hc.Name == "deep",
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    },
    ResponseWriter = async (ctx, report) =>
    {
        ctx.Response.ContentType = "application/json";
        var entry = report.Entries.FirstOrDefault().Value;
        object? Value(string key) => entry.Data.TryGetValue(key, out var v) ? v : null;

        await ctx.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            mode = Value("mode") ?? "unknown",
            error = Value("errorCode") is { } code ? $"Error {code}" : null,
            errorCode = Value("errorCode"),
            detail = Value("reason"),
            checks = report.Entries.Select(e => new { name = e.Key, status = e.Value.Status.ToString() })
        }));
    }
});

// Sembrar datos iniciales
app.Services.SeedDatabase();

app.Run();

public partial class Program { }
