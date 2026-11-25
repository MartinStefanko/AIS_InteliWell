# 0010 – IMPLEMENT_CHANGE – implementácia UI pre FR1 (FR1-UI)

| Pole | Hodnota |
|---|---|
| Dátum | 2026-11-25 |
| Poradové číslo | 0010 |
| Režim | IMPLEMENT_CHANGE |
| Verzia šablóny promptu | spec/prompts/implement-change.md, prompt_template_version 1.0 |
| Specification baseline | 0.4.0 (approved) |
| Architecture baseline | B-01 |
| Model | Claude Opus 5.5 (`claude-opus-5-5`), Claude Code |
| Commit | (doplní človek) |

## Prompt

~~~text
# Úloha 0010: implementácia UI pre FR1

Mode: IMPLEMENT_CHANGE
Change: FR1-UI (change experiment, bod E)
Specification baseline: 0.4.0 (approved)
Approved contract: API-Reservations 0.2.1 (bez zmeny)
Prompt template: spec/prompts/implement-change.md v1.0

Over manifest 0.4.0 approved, ADR-001 až ADR-010 Accepted a approval.yaml pre 0.2.1; inak DECISION REQUIRED.

## FÁZA 1 (teraz): iba plán
Priprav impact analysis a generation plan po súboroch pre UI podľa ADR-010, FR1-UI a US-002 (SC-002-01 až SC-002-10):
statická stránka v wwwroot servírovaná ART-SoftverIntelliWell, bez nového NuGet balíka a bez frontend buildu.
Uveď, ktoré testy pokryjú SC-002-xx automaticky (bez prehliadača) a ktoré pôjdu do ručného protokolu (DR-0915).
Existujúce správanie API a testy FR1 sa nesmú zmeniť. Kód zatiaľ NEPÍŠ, počkaj na „plán schvaľujem“.

## FÁZA 2 (až po mojom „plán schvaľujem“)
Vygeneruj UI a testy, spusti dotnet build (warnings as errors) a dotnet test (všetky, vrátane pôvodných 52),
priprav šablónu ručného protokolu do reports/ a do src/IntelliWell/README.md doplň, ako UI spustiť a otvoriť v prehliadači.
Nemeň approved contract ani specification. Ak narazíš na chybu contractu, zastav sa s DECISION REQUIRED.

Log podľa CLAUDE.md (0010-2026-11-25-IMPLEMENT_CHANGE.md), obe fázy do toho istého logu. Necommituj.
Odpoveď štruktúruj podľa kapitoly 15 AGENTS.md.
~~~

## Výstup AI

### Fáza 1 – plán

- Vstupná kontrola PASS: manifest 0.4.0 approved (B-01), ADR-001 – ADR-010 Accepted, `approval.yaml` API-Reservations
  0.2.1 approved (`unresolved_decisions: []`), US-002 / US-002.feature / FR1-UI approved.
- Vytvorený `reports/implementation-plan-0010.md`: impact analysis, kontrola architektúry, generation plan po súboroch
  (Program.cs: `UseDefaultFiles` + `UseStaticFiles`; `wwwroot/index.html`, `js/bratislava-time.js`, `js/messages.js`,
  `js/app.js`; `tests/IntelliWell.Tests/Ui/UiStaticPageTests.cs`; voliteľne `tests/ui/*.test.mjs`; ručný protokol;
  README; implementation report), tabuľka pokrytia SC-002-01 – SC-002-10 (automaticky / ručne).
- Kód ani testy sa nezmenili. Specification ani contract sa nezmenili.
- Neblokujúce zistenia F-01 (zastarané „ADR-010 (Proposed)“ v US-002.md, deployment.yaml, technology-context.yaml,
  ADR-010), F-02 (manifest `working.proposed_contracts` ukazuje na `approved/`), F-03 (SC-002-09 stále „návrh AI“).
- Návrh AI na potvrdenie v ručnom protokole: znenie potvrdenia pri 201.

Otvorené DECISION REQUIRED:
- **DR-1001** – nástroj na automatický test JS logiky (prepočet času, mapovanie hlášok): A. Node.js `node --test`
  (lokálne v22.14.0, bez npm balíkov, mimo `dotnet test`; nová nástrojová závislosť – zápis do ADR-008 / technology-context),
  B. bez automatického testu JS logiky, prepočet iba ručným protokolom. Odporúčanie AI: A.
- **DR-1002** – odpoveď servera bez problem details (napr. 500, telo nie je JSON): A. zobraziť schválený text SC-002-10,
  B. nový text (vyžaduje schválenie). Odporúčanie AI: A.

### Fáza 2 – prompt

~~~text
Plán schvaľujem. Rozhodol: Martin Štefanko (Software architect).
- DR-1001: A – JS logika UI sa testuje cez `node --test` (bez npm balíkov). Zapísal som ručne ako doplnenie ADR-008.
- DR-1002: A – pri odpovedi bez problem details UI zobrazí schválený text zo SC-002-10. Zapíš do open-decisions.md.
- Znenie potvrdenia pri 201 („Rezervácia je uložená na termín 1. 12. 2026 14:00–15:00.“) schvaľujem.
- F-02 som opravil ručne. F-01 a F-03 nechaj, vyriešime ich neskôr.
Pokračuj FÁZOU 2 podľa schváleného plánu. Na konci spusti dotnet build (warnings as errors), dotnet test a node --test,
vypíš výsledky a presný návod, ako UI spustiť a otvoriť v prehliadači vrátane vytvorenia tokenu. Necommituj.
~~~

### Fáza 2 – výstup AI

Podrobne: `reports/implementation-0010.md`.

- **Kód:** `Program.cs` (`UseDefaultFiles` + `UseStaticFiles`), nový `src/IntelliWell/wwwroot/` (`index.html`,
  `js/app.js`, `js/bratislava-time.js`, `js/messages.js`). Žiadny nový NuGet balík, žiadny build frontendu.
- **Testy:** `tests/IntelliWell.Tests/Ui/UiStaticPageTests.cs` (34), `tests/ui/*.test.mjs` (44, `node --test`).
- **Reporty:** `reports/manual-test-protocol-US-002.md` (šablóna), `reports/implementation-0010.md`.
- **README:** sekcia UI (token, spustenie, prehliadač) a príkazy UI testov.
- **Working:** `spec/drafts/open-decisions.md` – sekcia „Rozhodnuté v úlohe 0010“ (DR-1001, DR-1002, potvrdenie 201).
- **Výsledky:** `dotnet build -warnaserror` 0 warnings / 0 errors; `dotnet test` 86/86 PASS (pôvodných 52/52);
  `node --test` 44/44 PASS; smoke test `dotnet run`: `/` 200, `/js/app.js` 200, POST bez tokenu 401 (API bez zmeny).
- **Nezmenené:** approved contract, specification, csproj, kód modulu, pôvodné testy. Necommitnuté.
- **Na potvrdenie (návrh AI):** pomocné texty stránky a priebežný stav „Ukladám rezerváciu…“.
- Otvorené DECISION REQUIRED: žiadne. Chyba contractu: žiadna.

## Ľudské rozhodnutie
- **Rozhodol:** Martin Štefanko (Software architect), 2026-11-25
- **Fáza 1:** plán schválený; DR-1001 = A (node --test, doplnenie ADR-008), DR-1002 = A, text potvrdenia pri 201 schválený.
- **Fáza 2:** UI prijímam. dotnet build bez warningov, dotnet test 86/86, node --test 44/44.
- **Ručný test:** UI som spustil v prehliadači a vyplnil reports/manual-test-protocol-US-002.md.
- **Texty UI:** nadpis, popisy polí a stav „Ukladám rezerváciu…“ potvrdzujem.
- **Business význam:** UI umožňuje správcovi návštev vytvoriť rezerváciu podľa FR1 a FR1-UI.

