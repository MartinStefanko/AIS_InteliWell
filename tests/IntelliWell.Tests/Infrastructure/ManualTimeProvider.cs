namespace IntelliWell.Tests.Infrastructure;

/// <summary>TimeProvider s nastaviteľným „teraz“ (rozhodnutie 0007: čas cez TimeProvider; bez ďalšieho balíka – ADR-008).</summary>
public sealed class ManualTimeProvider(DateTimeOffset utcNow) : TimeProvider
{
    public DateTimeOffset UtcNow { get; set; } = utcNow;

    public override DateTimeOffset GetUtcNow() => UtcNow;
}
