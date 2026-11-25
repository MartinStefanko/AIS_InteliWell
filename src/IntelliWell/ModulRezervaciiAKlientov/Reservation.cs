using System.Globalization;
using System.Text.Json.Serialization;

namespace IntelliWell.ModulRezervaciiAKlientov;

/// <summary>Uložená rezervácia – DO-Rezervacie (vlastník AC-ModulRezervaciiAKlientov, ADR-006).</summary>
public sealed record Reservation(
    Guid Id,
    Guid ClientId,
    DateTimeOffset StartUtc,
    DateTimeOffset EndUtc,
    string? Note,
    string Status,
    string CreatedBy,
    DateTimeOffset CreatedAtUtc)
{
    /// <summary>Stav „vytvorená“ v API (lifecycles.yaml api_value, DR-C11).</summary>
    public const string StatusCreated = "created";
}

/// <summary>JSON tvar Reservation z approved contractu 0.2.1 (lowerCamelCase, DR-C07).</summary>
public sealed record ReservationResponse(
    string Id,
    string ClientId,
    string StartTime,
    string EndTime,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Note,
    string Status,
    string CreatedBy,
    string CreatedAt)
{
    public static ReservationResponse From(Reservation r) => new(
        r.Id.ToString("D"),
        r.ClientId.ToString("D"),
        FormatUtc(r.StartUtc),
        FormatUtc(r.EndUtc),
        r.Note,
        r.Status,
        r.CreatedBy,
        FormatUtc(r.CreatedAtUtc));

    /// <summary>RFC 3339 v UTC s príponou Z (DR-08, DR-C03); zlomky sekundy iba ak nie sú nulové.</summary>
    public static string FormatUtc(DateTimeOffset value) =>
        value.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.FFFFFFF'Z'", CultureInfo.InvariantCulture);
}
