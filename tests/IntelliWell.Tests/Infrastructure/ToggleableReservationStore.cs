using IntelliWell.ModulRezervaciiAKlientov;

namespace IntelliWell.Tests.Infrastructure;

/// <summary>
/// Dekorátor úložiska pre SC-001-15 (QR-002): vo vypnutom stave je evidencia rezervácií „dočasne nedostupná“.
/// Reálne mapovanie SqliteException → 503 (DR-I04) overuje samostatný contract test so zamknutou databázou.
/// </summary>
public sealed class ToggleableReservationStore(IReservationStore inner) : IReservationStore
{
    public bool Available { get; set; } = true;

    public Task<IReservationWriteTransaction> BeginWriteAsync(CancellationToken cancellationToken) =>
        Available
            ? inner.BeginWriteAsync(cancellationToken)
            : throw new StorageUnavailableException(new IOException("Úložisko je v teste vypnuté."));
}
