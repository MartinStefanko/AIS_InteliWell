# Contract review – API-Reservations 0.2.1 (proposed)

| Pole | Hodnota |
|---|---|
| Režim | DESIGN_CONTRACT (úloha 0006, log `spec/prompts/log/0006-2026-11-25-DESIGN_CONTRACT.md`) |
| Contract | `spec/contracts/proposed/openapi-reservations.yaml`, verzia 0.2.1, status **proposed** |
| Mapping | `spec/contracts/proposed/contract-mapping.yaml` (0.2.1) |
| Predchádzajúca verzia | 0.2.0 (proposed, neschválená) – `reports/contract-review-0.2.0.md` |
| Specification baseline | 0.3.1 (manifest: approved) / architecture baseline B-01 |
| Profil | `spec/policies/contract-design-profile.yaml` (status approved) |
| Pripravila | AI (Claude Opus 5.5). Report nie je schválenie. |

## 1. Vstupná kontrola

| Kontrola | Výsledok | Dôkaz |
|---|---|---|
| Baseline 0.3.1 approved | PASS | `spec/manifest.yaml`: `specification.version: "0.3.1"`, `status: "approved"`; log 0005 „Ľudské rozhodnutie“: „specification 0.3.1“; commit 7705cdf |
| ADR-001 až ADR-007 Accepted | PASS | sekcia `## Status` = `Accepted` vo všetkých 7 súboroch `spec/architecture/decisions/ADR-00[1-7]-*.md` |
| ADR-007 má doplnenie DR-C12 | PASS | ADR-007, sekcia „Doplnenie 2026-11-25 (DR-C12)“: token bez neprázdneho `sub` → neprihlásená osoba → 401 unauthenticated |
| Contract design profile approved | PASS | `contract_design_profile.status: "approved"`, `approved_by` aj `reviewed_by` vyplnené |
| Otvorené DR v specification | PASS | `spec/drafts/open-decisions.md` r. 10: „Žiadna DR nie je otvorená.“; DR-C12 je v tabuľke „Rozhodnuté k contract review 0.2.0“ |

## 2. Impact analysis (AGENTS.md kap. 14)

```yaml
change:
  mode: "DESIGN_CONTRACT"
  requirement: "FR1"
  specification_baseline: "0.3.1"
  reason: >
    Zapracovanie DR-C12 (ADR-007 doplnenie 2026-11-25): platný bearer token bez neprázdneho claimu sub
    sa posudzuje ako neprihlásená osoba → 401 unauthenticated. Contract 0.2.0 → 0.2.1.

affected:
  goals: [] # bez zmeny
  actors_roles: ["ROLE-SpravcaNavstev"] # iba spresnenie, kedy je osoba „prihlásená“ (NFR4)
  business_processes: [] # bez zmeny
  business_services: [] # bez zmeny
  application_services: ["AS-SpravaRezervaciiAKlientov"] # bez zmeny poskytovateľa ani operácií
  application_components: ["AC-ModulRezervaciiAKlientov"] # bez zmeny
  interfaces_contracts: ["API-Reservations 0.2.0 → 0.2.1 (proposed)"]
  data_objects: [] # createdBy (DO-Rezervacie) sa nemení; DR-C12 iba zaručuje, že sub existuje pred uložením
  technology_elements: [] # bez zmeny
  work_packages_deliverables: [] # až IMPLEMENT_CHANGE
  decisions: ["ADR-007 (doplnenie DR-C12)"]
  scenarios: ["SC-001-13"] # 401; nový scenár pre token bez sub v US-001 nie je – N-10
  tests: [] # vzniknú v IMPLEMENT_CHANGE

open_questions: []
```

## 3. Zmena 0.2.0 → 0.2.1

| Miesto | 0.2.0 | 0.2.1 | Opora |
|---|---|---|---|
| Verzie (`info.version`, `x-sdd`, mapping) | contract 0.2.0, previous 0.1.0, spec 0.3.0 | contract 0.2.1, previous 0.2.0, spec 0.3.1 | manifest |
| Hlavičkové komentáre oboch súborov | úloha 0005, review 0.2.0 | úloha 0006, review 0.2.1 | – |
| Popis operácie, poradie BR-012 pozícia 1 | „401 `unauthenticated` – osoba nie je prihlásená (BR-011)“ | + „vrátane tokenu bez neprázdneho claimu `sub` (BR-011, ADR-007 / DR-C12)“ | ADR-007 doplnenie |
| Odpoveď 401 – popis | chýba/neplatný token | + platný token bez neprázdneho `sub` → 401 bez ohľadu na claim `role` | ADR-007 doplnenie; „bez ohľadu na `role`“ vyplýva z BR-012 (pozícia 1 pred 2) |
| Odpoveď 401 – príklady | `SC-001-13` | + `unauthenticated-missing-sub` (request aj response, bez scenára v US-001 – rovnaký vzor ako `invalid-request-time-offset`) | DR-C12 |
| `securitySchemes.bearerAuth.description` | rola a `sub` pre `createdBy` | + veta o tokene bez `sub` → 401 | ADR-007 doplnenie |
| Mapping `contract.resolved_decisions` | DR-C01 – DR-C11 | DR-C01 – DR-C12 | open-decisions.md |
| Mapping `contract.open_decisions` | `["DR-C12"]` | `[]` | – |
| Mapping `security` | `open:` „správanie … nie je určené – DR-C12“ | `missing_sub:` „… → 401 unauthenticated (ADR-007 / DR-C12; BR-011; BR-012 pozícia 1)“ | ADR-007 |
| Mapping odpoveď 401 | `decisions: [ADR-004]` | + `outcome`, `examples` + `unauthenticated-missing-sub`, `decisions: [ADR-004, ADR-007, DR-C12]` | ADR-007 |

Bez zmeny: operácia, cesta, schémy (`Reservation`, `CreateReservationRequest`, `ProblemDetails`, `UtcDateTime`), enumy,
ostatné odpovede, poradie chýb, hlavičky. Nová ani zmenená odpoveď či kód chyby nevznikli (401 a jeho `type` existovali).
Zmena nie je breaking: spresňuje dovtedy neurčený prípad.

## 4. Drobnosti v podkladoch (iba vypísané; approved súbory AI nemenila)

| ID | Zistenie | Miesto | Návrh pre človeka |
|---|---|---|---|
| N-01 | Zastarané komentáre „návrh 0004“ / „čakajú na kontrolu človekom“, hoci hodnoty sú approved | `spec/manifest.yaml` r. 5, 8, 9; `business-rules.yaml` r. 7, 14; `quality-requirements.yaml` r. 5, 12; `requirements.yaml` r. 13; `traceability.yaml` r. 11; `glossary.md` r. 7; `spec/stories/US-001.md` r. 7; `contract-design-profile.yaml` r. 4, 11 | odstrániť komentáre pri ďalšom PREPARE_SPEC alebo ručne |
| N-02 | `specification_version` v lifecycles.yaml je „0.2.0“, obsah (`api_value: created`, DR-C11) zodpovedá 0.3.x | `spec/application/lifecycles.yaml` r. 10 | zjednotiť metadátum |
| N-02a | Manifest uvádza 0.3.1, ale `specification_version` v súboroch je „0.3.0“ (vrátane `business-rules.yaml`, ktorý DR-C12 menil) | súbory z N-01 | rozhodnúť, či súbory nesú verziu balíka, alebo verziu poslednej zmeny |
| N-02b | Pätička ADR-007 uvádza „0.3.0 (draft)“ | `ADR-007…md` r. 54 | zmeniť na 0.3.1 |
| N-03 | `traceability.yaml` `missing_links`: „FR1 → approved contract … úprava v 0005“ | `spec/traceability/traceability.yaml` | aktualizovať až po schválení contractu |
| N-09 | Znenie DR-C12 je zapísané do `BR-010.statement` (r. 152), kým `open-decisions.md` (r. 18), ADR-007 a zadanie úlohy uvádzajú BR-011 | `spec/requirements/business-rules.yaml` BR-010 | presunúť vetu do BR-011, alebo opraviť odkaz v open-decisions.md. Význam je vo všetkých zdrojoch rovnaký (401), preto contract neblokuje. Mapping cituje BR-011 (pravidlo pre neprihlásenú osobu) a uvádza miesto textu v komentári |
| N-10 | Pre token bez `sub` neexistuje scenár v US-001.feature | `spec/stories/features/US-001.feature` | odporúčanie: doplniť negatívny scenár (napr. SC-001-17) v PREPARE_SPEC; dovtedy ho nahrádza pomenovaný príklad `unauthenticated-missing-sub` |

## 5. Human contract approval gate (AGENTS.md kap. 9)

| # | Podmienka | Výsledok | Dôkaz |
|---|---|---|---|
| 1 | Každá operation má business dôvod a Requirement ID | PASS | 1 operácia `createReservation`; `x-sdd.requirements: [FR1, NFR1, NFR4, QR-001, QR-002]`; `business_reason` v mappingu |
| 2 | Každý prvok mapuje na Application Service a poskytujúci Component | PASS | AS-SpravaRezervaciiAKlientov / AC-ModulRezervaciiAKlientov (`service-realization.yaml`, approved). Obmedzenie bez zmeny oproti 0.2.0: views majú `reviewed_by: null`, väzbu potvrdzuje approved textový podklad, nie overenie modelu v Sparx EA |
| 3 | Názvy používajú glossary | PASS | Polia a enum anglicky (DR-C07, DR-C11) s priradením k pojmom glossary v mappingu (`request.fields`, `response_fields`); titulky chýb = `reason` z BR-012 (pre 400 bez vysvetľujúcej zátvorky); 0.2.1 nepridala nový názov |
| 4 | Pozitívne aj negatívne scenarios majú pozorovateľný výsledok | PASS | 16/16 scenárov SC-001-01 – SC-001-16 má request aj response príklad s rovnakým názvom (skript, časť 7). Nový prípad DR-C12 má pozorovateľný výsledok 401 + príklad `unauthenticated-missing-sub`; scenár chýba (N-10, neblokuje – rovnako ako 400) |
| 5 | Rules, invariants a lifecycle transitions sú vynútiteľné alebo overiteľné | PASS | BR-001 – BR-012 → odpovede (`contract-review-0.2.0.md` časť 5, bez zmeny); invariant BR-003 aj pri súbehu → 409; prechod (none) → `vytvorena` / `created`; DR-C12 overiteľné tokenom bez `sub` → 401 |
| 6 | Authentication, authorization, audit a citlivé údaje sú explicitné | PASS | JWT bearer (ADR-004); `role` = `SpravcaNavstev`, iná/chýbajúca → 403 (ADR-007 / DR-C01); **token bez neprázdneho `sub` → 401 (ADR-007 / DR-C12)**; audit iba úspechu, `createdBy` = `sub` (QR-001, DR-26, DR-C02); poznámka `x-sdd-personal-data`, `x-sdd-excluded-from-application-log` (CON-003) |
| 7 | Error behavior a retry/idempotency sú jasné | PASS | Poradie BR-012 = mapping `error_order` (skript); 503 bez automatického opakovania (QR-002); neidempotentné, opakovanie → 409 (DR-28); `X-Correlation-ID` chýbajúce/neplatné → UUID (DR-C05, DR-C09); čas bez `Z` → 400 (DR-C03); token bez `sub` → 401 (DR-C12) |
| 8 | Project policy je dodržaná | PASS | `policy_rules_used` v mappingu (bez zmeny oproti 0.2.0); 0.2.1 nepridala pole, cestu ani typ chyby |
| 9 | Breaking changes majú compatibility alebo migration rozhodnutie | PASS (potvrdí človek) | 0.2.0 → 0.2.1 nie je breaking (spresnenie neurčeného prípadu, existujúci kód 401). Žiadna predchádzajúca verzia nebola schválená ani implementovaná: `spec/contracts/approved/` je prázdny, manifest `approved_contracts: null`, `realization.code: null`. Politika pre budúce zmeny: profil `compatibility` (DR-31) |
| 10 | Všetky DECISION REQUIRED relevantné pre contract sú uzavreté | PASS | mapping `open_decisions: []`; DR-C01 – DR-C12 v `resolved_decisions`; `open-decisions.md` r. 10 „Žiadna DR nie je otvorená.“ |

## 6. DECISION REQUIRED

Žiadna nová DECISION REQUIRED. Zistenia N-01 – N-10 sú redakčné a neblokujú schválenie contractu. N-09 nie je rozpor vo význame:
ADR-007, BR-010.statement a open-decisions.md zhodne určujú 401; líši sa iba miesto zápisu.

## 7. Validácia

Nástroje boli nainštalované iba do scratchpadu (`pip --target`), nie do projektu.

| Kontrola | Výsledok | Dôkaz |
|---|---|---|
| YAML syntax oboch súborov | PASS | `yaml.safe_load` (PyYAML 6.0) bez chyby |
| OpenAPI 3.1 schema | PASS | `openapi-spec-validator` 0.9.0, `validate()` bez chyby |
| Príklady vs. schémy | PASS | 37 príkladov (18 request, 19 response) overených `jsonschema` 4.26, Draft 2020-12, format checker (`date-time`, `uuid`); `invalid-request-time-offset` podľa očakávania porušuje `pattern: 'Z$'`; `status` v problem details = HTTP kód odpovede |
| Pokrytie scenárov | PASS | všetkých 16 SC z `US-001.feature` má request aj response príklad; žiadny request príklad bez response príkladu |
| Poradie BR-012 | PASS | mapping `error_order` = `BR-012.order` (pozícia, HTTP, kód) |
| Titulky chýb | PASS | `title` všetkých problem details príkladov = `reason` z BR-012; pri 400 je `reason` „neplatná požiadavka (tvar vstupu, vrátane času mimo UTC – DR-C03)“ a `title` „neplatná požiadavka“ – zátvorka je vysvetlenie, nie súčasť pomenovania (bez zmeny od 0.2.0) |
| Existencia IDs | PASS | 81 IDs z contractu a mappingu existuje v `spec/` (mimo `contracts/` a logov); nové ID nevzniklo |
| Rozsah zmeny | PASS | `diff` 0.2.0 → 0.2.1: iba miesta z časti 3 |
| Lint (Spectral/Redocly) | UNKNOWN | nástroj nebol spustený |

## 8. Záver

Contract 0.2.1 zapracúva DR-C12 a ostatný obsah 0.2.0 nemení. Všetkých 10 bodov approval gate je PASS
(bod 9 potvrdzuje človek). **Contract je pripravený na ľudské schválenie.** Schválenie (presun do `spec/contracts/approved/`,
záznam o schválení) robí iba človek. Odporúčané pred alebo po schválení: N-09 a N-10 v ďalšom PREPARE_SPEC.

Report nemení specification, stav contractu ani rozhodnutia.
