namespace IntelliWell.ModulRezervaciiAKlientov;

/// <summary>Business pravidlá FR1 bez prístupu k úložisku.</summary>
public static class ReservationRules
{
    /// <summary>BR-004: pevná dĺžka termínu 60 minút; interval [začiatok, koniec) (DR-19).</summary>
    public static readonly TimeSpan SlotLength = TimeSpan.FromMinutes(60);

    /// <summary>BR-005: otváracie hodiny 10:00–22:00 v Europe/Bratislava, každý deň rovnako (DR-01, DR-18, DR-20).</summary>
    public static readonly TimeSpan Opening = new(10, 0, 0);
    public static readonly TimeSpan Closing = new(22, 0, 0);
    public static readonly TimeZoneInfo BusinessTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Bratislava");

    /// <summary>BR-008: poznámka má najviac 500 znakov (Unicode code points, DR-C08).</summary>
    public const int NoteMaxCodePoints = 500;

    public static DateTimeOffset EndOf(DateTimeOffset startUtc) => startUtc + SlotLength;

    /// <summary>BR-006: začiatok je v budúcnosti v čase uloženia.</summary>
    public static bool StartsInFuture(DateTimeOffset startUtc, DateTimeOffset nowUtc) => startUtc > nowUtc;

    /// <summary>BR-005: začiatok aj koniec ležia v otváracích hodinách toho istého miestneho dňa.</summary>
    public static bool WithinOpeningHours(DateTimeOffset startUtc, DateTimeOffset endUtc)
    {
        var localStart = TimeZoneInfo.ConvertTime(startUtc, BusinessTimeZone);
        var localEnd = TimeZoneInfo.ConvertTime(endUtc, BusinessTimeZone);
        return localStart.TimeOfDay >= Opening
            && localEnd.Date == localStart.Date
            && localEnd.TimeOfDay <= Closing;
    }

    /// <summary>BR-008: poznámka je voliteľná; ak je zadaná, má najviac 500 Unicode code points.</summary>
    public static bool NoteWithinLimit(string? note) =>
        note is null || note.EnumerateRunes().Count() <= NoteMaxCodePoints;
}
