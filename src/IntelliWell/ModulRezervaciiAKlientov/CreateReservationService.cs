using IntelliWell.Hosting;

namespace IntelliWell.ModulRezervaciiAKlientov;

/// <summary>Výsledok vytvorenia: uložená rezervácia alebo pomenovaný dôvod odmietnutia (BR-010).</summary>
public sealed record CreateReservationResult(Reservation? Reservation, ProblemKind? Rejection)
{
    public static CreateReservationResult Created(Reservation reservation) => new(reservation, null);
    public static CreateReservationResult Rejected(ProblemKind reason) => new(null, reason);
}

/// <summary>
/// Vytvorenie rezervácie – AS-SpravaRezervaciiAKlientov / AC-ModulRezervaciiAKlientov (FR1, operation createReservation).
/// Poradie kontrol BR-012 pozície 4–9 (pozície 1–3 rieši autentifikácia, autorizácia a parser pred volaním služby).
/// Pri každom odmietnutí sa transakcia vráti späť: rezervácia nevznikne a termín sa nezablokuje (lifecycles.yaml).
/// </summary>
public sealed class CreateReservationService(
    IReservationStore store,
    TimeProvider timeProvider,
    ILogger<CreateReservationService> logger)
{
    public async Task<CreateReservationResult> CreateAsync(
        CreateReservationCommand command, string createdBy, CancellationToken cancellationToken)
    {
        try
        {
            // 4: úložisko dostupné (QR-002). Zápisová transakcia drží zámok až po uloženie (ADR-001, ADR-009).
            await using var transaction = await store.BeginWriteAsync(cancellationToken);

            // 5: BR-007 – klient existuje v evidencii klientov.
            if (!await transaction.ClientExistsAsync(command.ClientId, cancellationToken))
            {
                return CreateReservationResult.Rejected(ProblemCatalog.ClientNotFound);
            }

            // 6: BR-006 – začiatok je v budúcnosti v čase uloženia.
            var now = timeProvider.GetUtcNow();
            if (!ReservationRules.StartsInFuture(command.StartUtc, now))
            {
                return CreateReservationResult.Rejected(ProblemCatalog.StartNotInFuture);
            }

            // 7: BR-005 – termín (BR-004: začiatok + 60 min) leží celý v otváracích hodinách.
            var end = ReservationRules.EndOf(command.StartUtc);
            if (!ReservationRules.WithinOpeningHours(command.StartUtc, end))
            {
                return CreateReservationResult.Rejected(ProblemCatalog.OutsideOpeningHours);
            }

            // 8: BR-008 – poznámka najviac 500 Unicode code points.
            if (!ReservationRules.NoteWithinLimit(command.Note))
            {
                return CreateReservationResult.Rejected(ProblemCatalog.NoteTooLong);
            }

            // 9: BR-003 / BR-009 – termín sa neprekrýva s uloženou rezerváciou (aj pri súbehu a opakovaní, DR-28).
            if (await transaction.HasOverlapAsync(command.StartUtc, end, cancellationToken))
            {
                return CreateReservationResult.Rejected(ProblemCatalog.SlotOccupied);
            }

            // Uloženie v stave „vytvorená“ (created) so záznamom kto a kedy (QR-001, DR-I03); BR-002 – termín je zablokovaný.
            var reservation = new Reservation(
                Guid.NewGuid(),
                command.ClientId,
                command.StartUtc,
                end,
                command.Note,
                Reservation.StatusCreated,
                createdBy,
                now);

            await transaction.InsertAsync(reservation, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            // CON-003: do aplikačného logu sa nezapisuje poznámka ani telo požiadavky.
            logger.LogInformation("Rezervácia {ReservationId} bola vytvorená.", reservation.Id);
            return CreateReservationResult.Created(reservation);
        }
        catch (StorageUnavailableException ex)
        {
            // QR-002: rezervácia sa neuloží a uloženie sa automaticky neopakuje.
            logger.LogWarning(
                "Úložisko rezervácií nie je dostupné ({ErrorType}).",
                ex.InnerException?.GetType().Name ?? "neznáma chyba");
            return CreateReservationResult.Rejected(ProblemCatalog.ServiceUnavailable);
        }
    }
}
