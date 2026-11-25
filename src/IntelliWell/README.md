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

## UI – vytvorenie rezervácie v prehliadači (FR1-UI, ADR-010)

Jedna statická stránka v `src/IntelliWell/wwwroot` (HTML + vanilla JavaScript), ktorú servíruje tá istá aplikácia.
UI je iba konzument API-Reservations 0.2.1; pravidlá termínu vyhodnocuje server.

1. Vytvoriť token pre správcu návštev (z koreňa repozitára; vypíše iba token):

   ```bash
   dotnet user-jwts create --project src/IntelliWell --name jana --role SpravcaNavstev --output token
   ```

   Pri prvom vytvorení tokenu nástroj zapíše issuer a audience do `appsettings.Development.json` – aplikáciu,
   ktorá už beží, treba reštartovať. Token bez roly (na overenie 403): `--name peter` bez `--role`.
2. Spustiť aplikáciu: `dotnet run --project src/IntelliWell --launch-profile http`
   a počkať na riadok `Now listening on: http://localhost:5080`.
3. Otvoriť v prehliadači **http://localhost:5080/** (stránku smie otvoriť aj neprihlásená osoba, DR-0913).
4. Do poľa „Prihlasovací token“ vložiť celý token z kroku 1 (bez predpony `Bearer`).
5. Identifikátor klienta je predvyplnený seed klientkou `7c9e6679-7425-40de-944b-e07fc1f90ae7`.
   Zadať začiatok v miestnom čase Europe/Bratislava (UI ho prepočíta na UTC so `Z`), voliteľne poznámku, a uložiť.

Token je iba v pamäti stránky – po obnovení stránky ho treba vložiť znova (DR-0910). Čas, ktorý pri zmene času
neexistuje alebo je dvojznačný (napr. 2:30 v deň zmeny), UI odmietne a nič neodošle (DR-0911).
Ručné overenie: `reports/manual-test-protocol-US-002.md`.

## Testy

```bash
dotnet build IntelliWell.slnx -warnaserror
dotnet test IntelliWell.slnx
node --test "tests/ui/*.test.mjs"
```

Acceptance tests majú trait `Scenario` = ID scenára z `spec/stories/features/US-001.feature`, napr.
`dotnet test IntelliWell.slnx --filter "Scenario=SC-001-06"`.

UI testy (US-002):

- `dotnet test IntelliWell.slnx --filter "Requirement=FR1-UI"` – servírovanie stránky a statická kontrola obsahu
  (`tests/IntelliWell.Tests/Ui/`, trait `Scenario` = SC-002-xx).
- `node --test "tests/ui/*.test.mjs"` – JS logika prepočtu času a hlášok (Node.js 22.7+ – načíta ES moduly z wwwroot/js bez package.json; bez npm balíkov; DR-1001,
  ADR-008 doplnenie). Spúšťa sa z koreňa repozitára, mimo `dotnet test`.
