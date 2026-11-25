# Contract review – API-Reservations 0.2.0 (proposed)

| Pole | Hodnota |
|---|---|
| Režim | DESIGN_CONTRACT (úloha 0005, log `spec/prompts/log/0005-2026-11-25-DESIGN_CONTRACT.md`) |
| Contract | `spec/contracts/proposed/openapi-reservations.yaml`, verzia 0.2.0, status **proposed** |
| Mapping | `spec/contracts/proposed/contract-mapping.yaml` (0.2.0) |
| Predchádzajúca verzia | 0.1.0 (proposed, neschválená) – `reports/contract-review-0.1.0.md` |
| Specification baseline | 0.3.0 (manifest: approved) / architecture baseline B-01 |
| Profil | `spec/policies/contract-design-profile.yaml` (status approved) |
| Pripravila | AI (Claude Opus 5.5). Report nie je schválenie. |

## 1. Vstupná kontrola

| Kontrola | Výsledok | Dôkaz |
|---|---|---|
| Baseline 0.3.0 approved | PASS | `spec/manifest.yaml`: `specification.version: "0.3.0"`, `status: "approved"`; log 0004 „Ľudské rozhodnutie“: „Specification 0.3.0 schvaľujem“; commit 5d53770 |
| ADR-001 až ADR-007 Accepted | PASS | sekcia `## Status` = `Accepted` vo všetkých 7 súboroch `spec/architecture/decisions/ADR-00[1-7]-*.md` |
| Contract design profile approved | PASS | `contract_design_profile.status: "approved"`, `approved_by` aj `reviewed_by` vyplnené |
| Otvorené DR v specification | PASS | `spec/drafts/open-decisions.md` r. 10: „Žiadna DR nie je otvorená.“; log 0004: „nič neblokuje DESIGN_CONTRACT“ |

Zistenia v podkladoch (neblokujú contract; na opravu človekom alebo v ďalšom PREPARE_SPEC):

- **N-01 Zastarané komentáre „návrh 0004“.** Hodnoty sú approved, ale komentáre stále uvádzajú návrh: `spec/manifest.yaml`
  (r. 5, 8, 9), `business-rules.yaml`, `quality-requirements.yaml`, `requirements.yaml`, `traceability.yaml`
  (`specification_version: "0.3.0" # návrh 0004`), `glossary.md`, hlavička profilu („čakajú na kontrolu človekom“),
  `open-decisions.md` r. 4. Pätička ADR-007 uvádza „0.3.0 (draft)“. Platí hodnota `status`, nie komentár.
- **N-02 Verzia v lifecycles.yaml.** `meta.specification_version: "0.2.0"`, hoci `api_value: "created"` (DR-C11) pribudlo
  v 0004. Obsah zodpovedá 0.3.0; ide iba o metadátum.
- **N-03 Traceability.** `traceability.yaml` `missing_links` uvádza „FR1 → approved contract … úprava v 0005“. Po schválení
  contractu treba stopu aktualizovať (AI approved specification nemení).

## 2. Impact analysis (AGENTS.md kap. 14) – zmeny oproti 0.1.0

```yaml
change:
  mode: "DESIGN_CONTRACT"
  requirement: "FR1"
  specification_baseline: "0.3.0"
  reason: >
    Revízia proposed contractu API-Reservations 0.1.0 → 0.2.0 podľa DR-C01 až DR-C11, ktoré sú zapísané
    v approved specification 0.3.0 (ADR-007, BR-012, QR-002, SC-001-15, SC-001-16, profil).

affected:
  goals: ["G-001"] # bez zmeny
  actors_roles: ["ACT-Recepcna", "ROLE-SpravcaNavstev"] # hodnota claimu role SpravcaNavstev (ADR-007)
  business_processes: ["BP-SpravaNavstev"] # kroky -01, -02, -04; bez zmeny
  business_services: ["BS-RezervaciaTerminu"] # bez zmeny
  application_services: ["AS-SpravaRezervaciiAKlientov"] # bez zmeny
  application_components: ["AC-ModulRezervaciiAKlientov"] # bez zmeny
  interfaces_contracts: ["API-Reservations 0.1.0 → 0.2.0 (proposed)"]
  data_objects: ["DO-Rezervacie", "DO-EvidenciaKlientov"] # stav v API created (DR-C11); dostupnosť úložiska (QR-002)
  technology_elements: [] # ADR-003 iba ako súvisiace rozhodnutie QR-002; contract technológiu nemení
  work_packages_deliverables: [] # až IMPLEMENT_CHANGE
  decisions: ["ADR-001", "ADR-004", "ADR-005", "ADR-006", "ADR-007"] # nové: ADR-007
  scenarios: ["SC-001-01 až SC-001-14", "SC-001-15 (nový)", "SC-001-16 (nový)"]
  tests: [] # vzniknú v IMPLEMENT_CHANGE

open_questions: ["DR-C12"] # časť 7
```

Nové requirements a rules v contracte: QR-002, BR-012. Žiadny nový element modelu ani nová operácia.

## 3. Porovnanie 0.1.0 → 0.2.0

| Oblasť | 0.1.0 | 0.2.0 | Opora |
|---|---|---|---|
| Verzie | contract 0.1.0, spec 0.2.0 | contract 0.2.0, spec 0.3.0, `previous_version: 0.1.0` | manifest |
| Requirements / rules / ADR | FR1, NFR1, NFR4, QR-001; BR-001–011; ADR-001, 004–006 | + QR-002; + BR-012; + ADR-007 | requirements.yaml, BR-012, QR-002, ADR-007 |
| Scenáre | SC-001-01 – 14 | SC-001-01 – 16 | US-001.feature |
| Poradie chýb | 401 → 403 → 400 → 422 → 409, označené „návrh – DR-C04“ | 401 → 403 → 400 → **503** → 422 (client-not-found → start-not-in-future → outside-opening-hours → note-too-long) → 409; mapping `error_order` | BR-012.order (DR-C04, DR-C10) |
| 503 | neexistovala | nová odpoveď `service-unavailable`, „služba je dočasne nedostupná“, bez automatického opakovania; príklad SC-001-15 | QR-002 (DR-C06) |
| `ProblemDetails` | `type` 8 URN, `status` 400/401/403/409/422 | + `urn:intelliwell:problem:service-unavailable`, + 503; poradie enum podľa BR-012 | QR-002, DR-C07 |
| Titulky chýb | 401/403/400 „návrh AI“ | všetky titulky = `reason` z BR-012 (overené skriptom) | BR-012, BR-010, QR-002 |
| Čas | `format: date-time`, „tvar návrh – DR-C03“ | schéma `UtcDateTime` = `date-time` + `pattern: 'Z$'` pre `startTime`, `endTime`, `createdAt`; `+01:00` → 400 (príklad `invalid-request-time-offset`) | profil `http.date_time_input` (DR-C03) |
| Poznámka (request) | `maxLength: 500`, „počítanie návrh – DR-C08“ | `maxLength` **odstránený** zo vstupu; `x-sdd-business-max-length: 500`, `x-sdd-length-unit: unicode-code-point`; prekročenie → 422 | BR-008 (DR-C08), BR-012 poz. 8 – pozri N-04 |
| Poznámka (response) | `maxLength: 500` | `maxLength: 500` (JSON Schema počíta code points) | BR-008, DR-C08 |
| `status` | `enum: [vytvorena]` | `enum: [created]` | lifecycles.yaml `api_value`, profil `enum_values` (DR-C11) |
| Claim roly (403) | `SpravcaNavstev` (návrh – DR-C01) | `SpravcaNavstev`; iná hodnota alebo chýbajúci claim → 403 | ADR-007 |
| `createdBy` | claim `sub` (návrh – DR-C02) | claim `sub`; `minLength: 1` | ADR-007, QR-001 |
| `X-Correlation-ID` (request) | `type: string`, tvar nerozhodnutý | popis: platné 1–128 znakov 0x21–0x7E, prázdna hodnota neplatná; chýbajúca/neplatná → UUID, bez odmietnutia; schéma úmyselne bez obmedzenia (inak by neplatná hodnota viedla k 400); `x-sdd-valid-format` | profil `correlation_id_*` (DR-C05, DR-C09) |
| `X-Correlation-ID` (response) | `type: string` | `pattern: '^[\x21-\x7E]{1,128}$'` | DR-C05, DR-C09 |
| Pomenované príklady | 14 request, 9 response | 17 request, 18 response; každý SC má request aj response príklad s rovnakým názvom; poznámky v SC-001-11/16 majú skutočne 501 znakov | časť 4 |
| Značky „(návrh – DR-Cxx)“ | 9 výskytov | 0 | – |
| Mapping | – | nové sekcie `error_order`, `response_fields`, `correlation_id`, `policy_rules_used`, `examples` pri odpovediach; `open_decisions: [DR-C12]` | – |
| Operácie, cesta, autentifikácia | `POST /api/v1/reservations`, JWT bearer | bez zmeny | ADR-004, ADR-005, DR-31 |

**N-04 Prečo `maxLength` vypadol zo vstupu.** BR-012 dáva `note-too-long` (422) na pozíciu 8 a `invalid-request` (400) na pozíciu 3.
Ak by vstupná schéma mala `maxLength: 500`, implementácia validujúca schému by 501-znakovú poznámku odmietla ako 400. SC-001-16
by potom namiesto `client-not-found` vrátil 400. Limit preto zostáva ako business pravidlo v popise a v `x-sdd-*` rozšíreniach.

**N-05 Doslovné „Z“.** `pattern: 'Z$'` vyžaduje veľké `Z` podľa doslovného znenia DR-C03 („s príponou „Z““). RFC 3339 pripúšťa aj
malé `z`, contract ho teda odmietne ako 400. Ak to človek chce inak, ide o zmenu profilu, nie contractu.

## 4. Pokrytie scenárov

Operácia `POST /api/v1/reservations` (`createReservation`). Časy scenárov sú v Europe/Bratislava (december = CET = UTC+1).
Názov request aj response príkladu = ID scenára.

| Scenár | Vstup (UTC) | Pozorovateľný výsledok v contracte | Príklad odpovede | Rules |
|---|---|---|---|---|
| SC-001-01 | 2026-12-01T13:00Z, note | 201 `Reservation`: `status: created`, `endTime` 14:00Z, `createdBy: jana`, `createdAt` | 201 `SC-001-01` | BR-001, 002, 004, 008, QR-001 |
| SC-001-02 | 2026-12-02T10:00Z, bez note | 201, pole `note` chýba | 201 `SC-001-02` | BR-008 |
| SC-001-03 | 2026-12-03T20:00Z | 201, `endTime` 21:00Z (= 22:00 miestneho času) | 201 `SC-001-03` | BR-005 |
| SC-001-04 | 2026-12-01T13:00Z (obsadené) | 409 `slot-occupied` „termín obsadený“; iný termín sa neponúka; pôvodná rezervácia sa nemení | 409 `SC-001-04` | BR-003, BR-010 |
| SC-001-05 | 2026-12-01T13:30Z | 409 `slot-occupied` | 409 `SC-001-05` | BR-003, BR-010 |
| SC-001-06 | 2× 2026-12-04T15:00Z súčasne | prvá uložená 201, druhá 409 `slot-occupied` | 201 `SC-001-06`, 409 `SC-001-06` | BR-003, BR-009, ADR-001 |
| SC-001-07 | neznámy `clientId` | 422 `client-not-found` | 422 `SC-001-07` | BR-007, BR-010 |
| SC-001-08 | 2026-12-01T20:30Z | 422 `outside-opening-hours` | 422 `SC-001-08` | BR-005, BR-010 |
| SC-001-09 | 2026-12-01T08:30Z | 422 `outside-opening-hours` | 422 `SC-001-09` | BR-005, BR-010 |
| SC-001-10 | 2026-11-25T10:00Z (teraz 11:00Z) | 422 `start-not-in-future` | 422 `SC-001-10` | BR-006, BR-010 |
| SC-001-11 | note 501 code points | 422 `note-too-long` | 422 `SC-001-11` | BR-008, BR-010 |
| SC-001-12 | token bez `role: SpravcaNavstev` | 403 `forbidden`; rezervácia nevznikne | 403 `SC-001-12` | BR-011, ADR-007 |
| SC-001-13 | bez tokenu | 401 `unauthenticated` + `WWW-Authenticate: Bearer`; rezervácia nevznikne | 401 `SC-001-13` | BR-011 |
| SC-001-14 | 2026-12-01T14:00Z po 13:00–14:00Z | 201, interval [14:00Z, 15:00Z) | 201 `SC-001-14` | BR-003, BR-004 |
| SC-001-15 | 2026-12-05T13:00Z, úložisko nedostupné | 503 `service-unavailable` „služba je dočasne nedostupná“; rezervácia sa neuloží, bez automatického opakovania; po obnovení je termín voľný | 503 `SC-001-15` | QR-002 |
| SC-001-16 | neznámy klient, 2026-11-25T08:00Z (minulosť aj pred otvorením), note 501 | 422 **iba** `client-not-found` (BR-012 pozícia 5 pred 6, 7, 8) | 422 `SC-001-16` | BR-012, BR-010 |

Poznámka: Then-kroky o stave termínu („zostane voľný“, „po obnovení dostupnosti je termín voľný“, „existuje iba pôvodná
rezervácia“) contract vyjadruje garanciou „rezervácia nevznikne / sa neuloží“ pri každom odmietnutí. Operácia na čítanie
rezervácií v contracte nie je (bez business dôvodu v FR1). Acceptance testy môžu stav overiť ďalším volaním tej istej operácie
(očakávaná 201). Odpoveď 400 nemá scenár v US-001; má pomenovaný príklad `invalid-request-time-offset`.

## 5. Prevod BR na HTTP (poradie BR-012)

| Poz. | Rule | Dôvod (`title`) | HTTP | `type` | Scenár |
|---|---|---|---|---|---|
| 1 | BR-011 (neprihlásený) | osoba nie je prihlásená | 401 | `urn:intelliwell:problem:unauthenticated` | SC-001-13 |
| 2 | BR-011 (iná rola), ADR-007 | nedostatočné oprávnenie | 403 | `urn:intelliwell:problem:forbidden` | SC-001-12 |
| 3 | NFR1 (tvar), DR-C03 | neplatná požiadavka | 400 | `urn:intelliwell:problem:invalid-request` | – |
| 4 | QR-002 | služba je dočasne nedostupná | 503 | `urn:intelliwell:problem:service-unavailable` | SC-001-15 |
| 5 | BR-007 | klient neexistuje v evidencii klientov | 422 | `urn:intelliwell:problem:client-not-found` | SC-001-07, SC-001-16 |
| 6 | BR-006 | začiatok termínu nie je v budúcnosti | 422 | `urn:intelliwell:problem:start-not-in-future` | SC-001-10 |
| 7 | BR-005 | termín mimo otváracích hodín | 422 | `urn:intelliwell:problem:outside-opening-hours` | SC-001-08, SC-001-09 |
| 8 | BR-008 | poznámka je dlhšia ako 500 znakov | 422 | `urn:intelliwell:problem:note-too-long` | SC-001-11 |
| 9 | BR-003 / BR-009 | termín obsadený | 409 | `urn:intelliwell:problem:slot-occupied` | SC-001-04, 05, 06 |

BR-001 (overenie pred uložením) sa prejaví pozíciami 6, 7 a 9. BR-002 a BR-004 sa prejavia v odpovedi 201 (`endTime`,
následná 409). BR-010 (pomenovaný dôvod, bez ponuky iného termínu) platí pre všetky odpovede 4xx a 503.

## 6. Human contract approval gate (AGENTS.md kap. 9)

| # | Podmienka | Výsledok | Dôkaz |
|---|---|---|---|
| 1 | Každá operation má business dôvod a Requirement ID | PASS | 1 operácia `createReservation`; `x-sdd.requirements: [FR1, NFR1, NFR4, QR-001, QR-002]`; `business_reason` v mappingu |
| 2 | Každý prvok mapuje na Application Service a poskytujúci Component | PASS | AS-SpravaRezervaciiAKlientov / AC-ModulRezervaciiAKlientov (service-realization.yaml, approved). Views majú stále `reviewed_by: null` a typy väzieb nie sú overené v Sparx EA (`model_relationship_types_verified: false`). Väzbu potvrdzuje approved textový podklad, nie overenie modelu |
| 3 | Názvy používajú glossary | PASS | Polia anglicky lowerCamelCase (DR-C07), enum anglicky (DR-C11); `request.fields` a `response_fields` v mappingu priraďujú každé pole k pojmu glossary; `created` ↔ „vytvorená“ (glossary Rezervácia, lifecycles `api_value`); titulky chýb = BR-012 `reason` |
| 4 | Pozitívne aj negatívne scenarios majú pozorovateľný výsledok | PASS | 16/16 scenárov (4 pozitívne, 11 negatívnych, SC-001-06 má pozitívny aj negatívny výsledok) má HTTP status, telo a pomenovaný príklad (časť 4; skript: „COVER“ pre všetky SC) |
| 5 | Rules, invariants a lifecycle transitions sú vynútiteľné alebo overiteľné | PASS | BR-001 – BR-012 → odpovede (časť 5); invariant BR-003 aj pri súbehu → 409 (ADR-001); prechod (none) → `vytvorena` / `created`; BR-012 overiteľné cez SC-001-16 |
| 6 | Authentication, authorization, audit a citlivé údaje sú explicitné | UNKNOWN | JWT bearer (ADR-004); `role` = `SpravcaNavstev`, chýbajúci alebo iný claim → 403 (ADR-007); audit iba úspechu, `createdBy` = `sub` (QR-001, DR-26, ADR-007); poznámka `x-sdd-personal-data`, `x-sdd-excluded-from-application-log` (CON-003). **Chýba:** výsledok pre platný token bez claimu `sub` – DR-C12 |
| 7 | Error behavior a retry/idempotency sú jasné | UNKNOWN | Poradie BR-012 vrátane 503 (PASS); 503 bez automatického opakovania (QR-002); neidempotentné, opakovanie → 409 (DR-28); `X-Correlation-ID` pri chýbajúcej/neplatnej hodnote → UUID (DR-C05, DR-C09); čas bez `Z` → 400 (DR-C03). **Chýba:** error behavior pre token bez `sub` – DR-C12 (ten istý bod ako 6) |
| 8 | Project policy je dodržaná | PASS | Použité pravidlá profilu sú vypísané v `policy_rules_used` v mappingu: OpenAPI 3.1; `/api/v1/reservations`; `operationId` lowerCamelCase; polia lowerCamelCase; enum anglicky; RFC 3339 UTC + `Z`; problem details + URN; `X-Correlation-ID` v každej odpovedi; UUID; bez idempotencie; CON-003 |
| 9 | Breaking changes majú compatibility alebo migration rozhodnutie | not applicable (potvrdí človek) | 0.1.0 → 0.2.0 obsahuje nekompatibilné zmeny (`vytvorena` → `created`, len `Z`, nová 503). Verzia 0.1.0 však nebola schválená ani implementovaná: `spec/contracts/approved/` je prázdny, manifest `approved_contracts: null`, `realization.code: null`. Politika pre budúce zmeny: profil `compatibility` (DR-31) |
| 10 | Všetky DECISION REQUIRED relevantné pre contract sú uzavreté | FAIL | DR-C01 – DR-C11 sú uzavreté a zapracované. Otvorená je **DR-C12** (časť 7) |

## 7. DECISION REQUIRED

### DR-C12

```text
DECISION REQUIRED

Kontext:
FR1, NFR4, QR-001, BR-011, BR-012; ADR-004, ADR-007 (Accepted); contract API-Reservations 0.2.0
(Reservation.createdBy, odpovede 401/403); baseline 0.3.0 / B-01

Čo specification neurčuje alebo v čom si odporuje:
ADR-007 určuje, že autor rezervácie je hodnota claimu `sub`, ale nikde nie je určené, ako systém odpovie na
platný a správne podpísaný token s rolou SpravcaNavstev, ktorý claim `sub` nemá alebo ho má prázdny. QR-001
pritom vyžaduje autora pri každej uloženej rezervácii.

Možnosti:
A. Token bez neprázdneho `sub` je neplatný na autentifikáciu → 401 `unauthenticated` (BR-012 pozícia 1).
   Zodpovedá NFR4: každý zásah je priradený konkrétnej prihlásenej osobe.
B. Token je platný, ale osoba nemá oprávnenie uložiť rezerváciu → 403 `forbidden` (BR-012 pozícia 2).
C. Prípad sa nerieši v contracte, lebo vydávanie tokenov (dotnet user-jwts) `sub` vždy vyplní. Dôsledok: správanie
   neurčuje contract a contract testy ho neoveria.

Odporúčanie AI:
A – bez identity osoby nejde o prihlásený personál podľa NFR4 a nevznikne nový kód chyby. Ide iba o odporúčanie.

Otázka pre človeka:
Akú odpoveď vráti vytvorenie rezervácie, ak bearer token nemá neprázdny claim `sub`?

Kam zapísať rozhodnutie:
ADR (doplnenie ADR-007 alebo nové ADR) + Requirement (NFR4 / QR-001); potom contract 0.2.x (popis 401 alebo 403)
```

Contract 0.2.0 tento prípad zámerne neopisuje (žiadny text bez opory v specification). Po rozhodnutí je zmena v contracte
malá: jedna veta v popise 401 alebo 403 a jeden riadok v mappingu.

## 8. Validácia

Nástroje boli nainštalované iba do scratchpadu (`pip --target`), nie do projektu.

| Kontrola | Výsledok | Dôkaz |
|---|---|---|
| YAML syntax oboch súborov | PASS | `yaml.safe_load` (PyYAML) bez chyby |
| OpenAPI 3.1 schema | PASS | `openapi-spec-validator` 0.9.0, `validate()` bez chyby |
| Príklady vs. schémy | PASS | 35 príkladov (17 request, 18 response) overených `jsonschema` 4.26 Draft 2020-12 s format checkerom (`date-time`, `uuid`); `invalid-request-time-offset` podľa očakávania porušuje `pattern: 'Z$'`; `status` v problem details = HTTP kód odpovede |
| Pokrytie scenárov | PASS | každý SC-001-01 – SC-001-16 z `US-001.feature` má request aj response príklad s rovnakým názvom |
| Poradie BR-012 | PASS | mapping `error_order` = `BR-012.order` (pozícia, HTTP, kód); popis operácie obsahuje všetkých 9 kódov; titulky všetkých príkladov = `reason` z BR-012 |
| Dĺžka poznámky v príkladoch | PASS | SC-001-11 a SC-001-16: 501 Unicode code points |
| Formát `X-Correlation-ID` (response) | PASS | pattern prijme UUID, 1 aj 128 znakov; odmietne prázdnu hodnotu, medzeru a 129 znakov |
| Značky „(návrh – DR-Cxx)“ | PASS | 0 výskytov v contracte aj mappingu; `vytvorena` nie je v enum |
| Existencia IDs | PASS | 85 IDs z contractu a mappingu existuje v `spec/` (mimo `contracts/`); nové je iba DR-C12 (otázka k contractu) |
| Lint (Spectral/Redocly) | UNKNOWN | nástroj nebol spustený |

## 9. Ďalšie poznámky (neblokujú)

- **N-06** Contract neurčuje správanie pri neznámych poliach v tele (`additionalProperties`) ani všeobecnú chybu servera (500).
  Specification ich neurčuje. DR-C06 rieši iba nedostupné úložisko. Contract preto nič nedopĺňa.
- **N-07** `Retry-After` pri 503 nie je v contracte, lebo QR-002 ho neurčuje (`measure: null`).
- **N-08** Vzorové UUID, hodnota `sub` (`jana`) a korelačné ID v príkladoch sú ilustračné, nie seed dáta.

## 10. Záver

Contract 0.2.0 zapracúva DR-C01 až DR-C11, pokrýva všetkých 16 scenárov a je syntakticky platný (OpenAPI 3.1).
**Nie je pripravený na ľudské schválenie.** Bod 10 approval gate je FAIL a body 6 a 7 sú UNKNOWN pre otvorenú DR-C12. Po rozhodnutí
DR-C12 (zápis do specification) treba vydať contract 0.2.1 a potvrdiť bod 9 (not applicable).

Report nemení specification, stav contractu ani rozhodnutia.
