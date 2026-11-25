using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace IntelliWell.Hosting;

/// <summary>
/// Autentifikácia JWT bearer a autorizácia podľa roly (ADR-004, ADR-007, BR-011, NFR4).
/// Konfigurácia issuer/audience/podpisových kľúčov sa načíta zo sekcie Authentication:Schemes:Bearer,
/// ktorú pre prototyp napĺňa dotnet user-jwts (DR-23).
/// </summary>
public static class IntelliWellAuthentication
{
    /// <summary>Policy pre vytvorenie rezervácie: iba ROLE-SpravcaNavstev.</summary>
    public const string SpravcaNavstevPolicy = "SpravcaNavstev";

    /// <summary>Hodnota claimu role pre ROLE-SpravcaNavstev (ADR-007, DR-C01).</summary>
    public const string SpravcaNavstevRoleValue = "SpravcaNavstev";

    /// <summary>Claim s rolou (ADR-004, DR-23).</summary>
    public const string RoleClaim = "role";

    /// <summary>Claim s identitou autora rezervácie – createdBy, QR-001 (ADR-007, DR-C02).</summary>
    public const string SubjectClaim = "sub";

    public static IServiceCollection AddIntelliWellAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false; // claimy role a sub zostanú pod pôvodnými názvami
                options.TokenValidationParameters.RoleClaimType = RoleClaim;
                options.TokenValidationParameters.NameClaimType = SubjectClaim;
                options.Events = new JwtBearerEvents
                {
                    // ADR-007 doplnenie (DR-C12): platný token bez neprázdneho claimu sub = neprihlásená osoba → 401.
                    OnTokenValidated = context =>
                    {
                        var subject = context.Principal?.FindFirst(SubjectClaim)?.Value;
                        if (string.IsNullOrEmpty(subject))
                        {
                            context.Fail("Token nemá neprázdny claim sub.");
                        }
                        return Task.CompletedTask;
                    },
                    // BR-012 pozícia 1: 401 unauthenticated.
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.Headers.WWWAuthenticate = "Bearer";
                        await Problems.WriteAsync(context.HttpContext, ProblemCatalog.Unauthenticated);
                    },
                    // BR-012 pozícia 2: 403 forbidden (iná hodnota alebo chýbajúci claim role).
                    OnForbidden = context => Problems.WriteAsync(context.HttpContext, ProblemCatalog.Forbidden),
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(SpravcaNavstevPolicy, policy => policy
                .RequireAuthenticatedUser()
                .RequireRole(SpravcaNavstevRoleValue));

        return services;
    }
}
