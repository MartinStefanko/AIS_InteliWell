# 0007 – IMPLEMENT_CHANGE – implementácia FR1 (API-Reservations 0.2.1)

| Pole | Hodnota |
|---|---|
| Dátum | 2026-11-25 |
| Poradové číslo | 0007 |
| Režim | IMPLEMENT_CHANGE |
| Verzia šablóny promptu | spec/prompts/implement-change.md, prompt_template_version 1.0 |
| Specification baseline | 0.3.1 (approved) |
| Architecture baseline | B-01 |
| Model | Claude Opus 5.5 (`claude-opus-5-5`), Claude Code |
| Commit | (doplní človek) |

## Prompt

~~~text
# Úloha 0007: implementácia FR1

Mode: IMPLEMENT_CHANGE
Change: FR1
Specification baseline: 0.3.1 (approved)
Approved contract: API-Reservations 0.2.1 (spec/contracts/approved/, approval.yaml)
Prompt template: spec/prompts/implement-change.md v1.0

Over manifest 0.3.1 approved, ADR-001 až ADR-007 Accepted a approval.yaml pre 0.2.1; inak DECISION REQUIRED.

Moje rozhodnutia pre implementáciu (Martin Štefanko, Software architect):
- Prototyp sa spúšťa lokálne cez `dotnet run`; kontajner ani CI nie sú v rozsahu.
- Audit záznam (QR-001) sa ukladá do SQLite spolu s rezerváciou; doba uchovania je v prototype mimo rozsahu.
- Testy: xUnit + WebApplicationFactory; každý scenár SC-001-xx je jeden test označený traitom s jeho ID.
- Čas sa v kóde berie cez TimeProvider, aby testy vedeli nastaviť „teraz“.
- Klienti sú seed dáta (DR-03).

## FÁZA 1 (teraz): iba plán
Priprav impact analysis a generation plan po súboroch (src/, tests/, config/): čo vznikne, ktorý súbor realizuje ktorú časť
contractu, BR, QR a AC-ModulRezervaciiAKlientov, a ktoré testy pokryjú SC-001-01 až SC-001-16 a príklad unauthenticated-missing-sub.
Uveď všetky NuGet balíky s verziou. Nový balík alebo knižnica, ktorá nie je v ADR, je DECISION REQUIRED alebo draft ADR.
Kód zatiaľ NEPÍŠ. Počkaj na moje schválenie plánu.

## FÁZA 2 (až po mojom „plán schvaľujem“)
Vygeneruj najmenší candidate code a testy podľa plánu, spusti `dotnet build` a `dotnet test` a vypíš výsledky.
Nemeň approved contract, specification ani component boundaries. Ak implementácia odhalí chybu contractu, zastav sa a vráť DECISION REQUIRED.

Log podľa CLAUDE.md (0007-2026-11-25-IMPLEMENT_CHANGE.md), obe fázy do toho istého logu. Necommituj.
Odpoveď štruktúruj podľa kapitoly 15 AGENTS.md.
~~~

## Výstup AI

### Fáza 1 – plán (bez kódu)

**Vstupná kontrola:** manifest 0.3.1 approved – PASS; ADR-001 až ADR-007 Accepted – PASS; `approval.yaml` pre
API-Reservations 0.2.1 (approved, spec 0.3.1, bez otvorených DR) – PASS; approved contract = proposed 0.2.1 okrem
`status` – PASS. .NET SDK 10.0.404 / runtime 10.0.15.

**Vytvorené (working / draft):**

- `reports/implementation-plan-0007.md` – vstupná kontrola, impact analysis, kontrola architektúry, generation plan
  po súboroch (`IntelliWell.slnx`, `.gitignore`, `src/IntelliWell/…`, `config/seed-clients.json`,
  `tests/IntelliWell.Tests/…`), pokrytie SC-001-01 – SC-001-16 (acceptance, trait `Scenario`) a contract tests
  (`unauthenticated-missing-sub`, `invalid-request-time-offset`, poradie BR-012, DR-28, DR-C08, correlation ID,
  CON-003), NuGet balíky s verziou.
- `spec/architecture/decisions/ADR-008-kniznice-prototypu-fr1.md` – **Proposed**: JwtBearer 10.0.15,
  Microsoft.Data.Sqlite 10.0.15, Mvc.Testing 10.0.15, Microsoft.NET.Test.Sdk 18.12.0, xunit.v3 4.0.3,
  xunit.runner.visualstudio 4.0.1.
- `spec/architecture/decisions/ADR-009-vynutenie-br-003-nad-sqlite.md` – **Proposed**: vynútenie BR-003/BR-009
  transakciou `BEGIN IMMEDIATE`.

Kód ani testy nevznikli. Approved contract, specification a manifest sa nemenili.

**Neblokujúce zistenia:** I-01 approved contract má v hlavičke „PROPOSED … Nie je schválený“ a `approved_by: null`,
mapping odkazuje na `proposed/`; I-02 manifest `approved_contracts: null`, `realization.code: null`; I-03 prázdny
adresár `spec/contracts/approved copy/`; I-04 rozhodnutie „lokálne `dotnet run`“ nie je v deployment.yaml /
technology-context.yaml; I-05 `allowed_dependencies: null` (plán nepridáva závislosť na inom AC); I-06 chýba scenár
pre token bez `sub` (N-10); I-07 testovacia hodnota claimu `role` pre SC-001-12.

**Otvorené DECISION REQUIRED:**

- DR-I01 – NuGet balíky (ADR-008); odporúčanie A: minimálna sada.
- DR-I02 – vynútenie BR-003/BR-009 nad SQLite (ADR-009); odporúčanie A: `BEGIN IMMEDIATE`.
- DR-I03 – forma audit záznamu QR-001; odporúčanie A: stĺpce `created_by`, `created_at` v riadku rezervácie.
- DR-I04 – čo je „úložisko nedostupné“ (QR-002); odporúčanie A: každá `SqliteException` → 503 bez opakovania.
- DR-I05 – seed klientov; odporúčanie A: `config/seed-clients.json` iba s UUID, `INSERT OR IGNORE` pri štarte.

### Fáza 2 – implementácia

**Doplňujúci prompt (doslovne):**

~~~text
Plán schvaľujem, DR-I01 až DR-I05 podľa odporúčania A.
Rozhodnutia som ručne zapísal do ADR-008 a ADR-009 (doplnenia) a oba ADR som nastavil na Accepted.
I-01, I-02, I-03 som opravil ručne. I-07: testovacia hodnota roly je v poriadku, ponechaj označenie „nie rozhodnutie“.
Pokračuj FÁZOU 2 podľa schváleného plánu. Na konci spusti dotnet build (warnings as errors) a dotnet test
a vypíš výsledky, zoznam vytvorených súborov a pokrytie SC-001-01 až SC-001-16. Necommituj.
~~~

**Vytvorené (realization):** `IntelliWell.slnx`, `.gitignore`, `config/seed-clients.json`, `src/IntelliWell/` (csproj, Program.cs,
appsettings, launchSettings, README, `Hosting/` – Problems, CorrelationId, Authentication; `ModulRezervaciiAKlientov/` –
ReservationEndpoints, CreateReservationRequestParser, ReservationRules, CreateReservationService, ReservationStore, ClientSeeder,
Reservation), `tests/IntelliWell.Tests/` (csproj, 7 súborov `Infrastructure/`, `Acceptance/CreateReservationScenarios.cs`,
`Contract/CreateReservationContractTests.cs`). Report: `reports/implementation-0007.md`.

**Výsledky:** `dotnet build IntelliWell.slnx -warnaserror` – 0 warnings, 0 errors. `dotnet test IntelliWell.slnx` – 52 passed,
0 failed. SC-001-01 – SC-001-16: každý jeden test s traitom `Scenario`, všetky PASS; `unauthenticated-missing-sub` PASS.
Kontrola testu súbehu: pri `BEGIN DEFERRED` namiesto `BEGIN IMMEDIATE` SC-001-06 zlyhá (3/3) – test vynútenie ADR-009 overuje.

**Odchýlky:** O-01 – testovací projekt má `UseMicrosoftTestingPlatformRunner=false` a `IsTestingPlatformApplication=false`, lebo
xunit.v3 4.0.3 zapína Microsoft.Testing.Platform, ktorú .NET 10 `dotnet test` cez VSTest odmietne; bez nového balíka. O-02, O-03 –
pomocné testovacie súbory a test zamknutej DB (DR-I04).

Approved contract, specification, manifest ani component boundaries sa nemenili. Chyba contractu sa neodhalila.

**Otvorené body (nie DECISION REQUIRED):** potvrdenie business významu; doplnenie stôp do `traceability.yaml`; I-04 (deployment.yaml);
potvrdenie O-01; sekcia `## Decision` v ADR-008/009 má text šablóny.

## Ľudské rozhodnutie
- **Rozhodol:** Martin Štefanko (Software architect), 2026-11-25
- **Fáza 1:** Generation plan som schválil. DR-I01 až DR-I05 = A, zapísané v ADR-008 a ADR-009 (Accepted).
- **Fáza 2:** Candidate code a testy prijímam. dotnet build bez warningov, dotnet test 52/52 PASS – overil som aj sám lokálne.
- **Business význam:** potvrdzujem, že správanie zodpovedá FR1 a scenárom SC-001-01 až SC-001-16.
- **O-01:** súhlasím s behom testov cez VSTest (bez nového balíka). O-02 a O-03 akceptujem.
- **Odložené:** traceability FR1 → kód → testy, packaging/prototype_environment v deployment.yaml (I-04), scenár pre token bez sub (N-10)
  a kozmetické nálezy N-01, N-02 – v ďalšom PREPARE_SPEC.

