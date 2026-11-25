# Implementation report – FR1 / API-Reservations 0.2.1 (úloha 0007, fáza 2)

| Pole | Hodnota |
|---|---|
| Režim | IMPLEMENT_CHANGE – fáza 2 (candidate code a tests) |
| Plán | `reports/implementation-plan-0007.md` (schválený človekom, DR-I01 – DR-I05 = A) |
| Log | `spec/prompts/log/0007-2026-11-25-IMPLEMENT_CHANGE.md` |
| Specification baseline | 0.3.1 (approved) / B-01 |
| Approved contract | API-Reservations 0.2.1 (`approval.yaml`) – nezmenený |
| ADR | ADR-001 – ADR-009 Accepted (ADR-008, ADR-009 doplnenia DR-I01 – DR-I05 zapísal človek) |
| Pripravila | AI (Claude Opus 5.5). Business význam zmeny potvrdzuje človek (AGENTS.md kap. 16). |

## 1. Quality gates

| Gate | Príkaz | Výsledok |
|---|---|---|
| Build (warnings as errors) | `dotnet build IntelliWell.slnx -warnaserror` | PASS – 0 warnings, 0 errors |
| Testy | `dotnet test IntelliWell.slnx` | PASS – 52 passed, 0 failed, 0 skipped |
| Kontrola testu súbehu | dočasná zmena `BEGIN IMMEDIATE` → `BEGIN DEFERRED`, `--filter Scenario=SC-001-06` (3×) | test 3× FAILED (očakávané `[Created, Conflict]`) → test vynútenie ADR-009 skutočne overuje; zmena vrátená |
| Balíky | `dotnet list IntelliWell.slnx package --include-transitive` | priame balíky = ADR-008; transitívne: SQLitePCLRaw.* 2.1.13, Microsoft.IdentityModel.* / System.IdentityModel.Tokens.Jwt 8.19.2 |

## 2. Odchýlky od plánu

| # | Odchýlka | Dôvod | Dopad |
|---|---|---|---|
| O-01 | `IntelliWell.Tests.csproj`: `UseMicrosoftTestingPlatformRunner=false`, `IsTestingPlatformApplication=false` | `xunit.v3` 4.0.3 predvolene zapína Microsoft.Testing.Platform; `dotnet test` na .NET 10 SDK potom odmietne VSTest (chyba MTP „Testing with VSTest target is no longer supported…“). Vlastnosti vrátia beh cez VSTest – `Microsoft.NET.Test.Sdk` + `xunit.runner.visualstudio` podľa ADR-008. | Žiadny nový balík ani súbor mimo projektu. Alternatíva (prechod na MTP cez `global.json`) by bola zmena prostredia – nepoužitá. |
| O-02 | Pomocné testovacie súbory navyše: `Infrastructure/ReservationsApi.cs` (volanie API a asserty tvaru odpovedí), `Infrastructure/CapturingLoggerProvider.cs` | plán uvádzal zachytávanie logov vo factory; oddelené do súborov | iba testy |
| O-03 | Contract test navyše: zamknutá databáza → 503 (`ZamknutaDatabaza_PoVyprsaniZamku_Vrati503`) | overuje reálne mapovanie `SqliteException` (vypršanie zámku) → 503 podľa DR-I04, nielen testovací dekorátor | iba testy |

Contract, specification ani component boundaries sa nemenili. Chyba contractu sa neodhalila.

## 3. Vytvorené súbory

Koreň: `IntelliWell.slnx`, `.gitignore`.

`src/IntelliWell/` (ART-SoftverIntelliWell):

| Súbor | Realizuje |
|---|---|
| `IntelliWell.csproj` | ADR-002, ADR-008; kópia `config/seed-clients.json` |
| `Program.cs` | skladanie hostu, `TimeProvider.System` |
| `appsettings.json`, `appsettings.Development.json`, `Properties/launchSettings.json` | ADR-003 (SQLite súbor), DR-23 (user-jwts), CON-003 (bez HttpLogging) |
| `README.md` | spustenie, token, testy |
| `Hosting/Problems.cs` | BR-012 katalóg, problem details (DR-08, DR-C07, BR-010) |
| `Hosting/CorrelationId.cs` | X-Correlation-ID (DR-31, DR-C05, DR-C09) |
| `Hosting/Authentication.cs` | JWT bearer, `sub`/`role`, DR-C12 → 401, 401/403 problem details (ADR-004, ADR-007, BR-011) |
| `ModulRezervaciiAKlientov/ReservationEndpoints.cs` | `POST /api/v1/reservations` (`createReservation`), 201 |
| `ModulRezervaciiAKlientov/CreateReservationRequestParser.cs` | 400 `invalid-request` (NFR1, DR-C03) |
| `ModulRezervaciiAKlientov/ReservationRules.cs` | BR-004, BR-005 (DR-18), BR-006, BR-008 (DR-C08) |
| `ModulRezervaciiAKlientov/CreateReservationService.cs` | poradie BR-012 poz. 4–9, BR-001/002/003/009/010, QR-001, QR-002, DR-28 |
| `ModulRezervaciiAKlientov/ReservationStore.cs` | SQLite, `BEGIN IMMEDIATE` (ADR-009 / DR-I02), audit stĺpce (DR-I03), `SqliteException` → 503 (DR-I04) |
| `ModulRezervaciiAKlientov/ClientSeeder.cs` | seed UUID, `INSERT OR IGNORE` (DR-03, DR-I05) |
| `ModulRezervaciiAKlientov/Reservation.cs` | DO-Rezervacie, JSON `Reservation` (DR-C07, DR-C11) |

`config/seed-clients.json` – 1 UUID (7c9e6679-…), žiadne osobné údaje.

`tests/IntelliWell.Tests/`: `IntelliWell.Tests.csproj`; `Infrastructure/` – `IntelliWellFactory.cs`, `ManualTimeProvider.cs`,
`TestTokens.cs`, `ToggleableReservationStore.cs`, `ReservationDbProbe.cs`, `ReservationsApi.cs`, `CapturingLoggerProvider.cs`;
`Acceptance/CreateReservationScenarios.cs` (16 testov); `Contract/CreateReservationContractTests.cs` (36 testov vrátane Theory prípadov).

## 4. Traceability summary – scenáre → testy

| Scenár | Test (trait `Scenario`) | Pravidlá / požiadavky | Výsledok |
|---|---|---|---|
| SC-001-01 | `SC_001_01_SpravcaNavstevVytvoriRezervaciuNaVolnyTermin` | FR1, BR-001, BR-002, BR-004, BR-008, QR-001 | PASS |
| SC-001-02 | `SC_001_02_RezervaciaBezPoznamkySaUlozi` | BR-008 | PASS |
| SC-001-03 | `SC_001_03_TerminKonciaciPresnePriZatvoreniSaUlozi` | BR-005 | PASS |
| SC-001-04 | `SC_001_04_RezervaciaNaObsadenyTerminJeOdmietnuta` | BR-003, BR-010 | PASS |
| SC-001-05 | `SC_001_05_RezervaciaNaCiastocnePrekryvajuciSaTerminJeOdmietnuta` | BR-003, BR-010 | PASS |
| SC-001-06 | `SC_001_06_PriSubehuSaUloziIbaPrvaRezervacia` | BR-003, BR-009, ADR-001, ADR-009 | PASS |
| SC-001-07 | `SC_001_07_RezervaciaPreKlientaMimoEvidencieJeOdmietnuta` | BR-007, BR-010, NFR1 | PASS |
| SC-001-08 | `SC_001_08_TerminPresahujuciZatvaraciuHodinuJeOdmietnuty` | BR-005, BR-010 | PASS |
| SC-001-09 | `SC_001_09_TerminZacinajuciPredOtvorenimJeOdmietnuty` | BR-005, BR-010 | PASS |
| SC-001-10 | `SC_001_10_TerminSoZaciatkomVMinulostiJeOdmietnuty` | BR-006, BR-010 | PASS |
| SC-001-11 | `SC_001_11_PrilisDlhaPoznamkaJeOdmietnuta` | BR-008, BR-010, CON-003 | PASS |
| SC-001-12 | `SC_001_12_OsobaBezRolySpravcuNavstevNemozeVytvoritRezervaciu` | NFR4, BR-011, ADR-004 (testovacia hodnota roly – nie rozhodnutie, I-07) | PASS |
| SC-001-13 | `SC_001_13_NeprihlasenaOsobaNemozeVytvoritRezervaciu` | NFR4, BR-011, ADR-004 | PASS |
| SC-001-14 | `SC_001_14_RezervaciaNadvazujucaNaKoniecPredchadzajucejSaUlozi` | BR-003, BR-004, DR-19 | PASS |
| SC-001-15 | `SC_001_15_PriNedostupnejEvidenciiRezervaciiSaRezervaciaNeulozi` | QR-002 | PASS |
| SC-001-16 | `SC_001_16_PriViacerychPoruseniachSystemUvedieIbaPrvyDovod` | BR-012, BR-010, NFR1 | PASS |

Contract príklady bez scenára: `unauthenticated-missing-sub` (trait `ContractExample`, 2 varianty: bez `sub`, prázdny `sub`) – PASS;
`invalid-request-time-offset` – PASS.

Ďalšie contract tests (všetky PASS): 400 tvar vstupu (12 prípadov), neznámy podpisový kľúč → 401, token bez `role` → 403, poradie
BR-012 pozície 1, 2, 3, 4, 6, 7, 8, zamknutá DB → 503 (DR-I04), DR-28 opakovanie → 409, poznámka 500/501 code points mimo BMP
(DR-C08), X-Correlation-ID (platné, 4 neplatné varianty, prítomnosť v 201/400/401/403/409/422/503), tvar 201, CON-003 (logy aj
problem details).

## 5. Otvorené body pre človeka

- Potvrdenie business významu zmeny (Definition of Done IMPLEMENT_CHANGE).
- `spec/traceability/traceability.yaml` (approved) neobsahuje stopy FR1 → kód → testy; AI ho nemenila. Návrh: doplniť podľa časti 4.
- I-04 z plánu: `deployment.yaml` / `technology-context.yaml` ešte neuvádzajú `packaging` / `prototype_environment` podľa rozhodnutia
  v ADR-008 doplnení.
- O-01: potvrdiť nastavenie behu testov cez VSTest (alebo rozhodnúť o prechode na Microsoft.Testing.Platform).
- ADR-008 a ADR-009 majú v sekcii `## Decision` stále text šablóny „Rozhodnutie doplní zodpovedný človek.“; rozhodnutie je v sekcii
  „Doplnenie“ – redakčné.
