using Microsoft.AspNetCore.Authentication.JwtBearer;
using Nur.Tricenter.Security;

namespace GestionClinicaNutricionalService;

/// <summary>
/// Nur-Tricenter platform wiring: JWT validation against tokens minted by nur-tricenter-identity and
/// the <c>/health</c> endpoint Consul probes.
/// </summary>
public static class PlatformExtensions
{
    /// <summary>
    /// Validates bearer tokens and requires one on every endpoint that does not opt out. MS1 has no
    /// permissions in the <c>Nur.Tricenter.Access</c> kernel yet, so the check is "authenticated", not
    /// "has scope X" (the kernel supports this: a service with no permissions still registers the scheme).
    /// </summary>
    /// <remarks>
    /// <c>Jwt:SecretKey</c>/<c>ValidIssuer</c>/<c>ValidAudience</c> must be byte-for-byte the values identity
    /// signs with. A mismatch is not a startup error — it is a 401 on every request.
    /// </remarks>
    public static IServiceCollection AddPlatformSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

        services.AddSecurity(jwtOptions, permissions: []);

        // The kernel's policies read the literal "scope" claim; with the default inbound mapping the
        // handler renames claims to WS-Federation URIs.
        services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme,
            options => options.MapInboundClaims = false);

        services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());

        return services;
    }

    /// <summary>
    /// <c>/health</c> probes Postgres so an instance without its database stops receiving traffic, and
    /// registers in Consul when <c>ServiceRegistration:ServiceDiscoveryAddress</c> is set (platform mode).
    /// </summary>
    public static IServiceCollection AddPlatformHealthAndDiscovery(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;

        services.AddHealthChecks().AddNpgSql(connectionString, name: "postgres");
        services.AddConsulServiceRegistration(configuration);

        return services;
    }
}
