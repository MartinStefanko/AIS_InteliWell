using Microsoft.Data.Sqlite;

namespace IntelliWell.Tests.Infrastructure;

/// <summary>Priame čítanie testovacej SQLite databázy – overenie „termín je voľný / existuje práve jedna rezervácia“.</summary>
public sealed class ReservationDbProbe(string connectionString)
{
    /// <summary>Počet uložených rezervácií, ktorých termín sa prekrýva s [start, start + 60 min).</summary>
    public int CountInSlot(DateTimeOffset startUtc)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT COUNT(*) FROM reservations WHERE start_utc_ticks < $end AND $start < end_utc_ticks;";
        command.Parameters.AddWithValue("$start", startUtc.UtcTicks);
        command.Parameters.AddWithValue("$end", startUtc.AddMinutes(60).UtcTicks);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public int CountAll()
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM reservations;";
        return Convert.ToInt32(command.ExecuteScalar());
    }

    /// <summary>Audit záznam QR-001 v riadku rezervácie (DR-I03).</summary>
    public (string CreatedBy, DateTimeOffset CreatedAt) AuditOf(Guid reservationId)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT created_by, created_at_utc_ticks FROM reservations WHERE id = $id;";
        command.Parameters.AddWithValue("$id", reservationId.ToString("D"));
        using var reader = command.ExecuteReader();
        Assert.True(reader.Read(), "Rezervácia nie je v úložisku.");
        return (reader.GetString(0), new DateTimeOffset(reader.GetInt64(1), TimeSpan.Zero));
    }
}
