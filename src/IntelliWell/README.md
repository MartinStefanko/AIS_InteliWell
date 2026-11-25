# IntelliWell – prototyp FR1 (Vytvorenie rezervácie)

Candidate code z úlohy 0007 (IMPLEMENT_CHANGE) podľa approved contractu API-Reservations 0.2.1
(`spec/contracts/approved/`). Prototyp sa spúšťa lokálne cez `dotnet run`; kontajner ani CI nie sú v rozsahu (ADR-008).

## Spustenie

```bash
dotnet run --project src/IntelliWell --launch-profile http
```

- Úložisko: SQLite súbor `intelliwell-prototyp.db` v adresári `src/IntelliWell` (ADR-003). Pri štarte sa vytvorí schéma
  a seed klienti z `config/seed-clients.json` (iba UUID, DR-03, DR-I05).
- API: `POST http://localhost:5080/api/v1/reservations`.

## Token (iba vývojové prostredie, DR-23)

```bash
dotnet user-jwts create --project src/IntelliWell --name jana --role SpravcaNavstev
```

`dotnet user-jwts` nastaví claim `sub` = `jana` (autor rezervácie, ADR-007) a claim `role` = `SpravcaNavstev`.
Podpisový kľúč uloží do user secrets a issuer/audience zapíše do `appsettings.Development.json`.

## Príklad

```bash
curl -i -X POST http://localhost:5080/api/v1/reservations \
  -H "Authorization: Bearer <token>" -H "Content-Type: application/json" \
  -d '{"clientId":"7c9e6679-7425-40de-944b-e07fc1f90ae7","startTime":"2026-12-01T13:00:00Z","note":"prvá návšteva"}'
```

## Testy

```bash
dotnet build IntelliWell.slnx -warnaserror
dotnet test IntelliWell.slnx
```

Acceptance tests majú trait `Scenario` = ID scenára z `spec/stories/features/US-001.feature`, napr.
`dotnet test IntelliWell.slnx --filter "Scenario=SC-001-06"`.
