# 0001 – SETUP_FRAMEWORK – založenie SDD repozitára (checkpoint A / G0)

| Pole | Hodnota |
|---|---|
| Dátum | 2026-11-25 |
| Poradové číslo | 0001 |
| Režim | SETUP_FRAMEWORK (príprava frameworku, nie jeden zo štyroch režimov) |
| Verzia šablóny promptu | žiadna (jednorazový prompt, šablóny v tomto kroku vznikli) |
| Specification baseline | 0.1.0 (vzniká v tomto kroku, draft) |
| Architecture baseline | B-01 |
| Model | Claude Opus 5.5 (`claude-opus-5-5`), Claude Code |
| Commit | (doplní človek) |

## Presný text promptu

~~~text
# Úloha: založenie SDD repozitára pre IntelliWell (checkpoint A / G0 – Framework)

## Kontext
Som Martin, softvérový architekt tímového projektu IntelliWell (FIIT STU, predmet Architektúra informačných systémov).
Architektonický projekt v ArchiMate (Sparx EA) je hotový po Layered View. Teraz robíme SDD (Specification Driven Design)
pre JEDNU vybranú funkcionalitu: **FR1 Vytvorenie rezervácie (UC01)**.

Zdrojové dokumenty sú v `docs/source/`:
- `AIS_IntelliWell_4.docx` – náš architektonický projekt (jediný zdroj obsahu o IntelliWell)
- `SDD_PROJECT_TEMPLATE_v4.pdf` – šablóna štruktúry repozitára a formátov YAML (záväzná pre štruktúru)
- `SDD_AI_RULES_v4.pdf` – normatívne pravidlá práce AI
- `SDD_FIIT_STU_architect_v4.pdf` – metodika SDD (prednáška)

Najprv si všetky štyri dokumenty celé prečítaj. Pri .docx použi napr. pandoc alebo python-docx.

## Dôležité: tento krok NIE JE jeden zo štyroch režimov
Ide o prípravu frameworku (fáza A, checkpoint G0), ktorá predchádza režimom PREPARE_SPEC, DESIGN_CONTRACT,
IMPLEMENT_CHANGE a VERIFY_CHANGE. Pravidlá zo SDD_AI_RULES však platia už teraz:
- nič neschvaľuj, každý obsahový súbor má `status: "draft"`, `approved_by: null`, `reviewed_by: null`,
- nevymýšľaj business rules, práva, stavy, error behavior, technológie, DBMS, čísla (SLA, limity),
- každý údaj musí mať oporu v `AIS_IntelliWell_4.docx`; čo tam nie je, zapíš ako otvorenú otázku (`open_questions` / `null`),
- NEVYTVÁRAJ `src/`, kód, testy ani contract. NEVYBERAJ technológie.

## Čo vytvoriť

### 1. Pravidlá AI
- `AGENTS.md` – verný prepis celého `SDD_AI_RULES_v4.pdf` do Markdownu (bez skracovania a bez vlastných úprav).
- `CLAUDE.md` – iba riadok `@AGENTS.md` a krátka poznámka, že pravidlá v AGENTS.md sú záväzné.
- `README.md` – čo je repozitár, zvolená funkcionalita FR1, postup režimov, kde sú prompty a ich log.

### 2. Štruktúra adresárov podľa kapitoly 2 šablóny
Vytvor štruktúru `spec/…`, `reports/`, `config/`, `tests/` (prázdne adresáre s `.gitkeep`). `src/` zatiaľ nevytváraj.
Nevytváraj `spec/architecture/model/project-model.qea` (model zostáva v Sparx EA).

### 3. Architektonické obrázky
Rozbaľ obrázky z `AIS_IntelliWell_4.docx` (word/media). Podľa popisov v dokumente identifikuj views relevantné pre FR1:
Requirements Realization View, Business Process Cooperation View TO-BE, Application Cooperation View TO-BE,
Application Usage View TO-BE, Layered View TO-BE, prípadne Motivation View a Implementation and Migration View.
Ulož ich do `spec/architecture/views/` s názvami `B-01-<viewpoint>-tobe.png`.
Ak si priradením obrázka k view nie si istý, nehádaj: ulož ho ako `B-01-unidentified-<n>.png` a uveď to v otázkach.
POZOR: obrázky z .docx sú dočasný podklad. V manifeste majú `status: "draft"` a `reviewed_by: null`. Ja ich porovnám
s modelom v Sparx EA a prípadne nahradím exportom.

### 4. Manifest
`spec/manifest.yaml` podľa vzoru v kapitole 4.3 šablóny: `version: "0.1.0"`, `status: "draft"`,
`architecture_baseline: "B-01"`, `ai_rules_version: "4.0"`. Registruj iba súbory, ktoré skutočne existujú.
Nepoužité voliteľné podklady nastav na `null`.

### 5. Draft obsahových súborov pre FR1 (iba z dokumentu)
Vyplň podľa formátov zo šablóny (kapitoly 5–22):
- `spec/glossary.md` – pojmy z dokumentu (rezervácia, termín, klient, profil klienta, recepčná, správca návštev, …)
- `spec/requirements/requirements.yaml` – FR1, NFR1, NFR4; constraints „Požiadavky prichádzajú telefonicky, e-mailom, SMS alebo osobne“ a „Interný systém len pre personál“
- `spec/requirements/business-rules.yaml` – iba pravidlá, ktoré dokument naozaj uvádza (napr. zablokovanie termínu po uložení rezervácie)
- `spec/requirements/quality-requirements.yaml` – NFR1, NFR4 (bez čísel, ktoré dokument neuvádza)
- `spec/business/actors-processes.yaml` – roly a proces Správa návštev (TO-BE), vrátane výnimiek, ktoré dokument uvádza
- `spec/application/components.yaml`, `service-realization.yaml`, `interactions.yaml`, `data-objects.yaml`, `lifecycles.yaml`
  (`communication_style: "undecided"`; stavy rezervácie iba ak vyplývajú z dokumentu, inak otvorená otázka)
- `spec/technology/technology-context.yaml` – opíš, čo dokument uvádza (server IntelliWell, databázový systém, sieť);
  NEOZNAČUJ `not applicable`, to rozhodnem ja
- `spec/architecture/continuum/reuse-catalog.yaml`, `spec/architecture/migration/implementation-context.yaml`,
  `spec/architecture/migration/gap-analysis.md` – prevezmi z kapitol 12–14 dokumentu v reálnom stave, nič nedopĺňaj
- `spec/traceability/traceability.yaml` – draft stôp Goal → FR1 → proces → služba → komponent
- `spec/policies/contract-design-profile.yaml` – iba kostra so `status: "draft"` a hodnotami `null` (rozhodnem ja)
- user story a Gherkin zatiaľ NEROB; tie vzniknú v PREPARE_SPEC

### 6. Stabilné IDs
Dokument má IDs iba pre FR1–FR6, NFR1–NFR4 a UC01–UC06. Tie zachovaj presne.
Pre ostatné prvky NAVRHNI IDs (G-xx, ROLE-…, BP-…, BS-…, AS-…, AC-…, DO-…, BO-…, CON-…, WP-…, PL-…) a vytvor
`spec/drafts/id-mapping.yaml`: id → presný názov prvku v dokumente → view, kde je viditeľný → `status: "proposed"`.
Tieto IDs nevydávaj za IDs z modelu. Ja ich potvrdím a doplním do Sparx EA.

### 7. Prompty a ich log
- `spec/prompts/prepare-spec.md`, `design-contract.md`, `implement-change.md`, `verify-change.md` – šablóny podľa kapitoly 17,
  upravené na FR1 a náš manifest, s hlavičkou `prompt_template_version: "1.0"`.
- `spec/prompts/log/README.md` – formát logu: dátum, poradové číslo, režim, verzia šablóny promptu, specification baseline,
  model, presný text promptu, stručný výstup AI, **ľudské rozhodnutie** (s odkazom na ADR/súbor), commit.
- `spec/prompts/log/0001-2026-11-25-SETUP_FRAMEWORK.md` – ulož doslovne CELÝ tento prompt a na konci sekciu
  „Výstup AI“ so zhrnutím, čo si vytvoril. Sekciu „Ľudské rozhodnutie“ nechaj prázdnu pre mňa.

### 8. ADR
- `spec/architecture/decisions/ADR-000-template.md` – šablóna podľa kapitoly 18.
- Nevytváraj žiadne rozhodnutie. Iba zoznam otvorených rozhodnutí (bod 9).

## 9. Výstup na konci (v odpovedi mne)
Použi štruktúru odpovede z kapitoly 15 SDD_AI_RULES a pridaj:
1. strom vytvorených súborov,
2. tabuľku navrhnutých IDs, ktoré mám potvrdiť,
3. zoznam obrázkov a ich priradenie k views (s mierou istoty),
4. **zoznam otvorených rozhodnutí vo formáte DECISION REQUIRED**. Určite aspoň: definícia termínu a dĺžka/sloty,
   súbeh dvoch rezervácií na ten istý termín, rezervácia bez registrovaného klienta, stavy rezervácie (zrušenie?),
   implementačná platforma (.NET je v dokumente iba „predpokladaná“), konkrétny DBMS, spôsob prihlasovania personálu (NFR4),
   štýl API (contract design profile), technology context applicable / not applicable,
5. návrh commit message. Necommituj, najprv to skontrolujem. Iba spusti `git init`, ak repozitár ešte neexistuje.

Ak narazíš na rozpor v dokumente alebo na niečo, čo nevieš jednoznačne určiť, nepokračuj odhadom. Zapíš to do otázok.
~~~

## Výstup AI (stručne)

**Zistené rozdiely oproti promptu**

- Zdroje sú v `docs/sources/` (nie `docs/source/`) a architektonický projekt sa volá `AIS_IntelliWell.docx` (nie `AIS_IntelliWell_4.docx`). Použil sa tento súbor. Či ide o verziu 4, musí potvrdiť človek.
- Git repozitár už existoval (remote `origin`), `git init` sa nespúšťal.
- Pôvodný `README.md` (UTF-16, obsah `# AIS_InteliWell`) bol prepísaný novým obsahom v UTF-8.

**Vytvorené**

- `AGENTS.md` (prepis SDD_AI_RULES_v4), `CLAUDE.md`, `README.md`.
- Štruktúra `spec/`, `reports/`, `config/`, `tests/` s `.gitkeep`; `src/` a `.qea` nevytvorené.
- 8 obrázkov v `spec/architecture/views/` (všetky priradené s vysokou istotou podľa titulku diagramu a kapitoly docx; navyše Organization View TO-BE kvôli priradeniu Recepčná → Správca návštev). Žiadny `unidentified`.
- `spec/manifest.yaml` (0.1.0, draft, B-01, AI rules 4.0) s rozšírením `supporting.source_documents` na potvrdenie.
- Drafty: glossary, requirements (FR1, NFR1, NFR4, CON-001, CON-002), business-rules (BR-001, BR-002), quality-requirements, actors-processes, components, service-realization, interactions, data-objects, lifecycles (bez stavov), technology-context (status null), reuse-catalog, implementation-context, gap-analysis, traceability, contract-design-profile (kostra).
- `spec/drafts/id-mapping.yaml` – navrhnuté IDs so stavom proposed.
- Prompt šablóny pre 4 režimy (`prompt_template_version 1.0`), formát logu, šablóna ADR-000.
- Otvorené rozhodnutia DR-01 až DR-17: prehľad v `spec/drafts/open-decisions.md`, plné znenie DECISION REQUIRED v odpovedi AI. Žiadne ADR nevzniklo.
- Validácia: všetkých 16 YAML súborov je syntakticky platných; všetky cesty v manifeste existujú; všetky použité IDs sú definované v `id-mapping.yaml` (okrem FR2/FR5, ktoré sú IDs z docx a spomínajú sa iba ako kontext).

## Ľudské rozhodnutie

- **Rozhodol:** Martin Štefanko (Software architect)
- **Dátum:** 2026-11-25
- **Rozhodnutie:** Výstup G0 som skontroloval a súhlasím s ním ako s draftom (spec 0.1.0, B-01). Potvrdzujem DR-16, navrhnuté IDs, príponu `-tobe`, `source_documents` v manifeste a Organization View.
- **Otvorené:** DR-01 až DR-15 a DR-17 rozhodnem v kroku 0002 (PREPARE_SPEC).
