using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace IntelliWell.Tests.Infrastructure;

/// <summary>
/// Testovacie JWT. V prototype tokeny vydáva dotnet user-jwts (DR-23); v testoch ich podpisuje testovací kľúč,
/// ktorý IntelliWellFactory nastaví do JwtBearerOptions. Validácia (issuer, audience, podpis, sub, role) je reálna.
/// </summary>
public static class TestTokens
{
    public const string Issuer = "intelliwell-tests";
    public const string Audience = "intelliwell-tests";

    public static readonly SymmetricSecurityKey SigningKey =
        new("IntelliWell-FR1-testovaci-podpisovy-kluc-256-bit!!"u8.ToArray());

    private static readonly SymmetricSecurityKey OtherKey =
        new("Iny-kluc-ktory-aplikacia-nepozna-0123456789abcdef"u8.ToArray());

    /// <summary>Hodnota claimu role pre ROLE-SpravcaNavstev (ADR-007, DR-C01).</summary>
    public const string SpravcaNavstev = "SpravcaNavstev";

    /// <summary>
    /// Iba testovacia hodnota, NIE rozhodnutie: hodnota claimu role pre ROLE-PoskytovatelDoplnkovejSluzby
    /// nie je v specification určená (I-07). Contract: akákoľvek iná hodnota ako SpravcaNavstev → 403.
    /// </summary>
    public const string PoskytovatelDoplnkovejSluzbyTestValue = "PoskytovatelDoplnkovejSluzby";

    /// <summary>Recepčná „Jana“, prihlásená v role správca návštev.</summary>
    public static string Jana => Create(("sub", "jana"), ("role", SpravcaNavstev));

    /// <summary>Recepčná „Mária“, prihlásená v role správca návštev.</summary>
    public static string Maria => Create(("sub", "maria"), ("role", SpravcaNavstev));

    /// <summary>Saunový majster „Tomáš“, prihlásený iba v role poskytovateľ doplnkovej služby.</summary>
    public static string Tomas => Create(("sub", "tomas"), ("role", PoskytovatelDoplnkovejSluzbyTestValue));

    public static string WithoutSub => Create(("role", SpravcaNavstev));

    public static string WithEmptySub => Create(("sub", ""), ("role", SpravcaNavstev));

    public static string WithoutRole => Create(("sub", "jana"));

    public static string SignedWithUnknownKey => Create(OtherKey, ("sub", "jana"), ("role", SpravcaNavstev));

    private static string Create(params (string Type, object Value)[] claims) => Create(SigningKey, claims);

    private static string Create(SecurityKey key, params (string Type, object Value)[] claims)
    {
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Audience = Audience,
            // Široké okno platnosti – nezávislé od reálneho aj testovacieho času.
            IssuedAt = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            NotBefore = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Expires = new DateTime(2100, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Claims = claims.ToDictionary(c => c.Type, c => c.Value),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
        };
        return new JsonWebTokenHandler { SetDefaultTimesOnTokenCreation = false }.CreateToken(descriptor);
    }
}
