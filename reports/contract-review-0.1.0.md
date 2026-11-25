# Contract review – API-Reservations 0.1.0 (proposed)

| Pole | Hodnota |
|---|---|
| Režim | DESIGN_CONTRACT (úloha 0003, log `spec/prompts/log/0003-2026-11-25-DESIGN_CONTRACT.md`) |
| Contract | `spec/contracts/proposed/openapi-reservations.yaml`, verzia 0.1.0, status **proposed** |
| Mapping | `spec/contracts/proposed/contract-mapping.yaml` |
| Specification baseline | 0.2.0 (manifest: approved) / architecture baseline B-01 |
| Profil | `spec/policies/contract-design-profile.yaml` (status approved) |
| Pripravila | AI (Claude Opus 5.5). Report nie je schválenie. |

## 1. Vstupná kontrola

| Kontrola | Výsledok | Dôkaz |
|---|---|---|
| Baseline 0.2.0 | PASS | `spec/manifest.yaml`: `specification.version: "0.2.0"`, `status: "approved"` |
| ADR-001 až ADR-006 Accepted | PASS | sekcia `## Status` = `Accepted` v každom z 6 súborov `spec/architecture/decisions/ADR-00[1-6]-*.md` |
| Contract design profile approved | PASS | `contract_design_profile.status: "approved"` |
| Otvorené DR v specifikácii | PASS | `spec/drafts/open-decisions.md` r. 8: „Žiadna DR nie je otvorená.“ |

Zistenia v podkladoch (neblokujú návrh contractu; na ľudskú opravu v PREPARE_SPEC):

- **F-01 Zastarané komentáre o stave ADR.** Súbory ADR majú stav Accepted, no niektoré texty stále uvádzajú „Proposed“:
  `spec/manifest.yaml` (komentár pri `decisions`), hlavička `contract-design-profile.yaml`, `traceability.yaml`
  (komentár „ADR, všetky Proposed“, `evidence` ownership, `missing_links`), `US-001.md` (Decisions), `components.yaml`,
  `data-objects.yaml`, `technology-context.yaml`, `deployment.yaml`. Pätička každého ADR uvádza „0.2.0 (draft)“.
  Platný je stav v ADR súbore (manifest: „Stav každého súboru je uvedený v súbore samotnom“).
- **F-02 Chybná hodnota v profile.** `approved_by: Martin Štefanko (Software architect)"` – YAML je platný, ale hodnota
  končí znakom `"` (chýba úvodná úvodzovka). `reviewed_by: null`.
- **F-03 Stav súborov vs. stav baseline.** Manifest uvádza baseline 0.2.0 approved. Súbory `requirements.yaml`,
  `business-rules.yaml`, `quality-requirements.yaml` majú `meta.status: approved`, ale `approved_by: null` a jednotlivé
  položky `status: "draft"`. `glossary.md`, `US-001.md`, `US-001.feature`, `data-objects.yaml`, `components.yaml`,
  `lifecycles.yaml`, `service-realization.yaml`, `actors-processes.yaml` majú status draft. Obrázky views majú
  `reviewed_by: null`. Človek má potvrdiť, že tieto súbory sú súčasťou schváleného baseline 0.2.0.

## 1a. Impact analysis (AGENTS.md kap. 14)

```yaml
change:
  mode: "DESIGN_CONTRACT"
  requirement: "FR1"
  specification_baseline: "0.2.0"
  reason: "Návrh HTTP API contractu pre vytvorenie rezervácie (UC01, US-001)."

affected:
  goals: ["G-001"]
  actors_roles: ["ACT-Recepcna", "ROLE-SpravcaNavstev"]
  business_processes: ["BP-SpravaNavstev"] # kroky -01, -02, -04
  business_services: ["BS-RezervaciaTerminu"]
  application_services: ["AS-SpravaRezervaciiAKlientov"]
  application_components: ["AC-ModulRezervaciiAKlientov"]
  interfaces_contracts: ["API-Reservations 0.1.0 (proposed)"]
  data_objects: ["DO-Rezervacie", "DO-EvidenciaKlientov"]
  technology_elements: [] # contract nezávisí od technológie; ADR-002/ADR-003 sa týkajú IMPLEMENT_CHANGE
  work_packages_deliverables: [] # D-002 / WP-002 až pri implementácii
  decisions: ["ADR-001", "ADR-004", "ADR-005", "ADR-006"]
  scenarios: ["SC-001-01 až SC-001-14"]
  tests: [] # vzniknú v IMPLEMENT_CHANGE

open_questions: ["DR-C01", "DR-C02", "DR-C03", "DR-C04", "DR-C05", "DR-C06", "DR-C07", "DR-C08"]
```

## 2. Pokrytie scenárov (pozorovateľný výsledok v contracte)

Operácia: `POST /api/v1/reservations` (`createReservation`). Časy scenárov sú Europe/Bratislava (CET = UTC+1).

| Scenár | Vstup (UTC) | Pozorovateľný výsledok v contracte | Rules |
|---|---|---|---|
| SC-001-01 | 2026-12-01T13:00Z, note | 201 `Reservation`: status `vytvorena`, endTime 14:00Z, `createdBy`, `createdAt`; blokovanie overiteľné ďalšou požiadavkou → 409 | BR-001, BR-002, BR-004, BR-008, QR-001 |
| SC-001-02 | 2026-12-02T10:00Z, bez note | 201, `note` chýba | BR-008 |
| SC-001-03 | 2026-12-03T20:00Z | 201, endTime 21:00Z (= 22:00 miestneho času) | BR-005 |
| SC-001-04 | 2026-12-01T13:00Z (obsadené) | 409 `slot-occupied` „termín obsadený“; bez ponuky iného termínu; pôvodná rezervácia sa nemení | BR-003, BR-010 |
| SC-001-05 | 2026-12-01T13:30Z | 409 `slot-occupied` | BR-003, BR-010 |
| SC-001-06 | 2× 2026-12-04T15:00Z súčasne | jedna 201, druhá 409 `slot-occupied` | BR-003, BR-009, ADR-001 |
| SC-001-07 | neznámy `clientId` | 422 `client-not-found` | BR-007, BR-010 |
| SC-001-08 | 2026-12-01T20:30Z | 422 `outside-opening-hours` | BR-005, BR-010 |
| SC-001-09 | 2026-12-01T08:30Z | 422 `outside-opening-hours` | BR-005, BR-010 |
| SC-001-10 | 2026-11-25T10:00Z (teraz 11:00Z) | 422 `start-not-in-future` | BR-006, BR-010 |
| SC-001-11 | note 501 znakov | 422 `note-too-long` | BR-008, BR-010 |
| SC-001-12 | token bez roly SpravcaNavstev | 403 `forbidden`; rezervácia nevznikne (termín voľný – overiteľné následnou 201 oprávnenej osoby) | BR-011 |
| SC-001-13 | bez tokenu | 401 `unauthenticated`; rezervácia nevznikne | BR-011 |
| SC-001-14 | 2026-12-01T14:00Z po 13:00–14:00Z | 201, interval [14:00Z, 15:00Z) | BR-003, BR-004 |

Poznámka: Then-kroky o stave termínu („zostane voľný“, „existuje iba pôvodná rezervácia“) contract vyjadruje
garanciou „rezervácia nevznikne“ pri každom odmietnutí. Operácia na čítanie rezervácií nie je v contracte, lebo
nemá business dôvod v FR1 slice. Acceptance testy (IMPLEMENT_CHANGE) môžu stav overiť ďalším volaním tej istej operácie.

## 3. Mapovanie dôvodov odmietnutia (BR-010) na HTTP

| Rule | Dôvod (title) | HTTP | type | Znenie |
|---|---|---|---|---|
| BR-003 / BR-009 | termín obsadený | 409 | `urn:intelliwell:problem:slot-occupied` | DR-02 (doslovne) |
| BR-005 | termín mimo otváracích hodín | 422 | `urn:intelliwell:problem:outside-opening-hours` | návrh AI (BR-010) |
| BR-006 | začiatok termínu nie je v budúcnosti | 422 | `urn:intelliwell:problem:start-not-in-future` | návrh AI (BR-010) |
| BR-007 | klient neexistuje v evidencii klientov | 422 | `urn:intelliwell:problem:client-not-found` | návrh AI (BR-010) |
| BR-008 | poznámka je dlhšia ako 500 znakov | 422 | `urn:intelliwell:problem:note-too-long` | návrh AI (BR-010) |
| BR-011 (neprihlásený) | osoba nie je prihlásená | 401 | `urn:intelliwell:problem:unauthenticated` | návrh AI (podľa SC-001-13) |
| BR-011 (iná rola) | nedostatočné oprávnenie | 403 | `urn:intelliwell:problem:forbidden` | návrh AI (podľa SC-001-12) |
| – (NFR1, tvar vstupu) | neplatná požiadavka | 400 | `urn:intelliwell:problem:invalid-request` | návrh AI |

Odôvodnenie návrhu: 409 vyjadruje konflikt so stavom uložených rezervácií (vrátane súbehu a opakovaného odoslania).
422 znamená platný tvar vstupu, ktorý porušuje business pravidlo. 400 platí iba pre neplatný tvar. `type` používa URN,
lebo profil nemá doménu pre URI typov problémov (DR-C07).

## 4. Human contract approval gate (AGENTS.md kap. 9)

| # | Podmienka | Výsledok | Dôkaz |
|---|---|---|---|
| 1 | Každá operation má business dôvod a Requirement ID | PASS | 1 operácia `createReservation`; `x-sdd.requirements: [FR1, NFR1, NFR4, QR-001]`; `business_reason` v contract-mapping.yaml |
| 2 | Každý prvok mapuje na Application Service a poskytujúci Component | PASS | AS-SpravaRezervaciiAKlientov / AC-ModulRezervaciiAKlientov (service-realization.yaml; Layered a Application Usage View B-01). Pozn.: views majú `reviewed_by: null`, typy väzieb neoverené v Sparx EA (`model_relationship_types_verified: false`) – F-03 |
| 3 | Názvy používajú glossary | UNKNOWN | Popisy odkazujú na pojmy glossary (Termín, Poznámka, Klient, Evidencia klientov, Rezervácia, vytvorená, termín obsadený). Anglické názvy polí, ciest a typov problémov profil neurčuje – DR-C07 |
| 4 | Pozitívne aj negatívne scenarios majú pozorovateľný výsledok | PASS | Tabuľka v časti 2: všetkých 14 scenárov (4 pozitívne, 10 negatívnych) má HTTP status a telo; príklady v contracte sú pomenované podľa SC-ID |
| 5 | Rules, invariants a lifecycle transitions sú vynútiteľné alebo overiteľné | PASS | BR-001 až BR-011 → odpovede (časť 3); invariant BR-003 aj pri súbehu → 409 (ADR-001); prechod (none) → `vytvorena`, `status` enum `[vytvorena]` (lifecycles.yaml) |
| 6 | Authentication, authorization, audit a citlivé údaje sú explicitné | UNKNOWN | JWT bearer + `security: bearerAuth: [SpravcaNavstev]` (ADR-004); audit iba úspechu (QR-001, DR-26); poznámka `x-sdd-personal-data`, `x-sdd-excluded-from-application-log` (CON-003). Otvorené: hodnota claimu `role` (DR-C01), claim identity pre `createdBy` (DR-C02) |
| 7 | Error behavior a retry/idempotency sú jasné | UNKNOWN | Idempotencia: PASS (DR-28 – nie je idempotentné, opakovanie → 409). Otvorené: prijímaný tvar `startTime` (DR-C03), poradie chýb (DR-C04), neplatné `X-Correlation-ID` (DR-C05), nedostupnosť úložiska / 5xx (DR-C06), počítanie znakov (DR-C08) |
| 8 | Project policy je dodržaná | PASS | OpenAPI 3.1; cesta `/api/v1/reservations` (kebab-case, množné číslo, prefix); `operationId` lowerCamelCase; RFC 3339 UTC; problem details `application/problem+json`; `X-Correlation-ID` v požiadavke aj každej odpovedi; UUID pre klienta; bez kľúča idempotencie (DR-08, DR-28, DR-30, DR-31) |
| 9 | Breaking changes majú compatibility alebo migration rozhodnutie | not applicable (na potvrdenie človekom) | Prvá verzia contractu; `spec/contracts/approved/` je prázdny (manifest `approved_contracts: null`), nič sa nemení. Politika budúcich zmien: profil `compatibility` (DR-31) |
| 10 | Všetky DECISION REQUIRED relevantné pre contract sú uzavreté | FAIL | Otvorené DR-C01 až DR-C08 (časť 6) |

Záver: contract **nie je pripravený na schválenie**, kým človek neuzavrie DR-C01 až DR-C08 a nepotvrdí F-03.

## 5. Validácia

| Kontrola | Výsledok | Dôkaz |
|---|---|---|
| YAML syntax oboch súborov | PASS | `yaml.safe_load` (PyYAML) bez chyby |
| OpenAPI 3.1 schema | PASS | `openapi-spec-validator` 0.9.0, `validate()` bez chyby (nainštalovaný iba do scratchpadu, nie do projektu) |
| Príklady vs. schémy | PASS | 14 request príkladov a 9 response príkladov overených `jsonschema` Draft 2020-12 s format checkerom; SC-001-11 (501 znakov) podľa očakávania porušuje `maxLength: 500` |
| Existencia IDs | PASS | 66 IDs z contractu a mappingu (FR1, NFR*, QR-001, CON-003, BR-*, SC-*, ADR-*, DR-01..31, AS-, AC-, BS-, BP-, DO-, ROLE-, ACT-, US-001, UC01, OUT-001) existuje v `spec/`. Nové sú iba DR-C01–DR-C08 (otázky k contractu) a `API-Reservations` (ID contractu) |
| CON-003 v popisoch logovania | PASS | Jediný popis záznamu (audit QR-001) uvádza id rezervácie, `createdBy`, `createdAt`; poznámka v ňom nie je. Odpovede 422 nesmú obsahovať text poznámky |
| Lint (štýl, Spectral/Redocly) | UNKNOWN | nástroj nie je v prostredí dostupný; nespustený |

## 6. DECISION REQUIRED

```text
DECISION REQUIRED – DR-C01

Kontext:
BR-011, NFR4, ADR-004, ROLE-SpravcaNavstev; baseline 0.2.0 / B-01

Čo specification neurčuje alebo v čom si odporuje:
ADR-004 určuje claim „role“, ale presnú hodnotu pre ROLE-SpravcaNavstev necháva na návrh contractu a schválenie človekom.

Možnosti:
A. „SpravcaNavstev“ – zodpovedá ID roly bez prefixu; token: dotnet user-jwts create --role SpravcaNavstev.
B. „ROLE-SpravcaNavstev“ – identické s ID v špecifikácii.

Odporúčanie AI:
A (použité v návrhu contractu).

Otázka pre človeka:
Akú hodnotu claimu „role“ schvaľujete pre ROLE-SpravcaNavstev?

Kam zapísať rozhodnutie:
Contract (a Project policy: security.role_claim)
```

```text
DECISION REQUIRED – DR-C02

Kontext:
QR-001, DR-13, ADR-004 („identita z tokenu je zdrojom údaja kto rezerváciu vytvoril“); baseline 0.2.0

Čo specification neurčuje alebo v čom si odporuje:
Nie je určené, ktorý claim tokenu identifikuje prihlásenú osobu pre createdBy.

Možnosti:
A. claim „sub“ – štandardný identifikátor subjektu; dotnet user-jwts ho nastaví na meno používateľa.
B. claim „unique_name“ / „name“ – meno osoby; môže sa meniť.

Odporúčanie AI:
A (použité v návrhu contractu).

Otázka pre človeka:
Ktorý claim tokenu je identitou osoby pre záznam „kto rezerváciu vytvoril“?

Kam zapísať rozhodnutie:
ADR-004 (dodatok) alebo Project policy (security)
```

```text
DECISION REQUIRED – DR-C03

Kontext:
DR-08, DR-18, profil http.date_time „RFC 3339 UTC“; pole startTime; baseline 0.2.0

Čo specification neurčuje alebo v čom si odporuje:
Nie je určené, či API prijme čas s iným posunom než UTC (napr. +01:00), alebo ho odmietne.

Možnosti:
A. Prijať iba UTC so suffixom „Z“; iný posun → 400 invalid-request.
B. Prijať ľubovoľný RFC 3339 posun a normalizovať na UTC; odpovede vždy v UTC.

Odporúčanie AI:
A – doslovne zodpovedá „API prijíma UTC“ (použité v návrhu).

Otázka pre človeka:
Má API odmietnuť startTime, ktorý nie je zapísaný v UTC („Z“)?

Kam zapísať rozhodnutie:
Project policy (http.date_time)
```

```text
DECISION REQUIRED – DR-C04

Kontext:
BR-010, NFR1, BR-005 až BR-008, BR-003; baseline 0.2.0

Čo specification neurčuje alebo v čom si odporuje:
Nie je určené, aký dôvod odmietnutia systém uvedie, ak požiadavka porušuje viac pravidiel naraz.

Možnosti:
A. Jeden problem details pre prvé porušené pravidlo v pevnom poradí 401 → 403 → 400 → 422 (note-too-long, client-not-found, start-not-in-future, outside-opening-hours) → 409.
B. Vrátiť všetky porušené validačné pravidlá (422) naraz v rozšírení problem details; 409 iba ak validácia prejde.

Odporúčanie AI:
A (použité v návrhu) – jednoduchšie testovateľné, každý scenár má jeden dôvod.

Otázka pre človeka:
Má systém pri viacerých porušeniach uviesť jeden dôvod podľa pevného poradia, alebo všetky?

Kam zapísať rozhodnutie:
Rule (BR-010) alebo Contract
```

```text
DECISION REQUIRED – DR-C05

Kontext:
DR-31, profil http.correlation_id_header / correlation_id_behavior; baseline 0.2.0

Čo specification neurčuje alebo v čom si odporuje:
Nie je určený tvar korelačného ID ani správanie, ak klient pošle neplatnú hodnotu (prázdnu, príliš dlhú).

Možnosti:
A. Prijať ľubovoľný neprázdny reťazec do 128 znakov; inú hodnotu ignorovať a vygenerovať nové UUID.
B. Prijať iba UUID; inú hodnotu odmietnuť 400.
C. Prijať ľubovoľný reťazec bez obmedzenia (riziko pre logy).

Odporúčanie AI:
A – korelácia nikdy nezablokuje business operáciu.

Otázka pre človeka:
Aký tvar X-Correlation-ID systém prijme a čo urobí s neplatnou hodnotou?

Kam zapísať rozhodnutie:
Project policy (http.correlation_id_behavior)
```

```text
DECISION REQUIRED – DR-C06

Kontext:
FR1, ADR-001, ADR-003 (SQLite), DR-28; baseline 0.2.0

Čo specification neurčuje alebo v čom si odporuje:
Nie je určené správanie pri nedostupnosti úložiska alebo neočakávanej chybe (status, typ problému, odporúčanie opakovania).

Možnosti:
A. 503 Service Unavailable, problem details `urn:intelliwell:problem:service-unavailable`; pri opakovaní platí DR-28 (môže prísť 409, ak prvý pokus uspel).
B. Iba generická 500 bez špecifikácie v contracte.

Odporúčanie AI:
Žiadne – ide o prevádzkové rozhodnutie (AGENTS.md kap. 5). Contract zatiaľ 5xx neuvádza.

Otázka pre človeka:
Má contract opísať odpoveď pri nedostupnosti úložiska, a akú?

Kam zapísať rozhodnutie:
Contract / Project policy
```

```text
DECISION REQUIRED – DR-C07

Kontext:
Glossary, profil http.naming (DR-31 určuje iba cesty a operationId); baseline 0.2.0

Čo specification neurčuje alebo v čom si odporuje:
Profil neurčuje jazyk a konvenciu názvov polí, hodnôt enum a identifikátorov typov problémov.

Možnosti:
A. Polia anglicky lowerCamelCase (clientId, startTime, note, createdBy …), stav `vytvorena` podľa lifecycles.yaml, typy `urn:intelliwell:problem:<kebab-case>`; mapovanie na glossary v contract-mapping.yaml.
B. Polia slovensky bez diakritiky (klientId, zaciatok, poznamka …).
C. Typy problémov ako HTTP URI (vyžaduje rozhodnutie o doméne).

Odporúčanie AI:
A (použité v návrhu) – zhodné s anglickou cestou z DR-31.

Otázka pre človeka:
Akú konvenciu názvov polí, hodnôt a typov problémov schvaľujete?

Kam zapísať rozhodnutie:
Project policy (http.naming)
```

```text
DECISION REQUIRED – DR-C08

Kontext:
BR-008, DR-22 (500 znakov); pole note; baseline 0.2.0

Čo specification neurčuje alebo v čom si odporuje:
Nie je určené, čo je „znak“ (Unicode znak / code point vs. UTF-16 jednotka v .NET string.Length); líši sa pri emoji.

Možnosti:
A. Unicode code point – zhodné s JSON Schema maxLength v contracte.
B. UTF-16 jednotka (string.Length v .NET).

Odporúčanie AI:
A – contract a implementácia počítajú rovnako.

Otázka pre človeka:
Ako sa počíta 500 znakov poznámky?

Kam zapísať rozhodnutie:
Rule (BR-008)
```

## 7. Čo report nemení

Nevznikol approved contract, `approval.yaml`, kód ani testy. Specification, ADR ani profil sa nemenili.
Contract nevyžaduje zmenu specification; zistenia F-01 až F-03 sú na opravu človekom (PREPARE_SPEC).
