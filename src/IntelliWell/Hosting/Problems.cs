using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntelliWell.Hosting;

/// <summary>
/// Typy problémov podľa BR-012 (order) a approved contractu API-Reservations 0.2.1 (ProblemDetails).
/// type = urn:intelliwell:problem:&lt;kód&gt; (DR-C07), title = reason z BR-012, status = HTTP kód.
/// </summary>
public sealed record ProblemKind(string Code, int Status, string Title)
{
    public string Type => "urn:intelliwell:problem:" + Code;
}

public static class ProblemCatalog
{
    // Poradie zodpovedá BR-012 pozíciám 1–9.
    public static readonly ProblemKind Unauthenticated = new("unauthenticated", 401, "osoba nie je prihlásená");
    public static readonly ProblemKind Forbidden = new("forbidden", 403, "nedostatočné oprávnenie");
    public static readonly ProblemKind InvalidRequest = new("invalid-request", 400, "neplatná požiadavka");
    public static readonly ProblemKind ServiceUnavailable = new("service-unavailable", 503, "služba je dočasne nedostupná");
    public static readonly ProblemKind ClientNotFound = new("client-not-found", 422, "klient neexistuje v evidencii klientov");
    public static readonly ProblemKind StartNotInFuture = new("start-not-in-future", 422, "začiatok termínu nie je v budúcnosti");
    public static readonly ProblemKind OutsideOpeningHours = new("outside-opening-hours", 422, "termín mimo otváracích hodín");
    public static readonly ProblemKind NoteTooLong = new("note-too-long", 422, "poznámka je dlhšia ako 500 znakov");
    public static readonly ProblemKind SlotOccupied = new("slot-occupied", 409, "termín obsadený");
}

/// <summary>Telo problem details (RFC 9457, DR-08). detail nikdy neobsahuje text poznámky (CON-003).</summary>
public sealed record ProblemBody(
    string Type,
    string Title,
    int Status,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Detail,
    string Instance);

public static class Problems
{
    public const string MediaType = "application/problem+json";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static ProblemBody Body(HttpContext context, ProblemKind kind, string? detail = null) =>
        new(kind.Type, kind.Title, kind.Status, detail, context.Request.Path.Value ?? string.Empty);

    public static IResult Result(HttpContext context, ProblemKind kind, string? detail = null) =>
        Results.Json(Body(context, kind, detail), JsonOptions, MediaType, kind.Status);

    public static Task WriteAsync(HttpContext context, ProblemKind kind, string? detail = null)
    {
        context.Response.StatusCode = kind.Status;
        return context.Response.WriteAsJsonAsync(Body(context, kind, detail), JsonOptions, MediaType);
    }
}
