# 0003 – DESIGN_CONTRACT – návrh HTTP API contractu pre FR1

| Pole | Hodnota |
|---|---|
| Dátum | 2026-11-25 |
| Poradové číslo | 0003 |
| Režim | DESIGN_CONTRACT |
| Verzia šablóny promptu | spec/prompts/design-contract.md, prompt_template_version 1.0 |
| Specification baseline | 0.2.0 (approved) |
| Architecture baseline | B-01 |
| Model | Claude Opus 5.5 (`claude-opus-5-5`), Claude Code |
| Commit | (doplní človek) |

## Prompt

~~~text
# Úloha 0003: návrh contractu pre FR1

Mode: DESIGN_CONTRACT
Change: FR1
Specification baseline: 0.2.0 (approved)
Prompt template: spec/prompts/design-contract.md v1.0

Prečítaj spec/manifest.yaml a over, že baseline je 0.2.0 a ADR-001 až ADR-006 sú Accepted.
Ak niečo z toho neplatí, zastav sa s DECISION REQUIRED.

## Úloha
1. Priprav impact analysis podľa kap. 14 AGENTS.md.
2. Navrhni HTTP API contract pre FR1 podľa approved contract-design-profile.yaml (OpenAPI 3.1):
   - iba operácie, ktoré majú business dôvod v FR1 a scenároch US-001; každú operáciu zdôvodni Requirement ID,
   - vstupy, výstupy a chyby (problem-details) tak, aby každý scenár SC-001-01 až SC-001-14 mal pozorovateľný výsledok v contracte,
   - každý dôvod odmietnutia z BR-* namapuj na konkrétny HTTP status a typ chyby,
   - security podľa ADR-004 (JWT bearer, rola v claime „role“, iba ROLE-SpravcaNavstev),
   - X-Correlation-ID, čas v UTC (RFC 3339), UUID identifikátory podľa profilu,
   - poznámka sa nesmie objaviť v žiadnom popise logovania (CON-003).
3. Výstupy ulož iba do spec/contracts/proposed/:
   - openapi-reservations.yaml (contract, verzia 0.1.0, status proposed),
   - contract-mapping.yaml: každá operácia → Requirement, Application Service, Component, Business Service, Business Process,
     scenáre, rules, Data Objects (formát podľa kap. 16 šablóny).
4. Do reports/ ulož contract-review-0.1.0.md s vyplneným Human contract approval gate checklistom (kap. 9 AGENTS.md):
   pri každom bode PASS / FAIL / UNKNOWN a dôkaz.
5. Over syntax OpenAPI (ak validátor nie je dostupný, označ UNKNOWN, nevymýšľaj výsledok).
6. Nevytváraj approved contract ani approval.yaml, nepíš kód ani testy. Nemeň approved špecifikáciu.
   Ak contract vyžaduje zmenu specification, zastav sa s DECISION REQUIRED.
7. Vytvor log spec/prompts/log/0003-2026-11-25-DESIGN_CONTRACT.md podľa pravidla v CLAUDE.md. Necommituj.

Odpoveď štruktúruj podľa kapitoly 15 AGENTS.md.
~~~

## Výstup AI

**Vstupná kontrola:** baseline 0.2.0 approved – PASS; ADR-001 až ADR-006 Accepted – PASS; profil approved – PASS.

**Vytvorené**

- `spec/contracts/proposed/openapi-reservations.yaml` – OpenAPI 3.1, `API-Reservations` 0.1.0, status proposed.
  Jediná operácia `POST /api/v1/reservations` (`createReservation`). Odpovede: 201 Reservation;
  400 `invalid-request`; 401 `unauthenticated` (SC-001-13); 403 `forbidden` (SC-001-12);
  409 `slot-occupied` (BR-003/BR-009); 422 `outside-opening-hours` (BR-005), `start-not-in-future` (BR-006),
  `client-not-found` (BR-007), `note-too-long` (BR-008). Príklady sú pomenované podľa SC-ID a časy sú prepočítané do UTC.
- `spec/contracts/proposed/contract-mapping.yaml` – mapping podľa kap. 16 šablóny, rozšírený o mapping odpovedí a polí.
- `reports/contract-review-0.1.0.md` – vstupná kontrola, pokrytie 14 scenárov, mapovanie BR → HTTP, approval gate, validácia, DR-C01 až DR-C08.

**Validácia:** YAML PASS; OpenAPI 3.1 PASS (`openapi-spec-validator` 0.9.0, iba v scratchpade); príklady vs. schémy PASS;
všetky použité IDs existujú; lint (Spectral/Redocly) UNKNOWN.

**Approval gate:** 1, 2, 4, 5, 8 PASS; 3, 6, 7 UNKNOWN; 9 not applicable (potvrdí človek); 10 FAIL (otvorené DR-C).

**Zistenia v podkladoch (F-01 až F-03):** zastarané komentáre „Proposed“ pri ADR, chybná úvodzovka v `approved_by` profilu,
rozdiel medzi stavom baseline (approved) a stavom jednotlivých súborov (draft, `approved_by: null`).

**Otvorené DECISION REQUIRED:** DR-C01 (hodnota claimu role), DR-C02 (claim identity pre createdBy), DR-C03 (iba UTC „Z“?),
DR-C04 (poradie chýb pri viacerých porušeniach), DR-C05 (tvar X-Correlation-ID), DR-C06 (nedostupnosť úložiska / 5xx),
DR-C07 (konvencia názvov polí a typov problémov), DR-C08 (počítanie znakov poznámky).

## Ľudské rozhodnutie

- **Rozhodol:** Martin Štefanko (Software architect)
- **Dátum:** 2026-11-25
- **Rozhodnutie:** Proposed contract 0.1.0 som skontroloval. Zatiaľ ho NESCHVAĽUJEM, lebo approval gate má bod 10 FAIL (otvorené DR-C01 až DR-C08).
- **Rozhodnutia k DR-C01 až DR-C08:** rozhodol som ich a vraciam do specification v kroku 0004 (PREPARE_SPEC). Contract sa upraví v kroku 0005.
- **Nálezy:** F-02 som opravil ručne. F-01 a F-03 sa riešia v kroku 0004.

