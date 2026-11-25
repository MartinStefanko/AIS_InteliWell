# Verification report – FR1 / API-Reservations 0.2.1 (úloha 0008)

| Pole | Hodnota |
|---|---|
| Režim | VERIFY_CHANGE |
| Change | FR1 Vytvorenie rezervácie (UC01) |
| Specification baseline | 0.3.1 (approved), architecture baseline B-01 – overené v `spec/manifest.yaml` |
| Approved contract | API-Reservations 0.2.1 – `spec/contracts/approved/approval.yaml` (approved, Martin Štefanko, 2026-11-25) |
| Realizácia | `src/IntelliWell/`, `tests/IntelliWell.Tests/`, `config/seed-clients.json` (commit 562ca62) |
| Prompt template | `spec/prompts/verify-change.md` v1.0 |
| Log | `spec/prompts/log/0008-2026-11-25-VERIFY_CHANGE.md` |
| Pripravila | AI (Claude Opus 5.5). Report nemení specification, contract, code, tests ani rozhodnutia. |

Výsledky: **PASS** – podmienka je splnená, **FAIL** – existuje konkrétny rozpor, **UNKNOWN** – chýba dôkaz.
Architektonické väzby sú overené iba voči textovým súborom a obrázkom B-01 (status draft, extrahované z docx).
Celý model v Sparx EA overený nebol.

## 1. Zhrnutie

| Výsledok | Počet |
|---|---|
| Quality gates Q-01 – Q-06 | 6 × PASS |
| Kontroly K-01 – K-59: PASS | 47 |
| Kontroly: FAIL | 7 – K-07, K-17, K-26, K-38, K-51, K-55, K-57 (K-17, K-38, K-57 sú známe odložené nálezy; K-55 obsahuje známe N-02b a nové V-04) |
| Kontroly: UNKNOWN | 5 – K-10, K-21, K-35, K-53, K-58 |
| not applicable | 4 (časť 6) |

Správanie kódu zodpovedá approved contractu 0.2.1 vo všetkých scenároch SC-001-01 – SC-001-16, v poradí BR-012
aj v security pravidlách. Build aj testy prešli. FAIL nálezy sú redakčné, metadátové alebo okrajové:

- **V-01** – metadáta approved contractu stále ukazujú na `proposed/` a majú `approved_by: null` (I-01 je opravené iba čiastočne).
- **V-02** – kód vráti 400 pre čas RFC 3339 s viac ako 7 desatinnými miestami sekundy, hoci ho schéma contractu prijíma.
- **V-03** – stĺpec auditu sa volá `created_at_utc_ticks`, ADR-009 (DR-I03) uvádza `created_at`.
- **V-04** – ADR-008 a ADR-009 majú stav Accepted, ale stále obsahujú text „Toto nie je rozhodnutie“.
- Známe odložené nálezy: chýbajúca traceability FR1 → kód → testy v `traceability.yaml`, N-01, N-02 (+ N-02a, N-02b), N-10.

Žiadny nález nemení business význam. Nová DECISION REQUIRED vzniká iba pri V-02 (pozri časť 7).

## 2. Dôkazy – quality gates

Prostredie: .NET SDK 10.0.404, Windows 11, 2026-11-25 18:23 (+01:00).

| # | Príkaz | Výsledok | Stav |
|---|---|---|---|
| Q-01 | `dotnet build IntelliWell.slnx -warnaserror` | Build succeeded, 0 Warning(s), 0 Error(s) | PASS |
| Q-02 | `dotnet build IntelliWell.slnx -warnaserror --no-incremental` (úplný rebuild) | Build succeeded, 0 Warning(s), 0 Error(s) | PASS |
| Q-03 | `dotnet test IntelliWell.slnx --no-build --logger trx` | Passed: 52, Failed: 0, Skipped: 0 (16 acceptance + 36 contract) | PASS |
| Q-04 | `dotnet test … --filter "Scenario~SC-001"` | Passed: 16, Failed: 0 | PASS |
| Q-05 | `dotnet test … --filter "ContractExample=unauthenticated-missing-sub"` | Passed: 2, Failed: 0 | PASS |
| Q-06 | `dotnet list IntelliWell.slnx package` | JwtBearer 10.0.15, Microsoft.Data.Sqlite 10.0.15, Mvc.Testing 10.0.15, Microsoft.NET.Test.Sdk 18.12.0, xunit.v3 4.0.3, xunit.runner.visualstudio 4.0.1 | PASS |

TRX súbor bol uložený iba do dočasného adresára mimo repozitára. `git status` po behu: zmenený iba
`spec/prompts/log/raw/prompts.jsonl` (hook), žiadny súbor v `src/`, `tests/` ani `spec/`.

Skript syntaktickej kontroly (Python 3.12, PyYAML 6.0.3, jsonschema 4.26.0, Draft 2020-12) bežal iba na čítanie,
mimo repozitára. Oficiálny OpenAPI validátor (napr. openapi-spec-validator, Redocly) nie je nainštalovaný a AI ho
neinštalovala.

## 3. Kontroly

### A. Syntax a schéma

| # | Kontrola | Výsledok | Dôkaz |
|---|---|---|---|
| K-01 | YAML syntax | PASS | 22/22 súborov `spec/**/*.yaml` sa načíta cez `yaml.safe_load` |
| K-02 | JSON syntax | PASS | `config/seed-clients.json`, `appsettings.json`, `appsettings.Development.json`, `launchSettings.json` |
| K-03 | Štruktúra OpenAPI 3.1 | PASS | `openapi: 3.1.0`; `info.title`, `info.version` 0.2.1, `paths`; 20/20 `$ref` sa rozriešia; `bearerAuth` definovaný; jediné `operationId` `createReservation` |
| K-04 | Príklady contractu voči schémam | PASS | 17/18 request príkladov platných voči `CreateReservationRequest`; `invalid-request-time-offset` schému zámerne porušuje (`pattern 'Z$'`); 19/19 response príkladov platných, `status` v problem details = HTTP kód. Obmedzenie: `FormatChecker` bez voliteľných knižníc nemusí overiť `format: date-time`/`uuid` |
| K-05 | Approved = proposed 0.2.1 | PASS | Po zjednotení `info.x-sdd.status` sú obe OpenAPI štruktúry zhodné; mapping sa líši iba v `status` (`diff`) |
| K-06 | Mapping ↔ OpenAPI | PASS | operácia POST `/api/v1/reservations` / `createReservation`; statusy [201, 400, 401, 403, 409, 422, 503] v oboch; `error_order` = enum `ProblemDetails.type` v rovnakom poradí |
| K-07 | Metadáta approved contractu | **FAIL** | V-01: `contract-mapping.yaml` r. 1 „Contract mapping – PROPOSED“, `contract.file: spec/contracts/proposed/…`, `contract.approved_by: null`; `openapi-reservations.yaml` `info.x-sdd.mapping: spec/contracts/proposed/contract-mapping.yaml`, `info.x-sdd.approved_by: null` |
| K-08 | Gherkin – štruktúra | PASS | `US-001.feature`: 16 scenárov s unikátnym ID, každý má Given/When/Then a práve jeden When |

### B. Identifikátory

| # | Kontrola | Výsledok | Dôkaz |
|---|---|---|---|
| K-09 | Existencia IDs v kóde, testoch, configu a approved contracte | PASS | 106 rôznych IDs (FR/NFR/QR/BR/CON/SC/DR/DR-C/DR-I/ADR/AC/AS/BS/BP/ROLE/ACT/DO/ART/INT/US/UC/OUT a čísla nálezov). Každé má definíciu v `spec/` alebo `reports/`. DR-I01 – DR-I05 sú v `reports/implementation-plan-0007.md` a v doplnení ADR-008/009. FR2 je ID zo zdrojového docx, uvedené iba ako mimo rozsahu |
| K-10 | Potvrdenie IDs modelu | UNKNOWN | V-05: G-001, G-002, G-005, OUT-001 a AS-SpravaRezervaciiAKlientov sú definované iba v `spec/drafts/id-mapping.yaml` (`status: draft`, `approved_by: null`, položky `proposed`). Approved súbory ich používajú. Zhodu s modelom v Sparx EA nemožno overiť z podkladov |

### C. Traceability FR1 → test

| # | Väzba | Výsledok | Dôkaz |
|---|---|---|---|
| K-11 | Goal → Requirement | PASS | `traceability.yaml`: G-001 → FR1, G-002 → NFR1, G-005 → NFR4; `requirements.yaml` FR1 `source_goals: [G-001]` |
| K-12 | Requirement → Process → Business Service | PASS | FR1 → BP-SpravaNavstev; BP-SpravaNavstev → BS-RezervaciaTerminu; kroky BP-SpravaNavstev-01, -02, -04 (`actors-processes.yaml`) |
| K-13 | Process/Requirement → Application Service → Component | PASS | BP-SpravaNavstev → AS-SpravaRezervaciiAKlientov; FR1 → AS; AS → AC-ModulRezervaciiAKlientov (`traceability.yaml`, `service-realization.yaml`) |
| K-14 | Component → Contract | PASS | `contract-mapping.yaml` `application_service`, `provided_by_component`; OpenAPI `info.x-sdd` |
| K-15 | Contract → Code | PASS (v kóde) | `ReservationEndpoints.cs:7-8, 24-25` (contract 0.2.1, `createReservation`); `CreateReservationService.cs:13-14` (AS, AC, BR-012) |
| K-16 | Code → Test | PASS | 16 testov s traitom `Scenario` = SC-001-01 – SC-001-16; trait `ContractExample` pre oba príklady bez scenára; trait `Contract` = „API-Reservations 0.2.1“ |
| K-17 | Traceability FR1 → kód → testy v authoritative `traceability.yaml` | **FAIL** (známy, odložený) | `traceability.yaml` nemá stopy na `src/` ani `tests/`; `missing_links` uvádza „FR1 → tests (IMPLEMENT_CHANGE)“. Položka „FR1 → approved contract (proposed API-Reservations 0.1.0 …)“ je zastaraná – contract 0.2.1 je schválený. Stopy sú iba v `reports/implementation-0007.md` (working). Odložené človekom v 0007 |

### D. Service Realization a Application Usage

| # | Kontrola | Výsledok | Dôkaz |
|---|---|---|---|
| K-18 | Service Realization | PASS | `service-realization.yaml` (approved): BS-RezervaciaTerminu ← BP-SpravaNavstev; podporuje AS-SpravaRezervaciiAKlientov, poskytuje AC-ModulRezervaciiAKlientov; FR1. Dôkaz: B-01-layered-tobe.png, B-01-application-usage-tobe.png, B-01-requirements-realization-tobe.png. `model_relationship_types_verified: false` – typy väzieb sú prečítané z obrázkov |
| K-19 | Application Usage | PASS | BP-SpravaNavstev používa AS-SpravaRezervaciiAKlientov, vykonáva ACT-Recepcna (rola ROLE-SpravcaNavstev); contract: `bearerAuth: [SpravcaNavstev]`, kód: policy `SpravcaNavstev` (`Authentication.cs:57-60`) |

### E. Hranice komponentov a závislosti

| # | Kontrola | Výsledok | Dôkaz |
|---|---|---|---|
| K-20 | Iba AC-ModulRezervaciiAKlientov | PASS | Kód je v `ModulRezervaciiAKlientov/` a v hostiteľskej vrstve `Hosting/` (autentifikácia, correlation ID, problem details). Grep `src/` nenašiel odkaz na moduly riadenia zariadení ani koordinácie saunových majstrov, `HttpClient` ani INT-001 – INT-003 |
| K-21 | Formálne allowed dependencies | UNKNOWN | `components.yaml`: `allowed_dependencies: null`, `uses_services: null` – nerozhodnuté. Pozorovaný kód nepridáva závislosť na inom AC (I-05 z plánu 0007) |
| K-22 | Vlastníctvo a prístup k Data Objects | PASS | DO-EvidenciaKlientov iba čítanie (`ClientExistsAsync`) + seed pri štarte (DR-03, DR-I05); DO-Rezervacie čítanie (`HasOverlapAsync`) a vytvorenie (`InsertAsync`) – zhodné s `contract-mapping.yaml data_access`; vlastník AC-ModulRezervaciiAKlientov (ADR-006) |
| K-23 | NuGet závislosti | PASS | Q-06 = zoznam ADR-008 (DR-I01 = A); žiadny ďalší priamy balík |

### F. Zhoda kódu s approved contractom

| # | Kontrola | Výsledok | Dôkaz |
|---|---|---|---|
| K-24 | Cesta, metóda, operationId | PASS | `ReservationEndpoints.cs:24-27` – `MapPost("/api/v1/reservations")`, `WithName("createReservation")`, `RequireAuthorization` |
| K-25 | Vstup – tvar (400 `invalid-request`) | PASS | `CreateReservationRequestParser.cs`: JSON objekt, `clientId` UUID, `startTime` s príponou `Z`, `note` reťazec; dĺžka poznámky sa tu nekontroluje (BR-008 → 422). Test `NeplatnyTvarVstupu_Vrati400` (12 prípadov), `InvalidRequestTimeOffset_…` |
| K-26 | Vstup – rozsah RFC 3339 | **FAIL** | V-02: regulárny výraz `CreateReservationRequestParser.cs:17` povolí najviac 7 desatinných miest (`\.\d{1,7}`); `DateTimeOffset` nepodporuje sekundu 60. Hodnota `2026-12-01T13:00:00.12345678Z` spĺňa schému `UtcDateTime` (`format: date-time`, `pattern: 'Z$'`), ale kód vráti 400. Žiadny scenár ani test tento prípad nepokrýva |
| K-27 | Výstup 201 `Reservation` | PASS | `Reservation.cs:22-44`: polia `id, clientId, startTime, endTime, note?, status, createdBy, createdAt`; `note` chýba, ak nebola zadaná; `status` = `created`; čas RFC 3339 so `Z`; `Content-Type: application/json`; bez hlavičky `Location`. Testy `Odpoved201_MaPovinnePoliaReservation`, SC-001-01, SC-001-02 |
| K-28 | Problem details – typy, titulky, statusy | PASS | `Problems.cs:18-26` porovnané s OpenAPI a BR-012 `order`: 9/9 kódov, HTTP statusov a znení `title` sa zhoduje; médium `application/problem+json`; `instance` = cesta. Poznámka V-07: testy berú očakávaný `type`/`title` z `ProblemCatalog` v produkčnom kóde, preto by samy rozdiel voči contractu neodhalili; zhoda je overená ručným porovnaním |
| K-29 | Poradie BR-012 (pozície 1–9) | PASS | 1–2 pred čítaním tela (`RequireAuthorization`), 3 parser, 4 `BeginWriteAsync` pred kontrolami, 5–9 v poradí v `CreateReservationService.cs:30-60`. Testy `Poradie1`, `Poradie2`, `Poradie3`, `Poradie4`, SC-001-16 (pozícia 5), `Poradie6`, `Poradie7`, `Poradie8` (8 pred 9) – PASS |
| K-30 | Security – autentifikácia a autorizácia | PASS | JWT bearer (`Authentication.cs`), `MapInboundClaims = false`, `RoleClaimType = role`; 401 s `WWW-Authenticate: Bearer` (SC-001-13, `TokenPodpisanyNeznamymKlucom_Vrati401`); prázdny alebo chýbajúci `sub` → 401 (DR-C12, `OnTokenValidated`; test `unauthenticated-missing-sub` ×2); iná rola → 403 (SC-001-12); chýbajúca rola → 403 (`TokenBezClaimuRole_Vrati403`); `createdBy` = claim `sub` (`ReservationEndpoints.cs:42`) |
| K-31 | X-Correlation-ID | PASS | `CorrelationId.cs`: formát `^[\x21-\x7E]{1,128}$`, inak nové UUID, požiadavka sa neodmietne; middleware je pred autentifikáciou, hlavička je v 201/400/401/403/409/422/503 (`CorrelationId_JeVKazdejOdpovediContractu`) |
| K-32 | Idempotencia (DR-28) | PASS | Opakovaná požiadavka → 409 `slot-occupied` (`OpakovanaRovnakaPoziadavka_PoUspechuVrati409`) |
| K-33 | QR-002 – nedostupné úložisko | PASS | Každá `SqliteException` → `StorageUnavailableException` → 503 (DR-I04); v kóde nie je slučka opakovania; SC-001-15 a `ZamknutaDatabaza_PoVyprsaniZamku_Vrati503` (reálna SQLite výnimka) – PASS |
| K-34 | QR-001 – audit | PASS | `created_by` = `sub`, čas vytvorenia v riadku rezervácie, iba pri úspechu (DR-26); SC-001-01 overuje odpoveď aj uložený záznam (`ReservationDbProbe.AuditOf`) |
| K-35 | Spustenie s tokenom z `dotnet user-jwts` (DR-23) | UNKNOWN | Testy používajú vlastný podpisový kľúč (`IntelliWellFactory.cs:57-62`). Tok z `README.md` (user-jwts, claimy `sub` a `role`) nebol overený: vyžaduje zápis do user secrets a `appsettings.Development.json`, čo VERIFY_CHANGE nesmie |

### G. Scenáre a invarianty

| # | Kontrola | Výsledok | Dôkaz |
|---|---|---|---|
| K-36 | Pokrytie SC-001-01 – SC-001-16 | PASS | Každý scenár = jeden test s traitom `Scenario`, vstupy zhodné s pomenovanými príkladmi contractu; všetky PASS (Q-03, Q-04). Tabuľka v časti 5 |
| K-37 | Príklady contractu bez scenára | PASS | `unauthenticated-missing-sub` (2 varianty) a `invalid-request-time-offset` – PASS |
| K-38 | Scenár pre token bez `sub` v US-001 | **FAIL** (známy N-10, odložený) | `US-001.feature` má 16 scenárov, DR-C12 nemá vlastný scenár. Správanie overuje iba contract test pomenovaného príkladu |
| K-39 | Invariant BR-003 (aj BR-009) | PASS | Prekrytie `start < $end AND $start < end` = polootvorený interval (DR-19); kontrola a vloženie v jednej transakcii `BEGIN IMMEDIATE` (ADR-009). Testy SC-001-04, -05, -06 (súbeh), -14 (nadväzujúci termín), DR-28 – PASS. Mutačnú kontrolu z 0007 (`BEGIN DEFERRED` → SC-001-06 FAIL) AI nezopakovala, lebo by menila kód |
| K-40 | Lifecycle DO-Rezervacie | PASS | Jediný stav `vytvorena` / `created`; pri odmietnutí rezervácia nevznikne – negatívne testy overujú `CountAll() == 0` alebo nezmenený počet; transakcia sa pri odmietnutí vráti (`DisposeAsync` → `ROLLBACK`) |

### H. CON-003

| # | Kontrola | Výsledok | Dôkaz |
|---|---|---|---|
| K-41 | Poznámka nie je v aplikačnom logu ani v odpovedi chyby | PASS | Kód loguje iba ID rezervácie a názov typu výnimky (`CreateReservationService.cs:77, 83-85`); nie je zapnuté HttpLogging; texty chýb parsera sú pevné. Test `Poznamka_NieJeVAplikacnomLoguAniVProblemDetails` zachytáva všetky úrovne (Trace) na cestách 201 a 422; SC-001-11 – PASS. Cestu 400 pokrýva iba čítanie kódu (žiadne logovanie) |

### I. Technology, deployment a ADR-001 – ADR-009

| # | Kontrola | Výsledok | Dôkaz |
|---|---|---|---|
| K-42 | ADR-001 súbeh | PASS | prvá uložená vyhrá, druhá 409; SC-001-06 |
| K-43 | ADR-002 .NET 10 / ASP.NET Core | PASS | `net10.0`, `Microsoft.NET.Sdk.Web`; SDK 10.0.404 |
| K-44 | ADR-003 SQLite | PASS | `Microsoft.Data.Sqlite`, `ConnectionStrings:Reservations` = súbor SQLite |
| K-45 | ADR-004 JWT bearer, claim `role` | PASS | K-30; tok user-jwts pozri K-35 (UNKNOWN) |
| K-46 | ADR-005 HTTP API, RFC 9457, RFC 3339 UTC | PASS | K-24 – K-28; výnimka v rozsahu RFC 3339 je V-02 (K-26) |
| K-47 | ADR-006 vlastníctvo dát | PASS | K-22 |
| K-48 | ADR-007 + doplnenie DR-C12 | PASS | K-30 |
| K-49 | ADR-008 balíky a lokálny `dotnet run` | PASS | Q-06; `launchSettings.json` profil `http`, `README.md`; bez Dockerfile a CI |
| K-50 | ADR-009 DR-I02, DR-I04, DR-I05 | PASS | `BEGIN IMMEDIATE` (`ReservationStore.cs:92`); `SqliteException` → 503; seed iba UUID, `INSERT OR IGNORE` (`ReservationStore.cs:77`, `config/seed-clients.json`) |
| K-51 | ADR-009 DR-I03 – názvy stĺpcov auditu | **FAIL** | V-03: ADR-009 doplnenie: „stĺpce `created_by` a `created_at`“; kód: `created_by` a `created_at_utc_ticks` (`ReservationStore.cs:65-66, 124`). Význam (kto, kedy, v riadku rezervácie) je zachovaný; rozdiel je v názve a v uložení času ako ticks |
| K-52 | Deployable Artifact ART-SoftverIntelliWell – rozhodnuté atribúty | PASS | `deployment.yaml`: form ASP.NET Core .NET 10 ✓, runtime SSW-RuntimeDotNet ✓, databáza SQLite ✓, user-jwts iba vývoj (`UserSecretsId`) ✓, CON-003 ✓ |
| K-53 | Deployment context – packaging, prostredie prototypu | UNKNOWN (známy I-04) | `deployment.yaml`: `packaging: null`, `required_by_implementation_task: null`, `open_questions` stále obsahuje prostredie a packaging; `technology-context.yaml`: `prototype_environment: null`. Rozhodnutie „lokálne `dotnet run`“ je iba v doplnení ADR-008. Rozpor medzi ADR-008 a deployment.yaml rieši človek (odložené v 0007) |

### J. Stav ADR a otvorených otázok

| # | Kontrola | Výsledok | Dôkaz |
|---|---|---|---|
| K-54 | Stav ADR-001 – ADR-009 | PASS | Sekcia `## Status` = Accepted vo všetkých 9 súboroch |
| K-55 | Redakčný súlad ADR | **FAIL** | V-04: ADR-008 a ADR-009 r. 3–4 „Draft pripravený AI … Toto nie je rozhodnutie. Stav mení iba človek.“ pri stave Accepted; ADR-007 r. 54 „0.3.0 (draft)“ (N-02b); `open-decisions.md` r. 9 vymenúva iba ADR-001 – ADR-007 |
| K-56 | Otvorené DR pre contract | PASS | `approval.yaml` `unresolved_decisions: []`; mapping `open_decisions: []`; `open-decisions.md`: „Žiadna DR nie je otvorená.“ DR-I01 – DR-I05 rozhodnuté (ADR-008/009) |
| K-57 | Verzie a komentáre v approved súboroch | **FAIL** (známe N-01, N-02, N-02a) | `lifecycles.yaml:10`, `components.yaml`, `data-objects.yaml`, `deployment.yaml`, `technology-context.yaml` `specification_version: "0.2.0"`; `requirements.yaml`, `business-rules.yaml`, `quality-requirements.yaml`, `traceability.yaml`, `contract-design-profile.yaml` „0.3.0 # návrh 0004“; manifest 0.3.1. Manifest: hodnoty z I-02 sú opravené (`approved_contracts`, `code`), ale komentáre „zatiaľ prázdny“, „src/ zatiaľ neexistuje“, „ADR-001 – ADR-007 Accepted“ a r. 5 „čakajú na kontrolu“ sú zastarané |
| K-58 | Otvorené otázky v specification relevantné pre FR1 | UNKNOWN | `deployment.yaml` a `technology-context.yaml` `open_questions` (packaging, prostredie – I-04); QR-001 doba uchovania záznamu (v 0007 mimo rozsahu prototypu); DR-06 produkčný DBMS – prototypu sa netýka. Uzavretie I-04 chýba |
| K-59 | Adresár `spec/contracts/approved copy/` (I-03) | PASS | Neexistuje; `spec/contracts/` obsahuje iba `approved/` a `proposed/` |

## 4. Nálezy

### Nové nálezy tejto verifikácie

| # | Závažnosť | Nález | Miesto | Návrh (rozhoduje človek) |
|---|---|---|---|---|
| V-01 | nízka (metadáta) | Approved contract odkazuje na `proposed/` a má `approved_by: null`. Hlavička mappingu je „PROPOSED“. I-01 je opravené iba v 1. riadku OpenAPI. Platný záznam o schválení je `approval.yaml` | `spec/contracts/approved/contract-mapping.yaml` r. 1, 8, 15; `openapi-reservations.yaml` `info.x-sdd.mapping`, `approved_by` | opraviť ručne v approved súboroch (zmena metadát, nie významu) |
| V-02 | nízka (okrajový vstup) | Platný čas RFC 3339 UTC s viac ako 7 desatinnými miestami (alebo sekundou 60) dostane 400, hoci ho schéma contractu prijíma | `CreateReservationRequestParser.cs:17, 70-79` | DECISION REQUIRED (časť 7) |
| V-03 | nízka | Názov stĺpca auditu `created_at_utc_ticks` ≠ `created_at` v ADR-009 (DR-I03) | `ReservationStore.cs:66, 124`; ADR-009 doplnenie | potvrdiť, či ADR uvádza záväzný názov; inak ponechať |
| V-04 | redakčná | ADR-008/009 so stavom Accepted obsahujú text „Toto nie je rozhodnutie“; `open-decisions.md` nespomína ADR-008/009 | ADR-008, ADR-009 r. 3–4; `open-decisions.md` r. 9 | redakčná úprava človekom |
| V-05 | informatívna (pre-existing) | G-001, G-002, G-005, OUT-001, AS-SpravaRezervaciiAKlientov sú definované iba v draft `id-mapping.yaml` | `spec/drafts/id-mapping.yaml` | potvrdiť IDs v modeli / povýšiť definície |
| V-06 | redakčná | `traceability.yaml missing_links` uvádza contract „proposed 0.1.0“ – zastarané | `traceability.yaml` r. 469 | zahrnúť do doplnenia traceability |
| V-07 | informatívna (kvalita testov) | Testy berú očakávaný `type`/`title` z `ProblemCatalog` v produkčnom kóde, nie z contractu. Zhoda je overená ručne (K-28) | `ReservationsApi.AssertProblemAsync` | voliteľne: v testoch použiť literály z contractu |
| V-08 | informatívna | Spustenie s tokenmi z `dotnet user-jwts` nie je overené testom ani v tejto verifikácii | `README.md`, `Authentication.cs` | ručný smoke test človekom |

### Známe odložené nálezy (stav k 2026-11-25)

| # | Pôvod | Stav teraz | Kontrola |
|---|---|---|---|
| Traceability FR1 → kód → testy | 0007, odložené človekom | stále chýba v `traceability.yaml` | K-17 FAIL |
| N-01 zastarané komentáre „návrh 0004“ / „čakajú na kontrolu“ | contract review 0.2.0/0.2.1 | stále prítomné; pribudli zastarané komentáre v manifeste po oprave I-02 | K-57 FAIL |
| N-02 `lifecycles.yaml` `specification_version: "0.2.0"` (+ N-02a 0.3.0 vs 0.3.1, N-02b pätička ADR-007) | contract review 0.2.1 | stále prítomné | K-57, K-55 FAIL |
| N-10 scenár pre token bez `sub` | contract review 0.2.1 | stále chýba; správanie overuje contract test | K-38 FAIL |
| I-04 packaging / prostredie prototypu v deployment.yaml | plán 0007 | stále `null`; rozhodnutie je iba v ADR-008 | K-53 UNKNOWN |

## 5. Traceability summary – FR1 → test

| Scenár | Pravidlá / požiadavky | Príklad contractu | Test (trait `Scenario`) | Výsledok |
|---|---|---|---|---|
| SC-001-01 | FR1, BR-001, BR-002, BR-004, BR-008, QR-001 | 201 SC-001-01 | `SC_001_01_SpravcaNavstevVytvoriRezervaciuNaVolnyTermin` | PASS |
| SC-001-02 | BR-008 | 201 SC-001-02 | `SC_001_02_RezervaciaBezPoznamkySaUlozi` | PASS |
| SC-001-03 | BR-005 | 201 SC-001-03 | `SC_001_03_TerminKonciaciPresnePriZatvoreniSaUlozi` | PASS |
| SC-001-04 | BR-003, BR-010 | 409 SC-001-04 | `SC_001_04_RezervaciaNaObsadenyTerminJeOdmietnuta` | PASS |
| SC-001-05 | BR-003, BR-010 | 409 SC-001-05 | `SC_001_05_RezervaciaNaCiastocnePrekryvajuciSaTerminJeOdmietnuta` | PASS |
| SC-001-06 | BR-003, BR-009, ADR-001, ADR-009 | 201 + 409 SC-001-06 | `SC_001_06_PriSubehuSaUloziIbaPrvaRezervacia` | PASS |
| SC-001-07 | BR-007, BR-010, NFR1 | 422 SC-001-07 | `SC_001_07_RezervaciaPreKlientaMimoEvidencieJeOdmietnuta` | PASS |
| SC-001-08 | BR-005, BR-010, NFR1 | 422 SC-001-08 | `SC_001_08_TerminPresahujuciZatvaraciuHodinuJeOdmietnuty` | PASS |
| SC-001-09 | BR-005, BR-010, NFR1 | 422 SC-001-09 | `SC_001_09_TerminZacinajuciPredOtvorenimJeOdmietnuty` | PASS |
| SC-001-10 | BR-006, BR-010, NFR1 | 422 SC-001-10 | `SC_001_10_TerminSoZaciatkomVMinulostiJeOdmietnuty` | PASS |
| SC-001-11 | BR-008, BR-010, CON-003 | 422 SC-001-11 | `SC_001_11_PrilisDlhaPoznamkaJeOdmietnuta` | PASS |
| SC-001-12 | NFR4, BR-011, ADR-004, ADR-007 | 403 SC-001-12 | `SC_001_12_OsobaBezRolySpravcuNavstevNemozeVytvoritRezervaciu` | PASS |
| SC-001-13 | NFR4, BR-011, ADR-004 | 401 SC-001-13 | `SC_001_13_NeprihlasenaOsobaNemozeVytvoritRezervaciu` | PASS |
| SC-001-14 | BR-003, BR-004, DR-19 | 201 SC-001-14 | `SC_001_14_RezervaciaNadvazujucaNaKoniecPredchadzajucejSaUlozi` | PASS |
| SC-001-15 | QR-002 | 503 SC-001-15 | `SC_001_15_PriNedostupnejEvidenciiRezervaciiSaRezervaciaNeulozi` | PASS |
| SC-001-16 | BR-012, BR-010, NFR1 | 422 SC-001-16 | `SC_001_16_PriViacerychPoruseniachSystemUvedieIbaPrvyDovod` | PASS |
| – (N-10) | DR-C12, BR-011 | 401 `unauthenticated-missing-sub` | `UnauthenticatedMissingSub_…` (ContractExample) ×2 | PASS |
| – | DR-C03, NFR1 | 400 `invalid-request-time-offset` | `InvalidRequestTimeOffset_…` (ContractExample) | PASS |

Reťazec: G-001 → FR1 → BP-SpravaNavstev (-01, -02, -04) → BS-RezervaciaTerminu / AS-SpravaRezervaciiAKlientov →
AC-ModulRezervaciiAKlientov → API-Reservations 0.2.1 `createReservation` → `ReservationEndpoints.cs` /
`CreateReservationService.cs` → testy vyššie. Posledné dva články nie sú zapísané v `traceability.yaml` (K-17).

## 6. Not applicable

Každú položku musí potvrdiť človek, ak ešte nie je potvrdená v uvedenom zdroji.

| Kontrola | Dôvod | Odkaz |
|---|---|---|
| Event contract / asynchrónna komunikácia | FR1 používa iba HTTP API; `events.specification: null` | `contract-design-profile.yaml` (DR-08), ADR-005 |
| Integrácie INT-001 – INT-003 | `relevance_for_fr1: false` | `interactions.yaml`; DR-25, DR-10 |
| Container image a CI | mimo rozsahu podľa ľudského rozhodnutia | ADR-008 doplnenie; log 0007 |
| GAP Analysis / Work Packages | úloha od nich nezávisí; FR1 je odôvodnená schváleným Requirementom | AGENTS.md kap. 6; `requirements.yaml` FR1 |

## 7. Decision required

```text
DECISION REQUIRED

Kontext:
FR1, specification 0.3.1, API-Reservations 0.2.1 (approved), schéma UtcDateTime, BR-012 pozícia 3,
CreateReservationRequestParser.cs; nález V-02.

Čo specification neurčuje alebo v čom si odporuje:
Contract prijíma každý čas RFC 3339 s príponou Z, ale neurčuje presnosť; kód odmietne viac ako 7 desatinných
miest sekundy a sekundu 60 kódom 400.

Možnosti:
A. Ponechať kód a v ďalšej verzii contractu (DESIGN_CONTRACT) spresniť presnosť času (napr. najviac 7 desatinných
   miest, bez sekundy 60). Kód sa nemení.
B. Upraviť kód (IMPLEMENT_CHANGE), aby prijal ľubovoľný počet desatinných miest. Treba rozhodnúť, či sa nadbytočné
   číslice orežú alebo zaokrúhlia, a čo so sekundou 60.
C. Prijať ako známe obmedzenie prototypu bez zmeny contractu aj kódu.

Odporúčanie AI:
A – recepčná zadáva začiatok na minúty; spresnenie contractu odstráni rozpor bez zmeny správania.

Otázka pre človeka:
Ktorá možnosť platí pre presnosť času startTime?

Kam zapísať rozhodnutie:
Contract (a contract-design-profile.yaml http.date_time_input)
```

Ostatné nálezy (V-01, V-03 – V-08 a známe odložené nálezy) nie sú DECISION REQUIRED. Ide o redakčné úpravy alebo
potvrdenia, ktoré patria človeku alebo do ďalšieho PREPARE_SPEC. Žiadny draft ADR nevznikol.
