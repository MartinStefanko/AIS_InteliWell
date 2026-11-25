# Verification report – FR1 + FR1-UI po change experimente (úloha 0011)

| Pole | Hodnota |
|---|---|
| Režim | VERIFY_CHANGE |
| Change | FR1 + FR1-UI (change experiment, bod E) |
| Specification baseline | 0.4.0 (approved), architecture baseline B-01 – overené v `spec/manifest.yaml` |
| Approved contract | API-Reservations 0.2.1 – `spec/contracts/approved/approval.yaml` (approved) |
| Realizácia | `src/IntelliWell/` (vrátane `wwwroot/`), `tests/IntelliWell.Tests/`, `tests/ui/`, `config/` – stav commitu f0d7725 |
| Porovnávací bod | commit 1e6d09e („SDD D: verifikácia FR1 (0008) … bod D uzavretý“) – pozri K-46 |
| Log | `spec/prompts/log/0011-2026-11-25-VERIFY_CHANGE.md` |
| Pripravila | AI (Claude Opus 5.5). Report nemení specification, contract, code, tests ani rozhodnutia. |

Výsledky: **PASS** – podmienka je splnená, **FAIL** – existuje konkrétny rozpor, **UNKNOWN** – chýba dôkaz.
Architektonické väzby sú overené iba voči textovým súborom a obrázkom B-01. Celý model v Sparx EA overený nebol.

## 1. Zhrnutie

| Výsledok | Počet |
|---|---|
| Quality gates Q-01 – Q-07 | 7 × PASS |
| Kontroly K-01 – K-46: PASS | 35 |
| Kontroly: FAIL | 7 – K-13, K-14, K-39, K-40, K-41, K-43, K-46 (všetky známe alebo redakčné; žiadny nemení správanie) |
| Kontroly: UNKNOWN | 4 – K-08, K-29, K-37, K-45 |
| not applicable | 4 (časť 6) |

Hlavné závery:

- **Build a testy:** build bez warningov; `dotnet test` 86/86 (pôvodných 52/52 + 34 UI); `node --test` 44/44.
- **Contract sa nezmenil:** approved OpenAPI a mapping sa od commitu 1e6d09e líšia iba v 2 + 2 metadátových kľúčoch
  (oprava V-01). Ostatný textový rozdiel je iba formátovanie YAML (úvodzovky, zápis zoznamov). `approval.yaml` je bez zmeny.
- **API kód sa nezmenil:** `ModulRezervaciiAKlientov/`, `Hosting/`, `*.csproj`, `config/` a pôvodné testy (Acceptance,
  Contract, Infrastructure) majú voči 1e6d09e prázdny diff. Jediná zmena hostiteľa sú 4 riadky v `Program.cs`.
- **UI** je iba konzument contractu 0.2.1 a je súčasťou ART-SoftverIntelliWell. Nepridáva závislosť medzi komponentmi.
- **SC-002-01 – SC-002-10:** každý scenár má automatický test (PASS). Ručný protokol má dôkaz z prehliadača iba
  pre časť scenárov (4 PASS / 0 FAIL / 19 UNKNOWN) – K-29 UNKNOWN.
- **Známe nálezy:** F-01 a F-03 (odložené), DR-0914 (medzera modelu), V-02 (akceptované obmedzenie DR-V02 = C)
  sú stále prítomné. Pribudli V-09 (FR1-UI → kód/testy chýba v `traceability.yaml`), V-10 (zastarané komentáre
  „draft“) a V-11 (zmena `appsettings.Development.json` nie je v implementation reporte).
- **Nová DECISION REQUIRED nevzniká.**

## 2. Dôkazy – quality gates

Prostredie: .NET SDK 10.0.404, Node.js v22.14.0, Windows 11, 2026-11-25 ~19:50 (+01:00).

| # | Príkaz | Výsledok | Stav |
|---|---|---|---|
| Q-01 | `dotnet build IntelliWell.slnx -warnaserror --no-incremental --artifacts-path bin/verify-0011` | Build succeeded, 0 Warning(s), 0 Error(s) | PASS |
| Q-02 | `dotnet test IntelliWell.slnx --no-build --artifacts-path bin/verify-0011 --logger trx` | Passed: 86, Failed: 0, Skipped: 0 | PASS |
| Q-03 | `… --filter "Requirement!=FR1-UI"` (pôvodné testy FR1) | Passed: 52, Failed: 0 | PASS |
| Q-04 | `… --filter "Requirement=FR1-UI"` | Passed: 34, Failed: 0 | PASS |
| Q-05 | `… --filter "Scenario~SC-001"` / `… --filter "Scenario~SC-002"` | 16/16 PASS / 24/24 PASS | PASS |
| Q-06 | `node --test tests/ui/*.test.mjs` | tests 44, suites 10, pass 44, fail 0 | PASS |
| Q-07 | `dotnet list IntelliWell.slnx package` | JwtBearer 10.0.15, Microsoft.Data.Sqlite 10.0.15, Mvc.Testing 10.0.15, Microsoft.NET.Test.Sdk 18.12.0, xunit.v3 4.0.3, xunit.runner.visualstudio 4.0.1 – zhodné s 0008 | PASS |

Poznámky k behu:

- Prvý pokus o build do štandardného `bin/` zlyhal (MSB3027, 0 warnings / 27 errors): súbor
  `src/IntelliWell/bin/Debug/net10.0/IntelliWell.exe` držal bežiaci proces `IntelliWell` (PID 11372, spustený 19:44:38,
  pravdepodobne server z ručného testu). Je to stav prostredia, nie chyba kódu. AI proces nezastavila.
  Build a testy preto bežali s `--artifacts-path bin/verify-0011` (adresár je v `.gitignore`, po behu bol zmazaný).
- Pokus s artifacts mimo repozitára dal 85/86: test `Skript_PokryvaPresneTypyProblemovZApprovedContractu` hľadá
  `IntelliWell.slnx` smerom nahor od `bin/` a mimo repozitára ho nenájde. V repozitári prešiel. Nejde o chybu UI.
- Vedľajší účinok: neúspešný `--no-incremental` build vyčistil časť výstupov v `src/IntelliWell/bin/` a
  `tests/IntelliWell.Tests/bin/` (ignorované súbory, nie zdrojové). Po zastavení bežiacej aplikácie ich obnoví
  bežný `dotnet build`.
- TRX súbory sú iba v dočasnom adresári mimo repozitára. `git status` po behu: zmenený iba
  `spec/prompts/log/raw/prompts.jsonl` (hook) a pribudli súbory tejto úlohy (report, log).
- JS syntax: `node --experimental-default-type=module --check` pre `app.js`, `bratislava-time.js`, `messages.js` – exit 0.

## 3. Kontroly

### A. Syntax a schéma

| # | Kontrola | Výsledok | Dôkaz |
|---|---|---|---|
| K-01 | YAML syntax | PASS | 28/28 súborov `spec/**/*.yaml` sa načíta cez `yaml.safe_load` (PyYAML) |
| K-02 | JSON syntax | PASS | `config/seed-clients.json`, `appsettings.json`, `appsettings.Development.json` (zmenený v f0d7725), `launchSettings.json` |
| K-03 | Štruktúra OpenAPI 3.1 a príklady | PASS | Approved OpenAPI je významovo zhodný s verziou overenou v 0008 (K-03, K-04 tam PASS) – pozri K-04 |
| K-04 | Approved contract bez zmeny obsahu | PASS | Porovnanie načítaných YAML štruktúr voči 1e6d09e: `openapi-reservations.yaml` – iba `info.x-sdd.mapping` (proposed → approved) a `info.x-sdd.approved_by` (null → Martin Štefanko); `contract-mapping.yaml` – iba `contract.file` a `contract.approved_by`; `approval.yaml` – 0 rozdielov. Textový diff (≈ 240 riadkov) je iba formátovanie |
| K-05 | Gherkin – štruktúra | PASS | `US-001.feature` 17 scenárov (SC-001-01 – SC-001-17), `US-002.feature` 10 scenárov (SC-002-01 – SC-002-10); ID unikátne, každý scenár má Given/When/Then a práve jeden When |
| K-06 | Syntax UI (HTML/JS) | PASS | 3/3 JS moduly prejdú `node --check`; `bratislava-time.js` a `messages.js` načíta `node --test`; `index.html` a `app.js` načítal prehliadač v ručnom teste (SC-002-01 PASS) |

### B. Identifikátory

| # | Kontrola | Výsledok | Dôkaz |
|---|---|---|---|
| K-07 | Existencia IDs | PASS | 151 rôznych IDs v kóde, UI, testoch, configu, approved contracte, stories, traceability, ADR-010, components/deployment/technology a ručnom protokole. 147 má definíciu v `spec/` alebo `reports/`. FR2, FR5, UC02, UC05 sú ID zo zdrojového docx, uvedené iba v `requirements.yaml` `out_of_scope` |
| K-08 | Potvrdenie IDs voči modelu | UNKNOWN | G-001, G-002, G-005, OUT-001 sú definované v `requirements.yaml` (DR-0907), ale s `model_confirmed: null`. Zhodu so Sparx EA nemožno overiť z podkladov (pôvodne V-05 – definícia je už v approved súbore) |

### C. Traceability FR1 a FR1-UI → kód → testy

| # | Väzba | Výsledok | Dôkaz |
|---|---|---|---|
| K-09 | FR1 → contract → kód → testy v `traceability.yaml` | PASS | `traceability.yaml` r. 470–630: AS / FR1 → API-Reservations 0.2.1 → `ReservationEndpoints.cs`, `CreateReservationService.cs`, … → SC-001-01 – SC-001-16 → testy `CreateReservationScenarios.*`; contract testy. 0008 K-17 je vyriešené |
| K-10 | SC-001-17 → test | PASS | `traceability.yaml` r. 633–640: SC-001-17 → `UnauthenticatedMissingSub_…` (2 varianty) – PASS. Trait `Scenario=SC-001-17` chýba (preto `Scenario~SC-001` = 16) – odložené DR-0908 |
| K-11 | FR1-UI → špecifikácia | PASS | `traceability.yaml` r. 643–700: FR1 → FR1-UI, ROLE-SpravcaNavstev → FR1-UI, FR1-UI → AS-SpravaRezervaciiAKlientov, ART-SoftverIntelliWell, ADR-010, US-002, SC-002-01..SC-002-10 |
| K-12 | FR1-UI → kód → testy (v kóde, testoch a reporte) | PASS | Komentáre `Program.cs`, `app.js`, `messages.js` (FR1-UI, ADR-010, DR-xxxx); trait `Requirement=FR1-UI` (34 testov) a `Scenario=SC-002-01` – `SC-002-10`; mená `node --test` testov obsahujú SC-002-xx; tabuľka v `reports/implementation-0010.md` časť 3 |
| K-13 | FR1-UI → kód → testy v authoritative `traceability.yaml` | **FAIL** | V-09: `traceability.yaml` nemá stopy na `src/IntelliWell/wwwroot/`, `UiStaticPageTests.cs` ani `tests/ui/`. `missing_links` r. 704 stále uvádza „FR1-UI → code, tests (IMPLEMENT_CHANGE po ADR-010)“. Režim 0010 specification meniť nesmel – doplnenie patrí do PREPARE_SPEC |
| K-14 | FR1-UI → Application Interface | **FAIL** (známe, akceptované DR-0914) | `components.yaml` `application_interfaces: []`; `deployment.yaml` `model_element: null`; `missing_links` r. 703. Views B-01 zobrazujú iba „Rozhranie saunového majstra“. Človek to zapísal ako známu medzeru modelu; FR1-UI sa viaže na AC-ModulRezervaciiAKlientov a AS-SpravaRezervaciiAKlientov |

### D. Service Realization a Application Usage

| # | Kontrola | Výsledok | Dôkaz |
|---|---|---|---|
| K-15 | Service Realization | PASS | `service-realization.yaml` 0.4.0: BS-RezervaciaTerminu ← BP-SpravaNavstev; AS-SpravaRezervaciiAKlientov (nová sekcia `application_services`, DR-0907) poskytuje AC-ModulRezervaciiAKlientov; FR1, FR1-UI. Typy väzieb sú prečítané z obrázkov |
| K-16 | Application Usage | PASS | BP-SpravaNavstev používa AS-SpravaRezervaciiAKlientov (ACT-Recepcna / ROLE-SpravcaNavstev). UI posiela token v `Authorization: Bearer` (`app.js:39-43`); oprávnenie vynucuje API (`bearerAuth: [SpravcaNavstev]`, testy SC-001-12, SC-001-13). Rozhranie, cez ktoré usage prebieha, v modeli chýba – K-14 |

### E. Hranice komponentov a závislosti

| # | Kontrola | Výsledok | Dôkaz |
|---|---|---|---|
| K-17 | UI je súčasťou ART-SoftverIntelliWell | PASS | `Program.cs:17-20` `UseDefaultFiles()` + `UseStaticFiles()` v tej istej aplikácii; `deployment.yaml` `user_interface.part_of_artifact: true`, `separate_client_artifact: false`; žiadny nový projekt v `IntelliWell.slnx` |
| K-18 | Žiadna nová závislosť medzi komponentmi | PASS | `git diff 1e6d09e HEAD` je prázdny pre `ModulRezervaciiAKlientov/`, `Hosting/`, `*.csproj`, `config/`. UI volá jedine `/api/v1/reservations` (`app.js:16`, jediný `fetch` na r. 79), bez absolútnych URL (`http(s)://` sa v `index.html` ani v JS nenachádza). `components.yaml` `allowed_dependencies.application_components: []` (DR-0906) |
| K-19 | Žiadny nový balík ani frontend build | PASS | Q-07 = zoznam ADR-008; `package.json` v repozitári neexistuje; Node.js iba ako vývojový nástroj na `node --test` (DR-1001, ADR-008 doplnenie) |
| K-20 | Statické súbory nemenia poradie API (BR-012) | PASS | Middleware je pred `UseAuthentication()` a obsluhuje iba existujúce súbory vo `wwwroot`. Pôvodných 52/52 testov vrátane `Poradie1`, `Poradie2` (401/403) – PASS (Q-03) |

### F. Zhoda kódu a UI s approved contractom 0.2.1

| # | Kontrola | Výsledok | Dôkaz |
|---|---|---|---|
| K-21 | API kód = stav overený v 0008 | PASS | API kód sa od 1e6d09e nezmenil (K-18). Závery 0008 K-24, K-25, K-27 – K-34 platia; Q-03 52/52 |
| K-22 | Požiadavka UI zodpovedá `CreateReservationRequest` | PASS | `app.js:31-44`: `POST /api/v1/reservations`, `Content-Type: application/json`, telo `clientId` (UUID z poľa), `startTime` (UTC so `Z` z `toUtcIso`, napr. `2026-12-01T13:00:00Z`), `note` iba ak nie je prázdna (pole je voliteľné). `X-Correlation-ID` UI neposiela – contract ho má voliteľný |
| K-23 | Spracovanie odpovedí UI | PASS | 201 → potvrdenie z `startTime`/`endTime` odpovede; `application/problem+json` → hláška podľa `type`; neznámy `type` → `title`; inak text SC-002-10 (DR-1002). 9 URN v `messages.js` = enum `ProblemDetails.type` approved contractu – test `Skript_PokryvaPresneTypyProblemovZApprovedContractu` číta priamo `spec/contracts/approved/openapi-reservations.yaml` – PASS |
| K-24 | UI netrafí obmedzenie V-02 | PASS | `toUtcIso` vracia čas bez desatinných miest sekundy (overené: `2026-12-01T14:00` → `2026-12-01T13:00:00Z`), čo parser prijíma (`CreateReservationRequestParser.cs:17`) |
| K-25 | ADR-010 Security v kóde UI | PASS | Grep `wwwroot/`: žiadne `innerHTML`/`outerHTML`/`insertAdjacentHTML`, `localStorage`/`sessionStorage`/`indexedDB`/`document.cookie`, `console.`, `setTimeout`/`setInterval`, `eval(`; výsledky iba cez `textContent` (`app.js:27`). Statické testy v `UiStaticPageTests.cs` – PASS |

### G. Scenáre a invarianty

| # | Kontrola | Výsledok | Dôkaz |
|---|---|---|---|
| K-26 | SC-001-01 – SC-001-16 | PASS | 16 testov s traitom `Scenario` – 16/16 PASS (Q-05); tabuľka v časti 5 |
| K-27 | SC-001-17 | PASS | Contract test `unauthenticated-missing-sub` (2 varianty) – PASS; trait scenára chýba (DR-0908, odložené) |
| K-28 | SC-002-01 – SC-002-10 – automatické testy | PASS | Každý scenár má ≥ 1 test s traitom `Scenario` v `UiStaticPageTests.cs` (24 testových prípadov, 24/24 PASS) a SC-002-01 – -07, -09, -10 aj `node --test` (44/44 PASS). Úroveň dôkazu podľa DR-0915: servírovanie, obsah, JS logika bez prehliadača |
| K-29 | SC-002 – ručný protokol v prehliadači | UNKNOWN | `reports/manual-test-protocol-US-002.md`: 4 PASS / 0 FAIL / 19 UNKNOWN. PASS: SC-002-01, SC-002-04, SC-002-05 (slot-occupied), DR-0913. Bez dôkazu z prehliadača: SC-002-02, -03, -05 (8 z 9 typov), -06, -07, -08, -09 (2×), -10, DR-0910, ADR-010 Security. Žiadna odchýlka vo vykonaných scenároch |
| K-30 | Invarianty BR-003, BR-009, lifecycle DO-Rezervacie | PASS | Kód modulu sa nezmenil; SC-001-04, -05, -06, -14 a DR-28 test – PASS. UI pravidlá neduplikuje (ADR-010) |
| K-31 | CON-003 v UI | PASS | Žiadne `console.` vo `wwwroot/` (K-25), test SC-002-08 – PASS; server: `Poznamka_NieJeVAplikacnomLoguAniVProblemDetails` – PASS. Ručný dôkaz SC-002-08 chýba (K-29) |

### H. ADR-001 – ADR-010, technology a deployment

| # | Kontrola | Výsledok | Dôkaz |
|---|---|---|---|
| K-32 | ADR-001 – ADR-007, ADR-009 | PASS | Kód, ktorý ich realizuje, sa nezmenil; závery 0008 K-42 – K-48, K-50 platia. V-03 vyriešené rozhodnutím DR-0905 – poznámka o `created_at_utc_ticks` v ADR-009 r. 53 |
| K-33 | ADR-008 + doplnenie DR-1001 | PASS | Q-07; Node.js v22.14.0 iba na `node --test`, bez npm balíkov (K-19) |
| K-34 | ADR-010 – rozhodnutia | PASS | Statická stránka vo `wwwroot` ✓, bez balíka a buildu ✓ (K-19), konzument 0.2.1 ✓ (K-22, K-23), Bearer token ✓, DR-0910 iba pamäť ✓ (K-25), seed UUID predvyplnené ✓ (test SC-002-01), DR-0911 Intl Europe/Bratislava nezávisle od pásma počítača ✓ (`node --test`), DR-0912 texty ✓, DR-0913 stránka bez prihlásenia ✓ (test + ručne), DR-0915 ✓ |
| K-35 | Deployment context ART-SoftverIntelliWell | PASS | `deployment.yaml` 0.4.0: `packaging` = `dotnet run` (I-04 vyriešené), `user_interface`, `configuration_files`, `required_by_implementation_task: true` (DR-0902); `launchSettings.json` profil http `http://localhost:5080`; `technology-context.yaml` `prototype_environment`. 0008 K-53 je vyriešené |
| K-36 | Spustenie s tokenom z `dotnet user-jwts` (0008 K-35) | PASS | Ručný protokol: token T-JANA z `dotnet user-jwts create … --role SpravcaNavstev`, rezervácia cez UI uložená (SC-002-01 PASS). `appsettings.Development.json` obsahuje `ValidIssuer: dotnet-user-jwts` a audience `http://localhost:5080` – iba vývoj (DR-23); podpisový kľúč je v user secrets, nie v repozitári. Pozri V-11 |
| K-37 | Podporované prehliadače UI | UNKNOWN | `technology-context.yaml` r. 104: „Podporované prehliadače UI – nerozhodnuté“. Ručný test prebehol v jednom prehliadači (podľa protokolu Chrome 131) |

### I. Stav ADR a otvorených otázok

| # | Kontrola | Výsledok | Dôkaz |
|---|---|---|---|
| K-38 | Stav ADR-001 – ADR-010 | PASS | `## Status` = Accepted v 10/10 súboroch; ADR-000 je šablóna (Proposed) |
| K-39 | F-01 – zastarané „ADR-010 (Proposed)“ | **FAIL** (známe, odložené v 0010) | `US-002.md:22`; `deployment.yaml:3, 33, 43, 67`; `technology-context.yaml:3, 36, 103`; `requirements.yaml:8, 140`; ADR-010 r. 3–4 a r. 38 („Stav ostáva Proposed“) pri stave Accepted. `deployment.yaml:67` a `technology-context.yaml:103` majú v `open_questions` „čaká na Accepted“ |
| K-40 | F-03 – SC-002-09 „návrh AI“ | **FAIL** (známe, odložené v 0010) | `US-002.feature:5` a `US-002.md:50` uvádzajú znenie SC-002-09 ako návrh AI; človek ho potvrdil v 0009 (log 0009, Ľudské rozhodnutie) |
| K-41 | Ostatné komentáre v approved súboroch | **FAIL** (redakčné) | V-10: „súbor je v stave draft“ / „schválenie zapíše človek“ pri `status: approved` – `components.yaml:4`, `deployment.yaml:4-5, 9`, `technology-context.yaml:4`, `traceability.yaml:6`, `requirements.yaml:8`; `deployment.yaml:62` `static_content` „vznikne v IMPLEMENT_CHANGE po schválení ADR-010“; `components.yaml` `code` komentár „UI … (IMPLEMENT_CHANGE po ADR-010)“; ADR-010 Consequences odkazuje na `spec/drafts/0009/deployment.yaml` namiesto `spec/technology/deployment.yaml` |
| K-42 | Otvorené DR | PASS | `open-decisions.md`: „Žiadna DR nie je otvorená“; DR-0901 – DR-0915, DR-1001, DR-1002 rozhodnuté s miestom zápisu; DR-V02 = C r. 13; DR-0908 odložené s miestom zápisu |
| K-43 | V-02 – presnosť `startTime` | **FAIL** (známe, akceptované DR-V02 = C) | Parser stále povolí najviac 7 desatinných miest (`CreateReservationRequestParser.cs:17`), schéma contractu viac. Človek to prijal ako známe obmedzenie prototypu (log 0008; `open-decisions.md:13`; `traceability.yaml:492`); spresnenie patrí do ďalšej verzie contractu. UI obmedzenie nespúšťa (K-24) |
| K-44 | `specification_version` podľa DR-0903 | PASS | Zmenené súbory 0.4.0; nezmenené ponechané (`data-objects.yaml` 0.2.0, `lifecycles.yaml` a `quality-requirements.yaml` 0.3.0) – zodpovedá pravidlu „verzia poslednej zmeny súboru“; manifest 0.4.0 approved |
| K-45 | Otvorené otázky specification relevantné pre FR1/FR1-UI | UNKNOWN | `technology-context.yaml` `open_questions`: podporované prehliadače (K-37); DR-0914 (doplnenie modelu mimo kroku); QR-001 doba uchovania záznamu a DR-06 produkčný DBMS – prototypu sa netýkajú (0008) |
| K-46 | Porovnávací bod `sdd-D-fr1` | **FAIL** | `git tag -l` je prázdny – tag `sdd-D-fr1` v lokálnom repozitári neexistuje. Zhrnutie v časti 8 je urobené voči commitu 1e6d09e, ktorého správa uvádza „bod D uzavretý“. Či tag mal ukazovať na tento commit, rozhodne človek |

## 4. Nálezy

### Nové nálezy tejto verifikácie

| # | Závažnosť | Nález | Miesto | Návrh (rozhoduje človek) |
|---|---|---|---|---|
| V-09 | nízka (traceability) | FR1-UI → kód → testy nie je v authoritative `traceability.yaml`; `missing_links` hovorí, že vznikne v IMPLEMENT_CHANGE | `traceability.yaml` r. 643–705 | v ďalšom PREPARE_SPEC doplniť stopy podľa `implementation-0010.md` časť 3 a odstrániť položku z `missing_links` |
| V-10 | redakčná | Komentáre „súbor je v stave draft“, „vznikne v IMPLEMENT_CHANGE“ a odkaz na `spec/drafts/0009/` v approved súboroch | K-41 | redakčná úprava spolu s F-01 |
| V-11 | informatívna | `appsettings.Development.json` sa v f0d7725 zmenil (sekcia `Authentication:Schemes:Bearer` od `dotnet user-jwts`). `implementation-0010.md` zmenu neuvádza – vznikla pri ručnom teste. Obsah zodpovedá DR-23 a `deployment.yaml configuration_files`; súbor stratil koncový znak nového riadku | `src/IntelliWell/appsettings.Development.json` | potvrdiť, že commit tejto vývojovej konfigurácie je zámerný |
| V-12 | informatívna | `node --test` (DR-1001) je zapísaný v ADR-008 a `open-decisions.md`, ale nie v `technology-context.yaml prototype_environment` | `technology-context.yaml` r. 28–36 | voliteľne doplniť ako vývojový nástroj |
| V-13 | informatívna (prostredie) | Test `Skript_PokryvaPresneTypyProblemovZApprovedContractu` závisí od umiestnenia `bin/` v repozitári (hľadá `IntelliWell.slnx` smerom nahor); pri `--artifacts-path` mimo repozitára zlyhá | `UiStaticPageTests.cs:249-258` | ponechať; prípadne zapísať do README |

### Známe nálezy (stav k 2026-11-25)

| # | Pôvod | Stav teraz | Kontrola |
|---|---|---|---|
| F-01 zastarané „ADR-010 (Proposed)“ | plán 0010, odložené človekom | stále prítomné (12 miest) | K-39 FAIL |
| F-03 SC-002-09 „návrh AI“ | plán 0010, odložené človekom | stále prítomné (2 miesta) | K-40 FAIL |
| DR-0914 Application Interface pre recepčnú | 0009, rozhodnuté ako známa medzera modelu | model B-01 sa nezmenil, `application_interfaces: []` | K-14 FAIL (akceptované) |
| V-02 presnosť `startTime` | 0008, DR-V02 = C | kód aj contract bez zmeny; obmedzenie platí, UI ho nespúšťa | K-43 FAIL (akceptované), K-24 PASS |
| DR-0908 trait SC-001-17 | 0009, odložené do ďalšej verzie contractu | trait chýba, správanie overuje contract test | K-27 PASS s poznámkou |
| 0008: K-17, I-04, V-01, V-03, V-04, N-01, N-02, N-10, K-35 | 0008 → 0009 | vyriešené (K-09, K-35, K-04, K-32, K-44, K-27, K-36) | PASS |

## 5. Traceability summary

### FR1 (API, nezmenené)

| Scenár | Test (trait `Scenario`) | Výsledok |
|---|---|---|
| SC-001-01 – SC-001-16 | `CreateReservationScenarios.SC_001_xx_*` (16 testov, mapovanie v `traceability.yaml` a v 0008 časť 5) | 16/16 PASS |
| SC-001-17 | `UnauthenticatedMissingSub_…` (ContractExample, 2 varianty) | PASS |

Reťazec: G-001 → FR1 → BP-SpravaNavstev → BS-RezervaciaTerminu / AS-SpravaRezervaciiAKlientov →
AC-ModulRezervaciiAKlientov → API-Reservations 0.2.1 `createReservation` → kód modulu → testy. Celý reťazec je
v `traceability.yaml`.

### FR1-UI

| Scenár | dotnet test (`UiStaticPageTests`) | node --test | Ručný protokol | API (US-001) |
|---|---|---|---|---|
| SC-002-01 | 3 testy – PASS | PASS | PASS | SC-001-01 |
| SC-002-02 | 1 (spoločný so SC-002-03) – PASS | PASS | UNKNOWN | – |
| SC-002-03 | 1 (spoločný so SC-002-02) – PASS | PASS | UNKNOWN | – |
| SC-002-04 | 1 (zhoda URN s contractom, spoločný so SC-002-05) – PASS | PASS | PASS | SC-001-04 |
| SC-002-05 | 2 testy (theory 9 prípadov + zhoda s contractom) – PASS | PASS | 1/9 PASS, 8 UNKNOWN | SC-001-xx |
| SC-002-06 | 2 – PASS | PASS | UNKNOWN (DR-0913 PASS) | SC-001-13 |
| SC-002-07 | 1 – PASS | PASS | UNKNOWN | SC-001-15 |
| SC-002-08 | 1 – PASS | – | UNKNOWN | CON-003 test |
| SC-002-09 | 1 theory – PASS | PASS | UNKNOWN | – |
| SC-002-10 | 1 – PASS | PASS | UNKNOWN | – |

Reťazec: FR1 → FR1-UI (ROLE-SpravcaNavstev) → AS-SpravaRezervaciiAKlientov → AC-ModulRezervaciiAKlientov
(Application Interface – medzera DR-0914) → ART-SoftverIntelliWell (`wwwroot/`, hostiteľská časť AC-IntelliWell
v `Program.cs`, DR-0906) → API-Reservations 0.2.1 → testy vyššie. Posledné články (kód, testy) nie sú v
`traceability.yaml` (K-13, V-09).

## 6. Not applicable

| Kontrola | Dôvod | Odkaz |
|---|---|---|
| Event contract / asynchrónna komunikácia | FR1 aj FR1-UI používajú iba HTTP API | `contract-design-profile.yaml` (DR-08), ADR-005 – potvrdené v 0008 |
| Integrácie INT-001 – INT-003 | `relevance_for_fr1: false`; UI ich nevolá (K-18) | `interactions.yaml`; potvrdené v 0008 |
| Container image a CI | mimo rozsahu | ADR-008 doplnenie; `deployment.yaml packaging_out_of_scope` |
| GAP Analysis / Work Packages | úloha od nich nezávisí; FR1 a FR1-UI sú odôvodnené approved Requirementmi | AGENTS.md kap. 6 – potvrdené v 0008 |

## 7. Decision required

Nová DECISION REQUIRED nevzniká. Otvorené položky sú už rozhodnuté alebo odložené človekom (F-01, F-03, DR-0914,
DR-V02, DR-0908). Na potvrdenie človekom (nie DR): V-09 – V-12, K-46 (tag), doplnenie ručného protokolu (K-29).
Žiadny draft ADR nevznikol.

## 8. Zhrnutie change experimentu (bod E)

**Porovnávací bod:** tag `sdd-D-fr1` neexistuje (K-46). Porovnanie je voči commitu 1e6d09e (stav po verifikácii 0008).
Zmenu tvoria 2 commity: e8f83eb (specification 0.4.0) a f0d7725 (UI a testy). Spolu 53 súborov, +3780 / −212 riadkov.

**Čo sa zmenilo**

| Oblasť | Zmena |
|---|---|
| Specification 0.3.1 → 0.4.0 | nový FR1-UI, US-002 + SC-002-01 – SC-002-10, SC-001-17, ADR-010; motivation IDs v `requirements.yaml`, AS v `service-realization.yaml`, `allowed_dependencies` v `components.yaml`, UI a packaging v `deployment.yaml` / `technology-context.yaml`, realizačné stopy FR1 v `traceability.yaml`, glossary; redakčné opravy ADR-007/008/009, BR-011 (N-11) |
| Approved contract | iba metadáta záznamu o schválení (V-01) a formátovanie; obsah 0.2.1 bez zmeny (K-04) |
| Kód | `Program.cs` +4 riadky (statické súbory), nový `wwwroot/` (`index.html`, `app.js`, `bratislava-time.js`, `messages.js`), README, `appsettings.Development.json` (user-jwts, V-11) |
| Testy | nové: `UiStaticPageTests.cs` (34), `tests/ui/*.test.mjs` (44) |
| Reporty | plán a implementation report 0010, ručný protokol US-002 so 2 snímkami obrazovky |

**Rozhodnutia, ktoré zmenu riadili**

- Zmena DR-17 (prompt 0009): jednoduché UI je v rozsahu prototypu, sumarizácia ostáva mimo.
- ADR-010 (Accepted): statická stránka HTML + vanilla JS vo `wwwroot`, bez balíka a buildu, iba konzument 0.2.1.
- DR-0910 (token iba v pamäti), DR-0911 (Europe/Bratislava cez Intl, odmietnutie neexistujúceho/dvojznačného času),
  DR-0912 (texty hlášok), DR-0913 (stránka bez prihlásenia), DR-0914 (medzera modelu), DR-0915 (spôsob overenia),
  DR-0902 (artifact v zadaní), DR-0903 (verzie súborov), DR-0905 – DR-0908.
- DR-1001 (`node --test`, doplnenie ADR-008), DR-1002 (odpoveď bez problem details → text SC-002-10), schválené
  potvrdenie pri 201.
- Rozhodnutie „contract sa nemení“ – DESIGN_CONTRACT sa preskočil (log 0009).

**Čo ostalo bez zmeny**

- Obsah API-Reservations 0.2.1 (operácia, vstup, výstup, chyby, security) a `approval.yaml`.
- API: `ModulRezervaciiAKlientov/`, `Hosting/`, `*.csproj`, NuGet balíky, `config/seed-clients.json`.
- Pôvodné testy (`Acceptance/`, `Contract/`, `Infrastructure/`) – bez zmeny a 52/52 PASS.

Záver experimentu: zmena rozsahu (DR-17) prešla cez PREPARE_SPEC → ľudské schválenie → IMPLEMENT_CHANGE
bez zmeny contractu a bez zásahu do API kódu. Zostali iba redakčné nálezy, chýbajúca stopa FR1-UI v `traceability.yaml`
a neúplný ručný protokol.
