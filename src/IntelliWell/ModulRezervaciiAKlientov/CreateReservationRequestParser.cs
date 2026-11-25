using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace IntelliWell.ModulRezervaciiAKlientov;

/// <summary>Vstup operácie createReservation po kontrole tvaru.</summary>
public sealed record CreateReservationCommand(Guid ClientId, DateTimeOffset StartUtc, string? Note);

/// <summary>
/// Kontrola tvaru vstupu CreateReservationRequest (BR-012 pozícia 3, NFR1, DR-C03; odpoveď 400 invalid-request).
/// Dĺžka poznámky sa tu nekontroluje – je to business pravidlo BR-008 (422 note-too-long).
/// </summary>
public static partial class CreateReservationRequestParser
{
    // RFC 3339 date-time v UTC s príponou Z (schéma UtcDateTime: format date-time, pattern 'Z$').
    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2}[Tt]\d{2}:\d{2}:\d{2}(\.\d{1,7})?Z$")]
    private static partial Regex Rfc3339Utc();

    public static async Task<(CreateReservationCommand? Command, string? Error)> ParseAsync(
        Stream body, CancellationToken cancellationToken)
    {
        JsonDocument document;
        try
        {
            document = await JsonDocument.ParseAsync(body, cancellationToken: cancellationToken);
        }
        catch (JsonException)
        {
            return (null, "Telo požiadavky nie je platný JSON.");
        }

        using (document)
        {
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                return (null, "Telo požiadavky musí byť JSON objekt.");
            }

            if (!root.TryGetProperty("clientId", out var clientIdElement)
                || clientIdElement.ValueKind != JsonValueKind.String
                || !Guid.TryParseExact(clientIdElement.GetString(), "D", out var clientId))
            {
                return (null, "Pole clientId musí byť UUID.");
            }

            if (!root.TryGetProperty("startTime", out var startElement)
                || startElement.ValueKind != JsonValueKind.String
                || !TryParseUtc(startElement.GetString()!, out var startUtc))
            {
                return (null, "Pole startTime musí byť čas RFC 3339 v UTC s príponou Z.");
            }

            string? note = null;
            if (root.TryGetProperty("note", out var noteElement))
            {
                if (noteElement.ValueKind != JsonValueKind.String)
                {
                    // Text poznámky sa do odpovede nevypisuje (CON-003).
                    return (null, "Pole note musí byť reťazec.");
                }
                note = noteElement.GetString();
            }

            return (new CreateReservationCommand(clientId, startUtc, note), null);
        }
    }

    private static bool TryParseUtc(string value, out DateTimeOffset result)
    {
        result = default;
        return Rfc3339Utc().IsMatch(value)
            && DateTimeOffset.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out result);
    }
}
