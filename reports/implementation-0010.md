# Implementation report – FR1-UI / US-002 (úloha 0010, fáza 2)

| Pole | Hodnota |
|---|---|
| Režim | IMPLEMENT_CHANGE – fáza 2 (candidate code a testy podľa schváleného plánu) |
| Plán | `reports/implementation-plan-0010.md` (schválil Martin Štefanko, 2026-11-25) |
| Log | `spec/prompts/log/0010-2026-11-25-IMPLEMENT_CHANGE.md` |
| Specification baseline | 0.4.0 (approved) / architecture baseline B-01 |
| Approved contract | API-Reservations 0.2.1 – bez zmeny |
| Rozhodnutia | ADR-010 (Accepted), ADR-008 doplnenie DR-1001 = A, DR-1002 = A, potvrdenie 201 schválené |
| Pripravila | AI (Claude Opus 5.5). Candidate code – business význam potvrdzuje človek. |

## 1. Zmenené a nové súbory

| Súbor | Zmena |
|---|---|
| `src/IntelliWell/Program.cs` | `UseDefaultFiles()` + `UseStaticFiles()` medzi `CorrelationIdMiddleware` a `UseAuthentication()` |
| `src/IntelliWell/wwwroot/index.html` | nová stránka (token, klient s predvoleným seed UUID, začiatok v miestnom čase, poznámka, výsledok `role="status"`) |
| `src/IntelliWell/wwwroot/js/bratislava-time.js` | `toUtcIso`, `parseWallTime`, `formatLocalRange` – Intl, Europe/Bratislava, detekcia neexistujúceho / dvojznačného času |
| `src/IntelliWell/wwwroot/js/messages.js` | 9 URN → schválené hlášky, `messageForProblem` (type → title → SC-002-10), konštanty SC-002-09/-10, `confirmationMessage` |
| `src/IntelliWell/wwwroot/js/app.js` | odoslanie formulára: 1× `fetch`, bez opakovania, `textContent`, token iba v poli stránky |
| `tests/IntelliWell.Tests/Ui/UiStaticPageTests.cs` | 34 testov (dotnet test), trait `Requirement=FR1-UI`, `Scenario=SC-002-xx` |
| `tests/ui/bratislava-time.test.mjs`, `tests/ui/messages.test.mjs` | 44 testov (`node --test`) |
| `reports/manual-test-protocol-US-002.md` | šablóna ručného protokolu SC-002-01 – SC-002-10 + doplnkové kontroly |
| `src/IntelliWell/README.md` | sekcia UI (token, spustenie, otvorenie v prehliadači) a príkazy UI testov |
| `spec/drafts/open-decisions.md` (working) | sekcia „Rozhodnuté v úlohe 0010“ (DR-1001, DR-1002, potvrdenie 201) |

Nezmenené: approved contract, specification (okrem working `open-decisions.md`), `*.csproj`, kód modulu
`ModulRezervaciiAKlientov/`, `Hosting/`, pôvodné testy.

## 2. Quality gates

| Gate | Výsledok |
|---|---|
| `dotnet build IntelliWell.slnx -warnaserror` | PASS – 0 warnings, 0 errors |
| `dotnet test IntelliWell.slnx` | PASS – 86/86 (pôvodných 52/52 s filtrom `Requirement!=FR1-UI`, nových 34/34 s `Requirement=FR1-UI`) |
| `node --test "tests/ui/*.test.mjs"` (Node v22.14.0) | PASS – 44/44, 10 suites, bez varovaní |
| Smoke test `dotnet run` (profil http) | PASS – `GET /` 200 `text/html`, `GET /js/app.js` 200 `text/javascript`, `POST /api/v1/reservations` bez tokenu 401 `application/problem+json` `unauthenticated` (API bez zmeny) |
| Ručný protokol US-002 | UNKNOWN – šablóna pripravená, vyplní tester |

Smoke test nevytváral token cez `dotnet user-jwts` (zmenil by `appsettings.Development.json` a user secrets vývojára);
úspešné vytvorenie cez UI overí ručný protokol.

## 3. Traceability summary

| Scenár | Kód | dotnet test (Ui) | node --test | Ručný protokol | API (US-001) |
|---|---|---|---|---|---|
| SC-002-01 | index.html, app.js, bratislava-time.js, messages.js | seed UUID, POST + Bearer, text potvrdenia | prepočet 14:00 → 13:00Z, termín, potvrdenie | áno | SC-001-01 |
| SC-002-02 | bratislava-time.js | Europe/Bratislava v kóde | 14:00 → 12:00Z, termín, nezávislosť od TZ | áno | – |
| SC-002-03 | bratislava-time.js | Europe/Bratislava v kóde | 16:00 → 15:00Z, termín, nezávislosť od TZ | áno | – |
| SC-002-04 | messages.js | URN = enum contractu | slot-occupied | áno | SC-001-04 |
| SC-002-05 | messages.js, app.js | 9 URN + texty, zhoda s enumom approved contractu | 9 typov, záloha title | áno | SC-001-xx |
| SC-002-06 | Program.cs, app.js | stránka a skripty bez tokenu 200 | unauthenticated | áno | contract 401 |
| SC-002-07 | app.js | jediný `fetch`, bez časovačov | service-unavailable | áno (hlavný dôkaz) | SC-001-15 |
| SC-002-08 | app.js | žiadne `console.` | – | áno | contract CON-003 |
| SC-002-09 | bratislava-time.js, app.js | texty hlášok | neexistujúci / dvojznačný + hranice | áno | – |
| SC-002-10 | app.js, messages.js | text hlášky | text + DR-1002 | áno (hlavný dôkaz) | – |
| ADR-010 Security, DR-0910 | app.js, index.html | bez HTML vkladania, bez úložiska prehliadača, bez externých zdrojov | – | áno | – |

Väzby: FR1-UI → FR1 → AS-SpravaRezervaciiAKlientov → AC-ModulRezervaciiAKlientov (Application Interface – známa medzera
DR-0914); hostiteľská časť (`Program.cs`) → AC-IntelliWell (DR-0906); artifact ART-SoftverIntelliWell (`wwwroot`).

## 4. Návrhy AI na potvrdenie

- Pomocné texty stránky (nadpis, popisy polí) a priebežný stav „Ukladám rezerváciu…“ – nie sú hláškami zo scenárov
  ani pravidlami; znenie navrhla AI.
- Prázdny alebo neúplný začiatok UI neposiela a rozhodne server (400 → hláška invalid-request) – UI neduplikuje validáciu.
- Počas čakania na odpoveď je odoslanie zablokované (zabráni dvojitému odoslaniu, nie je to opakovanie).

## 5. Otvorené

- F-01, F-03 (plán 0010) – odložené rozhodnutím človeka.
- Ručný protokol – vyplní tester.
- Žiadna nová DECISION REQUIRED, žiadna chyba contractu.
