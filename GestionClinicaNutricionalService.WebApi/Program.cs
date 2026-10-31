using GestionClinicaNutricional.Infrastructure;
using GestionClinicaNutricionalService;
using Microsoft.EntityFrameworkCore;

// `--migrate` is a one-shot job (platform compose runs it before the API starts): apply the schema and
// exit. It must be stripped from args because the command-line configuration provider rejects a bare switch.
var migrateOnly = args.Contains("--migrate");
args = args.Where(a => a != "--migrate").ToArray();

var builder = WebApplication.CreateBuilder(args);

// The Application assembly also holds the outbox handlers (PublishIntegrationEventWhenPacienteCreated),
// which need IExternalPublisher. Only the WorkerService registers it and dispatches those messages;
// the API just writes to the outbox. Without this, Development's build-time validation rejects the API.
builder.Host.UseDefaultServiceProvider(options => options.ValidateOnBuild = false);

builder.Services.AddControllers();//defines the services that are required by the MVC framework

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPlatformSecurity(builder.Configuration);
builder.Services.AddPlatformHealthAndDiscovery(builder.Configuration);

builder.Services.AddOpenApiDocument(config =>
{
    config.SchemaSettings.FlattenInheritanceHierarchy = true;
});

var app = builder.Build();

if (migrateOnly)
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<DatabaseContext>().Database.MigrateAsync();
    return;
}

// Swagger stays on unless a deployment turns it off (the platform compose does).
if (app.Configuration.GetValue("OpenApi:Enabled", true))
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

if (app.Configuration.GetValue("HttpsRedirection:Enabled", true))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health").AllowAnonymous();
app.MapControllers();// defines routes that will allow controllers to handle requests

app.Run();//start listening http request to asp.net core server

