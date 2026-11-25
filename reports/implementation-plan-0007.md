# Implementation plan – FR1 / API-Reservations 0.2.1 (úloha 0007, fáza 1)

| Pole | Hodnota |
|---|---|
| Režim | IMPLEMENT_CHANGE – fáza 1 (iba plán, bez kódu) |
| Log | `spec/prompts/log/0007-2026-11-25-IMPLEMENT_CHANGE.md` |
| Specification baseline | 0.3.1 (approved) / architecture baseline B-01 |
| Approved contract | API-Reservations 0.2.1 – `spec/contracts/approved/openapi-reservations.yaml`, `contract-mapping.yaml`, `approval.yaml` |
| Prompt template | `spec/prompts/implement-change.md` v1.0 |
| Pripravila | AI (Claude Opus 5.5). Plán nie je schválený; čaká na „plán schvaľujem“. |

## 1. Vstupná kontrola

| Kontrola | Výsledok | Dôkaz |
|---|---|---|
| Manifest 0.3.1 approved | PASS | `spec/manifest.yaml`: `specification.version: "0.3.1"`, `status: "approved"` |
| ADR-001 až ADR-007 Accepted | PASS | `## Status` = `Accepted` v `ADR-001` – `ADR-007`; ADR-007 má doplnenie DR-C12 |
| approval.yaml pre 0.2.1 | PASS | `contract_id: API-Reservations`, `version: "0.2.1"`, `status: approved`, `specification_version: "0.3.1"`, `approved_by: Martin Štefanko`, `unresolved_decisions: []` |
| Approved contract = posledný proposed | PASS | `diff` approved vs. proposed: líši sa iba `status` (approved/proposed) v oboch súboroch |
| .NET prostredie | PASS | SDK 10.0.404, runtime Microsoft.AspNetCore.App 10.0.15 (ADR-002) |

Neblokujúce zistenia v podkladoch (AI ich nemení, iba vypisuje):

| ID | Zistenie | Návrh pre človeka |
|---|---|---|
| I-01 | Approved `openapi-reservations.yaml` má v hlavičke komentár „PROPOSED contract … Nie je schválený“ a `x-sdd.approved_by: null`; `contract-mapping.yaml` má `file:` na `spec/contracts/proposed/…` a `approved_by: null`. Záznam o schválení je jednoznačný v `approval.yaml`. | opraviť hlavičku a odkazy v approved súboroch (redakčne, bez zmeny verzie) |
| I-02 | Manifest: `authoritative.approved_contracts: null`, `realization.code: null` („src/ zatiaľ neexistuje“); komentáre r. 5, 8, 9 sú zastarané (N-01). | návrh zmeny manifestu: `approved_contracts: "spec/contracts/approved/"`, `realization.code: "src/"` |
| I-03 | Prázdny adresár `spec/contracts/approved copy/` (commit 3fb384e). | odstrániť |
| I-04 | `deployment.yaml`: `required_by_implementation_task: null`, `packaging: null`; `technology-context.yaml`: `prototype_environment: null`. Rozhodnutie z úlohy 0007 („lokálne cez `dotnet run`, bez kontajnera a CI“) v nich nie je zapísané. | zapísať do deployment.yaml / technology-context.yaml |
| I-05 | `components.yaml`: `allowed_dependencies: null`, `uses_services: null`. Plán nevytvára závislosť na inom Application Componente; používa iba TS-DatabazovaSluzba (SQLite) a runtime podľa deployment.yaml. | neblokuje; potvrdiť pri schválení plánu |
| I-06 | Pre token bez `sub` (DR-C12) nie je scenár v US-001 (N-10). Pokryje ho test pomenovaného príkladu `unauthenticated-missing-sub`. | SC-001-17 v ďalšom PREPARE_SPEC (voliteľné) |
| I-07 | Hodnota claimu `role` pre ROLE-PoskytovatelDoplnkovejSluzby nie je rozhodnutá. Test SC-001-12 použije testovaciu hodnotu `PoskytovatelDoplnkovejSluzby`, označenú v kóde ako „iba testovacia hodnota, nie rozhodnutie“. Contract: akákoľvek iná hodnota → 403. | potvrdiť pri schválení plánu |

## 2. Impact analysis (AGENTS.md kap. 14)

```yaml
change:
  mode: "IMPLEMENT_CHANGE"
  requirement: "FR1"
  specification_baseline: "0.3.1"
  reason: >
    Candidate code a tests pre operáciu createReservation (POST /api/v1/reservations) podľa approved
    contractu API-Reservations 0.2.1. Fáza 1: iba plán.

affected:
  goals: ["G-001"] # cez FR1; bez zmeny
  actors_roles: ["ACT-Recepcna", "ROLE-SpravcaNavstev"]
  business_processes: ["BP-SpravaNavstev"] # kroky -01, -02, -04 (contract-mapping)
  business_services: ["BS-RezervaciaTerminu"]
  application_services: ["AS-SpravaRezervaciiAKlientov"]
  application_components: ["AC-ModulRezervaciiAKlientov", "AC-IntelliWell"] # modul ako súčasť IntelliWell
  interfaces_contracts: ["API-Reservations 0.2.1 (approved) – realizácia, bez zmeny"]
  data_objects: ["DO-Rezervacie (read, create)", "DO-EvidenciaKlientov (read; seed DR-03)"]
  technology_elements: ["ART-SoftverIntelliWell", "SSW-RuntimeDotNet", "SSW-DatabazovySystem (SQLite)", "TS-DatabazovaSluzba", "TS-HostovanieSystemu"]
  work_packages_deliverables: [] # D-003 Prihlasovanie personálu mimo slice (DR-23)
  decisions: ["ADR-001", "ADR-002", "ADR-003", "ADR-004", "ADR-005", "ADR-006", "ADR-007", "ADR-008 (Proposed)", "ADR-009 (Proposed)"]
  scenarios: ["SC-001-01 … SC-001-16", "contract example unauthenticated-missing-sub", "contract example invalid-request-time-offset"]
  tests: ["tests/IntelliWell.Tests (nové)"]

open_questions: ["DR-I01", "DR-I02", "DR-I03", "DR-I04", "DR-I05"]
```

## 3. Kontrola architektúry (AGENTS.md kap. 10, krok 3)

| Prvok | Zdroj | Dopad na kód |
|---|---|---|
| Application Structure | `components.yaml`: AC-ModulRezervaciiAKlientov je súčasťou AC-IntelliWell, vlastní DO-Rezervacie a DO-EvidenciaKlientov (ADR-006) | jeden projekt `src/IntelliWell` (= ART-SoftverIntelliWell); kód modulu iba v priečinku/namespace `ModulRezervaciiAKlientov`; hostiteľské prierezové časti v `Hosting` |
| Application Cooperation | `interactions.yaml`: INT-001 – INT-003 `relevance_for_fr1: false` | žiadna integrácia s inými modulmi |
| Application Usage | `service-realization.yaml`: BP-SpravaNavstev používa AS-SpravaRezervaciiAKlientov, vykonáva ACT-Recepcna | endpoint iba pre rolu `SpravcaNavstev` |
| Service Realization | AC-ModulRezervaciiAKlientov realizuje AS-SpravaRezervaciiAKlientov | endpoint je jediná operácia AS v slice |
| Deployment | `deployment.yaml`: ART-SoftverIntelliWell = ASP.NET Core na .NET 10, SQLite; rozhodnutie 0007: lokálne `dotnet run` | spustiteľný projekt; žiadny Dockerfile ani CI |
| Lifecycle | `lifecycles.yaml`: (none) → `vytvorena` (`created`) | `status: created`; iný stav nevzniká |

## 4. Generation plan po súboroch

### 4.1 Koreň repozitára

| Súbor | Obsah | Realizuje |
|---|---|---|
| `IntelliWell.slnx` | solution s 2 projektmi | – |
| `.gitignore` | `bin/`, `obj/`, `*.db`, `*.db-*` | – |

### 4.2 `src/IntelliWell/` – ART-SoftverIntelliWell (AC-IntelliWell, v prototype iba AC-ModulRezervaciiAKlientov)

| Súbor | Obsah | Contract / BR / QR |
|---|---|---|
| `IntelliWell.csproj` | `Microsoft.NET.Sdk.Web`, `net10.0`, `Nullable enable`, `TreatWarningsAsErrors`, `UserSecretsId` (pre `dotnet user-jwts`), PackageReference podľa ADR-008, kópia `config/seed-clients.json` do výstupu | ADR-002, ADR-008 |
| `Program.cs` | skladanie: `TimeProvider.System` (singleton), autentifikácia/autorizácia, correlation middleware, inicializácia schémy + seed, mapovanie endpointu; `public partial class Program` pre WebApplicationFactory | ADR-002, ADR-004 |
| `appsettings.json` | `ConnectionStrings:Reservations` = `Data Source=intelliwell-prototyp.db`; `SeedData:ClientsFile`; logovanie bez tela požiadaviek (žiadny HttpLogging) | ADR-003, DR-03, CON-003 |
| `appsettings.Development.json` | sekciu `Authentication:Schemes:Bearer` (ValidAudiences, ValidIssuer) dopĺňa `dotnet user-jwts`; podpisový kľúč je v user secrets | ADR-004, DR-23 |
| `Properties/launchSettings.json` | profil `http` (Development, napr. `http://localhost:5080`) – `dotnet user-jwts` z neho číta audience | DR-23 |
| `Hosting/CorrelationId.cs` | middleware `X-Correlation-ID`: platná hodnota `^[\x21-\x7E]{1,128}$` sa prevezme, inak (chýba / prázdna / neplatná) nový UUID; hlavička vo **všetkých** odpovediach vrátane 401/403 | DR-31, DR-C05, DR-C09; `components.parameters/headers.CorrelationId` |
| `Hosting/Problems.cs` | katalóg 9 typov z BR-012 (kód, HTTP, `title` = `reason`), zápis `application/problem+json` s `type` `urn:intelliwell:problem:<kód>`, `title`, `status`, `instance` = cesta; `detail` nikdy neobsahuje poznámku | ADR-005, DR-08, DR-C07, BR-010, BR-012, CON-003; `ProblemDetails` |
| `Hosting/Authentication.cs` | `AddJwtBearer` (konfigurácia z `Authentication:Schemes:Bearer`), `MapInboundClaims = false`, `RoleClaimType = "role"`, `NameClaimType = "sub"`; `OnTokenValidated`: chýbajúci/prázdny `sub` → zlyhanie autentifikácie (DR-C12); `OnChallenge` → 401 `unauthenticated` + `WWW-Authenticate: Bearer`; `OnForbidden` → 403 `forbidden`; policy `SpravcaNavstev` = `RequireRole("SpravcaNavstev")` | ADR-004, ADR-007, BR-011, BR-012 poz. 1–2, NFR4; `securitySchemes.bearerAuth`, odpovede 401, 403 |
| `ModulRezervaciiAKlientov/ReservationEndpoints.cs` | `POST /api/v1/reservations` (`createReservation`), `RequireAuthorization("SpravcaNavstev")`; volá parser → službu; 201 s telom `Reservation`, `Content-Type: application/json`; bez hlavičky `Location` | `paths./api/v1/reservations.post`, odpoveď 201; DR-31 |
| `ModulRezervaciiAKlientov/CreateReservationRequestParser.cs` | ručné čítanie JSON (`JsonDocument`), aby 400 malo tvar contractu: telo nie je JSON objekt; chýba/nie je reťazec `clientId` alebo `startTime`; `clientId` nie je UUID; `startTime` nie je RFC 3339 alebo nekončí `Z`; `note` je prítomné a nie je reťazec. Dĺžka poznámky sa tu **nekontroluje** | BR-012 poz. 3, NFR1, DR-C03; `CreateReservationRequest`, `UtcDateTime`, odpoveď 400 |
| `ModulRezervaciiAKlientov/ReservationRules.cs` | čisté funkcie: koniec = začiatok + 60 min (BR-004); začiatok > teraz (BR-006); otváracie hodiny – začiatok aj koniec prepočítané do `Europe/Bratislava`, začiatok ≥ 10:00, koniec ≤ 22:00 v ten istý miestny deň (BR-005, DR-18, DR-20); poznámka ≤ 500 Unicode code points (`EnumerateRunes`) (BR-008, DR-C08) | BR-004, BR-005, BR-006, BR-008 |
| `ModulRezervaciiAKlientov/CreateReservationService.cs` | poradie BR-012 poz. 4–9 v jednej transakcii: otvorenie transakcie (503) → klient existuje (BR-007) → BR-006 → BR-005 → BR-008 → prekrytie [začiatok, koniec) (BR-003, BR-009) → vloženie s `status = created`, `createdBy = sub`, `createdAt = TimeProvider.GetUtcNow()` → commit. Výsledok = rezervácia alebo kód odmietnutia | BR-001, BR-002, BR-003, BR-009, BR-010, BR-012, QR-001, QR-002, DR-28, ADR-001, ADR-009 |
| `ModulRezervaciiAKlientov/ReservationStore.cs` | `IReservationStore` + `SqliteReservationStore` (Microsoft.Data.Sqlite): `CREATE TABLE IF NOT EXISTS` pre `clients(id)` a `reservations(id, client_id, start_utc, end_utc, note, status, created_by, created_at)` – iba úložné polia z `data-objects.yaml content_in_fr1`; časy ako celé čísla (unix ms) pre porovnanie; `BEGIN IMMEDIATE` (ADR-009); `SqliteException` → `StorageUnavailableException` (DR-I04) | ADR-003, ADR-006, ADR-009, QR-001, QR-002; DO-Rezervacie, DO-EvidenciaKlientov |
| `ModulRezervaciiAKlientov/ClientSeeder.cs` | pri štarte načíta `seed-clients.json` a vloží UUID klientov (`INSERT OR IGNORE`) | DR-03, DR-30, DO-EvidenciaKlientov |
| `ModulRezervaciiAKlientov/Reservation.cs` | záznam rezervácie a JSON tvar `Reservation` (lowerCamelCase, `note` vynechané, ak je null; časy `yyyy-MM-ddTHH:mm:ss[.fffffff]Z`) | `Reservation`; DR-C07, DR-C11, DR-08 |
| `README.md` | spustenie: `dotnet run --project src/IntelliWell`, vydanie tokenu `dotnet user-jwts create --project src/IntelliWell --name jana --role SpravcaNavstev` (claim `sub` = meno), príklad `curl` | DR-23, rozhodnutie 0007 |

Aplikačný log: pri úspechu jeden riadok Information s ID rezervácie, bez poznámky a bez tela požiadavky (CON-003).

### 4.3 `config/`

| Súbor | Obsah | Realizuje |
|---|---|---|
| `config/seed-clients.json` | zoznam UUID klientov **bez akýchkoľvek osobných údajov**: `7c9e6679-7425-40de-944b-e07fc1f90ae7` (v scenároch „Eva Nováková“; meno sa neukladá). UUID `0b6f2a5e-3c1d-4f8a-9e2b-5d4c3b2a1f00` (Peter Horváth – SC-001-07, -16) v seed **nie je**. | DR-03, DR-30, DR-I05 |

### 4.4 `tests/IntelliWell.Tests/`

| Súbor | Obsah |
|---|---|
| `IntelliWell.Tests.csproj` | `net10.0`, `OutputType Exe`, balíky podľa ADR-008, `ProjectReference` na `src/IntelliWell` |
| `Infrastructure/IntelliWellFactory.cs` | `WebApplicationFactory<Program>`: samostatný dočasný SQLite súbor pre každý test; `ManualTimeProvider` nastavený na 2026-11-25T11:00:00Z (= 12:00 Europe/Bratislava, Background US-001); `PostConfigure<JwtBearerOptions>` s testovacím symetrickým kľúčom, issuerom a audience; zachytávanie logov (`ILoggerProvider`) pre CON-003; dekorátor úložiska s prepínačom dostupnosti (SC-001-15) |
| `Infrastructure/ManualTimeProvider.cs` | podtrieda `TimeProvider` s nastaviteľným `GetUtcNow()` (bez balíka) |
| `Infrastructure/TestTokens.cs` | podpis JWT cez `JsonWebTokenHandler`: `Jana`/`Maria` (`role=SpravcaNavstev`, `sub=jana`/`maria`), `Tomas` (`role=PoskytovatelDoplnkovejSluzby` – testovacia hodnota, I-07), token bez `sub`, token bez `role` |
| `Infrastructure/ToggleableReservationStore.cs` | dekorátor `IReservationStore`; vo vypnutom stave hodí `StorageUnavailableException` (SC-001-15) |
| `Infrastructure/ReservationDbProbe.cs` | priame čítanie testovacej SQLite databázy (počet rezervácií v intervale) – overenie „termín zostane voľný“ / „existuje iba pôvodná rezervácia“ |
| `Acceptance/CreateReservationScenarios.cs` | acceptance tests – 1 test = 1 scenár, `[Trait("Scenario", "SC-001-xx")]` (tabuľka 5.1) |
| `Contract/CreateReservationContractTests.cs` | contract tests voči approved 0.2.1, `[Trait("Contract", "API-Reservations 0.2.1")]` (tabuľka 5.2) |

## 5. Pokrytie testami

Aktuálny čas vo všetkých testoch: 2026-11-25 12:00 Europe/Bratislava (11:00Z). „Eva“ = seed UUID 7c9e6679-…, „Peter“ = 0b6f2a5e-… (mimo seed).

### 5.1 Acceptance tests (US-001.feature)

| Test (Trait) | Given (príprava) | When | Then (asserty) |
|---|---|---|---|
| SC-001-01 | token Jana | POST Eva, `2026-12-01T13:00:00Z`, note „prvá návšteva“ | 201; `status=created`, `endTime=…14:00:00Z`, `note`, `createdBy=jana`, `createdAt=2026-11-25T11:00:00Z` (QR-001); v DB práve 1 rezervácia v termíne a nová požiadavka na ten istý termín → 409 (BR-002) |
| SC-001-02 | Jana | Eva, `2026-12-02T10:00:00Z`, bez note | 201; pole `note` v tele chýba; `endTime=…11:00:00Z` |
| SC-001-03 | Jana | Eva, `2026-12-03T20:00:00Z` (21:00 miestne) | 201; `endTime=2026-12-03T21:00:00Z` (22:00 miestne) |
| SC-001-04 | uložená rezervácia 13:00Z | Eva, `2026-12-01T13:00:00Z` | 409 `slot-occupied`, `title` „termín obsadený“; v termíne iba 1 (pôvodná) rezervácia; problem details bez návrhu iného termínu |
| SC-001-05 | uložená rezervácia 13:00Z | Eva, `2026-12-01T13:30:00Z` | 409 `slot-occupied` |
| SC-001-06 | Jana a Mária | 2 súbežné požiadavky `2026-12-04T15:00:00Z` (`Task.WhenAll`, spoločný štart) | práve jedna 201 a jedna 409 `slot-occupied`; v DB práve 1 rezervácia v termíne |
| SC-001-07 | Jana | Peter, `2026-12-01T15:00:00Z` | 422 `client-not-found` |
| SC-001-08 | Jana | Eva, `2026-12-01T20:30:00Z` (21:30) | 422 `outside-opening-hours` |
| SC-001-09 | Jana | Eva, `2026-12-01T08:30:00Z` (09:30) | 422 `outside-opening-hours` |
| SC-001-10 | Jana | Eva, `2026-11-25T10:00:00Z` (11:00) | 422 `start-not-in-future` |
| SC-001-11 | Jana | Eva, `2026-12-01T17:00:00Z`, note 501 znakov | 422 `note-too-long`; telo odpovede neobsahuje text poznámky |
| SC-001-12 | token Tomáš (iná rola) | Eva, `2026-12-01T18:00:00Z` | 403 `forbidden`; termín v DB voľný |
| SC-001-13 | bez tokenu | Eva, `2026-12-01T19:00:00Z` | 401 `unauthenticated`, `WWW-Authenticate: Bearer`; termín v DB voľný |
| SC-001-14 | uložená rezervácia 13:00Z | Eva, `2026-12-01T14:00:00Z` | 201; `endTime=…15:00:00Z` (DR-19) |
| SC-001-15 | úložisko vypnuté | Eva, `2026-12-05T13:00:00Z` | 503 `service-unavailable`; po zapnutí: v DB 0 rezervácií v termíne a rovnaká požiadavka → 201 |
| SC-001-16 | Jana | Peter, `2026-11-25T08:00:00Z`, note 501 | 422 iba `client-not-found` |

### 5.2 Contract tests

| Test (Trait) | Overuje | Zdroj |
|---|---|---|
| `unauthenticated-missing-sub` (`ContractExample`) | platný token s `role=SpravcaNavstev`, bez `sub` (a s prázdnym `sub`) → 401 `unauthenticated` | ADR-007 / DR-C12, BR-012 poz. 1 |
| `invalid-request-time-offset` (`ContractExample`) | `startTime` `2026-12-01T14:00:00+01:00` → 400 `invalid-request` | DR-C03 |
| 400 – tvar vstupu (Theory) | telo nie je JSON; chýba `clientId`; chýba `startTime`; `clientId` nie je UUID; `startTime` nie je dátum; `note` nie je reťazec | NFR1, BR-012 poz. 3 |
| Token bez claimu `role` | 403 `forbidden` | contract 403, ADR-007 |
| Poradie BR-012 poz. 1–4 | bez tokenu + neplatné telo → 401; iná rola + neplatné telo → 403; neplatné telo + vypnuté úložisko → 400; vypnuté úložisko + neexistujúci klient → 503 | BR-012 (poz. 5–9 pokrýva SC-001-16) |
| Poradie BR-012 poz. 6–9 | minulý začiatok + mimo hodín → `start-not-in-future`; mimo hodín + note 501 → `outside-opening-hours`; note 501 + obsadený termín → `note-too-long` | BR-012 |
| DR-28 neidempotentnosť | rovnaká úspešná požiadavka 2× → 201, potom 409 | DR-28 |
| Poznámka 500 code points vrátane znakov mimo BMP (napr. emoji) | 201 (limit sa počíta v code points, nie UTF-16) | BR-008, DR-C08 |
| Correlation ID | platná hodnota sa vráti; chýbajúca, prázdna, s medzerou, 129 znakov → vráti sa UUID, požiadavka sa neodmietne; hlavička je v 201, 400, 401, 403, 409, 422, 503 | DR-31, DR-C05, DR-C09 |
| Tvar problem details | `Content-Type: application/problem+json`, `type` URN, `title` = `reason` BR-012, `status` = HTTP kód, `instance` = `/api/v1/reservations` | DR-08, DR-C07 |
| Tvar 201 | `Content-Type: application/json`; povinné polia `Reservation`; časy končia `Z` | `Reservation` |
| CON-003 | text poznámky sa nevyskytuje v žiadnom zachytenom logu (201 aj 422) ani v problem details | CON-003, DR-27 |

## 6. NuGet balíky (ADR-008 – Proposed)

| Projekt | Balík | Verzia |
|---|---|---|
| src/IntelliWell | Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.15 |
| src/IntelliWell | Microsoft.Data.Sqlite | 10.0.15 (transitívne SQLitePCLRaw.bundle_e_sqlite3; presnú verziu vypíše fáza 2) |
| tests | Microsoft.AspNetCore.Mvc.Testing | 10.0.15 |
| tests | Microsoft.NET.Test.Sdk | 18.12.0 |
| tests | xunit.v3 | 4.0.3 |
| tests | xunit.runner.visualstudio | 4.0.1 |

Žiadny iný balík. Tokeny v testoch: `Microsoft.IdentityModel.JsonWebTokens` (transitívne z JwtBearer).

## 7. Quality gates (fáza 2)

`dotnet build IntelliWell.slnx` (warnings as errors), `dotnet test IntelliWell.slnx`, `dotnet list package --include-transitive`
(záznam transitívnych verzií). Lint OpenAPI nie je potrebný – contract sa nemení.

## 8. DECISION REQUIRED

| ID | Otázka | Možnosti | Odporúčanie AI | Kam zapísať |
|---|---|---|---|---|
| DR-I01 | Ktoré NuGet balíky smie prototyp použiť? | A: minimálna sada (časť 6); B: + EF Core; C: + testovacie knižnice tretích strán | A | ADR-008 |
| DR-I02 | Ako vynútiť BR-003/BR-009 nad SQLite? | A: `BEGIN IMMEDIATE`; B: `SemaphoreSlim` v procese; C: trigger v DB | A | ADR-009 |
| DR-I03 | Kde je audit záznam QR-001 v SQLite? | A: stĺpce `created_by`, `created_at` v riadku rezervácie (jeden INSERT); B: samostatná audit tabuľka v tej istej transakcii | A | QR-001 / deployment.yaml (poznámka) |
| DR-I04 | Čo je „úložisko nedostupné“ (QR-002)? | A: každá `SqliteException` počas spracovania (vrátane vypršania zámku) → 503, bez opakovania; B: iba vybrané kódy (CANTOPEN, IOERR, BUSY, LOCKED, FULL, READONLY) → 503, ostatné → neošetrená chyba 500 mimo contractu | A | QR-002 (spresnenie) / Project policy |
| DR-I05 | Obsah a spôsob naplnenia seed dát klientov (DR-03) | A: `config/seed-clients.json` iba s UUID (bez osobných údajov), `INSERT OR IGNORE` pri štarte; B: aj fiktívne mená | A | data-objects.yaml (`seed_data_note`), deployment.yaml |

Implementácia sa nezačne pred „plán schvaľujem“ a odpoveďami na DR-I01 – DR-I05 (pri súhlase s odporúčaniami stačí uviesť „podľa odporúčania“).
