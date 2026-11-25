# Implementation plan – FR1-UI / US-002 (úloha 0010, fáza 1)

| Pole | Hodnota |
|---|---|
| Režim | IMPLEMENT_CHANGE – fáza 1 (iba plán, bez kódu) |
| Log | `spec/prompts/log/0010-2026-11-25-IMPLEMENT_CHANGE.md` |
| Specification baseline | 0.4.0 (approved) / architecture baseline B-01 |
| Approved contract | API-Reservations 0.2.1 – `spec/contracts/approved/` (bez zmeny; UI je konzument) |
| Prompt template | `spec/prompts/implement-change.md` v1.0 |
| Pripravila | AI (Claude Opus 5.5). Plán nie je schválený; čaká na „plán schvaľujem“. |

## 1. Vstupná kontrola

| Kontrola | Výsledok | Dôkaz |
|---|---|---|
| Manifest 0.4.0 approved | PASS | `spec/manifest.yaml`: `specification.version: "0.4.0"`, `status: "approved"`, `architecture_baseline: "B-01"` |
| ADR-001 až ADR-010 Accepted | PASS | `## Status` = `Accepted` v ADR-001 – ADR-010 |
| approval.yaml pre 0.2.1 | PASS | `contract_id: API-Reservations`, `version: "0.2.1"`, `status: approved`, `unresolved_decisions: []` |
| US-002 a US-002.feature approved | PASS | hlavičky `status: "approved"`, SC-002-01 – SC-002-10 |
| FR1-UI approved | PASS | `requirements.yaml` meta `approved`, FR1 scope odkazuje na FR1-UI a ADR-010 |
| Deployment – UI v ART-SoftverIntelliWell | PASS | `deployment.yaml` `user_interface.part_of_artifact: true`, `static_content: src/IntelliWell/wwwroot/` |
| .NET prostredie | PASS | SDK 10.0.404 (ADR-002) |

Neblokujúce zistenia v podkladoch (AI ich v IMPLEMENT_CHANGE nemení, iba vypisuje):

| ID | Zistenie | Návrh pre človeka |
|---|---|---|
| F-01 | Po Accepted ADR-010 ostali zastarané texty „ADR-010 (Proposed)“: `US-002.md` r. 22, `deployment.yaml` r. 3, 33, 43, 62 a `open_questions` r. 67 („čaká na Accepted“), `technology-context.yaml` r. 3, 36, 85; ADR-010 úvodná poznámka a koniec sekcie Decision („stav Proposed, kým človek nezapíše Accepted“). | redakčne opraviť na Accepted |
| F-02 | Manifest `working.proposed_contracts: "spec/contracts/approved/"` – má byť `spec/contracts/proposed/` (AGENTS.md kap. 8). | opraviť v manifeste |
| F-03 | `US-002.md` a hlavička `US-002.feature` uvádzajú „Znenie hlášky v SC-002-09 je návrh AI“, hoci človek ho potvrdil v logu 0009. | redakčne doplniť potvrdenie |

## 2. Impact analysis

```yaml
change:
  mode: "IMPLEMENT_CHANGE"
  requirement: "FR1-UI"
  specification_baseline: "0.4.0"
  reason: "Change experiment bod E – jednoduché UI pre FR1 podľa ADR-010 (Accepted), zmena DR-17."

affected:
  goals: ["G-002"]                      # FR1-UI → FR1/NFR1 (requirements.yaml)
  actors_roles: ["ACT-Recepcna", "ROLE-SpravcaNavstev"]
  business_processes: ["BP-SpravaNavstev"]
  business_services: ["BS-RezervaciaTerminu"]
  application_services: ["AS-SpravaRezervaciiAKlientov"]
  application_components: ["AC-ModulRezervaciiAKlientov", "AC-IntelliWell"]   # UI viazané cez AC-ModulRezervaciiAKlientov (DR-0914); Program.cs = hostiteľská časť AC-IntelliWell (DR-0906)
  interfaces_contracts: ["API-Reservations 0.2.1 (konzument, bez zmeny)"]      # Application Interface pre recepčnú: známa medzera B-01 (DR-0914)
  data_objects: ["DO-Rezervacie", "DO-EvidenciaKlientov"]                     # iba cez API; UI nič neukladá
  technology_elements: ["ART-SoftverIntelliWell", "SSW-RuntimeDotNet"]
  work_packages_deliverables: []
  decisions: ["ADR-010", "ADR-008", "ADR-004", "ADR-002", "DR-17", "DR-18", "DR-23", "DR-24", "DR-C03", "DR-0910", "DR-0911", "DR-0912", "DR-0913", "DR-0914", "DR-0915"]
  scenarios: ["SC-002-01 – SC-002-10"]
  tests: ["nové: tests/IntelliWell.Tests/Ui/", "regresia: existujúcich 52 testov FR1 bez zmeny"]

open_questions: ["DR-1001", "DR-1002"]
```

Kontrola architektúry (kap. 10 bod 3):

- **Application Structure / allowed dependencies (DR-0906):** UI nevolá nič okrem `POST /api/v1/reservations`. Žiadna nová
  závislosť medzi Application Componentmi. Kód v `Program.cs` (servírovanie statických súborov) patrí AC-IntelliWell.
- **Application Cooperation:** bez zmeny; UI je na rovnakom pôvode (same origin), CORS sa nepoužíva (ADR-010).
- **Application Usage:** Application Interface pre recepčnú v B-01 chýba – známa medzera (DR-0914). UI sa traceuje na
  AS-SpravaRezervaciiAKlientov / AC-ModulRezervaciiAKlientov. Nový prvok ani ID AI nenavrhuje.
- **Service Realization:** AS-SpravaRezervaciiAKlientov realizuje AC-ModulRezervaciiAKlientov – bez zmeny.
- **Deployment:** UI je súčasť ART-SoftverIntelliWell (`wwwroot`), žiadny nový artifact, bez balenia (DR-0902, ADR-008).
- **Balíky:** žiadny nový NuGet balík; `UseDefaultFiles` / `UseStaticFiles` sú súčasť `Microsoft.AspNetCore.App` (ADR-010).

## 3. Generation plan po súboroch

| # | Súbor | Zmena | Väzba |
|---|---|---|---|
| 1 | `src/IntelliWell/Program.cs` | vložiť `app.UseDefaultFiles(); app.UseStaticFiles();` medzi `CorrelationIdMiddleware` a `UseAuthentication()`. Middleware obsluhuje iba existujúce súbory vo `wwwroot`; `/api/v1/reservations` súbor nemá, takže požiadavka prejde bez zmeny (poradie 401/403 BR-012 ostáva). Statická stránka je bez autentifikácie (DR-0913). | ADR-010 Consequences, DR-0913 |
| 2 | `src/IntelliWell/wwwroot/index.html` | jedna stránka: polia token (iba v pamäti, `autocomplete="off"`), identifikátor klienta predvyplnený `7c9e6679-7425-40de-944b-e07fc1f90ae7`, začiatok (`datetime-local`, miestny čas), poznámka; oblasť výsledku s `role="status"`. Bez externých zdrojov (CDN), slovenčina, `lang="sk"`. Štýl inline v `<style>`. | FR1-UI, US-002 AK 1, 8; DR-24, DR-0910 |
| 3 | `src/IntelliWell/wwwroot/js/bratislava-time.js` | ES modul bez DOM: `toUtcIso(wallTime)` – prepočet miestneho času Europe/Bratislava na RFC 3339 UTC so `Z` cez `Intl.DateTimeFormat({ timeZone: "Europe/Bratislava" })` nezávisle od pásma počítača; vráti chybu `nonexistent` / `ambiguous` pre časy pri zmene času. `formatLocalRange(startIso, endIso)` – „1. 12. 2026 14:00–15:00“ pre potvrdenie. | DR-18, DR-C03, DR-0911; SC-002-02, -03, -09 |
| 4 | `src/IntelliWell/wwwroot/js/messages.js` | ES modul bez DOM: mapa 9 URN z enumu contractu → schválené hlášky SC-002-04/-05; `messageForProblem(problem)` → hláška podľa `type`, inak `title` (DR-0912); konštanty hlášok SC-002-09 a SC-002-10. | DR-0912; SC-002-04, -05, -06, -07, -09, -10 |
| 5 | `src/IntelliWell/wwwroot/js/app.js` | ES modul: pri odoslaní prepočíta čas (pri `nonexistent`/`ambiguous` zobrazí hlášku a nič nepošle), zavolá **raz** `fetch("/api/v1/reservations", POST)`; hlavička `Authorization: Bearer` iba ak je token vložený (inak server vráti 401 → SC-002-06). Prázdny začiatok sa neposiela, rozhodne server (400 `invalid-request`) – UI neduplikuje pravidlá. 201 → potvrdenie s termínom v miestnom čase; problem details → `messageForProblem`; výnimka `fetch` → text SC-002-10. Bez automatického opakovania, bez `console.*`, texty iba cez `textContent`, token iba v premennej (žiadny `localStorage`/`sessionStorage`/cookie). Počas požiadavky je odoslanie zablokované (zabráni dvojitému odoslaniu, nie je to opakovanie). | QR-002, CON-003, DR-0910, ADR-010 Security |
| 6 | `tests/IntelliWell.Tests/Ui/UiStaticPageTests.cs` | nové testy bez prehliadača cez existujúci `IntelliWellFactory` (sekcia 4). Trait `Scenario` = SC-002-xx, `Requirement` = FR1-UI. | DR-0915 |
| 7 | `tests/ui/*.test.mjs` | **iba pri DR-1001 = A:** `node --test` pre `bratislava-time.js` a `messages.js` (sekcia 4). | DR-0915, DR-1001 |
| 8 | `reports/manual-test-protocol-US-002.md` | šablóna ručného protokolu SC-002-01 – SC-002-10: predpoklady (token `dotnet user-jwts`, seed klient), kroky v business jazyku, očakávaný výsledok, skutočný výsledok, PASS/FAIL, dátum, tester. SC-002-07: nedostupnosť evidencie navodiť dočasným nastavením súboru SQLite len na čítanie (→ `SqliteException` → 503, DR-I04). SC-002-10: zastaviť server po načítaní stránky. SC-002-08: kontrola konzoly prehliadača a aplikačného logu. | DR-0915 |
| 9 | `src/IntelliWell/README.md` | nová sekcia „UI (FR1-UI)“: `dotnet run …`, otvoriť `http://localhost:5080/`, vytvorenie tokenu, vloženie tokenu, poznámka DR-0910 (po obnovení stránky vložiť znova); spustenie UI testov. | úloha 0010 |
| 10 | `reports/implementation-0010.md` | výsledky build/test, traceability summary FR1-UI → kód → testy → protokol. | kap. 10 bod 7 |

Nemení sa: approved contract, specification, `IntelliWell.csproj` (Web SDK zahŕňa `wwwroot` automaticky),
`IntelliWell.Tests.csproj`, existujúce testy a kód modulu. Pri chybe contractu → DECISION REQUIRED (návrat do DESIGN_CONTRACT).

Návrhy AI na potvrdenie v ručnom protokole (nie nové pravidlo): text potvrdenia pri 201
(„Rezervácia je uložená na termín 1. 12. 2026 14:00–15:00.“) – feature určuje iba obsah (termín), nie znenie.

## 4. Pokrytie scenárov SC-002-xx

Legenda: **A** = automaticky v `dotnet test` (C#, bez prehliadača), **N** = automaticky `node --test` (iba pri DR-1001 = A),
**R** = ručný protokol. API časť každého scenára už pokrývajú testy US-001 (SC-001-xx) – nemenia sa.

| Scenár | Automaticky (bez prehliadača) | Ručne (DR-0915) |
|---|---|---|
| SC-002-01 úspešné vytvorenie | A: `GET /` vráti 200 `text/html` bez tokenu; stránka obsahuje predvolené UUID seed klienta; `app.js` posiela na `/api/v1/reservations`. API časť: SC-001-01. N: `formatLocalRange` → „1. 12. 2026 14:00–15:00“ | R: potvrdenie a zablokovaný termín |
| SC-002-02 letný čas | N: `toUtcIso("2027-07-01T14:00")` = `2027-07-01T12:00:00Z` + popis potvrdenia | R (pri DR-1001 = B jediné overenie prepočtu) |
| SC-002-03 zimný čas | N: `toUtcIso("2026-12-01T16:00")` = `2026-12-01T15:00:00Z` + popis potvrdenia | R (pri B jediné overenie) |
| SC-002-04 obsadený termín | A: `messages.js` obsahuje URN `slot-occupied` a presný text. N: `messageForProblem` | R: zobrazenie hlášky |
| SC-002-05 9 typov odmietnutia | A: všetkých 9 URN z enumu contractu + presné texty z Examples (Theory, 9 riadkov). N: mapovanie + záloha `title` pre neznámy `type` | R: aspoň 401, 403, 400, 422 ×4, 409 cez UI; 503 spolu so SC-002-07 |
| SC-002-06 neprihlásená osoba | A: `GET /` aj `GET /js/app.js` bez tokenu = 200 (DR-0913). API 401: existujúce testy | R: hláška a voľný termín |
| SC-002-07 bez opakovania | A (statická kontrola, slabý dôkaz): `app.js` má jediné volanie `fetch` a žiadny `setTimeout`/`setInterval` | R: hlavný dôkaz |
| SC-002-08 CON-003 | A: žiadny `console.` v JS. Serverový log: existujúci test CON-003 v `CreateReservationContractTests.cs` | R: konzola prehliadača a log servera |
| SC-002-09 DST | A: texty hlášok prítomné. N: `28. 3. 2027 02:30` → `nonexistent`, `31. 10. 2027 02:30` → `ambiguous` | R: hláška a žiadna požiadavka (karta Sieť) |
| SC-002-10 server neodpovedá | A: text hlášky prítomný | R: hlavný dôkaz |
| Security ADR-010 / DR-0910 | A: v JS nie je `innerHTML`, `outerHTML`, `insertAdjacentHTML`, `document.write`, `localStorage`, `sessionStorage`, `document.cookie`; stránka nenačítava externé zdroje | – |
| Regresia API | A: všetkých 52 pôvodných testov bez zmeny | – |

Statické kontroly obsahu JS (A) dokazujú prítomnosť textov a absenciu zakázaných konštrukcií, nie správanie stránky.
Správanie overuje N (logika) a R (stránka).

## 5. Quality gates (fáza 2)

1. `dotnet build IntelliWell.slnx -warnaserror`
2. `dotnet test IntelliWell.slnx` – 52 pôvodných + nové UI testy, všetky PASS
3. pri DR-1001 = A: `node --test tests/ui/`
4. kontrola, že `git diff` nemení `spec/` ani pôvodné testy

## 6. DECISION REQUIRED

Pozri log 0010 a odpoveď AI: **DR-1001** (nástroj na test JS logiky), **DR-1002** (odpoveď bez problem details).
