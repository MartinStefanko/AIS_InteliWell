using Microsoft.Data.Sqlite;

namespace IntelliWell.ModulRezervaciiAKlientov;

/// <summary>Úložisko rezervácií nie je dostupné (QR-002; DR-I04: každá SqliteException vrátane vypršania zámku).</summary>
public sealed class StorageUnavailableException(Exception inner)
    : Exception("Úložisko rezervácií nie je dostupné.", inner);

/// <summary>Prístup modulu k DO-Rezervacie a DO-EvidenciaKlientov (vlastník AC-ModulRezervaciiAKlientov, ADR-006).</summary>
public interface IReservationStore
{
    /// <summary>Otvorí zápisovú transakciu, ktorá drží zámok počas kontroly aj uloženia (ADR-009).</summary>
    Task<IReservationWriteTransaction> BeginWriteAsync(CancellationToken cancellationToken);
}

public interface IReservationWriteTransaction : IAsyncDisposable
{
    /// <summary>BR-007: klient existuje v evidencii klientov.</summary>
    Task<bool> ClientExistsAsync(Guid clientId, CancellationToken cancellationToken);

    /// <summary>BR-003: existuje uložená rezervácia, ktorej termín sa prekrýva s [start, end).</summary>
    Task<bool> HasOverlapAsync(DateTimeOffset startUtc, DateTimeOffset endUtc, CancellationToken cancellationToken);

    /// <summary>Uloží rezerváciu vrátane audit stĺpcov created_by a created_at (QR-001, DR-I03).</summary>
    Task InsertAsync(Reservation reservation, CancellationToken cancellationToken);

    Task CommitAsync(CancellationToken cancellationToken);
}

/// <summary>
/// SQLite úložisko prototypu (ADR-003). Súbeh: BEGIN IMMEDIATE (ADR-009, DR-I02) – druhá zápisová transakcia
/// čaká na zámok (busy timeout pripojenia) a potom vidí už uloženú rezerváciu. Uloženie sa neopakuje (QR-002).
/// </summary>
public sealed class SqliteReservationStore : IReservationStore
{
    private readonly string _connectionString;

    public SqliteReservationStore(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Reservations")
            ?? throw new InvalidOperationException("Chýba ConnectionStrings:Reservations.");
    }

    /// <summary>Vytvorí úložné tabuľky, ak neexistujú, a vloží seed klientov (DR-03, DR-I05).</summary>
    public async Task InitializeAsync(IEnumerable<Guid> seedClientIds, CancellationToken cancellationToken)
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var transaction = (SqliteTransaction)await connection.BeginTransactionAsync(cancellationToken);

        await using (var schema = connection.CreateCommand())
        {
            schema.Transaction = transaction;
            schema.CommandText = """
                CREATE TABLE IF NOT EXISTS clients (
                    id TEXT NOT NULL PRIMARY KEY
                );
                CREATE TABLE IF NOT EXISTS reservations (
                    id TEXT NOT NULL PRIMARY KEY,
                    client_id TEXT NOT NULL REFERENCES clients(id),
                    start_utc_ticks INTEGER NOT NULL,
                    end_utc_ticks INTEGER NOT NULL,
                    note TEXT NULL,
                    status TEXT NOT NULL,
                    created_by TEXT NOT NULL,
                    created_at_utc_ticks INTEGER NOT NULL
                );
                CREATE INDEX IF NOT EXISTS ix_reservations_start ON reservations (start_utc_ticks);
                """;
            await schema.ExecuteNonQueryAsync(cancellationToken);
        }

        foreach (var clientId in seedClientIds)
        {
            await using var seed = connection.CreateCommand();
            seed.Transaction = transaction;
            seed.CommandText = "INSERT OR IGNORE INTO clients (id) VALUES ($id);";
            seed.Parameters.AddWithValue("$id", clientId.ToString("D"));
            await seed.ExecuteNonQueryAsync(cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<IReservationWriteTransaction> BeginWriteAsync(CancellationToken cancellationToken)
    {
        var connection = new SqliteConnection(_connectionString);
        try
        {
            await connection.OpenAsync(cancellationToken);
            await using var begin = connection.CreateCommand();
            begin.CommandText = "BEGIN IMMEDIATE;";
            await begin.ExecuteNonQueryAsync(cancellationToken);
            return new WriteTransaction(connection);
        }
        catch (SqliteException ex)
        {
            await connection.DisposeAsync();
            throw new StorageUnavailableException(ex);
        }
    }

    private sealed class WriteTransaction(SqliteConnection connection) : IReservationWriteTransaction
    {
        private bool _committed;

        public Task<bool> ClientExistsAsync(Guid clientId, CancellationToken cancellationToken) =>
            ScalarBoolAsync(
                "SELECT EXISTS (SELECT 1 FROM clients WHERE id = $id);",
                cancellationToken,
                ("$id", clientId.ToString("D")));

        public Task<bool> HasOverlapAsync(DateTimeOffset startUtc, DateTimeOffset endUtc, CancellationToken cancellationToken) =>
            ScalarBoolAsync(
                "SELECT EXISTS (SELECT 1 FROM reservations WHERE start_utc_ticks < $end AND $start < end_utc_ticks);",
                cancellationToken,
                ("$start", startUtc.UtcTicks),
                ("$end", endUtc.UtcTicks));

        public Task InsertAsync(Reservation r, CancellationToken cancellationToken) =>
            ExecuteAsync(
                """
                INSERT INTO reservations
                    (id, client_id, start_utc_ticks, end_utc_ticks, note, status, created_by, created_at_utc_ticks)
                VALUES ($id, $clientId, $start, $end, $note, $status, $createdBy, $createdAt);
                """,
                cancellationToken,
                ("$id", r.Id.ToString("D")),
                ("$clientId", r.ClientId.ToString("D")),
                ("$start", r.StartUtc.UtcTicks),
                ("$end", r.EndUtc.UtcTicks),
                ("$note", (object?)r.Note ?? DBNull.Value),
                ("$status", r.Status),
                ("$createdBy", r.CreatedBy),
                ("$createdAt", r.CreatedAtUtc.UtcTicks));

        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            await ExecuteAsync("COMMIT;", cancellationToken);
            _committed = true;
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                if (!_committed && connection.State == System.Data.ConnectionState.Open)
                {
                    await using var rollback = connection.CreateCommand();
                    rollback.CommandText = "ROLLBACK;";
                    await rollback.ExecuteNonQueryAsync();
                }
            }
            catch (SqliteException)
            {
                // Transakcia už nie je aktívna (napr. po chybe úložiska); zatvorenie pripojenia ju ukončí.
            }
            finally
            {
                await connection.DisposeAsync();
            }
        }

        private async Task<bool> ScalarBoolAsync(string sql, CancellationToken ct, params (string Name, object Value)[] parameters)
        {
            try
            {
                await using var command = Create(sql, parameters);
                return Convert.ToInt64(await command.ExecuteScalarAsync(ct)) == 1;
            }
            catch (SqliteException ex)
            {
                throw new StorageUnavailableException(ex);
            }
        }

        private async Task ExecuteAsync(string sql, CancellationToken ct, params (string Name, object Value)[] parameters)
        {
            try
            {
                await using var command = Create(sql, parameters);
                await command.ExecuteNonQueryAsync(ct);
            }
            catch (SqliteException ex)
            {
                throw new StorageUnavailableException(ex);
            }
        }

        private SqliteCommand Create(string sql, (string Name, object Value)[] parameters)
        {
            var command = connection.CreateCommand();
            command.CommandText = sql;
            foreach (var (name, value) in parameters)
            {
                command.Parameters.AddWithValue(name, value);
            }
            return command;
        }
    }
}
