using System.Text.RegularExpressions;

namespace IntelliWell.Hosting;

/// <summary>
/// Hlavička X-Correlation-ID (DR-31, DR-C05, DR-C09; contract components.parameters/headers.CorrelationId).
/// Platná hodnota (1–128 znakov 0x21–0x7E) sa prevezme; chýbajúca, prázdna alebo neplatná sa nahradí
/// novým UUID. Požiadavka sa pre neplatné ID neodmietne. ID sa vracia v každej odpovedi.
/// </summary>
public sealed partial class CorrelationIdMiddleware(RequestDelegate next)
{
    public const string HeaderName = "X-Correlation-ID";

    [GeneratedRegex(@"^[\x21-\x7E]{1,128}$")]
    private static partial Regex ValidFormat();

    public static bool IsValid(string? value) => value is not null && ValidFormat().IsMatch(value);

    public Task InvokeAsync(HttpContext context)
    {
        var values = context.Request.Headers[HeaderName];
        var incoming = values.Count == 1 ? values[0] : null;
        var correlationId = IsValid(incoming) ? incoming! : Guid.NewGuid().ToString();

        context.Response.Headers[HeaderName] = correlationId;
        return next(context);
    }
}
