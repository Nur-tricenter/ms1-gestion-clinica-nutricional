using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using GestionClinicaNutricional.Domain.Repositories;
using GestionClinicaNutricional.Infrastructure;
using GestionClinicaNutricionalService.WebApi.Controllers;
using Joseco.DDD.Core.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace IntegrationTests.Setup
{
    public class WebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly string _dbName = $"IntTests_Item_{Guid.NewGuid()}";

        // The API requires a bearer token on every endpoint; the tests sign their own with a throwaway key.
        private const string TestSigningKey = "integration-tests-signing-key-0123456789abcdef";
        private const string TestIssuer = "ms1-integration-tests";
        private const string TestAudience = "ms1-integration-tests";

        protected override void ConfigureClient(HttpClient client)
        {
            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSigningKey)), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                TestIssuer, TestAudience, [new Claim("sub", "integration-tests")],
                expires: DateTime.UtcNow.AddHours(1), signingCredentials: credentials);

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", new JwtSecurityTokenHandler().WriteToken(token));
            base.ConfigureClient(client);
        }

        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.UseSetting("Jwt:SecretKey", TestSigningKey);
            builder.UseSetting("Jwt:ValidIssuer", TestIssuer);
            builder.UseSetting("Jwt:ValidAudience", TestAudience);
            builder.UseSetting("HttpsRedirection:Enabled", "false");

            builder.ConfigureServices(
                services =>
                {
                    // var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IPacienteRepository));
                    // if (descriptor != null)
                    // {
                    //     services.Remove(descriptor);
                    // }
                    //
                    // var unitDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IUnitOfWork));
                    // if (unitDescriptor != null)
                    // {
                    //     services.Remove(unitDescriptor);
                    // }
                    // services.AddSingleton<IPacienteRepository, InMemoryItemRepository>();
                    // services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();

                    // Calling AddDbContext<PersistenceDbContext> again would ADD to, not replace, the
                    // Npgsql configuration already registered by Program.cs, causing EF Core to see two
                    // providers on the same context. Register an already-built DbContextOptions instance
                    // instead, bypassing that additive pipeline entirely.
                    services.RemoveAll<DbContextOptions<DatabaseContext>>();
                    services.AddSingleton(
                        new DbContextOptionsBuilder<DatabaseContext>()
                            .UseNpgsql(
                                Environment.GetEnvironmentVariable("MS1_TEST_CONNECTION")
                                ?? "Host=localhost;Port=5433;Database=ms1_clinica_nutricional_tests;Username=dev-only;Password=dev-only-password")
                            .Options);
                });
        }

        public async Task InitializeAsync()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            await db.Database.MigrateAsync();
        }

        public new Task DisposeAsync() => Task.CompletedTask;// no hay contenedor que tirar
    }
}