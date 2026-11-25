# 0006 – DESIGN_CONTRACT – contract FR1 0.2.1 (DR-C12)

| Pole | Hodnota |
|---|---|
| Dátum | 2026-11-25 |
| Poradové číslo | 0006 |
| Režim | DESIGN_CONTRACT |
| Verzia šablóny promptu | spec/prompts/design-contract.md, prompt_template_version 1.0 |
| Specification baseline | 0.3.1 (approved) |
| Architecture baseline | B-01 |
| Model | Claude Opus 5.5 (`claude-opus-5-5`), Claude Code |
| Commit | (doplní človek) |

## Prompt

~~~text
# Úloha 0006: contract FR1 0.2.1

Mode: DESIGN_CONTRACT
Change: FR1
Specification baseline: 0.3.1 (approved)
Prompt template: spec/prompts/design-contract.md v1.0

Prečítaj spec/manifest.yaml a over, že:
- baseline je 0.3.1 so stavom approved,
- ADR-001 až ADR-007 sú Accepted (ADR-007 má doplnenie DR-C12),
- contract-design-profile.yaml je approved,
- v spec/drafts/open-decisions.md nie je otvorená žiadna DR.
Ak niečo z toho neplatí, zastav sa s DECISION REQUIRED.

## Úloha
1. Zapracuj DR-C12 (ADR-007 doplnenie, BR-011: token bez neprázdneho claimu „sub“ → 401 unauthenticated)
   do spec/contracts/proposed/openapi-reservations.yaml a contract-mapping.yaml ako verziu 0.2.1.
   Odstráň DR-C12 z open_decisions v mappingu. Nič iné v contracte nemeň.
2. Drobnosti z reportu 0.2.0 (komentáre „návrh 0004“, specification_version v lifecycles.yaml) iba vypíš, approved súbory nemeň.
3. Vytvor reports/contract-review-0.2.1.md:
   - zmena 0.2.0 → 0.2.1,
   - Human contract approval gate checklist (kap. 9 AGENTS.md): pri každom bode PASS / FAIL / UNKNOWN a dôkaz.
4. Over syntax OpenAPI 3.1 a príklady voči schémam (validátor iba v scratchpade, nie v projekte). Ak nie je dostupný, označ UNKNOWN.
5. Nevytváraj approved contract ani approval.yaml, nepíš kód ani testy, nemeň approved specification.
   Ak contract vyžaduje zmenu specification, zastav sa s DECISION REQUIRED.
6. Log podľa CLAUDE.md (0006-2026-11-25-DESIGN_CONTRACT.md). Necommituj.

Odpoveď štruktúruj podľa kapitoly 15 AGENTS.md a na konci jasne napíš, či je contract pripravený na ľudské schválenie.
~~~

## Výstup AI

**Vstupná kontrola:** baseline 0.3.1 approved – PASS; ADR-001 až ADR-007 Accepted, ADR-007 má doplnenie DR-C12 – PASS;
profil approved – PASS; žiadna otvorená DR – PASS.

**Zmenené (proposed, neschválené)**

- `spec/contracts/proposed/openapi-reservations.yaml` → 0.2.1: verzie (`info.version`, `x-sdd`: contract 0.2.1, previous 0.2.0,
  spec 0.3.1) a hlavičkové komentáre; poradie BR-012 pozícia 1 doplnené o token bez `sub`; popis odpovede 401: platný token bez
  neprázdneho `sub` → 401 bez ohľadu na `role` (ADR-007 / DR-C12, BR-012); pomenovaný príklad `unauthenticated-missing-sub`
  (request aj 401 response); veta v `bearerAuth.description`. Schémy, ostatné odpovede a poradie chýb bez zmeny.
- `spec/contracts/proposed/contract-mapping.yaml` → 0.2.1: verzie; `resolved_decisions` + DR-C12; `open_decisions: []`;
  `security.open` nahradené `security.missing_sub`; odpoveď 401: `outcome`, `examples`, `decisions` + ADR-007, DR-C12.

**Vytvorené:** `reports/contract-review-0.2.1.md` – vstupná kontrola, impact analysis, zmena 0.2.0 → 0.2.1, drobnosti
v podkladoch, approval gate, validácia.

**Validácia (iba scratchpad):** YAML PASS; OpenAPI 3.1 PASS (`openapi-spec-validator` 0.9.0); 37 príkladov vs. schémy PASS
(`jsonschema` 4.26, Draft 2020-12); pokrytie 16 SC PASS; poradie BR-012 a titulky PASS; 81 IDs existuje PASS; lint UNKNOWN.

**Approval gate:** body 1 – 10 PASS (bod 9 potvrdí človek – žiadna breaking change voči schválenej verzii).

**Drobnosti (iba vypísané, approved súbory nemenené):** N-01 komentáre „návrh 0004“ / „čakajú na kontrolu“ v approved súboroch;
N-02 `lifecycles.yaml` `specification_version: 0.2.0`; N-02a súbory uvádzajú 0.3.0, manifest 0.3.1; N-02b pätička ADR-007
„0.3.0 (draft)“; N-03 `traceability.yaml` `missing_links`; **N-09** veta DR-C12 je zapísaná v `BR-010.statement`, nie v BR-011
(ako uvádza open-decisions.md, ADR-007 aj zadanie) – význam zhodný, neblokuje; N-10 token bez `sub` nemá scenár v US-001.

**Otvorené DECISION REQUIRED:** žiadne.

**Pripravený na schválenie:** áno.

Approved specification, `spec/contracts/approved/`, approval.yaml, kód ani testy sa nemenili. Necommitnuté.

## Ľudské rozhodnutie
- **Rozhodol:** Martin Štefanko (Software architect), 2026-11-25
- **Rozhodnutie:** Contract API-Reservations 0.2.1 SCHVAĽUJEM. Approval gate: 10/10 PASS (reports/contract-review-0.2.1.md).
  Skopíroval som ho do spec/contracts/approved/ a vytvoril approval.yaml.
- **Bod 9:** potvrdzujem – zmena nie je breaking, predchádzajúca verzia nebola schválená ani implementovaná.
- **N-09:** opravené ručne (veta presunutá do BR-011).
- **Odložené:** N-10 (scenár pre token bez sub) a kozmetické nálezy N-01, N-02 – vyriešim v ďalšom PREPARE_SPEC.
