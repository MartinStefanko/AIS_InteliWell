> **Stav:** obsahovo schválené človekom v doplňujúcom prompte 0009 a zapracované do autoritatívnych súborov (stav draft).
> Approved contract (V-01) AI nemenila – pozri log 0009. Tento súbor ostáva ako záznam návrhu.

# Návrh 0009 – jednoduché UI pre FR1 (FR1-UI), deployment/technology kontext a odložené nálezy z verification-0008

| Pole | Hodnota |
|---|---|
| Režim | PREPARE_SPEC (prompt 0009) |
| Change | FR1-UI (change experiment, bod E) + údržba specification po 0007/0008 |
| Specification baseline | 0.3.1 (approved) → **0.4.0 (draft)**, architecture baseline B-01 – overené v `spec/manifest.yaml` |
| Rozhodnutie človeka v prompte | zmena DR-17: prototyp má jednoduché UI (statická stránka HTML + vanilla JS v wwwroot), contract 0.2.1 sa nemení |
| Approved contract | API-Reservations 0.2.1 (`spec/contracts/approved/approval.yaml`) – **nemení sa** |
| Stav | **draft** – nič nie je schválené. AI nemenila žiadny approved súbor, ADR ani contract. |
| Log | `spec/prompts/log/0009-2026-11-25-PREPARE_SPEC.md` |

Súbory návrhu (všetky v `spec/drafts/0009/`):

| Súbor | Rieši | Cieľové umiestnenie po schválení |
|---|---|---|
| `ADR-010-jednoduche-ui-prototypu.md` | úloha 1 – technológia UI, zmena DR-17 (Proposed) | `spec/architecture/decisions/` |
| `requirement-FR1-UI.yaml` | úloha 2 – nový requirement FR1-UI + patche FR1 scope, glossary, US-001.md | `spec/requirements/requirements.yaml` |
| `US-002.md`, `US-002.feature` | úloha 3 – user story a scenáre SC-002-01 – SC-002-08 | `spec/stories/`, `spec/stories/features/` |

| Súbor | Rieši |
|---|---|
| `deployment.yaml` | I-04, UI v ART-SoftverIntelliWell (prompt 0009, ADR-010), seed_data (DR-I05) – úplná navrhovaná verzia |
| `technology-context.yaml` | I-04 (prototype_environment), K-58 – úplná navrhovaná verzia |
| `traceability-realization.yaml` | traceability FR1 → contract → kód → testy (K-17), V-06 |
| `US-001-SC-001-17.feature` | N-10 – scenár pre token bez `sub` |
| `components-dependencies.yaml` | allowed_dependencies / uses_services (K-21, I-05) – dve možnosti |
| `id-definitions.yaml` | IDs definované iba v id-mapping.yaml (V-05, K-10) |
| `navrh-0009.md` (tento) | redakčné patche V-01, V-03, V-04, N-01, N-02, N-02a, N-02b; DECISION REQUIRED; poradie aplikovania |

## 1. Prehľad nálezov a návrhov

| Nález | Typ riešenia | Blokuje ho DR? | Kto aplikuje |
|---|---|---|---|
| FR1-UI, US-002, ADR-010 | obsahový draft | DR-0910 – DR-0915 | človek; ADR-010 Accepted iba človek |
| Deployment / technology (I-04, UI) | obsahový draft | DR-0902, DR-0913 | človek (approved súbory) |
| V-01 metadáta approved contractu | redakčný patch (časť 2.1) | nie | iba človek – approved contract |
| V-03 názov stĺpca auditu | rozhodnutie | DR-0905 | človek (ADR) alebo IMPLEMENT_CHANGE |
| V-04 text „Toto nie je rozhodnutie“ v ADR-008/009 | redakčný patch (časť 2.2) | nie | človek (Accepted ADR) |
| Traceability FR1 → kód → testy | obsahový draft | nie (SC-001-17 časť závisí od DR-0908) | človek |
| N-01 zastarané komentáre | redakčný patch (časť 2.3) | nie | človek |
| N-02, N-02a verzie v súboroch | pravidlo verziovania | DR-0903 (verzia balíka 0.4.0 určená v prompte) | človek |
| N-02b pätička ADR-007 | redakčný patch (časť 2.4) | nie | človek |
| N-10 scenár SC-001-17 | Gherkin draft | DR-0908 (iba contract text a trait testu) | owner BR-011 potvrdí význam |
| IDs iba v id-mapping.yaml (V-05) | draft definícií | DR-0907 | človek po porovnaní so Sparx EA |
| allowed_dependencies | architektonické rozhodnutie | DR-0906 | človek |

V-02 je už rozhodnuté (DR-V02 = C) a do 0009 nepatrí. V-07, V-08 nie sú v zadaní 0009.

## 2. Redakčné patche (nemenia význam)

### 2.1 V-01 – metadáta approved contractu

Platný záznam o schválení je `approval.yaml`. AI nesmie meniť approved contract (AGENTS.md kap. 1); patch aplikuje človek.
Ide iba o metadáta, nie o operáciu, vstup, výstup, chyby ani security – verzia 0.2.1 sa nemení.

`spec/contracts/approved/contract-mapping.yaml`

| Riadok | Teraz | Navrhované |
|---|---|---|
| 1 | `# Contract mapping – PROPOSED (návrh AI, DESIGN_CONTRACT, úloha 0006; …)` | `# Contract mapping – APPROVED 0.2.1 (schválil Martin Štefanko, 2026-11-25; pozri approval.yaml). Návrh AI z úlohy 0006; predchádzajúce verzie 0.2.0 (0005), 0.1.0 (0003).` |
| 8 | `file: "spec/contracts/proposed/openapi-reservations.yaml"` | `file: "spec/contracts/approved/openapi-reservations.yaml"` |
| 15 | `approved_by: null` | `approved_by: "Martin Štefanko (Software architect)" # approval.yaml, 2026-11-25` |

`spec/contracts/approved/openapi-reservations.yaml` (`info.x-sdd`)

| Pole | Teraz | Navrhované |
|---|---|---|
| `mapping` | `spec/contracts/proposed/contract-mapping.yaml` | `spec/contracts/approved/contract-mapping.yaml` |
| `approved_by` | `null` | `Martin Štefanko (Software architect)` |

Kópie v `spec/contracts/proposed/` odporúčam ponechať bez zmeny ako históriu návrhu.

### 2.2 V-04 – ADR-008 a ADR-009 so stavom Accepted

AI nemení text Accepted ADR. Návrh pre človeka (rovnaký pre oba súbory, r. 3–4):

```text
Teraz:
> Draft pripravený AI v režime IMPLEMENT_CHANGE (úloha 0007, fáza 1 – plán). Toto nie je rozhodnutie.
> Stav mení iba človek.

Navrhované:
> Draft pripravila AI v režime IMPLEMENT_CHANGE (úloha 0007, fáza 1 – plán). Rozhodnutie zapísal človek
> v sekcii „Doplnenie 2026-11-25“ (Martin Štefanko, Software architect).
```

`spec/drafts/open-decisions.md` r. 9 (working súbor; zmenu urobí AI alebo človek po schválení):

```text
Teraz:   - ADR-001 až ADR-006 sú v stave **Accepted** (zapísal človek). ADR-007 (0004) je v stave **Accepted** (zapísal človek).
Navrhované: - ADR-001 až ADR-006 sú v stave **Accepted** (zapísal človek). ADR-007 (0004) je v stave **Accepted** (zapísal človek).
            ADR-008 a ADR-009 (0007, DR-I01 – DR-I05 = A) sú v stave **Accepted** (zapísal človek).
```

### 2.3 N-01 – zastarané komentáre

| Súbor | Riadok | Teraz | Navrhované |
|---|---|---|---|
| `spec/manifest.yaml` | 5 | `# Zmeny pre 0.3.0 navrhla AI v úlohe 0004 (DR-C01 až DR-C11); sú necommitnuté a čakajú na kontrolu človekom.` | `# Zmeny pre 0.3.0 navrhla AI v úlohe 0004 (DR-C01 až DR-C11); 0.3.0 schválil človek (commit 5d53770), 0.3.1 (DR-C12) človek (commit 7705cdf).` |
| `spec/manifest.yaml` | 8 | `version: "0.3.1" # návrh AI v 0004 (PREPARE_SPEC); predchádzajúci baseline 0.2.0` | `version: "0.4.0" # predchádzajúci baseline 0.3.1` (po schválení) |
| `spec/manifest.yaml` | 9 | `status: "approved" # návrh AI v 0004; …` | `status: "approved"` (bez komentára) |
| `spec/manifest.yaml` | 26 | `business_scenarios: null # … zatiaľ prázdny` | bez zmeny – adresár je stále prázdny (komentár platí) |
| `spec/manifest.yaml` | 36 | `# US-001.feature (draft, 0002; SC-001-15, SC-001-16 doplnené v 0004)` | `# US-001.feature (approved; SC-001-15, SC-001-16 v 0004; SC-001-17 podľa DR-0908)` |
| `spec/manifest.yaml` | 37 | `# ADR-001 – ADR-007 Accepted; šablóna ADR-000` | `# ADR-001 – ADR-009 Accepted; šablóna ADR-000` |
| `spec/manifest.yaml` | 38 | `# spec/contracts/approved/ existuje, zatiaľ prázdny` | `# API-Reservations 0.2.1 (approval.yaml)` |
| `spec/manifest.yaml` | 122 | `code: "src/" # src/ zatiaľ neexistuje` | `code: "src/" # src/IntelliWell (0007)` |
| `spec/manifest.yaml` | 124 | `tests: "tests/"` | `tests: "tests/" # tests/IntelliWell.Tests (0007)` |
| `business-rules.yaml` | 7 | `# Zmeny čakajú na kontrolu človekom; …` | odstrániť |
| `business-rules.yaml` | 14 | `specification_version: "0.3.0" # návrh 0004` | podľa DR-0903 (pozri 2.5) bez komentára |
| `business-rules.yaml` | 201 | `…a rezerváciu neuloží.Token bez…` | `…a rezerváciu neuloží. Token bez…` (chýbajúca medzera, nový nález N-11) |
| `quality-requirements.yaml` | 5 | `# Zmeny čakajú na kontrolu človekom; …` | odstrániť |
| `quality-requirements.yaml` | 12 | `"0.3.0" # návrh 0004` | `"0.3.0"` |
| `requirements.yaml` | 13 | `"0.3.0" # návrh 0004` | podľa 2.5 |
| `traceability.yaml` | 11 | `"0.3.0" # návrh 0004` | podľa 2.5 |
| `glossary.md` | 7 | `"0.3.0" (návrh 0004)` | `"0.3.0"` |
| `spec/stories/US-001.md` | 7 | `"0.3.0" (návrh 0004)` | `"0.3.0"` |
| `US-001.feature` | 2 | `specification_version: 0.3.0 (návrh)` | podľa 2.5 |
| `contract-design-profile.yaml` | 4 | `# Zmeny z 0004 … čakajú na kontrolu človekom. …` | `# Zmeny z 0004 (DR-C03, DR-C05, DR-C07, DR-C09, DR-C11) schválil človek v 0.3.0.` |
| `contract-design-profile.yaml` | 11 | `"0.3.0" # návrh 0004; profil bol schválený vo verzii 0.2.0` | `"0.3.0"` |
| hlavičky `# … (draft).` v approved súboroch | r. 1 | napr. `# Application Structure pre FR1 (draft).` | odstrániť „(draft)“ v súboroch s `status: approved` (components, data-objects, lifecycles, service-realization, business-rules, quality-requirements, requirements, traceability, deployment, technology-context, actors-processes, interactions*) |

\* `interactions.yaml`, `actors-processes.yaml`: stav overí človek – AI v 0009 nečítala ich `meta.status`.

### 2.4 N-02b – pätička ADR-007

```text
Teraz:        ## Specification baseline
              0.3.0 (draft) / architecture baseline B-01
Navrhované:   ## Specification baseline
              0.3.0 / architecture baseline B-01; doplnenie DR-C12 v 0.3.1
```

Aplikuje človek (Accepted ADR). Navrhovaný text platí pri DR-0903 = A aj B.

### 2.5 N-02, N-02a – `specification_version` v súboroch (závisí od DR-0903)

Pri DR-0903 = A (verzia poslednej obsahovej zmeny súboru) a baseline 0.4.0 (prompt 0009), ak sa schvália všetky návrhy 0009.
Súbory zmenené kvôli FR1-UI (requirements.yaml, glossary.md, US-001.md) dostanú 0.4.0 bez ohľadu na N-01:

| Súbor | Teraz | Navrhované | Dôvod |
|---|---|---|---|
| `lifecycles.yaml` | 0.2.0 | **0.3.0** | `api_value: created` (DR-C11) pribudlo v 0.3.0 (N-02) |
| `business-rules.yaml` | 0.3.0 | **0.4.0** | DR-C12 v 0.3.1 + BR-011 `scenarios` (SC-001-17) v 0009 |
| `requirements.yaml` | 0.3.0 | **0.4.0** | FR1-UI, FR1 scope (DR-17); pri DR-0907 = A aj sekcia `motivation` |
| `glossary.md`, `US-001.md` | 0.3.0 | **0.4.0** | text DR-17 o UI |
| `US-002.md`, `US-002.feature`, ADR-010 | – | **0.4.0** | nové |
| `service-realization.yaml` | – (pole chýba) | **0.4.0** / doplniť | DR-0907 = A (sekcia `application_services`) |
| `traceability.yaml` | 0.3.0 | **0.4.0** | stopy realizácie (K-17) |
| `components.yaml` | 0.2.0 | **0.4.0** | allowed_dependencies (DR-0906) |
| `deployment.yaml`, `technology-context.yaml` | 0.2.0 | **0.4.0** | I-04, UI |
| `US-001.feature` | 0.3.0 | **0.4.0** | SC-001-17 |
| `quality-requirements.yaml`, `contract-design-profile.yaml` | 0.3.0 | 0.3.0 | obsah sa nemení, iba komentár |
| `data-objects.yaml` | 0.2.0 | 0.2.0 | bez zmeny |
| ADR-007 pätička | 0.3.0 (draft) | pozri 2.4 | |

Pri DR-0903 = B majú všetky súbory hodnotu verzie balíka z manifestu (napr. 0.4.0) a menia sa pri každom vydaní.

## 3. Deployment a technology context (I-04, UI)

Navrhované úplné verzie: `spec/drafts/0009/deployment.yaml`, `spec/drafts/0009/technology-context.yaml`.
Zmeny sú označené `# ZMENA 0009`. Zhrnutie:

- `packaging`: bez balenia, spustenie zo zdrojového kódu `dotnet run`; container image a CI mimo rozsahu – **prevzaté
  z ADR-008 doplnenia** (rozhodnuté), nie nové rozhodnutie.
- `prototype_runs_on` / `prototype_environment`: lokálny počítač vývojára (ADR-008 doplnenie). Cieľ
  `deployed_on: NODE-ServerIntelliWell` ostáva. Nové NODE ID pre lokálny počítač AI nenavrhuje.
- `contains`: obsah artifactu – AC-ModulRezervaciiAKlientov (v prototype), ostatné moduly (mimo prototypu), spoločná
  hostiteľská časť (pozri DR-0906) a **UI** (v prototype; statická stránka v `wwwroot`, ADR-010 Proposed; prvok
  modelu chýba – DR-0914; prístup k stránke – DR-0913). Žiadny nový artifact.
- `seed_data`: zosúladené s DR-I05 / ADR-009 (predtým „nerozhodnuté“).
- `configuration_files`: iba existujúce súbory z realizácie.
- `required_by_implementation_task`: čaká na DR-0902.
- `open_questions`: odstránené otázky prostredia a packagingu (zodpovedané ADR-008); pridané DR-0913, ADR-010.
- `meta.status` je v drafte `draft`; po schválení nastaví človek `approved`.

## 4. Decision required

**DR-0901 (UI v prototype) – uzavreté človekom v prompte 0009:** UI je v rozsahu prototypu (zmena DR-17), technológia podľa ADR-010 (Proposed).

```text
DECISION REQUIRED – DR-0902

Kontext:
deployment.yaml ART-SoftverIntelliWell.required_by_implementation_task (null od 0002); AGENTS.md kap. 4 a 10;
ADR-008 doplnenie; verification-0008 K-52 (rozhodnuté atribúty artifactu PASS).

Čo specification neurčuje alebo v čom si odporuje:
Nie je zapísané, či deployable Artifact patrí do zadania IMPLEMENT_CHANGE pre FR1.

Možnosti:
A. Áno – v rozsahu „spustiteľný projekt src/IntelliWell + konfigurácia, spúšťaný cez dotnet run, bez balenia“.
   VERIFY_CHANGE potom kontroluje zhodu s deployment.yaml (ako v K-52).
B. Nie – artifact je iba cieľový prvok architektúry; kontrola deployable Artifactu je not applicable.

Odporúčanie AI:
A – zodpovedá tomu, čo 0007 dodala a 0008 overila.

Otázka pre človeka:
Je ART-SoftverIntelliWell (v rozsahu podľa A) súčasťou zadania IMPLEMENT_CHANGE pre FR1?

Kam zapísať rozhodnutie:
deployment.yaml (required_by_implementation_task)
```

```text
DECISION REQUIRED – DR-0903

Kontext:
N-02, N-02a (contract-review-0.2.1, verification-0008 K-57); manifest specification.version 0.3.1;
meta.specification_version v 12 súboroch (0.2.0 / 0.3.0).

Čo specification neurčuje alebo v čom si odporuje:
Nie je určené, či specification_version v súbore znamená verziu balíka, alebo verziu poslednej obsahovej zmeny súboru.

Možnosti:
A. Verzia poslednej obsahovej zmeny súboru; verzia balíka je iba v manifeste. Redakčná zmena (komentár) verziu nemení.
   Opraví sa iba lifecycles.yaml (0.3.0) a súbory zmenené v 0009 (tabuľka 2.5).
B. Verzia balíka; každé vydanie mení všetky súbory.

Odporúčanie AI:
A – menej zmien bez obsahu a z čísla vidno, kedy sa súbor naposledy menil.

Otázka pre človeka:
Čo znamená meta.specification_version v jednotlivom súbore?

Kam zapísať rozhodnutie:
Project policy (manifest – komentár pri specification.version, prípadne spec/policies/)
```

**DR-0904 (verzia balíka) – uzavreté človekom v prompte 0009:** 0.4.0 (draft).

```text
DECISION REQUIRED – DR-0905

Kontext:
V-03 (verification-0008 K-51); ADR-009 doplnenie DR-I03 „stĺpce created_by a created_at“; kód
ReservationStore.cs:65-66, 124 – created_by a created_at_utc_ticks (čas UTC ako ticks); QR-001.

Čo specification neurčuje alebo v čom si odporuje:
ADR-009 uvádza názov stĺpca created_at, kód používa created_at_utc_ticks; nie je určené, či ADR predpisuje
záväzný názov a formát stĺpca, alebo iba miesto uloženia auditu.

Možnosti:
A. ADR určuje iba miesto a význam (kto, kedy, v riadku rezervácie). Človek doplní do ADR-009 vetu „Názov stĺpca
   času je created_at_utc_ticks (UTC, ticks); názov v DR-I03 je opisný.“ Kód sa nemení.
B. Názov je záväzný: kód sa upraví na created_at (IMPLEMENT_CHANGE; existujúci súbor intelliwell-prototyp.db
   treba zmazať alebo migrovať, formát času rozhodnúť).
C. Ponechať bez zmeny ako známy rozdiel prototypu.

Odporúčanie AI:
A – význam QR-001 je splnený, zmena iba upresní ADR.

Otázka pre človeka:
Predpisuje DR-I03 v ADR-009 záväzný názov stĺpca času vytvorenia?

Kam zapísať rozhodnutie:
ADR (ADR-009 – doplnenie človekom)
```

```text
DECISION REQUIRED – DR-0906

Kontext:
components.yaml AC-ModulRezervaciiAKlientov: allowed_dependencies null, uses_services null (K-21, I-05);
B-01-application-cooperation-tobe.png (INT-001 – INT-003, typ väzby neurčený); interactions.yaml
relevance_for_fr1 false; kód FR1 v src/IntelliWell/ModulRezervaciiAKlientov/ a src/IntelliWell/Hosting/.

Čo specification neurčuje alebo v čom si odporuje:
Nie je rozhodnuté, od čoho smie AC-ModulRezervaciiAKlientov závisieť, a ku ktorému komponentu patrí spoločná
hostiteľská časť (autentifikácia, correlation ID, problem details).

Možnosti:
A. Rozsah FR1: žiadna závislosť na inom module; povolené TS-DatabazovaSluzba, TS-HostovanieSystemu a spoločná
   hostiteľská časť AC-IntelliWell (NFR4 → AC-IntelliWell). INT-001/INT-002 sa rozhodnú s požiadavkou, ktorá ich
   zahŕňa. (components-dependencies.yaml, option_A)
B. Cieľová architektúra: povolené INT-001, INT-002 podľa Application Cooperation view (štýl nerozhodnutý),
   FR1 ich nepoužíva; hostiteľská časť ako v A. (option_B)

Odporúčanie AI:
A – zodpovedá overenému kódu (K-20) a nepredbieha nerozhodnutý integračný štýl INT-*.

Otázka pre človeka:
Ktoré allowed_dependencies platia pre AC-ModulRezervaciiAKlientov a patrí hostiteľská časť k AC-IntelliWell?

Kam zapísať rozhodnutie:
components.yaml (allowed_dependencies, uses_services); pri B aj interactions.yaml
```

```text
DECISION REQUIRED – DR-0907

Kontext:
V-05 (verification-0008 K-10); G-001, G-002, G-005, OUT-001, AS-SpravaRezervaciiAKlientov sú definované iba
v spec/drafts/id-mapping.yaml (draft), approved súbory ich používajú. AI má iba obrázky B-01, nie model.

Čo specification neurčuje alebo v čom si odporuje:
Nie je určené, ktorý authoritative súbor definuje Goals, Outcome a Application Service.

Možnosti:
A. Sekcia motivation (goals, outcomes) v requirements.yaml a sekcia application_services v service-realization.yaml
   (id-definitions.yaml); id-mapping ostáva working register.
B. Celý id-mapping.yaml povýšiť na authoritative register (napr. spec/architecture/id-register.yaml) po overení
   všetkých položiek voči Sparx EA.

Odporúčanie AI:
A – rieši iba 5 chýbajúcich definícií; B vyžaduje overiť ~70 položiek.

Otázka pre človeka:
Kam sa zapíšu definície G-001, G-002, G-005, OUT-001 a AS-SpravaRezervaciiAKlientov (a potvrdzujete ich názvy
voči modelu)?

Kam zapísať rozhodnutie:
Requirement (requirements.yaml) a Service Realization (service-realization.yaml), alebo manifest pri B
```

```text
DECISION REQUIRED – DR-0908

Kontext:
N-10; návrh SC-001-17 (US-001-SC-001-17.feature); contract 0.2.1 príklad unauthenticated-missing-sub má summary
„bez scenára v US-001“; contract-mapping 401 scenarios: ["SC-001-13"]; test
UnauthenticatedMissingSub_… má trait ContractExample, nie Scenario.

Čo specification neurčuje alebo v čom si odporuje:
Po schválení SC-001-17 bude text approved contractu („bez scenára“) a označenie testu nepresné; nie je určené,
či sa to rieši hneď, alebo v ďalšej verzii.

Možnosti:
A. Contract 0.2.1 sa nemení; väzba SC-001-17 ↔ príklad ↔ test sa zapíše iba v traceability.yaml. Text contractu
   a mapping sa upravia v ďalšej verzii contractu (DESIGN_CONTRACT); trait Scenario sa doplní v ďalšom
   IMPLEMENT_CHANGE.
B. Človek hneď redakčne upraví approved contract (summary, mapping scenarios) spolu s V-01 a v IMPLEMENT_CHANGE
   sa doplní trait Scenario = SC-001-17.

Odporúčanie AI:
A – approved contract sa mení iba v metadátach (V-01), nie v obsahu príkladov.

Otázka pre človeka:
Má sa approved contract 0.2.1 a test upraviť pre SC-001-17 teraz (B), alebo až v ďalšej verzii (A)?

Kam zapísať rozhodnutie:
Contract / Scenario
```

### DECISION REQUIRED k FR1-UI (úlohy 1 – 3)

```text
DECISION REQUIRED – DR-0910

Kontext:
FR1-UI (draft), ADR-010 (Proposed), NFR4, DR-23 (token z dotnet user-jwts vkladá používateľ), ADR-004.

Čo specification neurčuje alebo v čom si odporuje:
Nie je určené, kde a ako dlho UI uchováva vložený JWT token.

Možnosti:
A. Iba v pamäti stránky – po obnovení alebo zatvorení stránky treba token vložiť znova.
B. sessionStorage – token prežije obnovenie stránky, zanikne so zatvorením karty.
C. localStorage – token prežije zatvorenie prehliadača až do expirácie.

Odporúčanie AI:
A – najmenšie riziko pri XSS a na zdieľanom zariadení recepcie; pre prototyp postačuje.

Otázka pre človeka:
Kde smie UI uchovávať vložený token?

Kam zapísať rozhodnutie:
ADR (ADR-010), Project policy (contract-design-profile.yaml security – ak sa má týkať aj ďalších klientov)
```

```text
DECISION REQUIRED – DR-0911

Kontext:
FR1-UI, DR-18 (Europe/Bratislava), DR-C03 (API iba UTC so Z), SC-002-02, SC-002-03, ADR-010 (vanilla JS).

Čo specification neurčuje alebo v čom si odporuje:
Nie je určené, či UI prepočíta zadaný čas podľa pevného pásma Europe/Bratislava, alebo podľa časového pásma
prehliadača; a ako naloží s časom, ktorý pri zmene času neexistuje alebo je dvojznačný.

Možnosti:
A. Pevne Europe/Bratislava (Intl API prehliadača), nezávisle od nastavenia počítača. Neexistujúci čas (jarná zmena)
   UI odmietne hláškou „Tento čas pri zmene času neexistuje.“; dvojznačný čas (jesenná zmena) použije prvý výskyt.
   Oba prípady sú mimo otváracích hodín (BR-005), server ich aj tak odmietne.
B. Časové pásmo prehliadača; predpoklad, že zariadenie recepcie je nastavené na Europe/Bratislava.

Odporúčanie AI:
A – zodpovedá DR-18 aj pri zle nastavenom počítači; výnimky zmeny času sa rezervácie prakticky netýkajú.

Otázka pre človeka:
Podľa ktorého časového pásma UI prepočítava zadaný čas a ako spracuje neexistujúci a dvojznačný čas?

Kam zapísať rozhodnutie:
ADR (ADR-010); Scenario (US-002)
```

```text
DECISION REQUIRED – DR-0912

Kontext:
FR1-UI, US-002 SC-002-04 – SC-002-07; contract 0.2.1 ProblemDetails (9 typov, slovenské `title`); BR-010
(znenie dôvodov okrem „termín obsadený“ je návrh AI); prompt 0009: „zrozumiteľná slovenská hláška pre každý typ“.

Čo specification neurčuje alebo v čom si odporuje:
Nie je určené znenie hlášok UI, ani čo UI zobrazí pri neznámom type problému alebo keď server neodpovie
(stav mimo contractu).

Možnosti:
A. Vlastné texty UI podľa `type` (tabuľka v SC-002-05); pri neznámom type `title` zo servera; bez odpovede
   „Server neodpovedá. Nie je isté, či sa rezervácia uložila – skontrolujte to pred ďalším pokusom.“
B. UI zobrazí priamo `title` z odpovede (bez vlastných textov); bez odpovede rovnaký text ako v A.

Odporúčanie AI:
A – texty môžu obsahovať pokyn, čo urobiť; `title` ostáva záložný.

Otázka pre človeka:
Schvaľujete texty hlášok z SC-002-05 (A), alebo má UI zobrazovať `title` z contractu (B)?

Kam zapísať rozhodnutie:
Scenario (US-002.feature); owner ACT-Manazer potvrdí znenie
```

```text
DECISION REQUIRED – DR-0913

Kontext:
FR1-UI, NFR4, CON-002 (interný systém len pre personál), ADR-004; login endpoint mimo rozsahu (DR-23).

Čo specification neurčuje alebo v čom si odporuje:
Nie je určené, či statickú stránku UI smie načítať aj neprihlásená osoba.

Možnosti:
A. Stránka je dostupná bez prihlásenia; neobsahuje osobné údaje (iba UUID seed klienta); všetky údaje chráni API (NFR4).
B. Stránka vyžaduje prihlásenie – bez login endpointu a cookies to v prototype nie je realizovateľné bez nového
   rozhodnutia o autentifikácii (zmena ADR-004).

Odporúčanie AI:
A – zodpovedá DR-23; ochrana údajov ostáva na API.

Otázka pre človeka:
Môže byť statická stránka UI dostupná bez prihlásenia?

Kam zapísať rozhodnutie:
ADR (ADR-010); deployment.yaml (anonymous_access_to_page)
```

```text
DECISION REQUIRED – DR-0914

Kontext:
FR1-UI; B-01-application-usage-tobe.png a B-01-implementation-migration-tobe.png zobrazujú jediný Application
Interface „Rozhranie saunového majstra“ (vnorený v Module koordinácie saunových majstrov, slúži Saunovému majstrovi).
Application Interface pre Recepčnú / Správcu návštev v B-01 views nie je. AI má iba obrázky, nie model.

Čo specification neurčuje alebo v čom si odporuje:
UI pre recepčnú nemá prvok v architektúre, takže chýba väzba FR1-UI → Application Interface → ACT-Recepcna.

Možnosti:
A. Človek doplní do modelu Application Interface pre recepčnú (analogicky k Rozhraniu saunového majstra – vnorený
   v AC-ModulRezervaciiAKlientov alebo v AC-IntelliWell, slúži Recepčnej / Správe návštev), exportuje nové views
   a určí ID. AI potom doplní traceability.
B. Bez nového prvku – UI sa vedie iba ako časť ART-SoftverIntelliWell a FR1-UI sa traceuje na
   AC-ModulRezervaciiAKlientov; nesúlad s modelom sa prijme pre prototyp.

Odporúčanie AI:
A – rovnaký vzor už model používa pri saunovom majstrovi. AI ID ani názov prvku nenavrhuje, kým ho človek nedoplní.

Otázka pre človeka:
Doplníte do modelu Application Interface pre recepčnú, a ak áno, do ktorého komponentu bude vnorený?

Kam zapísať rozhodnutie:
Model v Sparx EA → nové views v spec/architecture/views/ + manifest; components.yaml; traceability.yaml
```

```text
DECISION REQUIRED – DR-0915

Kontext:
US-002 (SC-002-01 – SC-002-08), ADR-010 (bez nového NuGet balíka, bez frontend buildu), ADR-008 (zoznam balíkov),
Definition of Done IMPLEMENT_CHANGE (acceptance tests musia prejsť).

Čo specification neurčuje alebo v čom si odporuje:
Bez nového balíka nie je k dispozícii automatizácia prehliadača; nie je určené, ako sa US-002 overí.

Možnosti:
A. Automaticky iba to, čo ide bez prehliadača (stránka sa servíruje, `/api` správanie sa nezmenilo, existujúcich
   52 testov prechádza); scenáre SC-002-* overí človek ručne podľa protokolu v reports/ (výsledok PASS/FAIL).
B. Pridať balík na automatizáciu prehliadača (napr. Playwright) – zmena ADR-008 a rozporuje prompt 0009.

Odporúčanie AI:
A – zachová obmedzenie „bez nového balíka“.

Otázka pre človeka:
Akým spôsobom sa budú overovať scenáre US-002?

Kam zapísať rozhodnutie:
ADR (ADR-010 Consequences) / Project policy
```

## 5. Navrhované poradie aplikovania (po rozhodnutí)

1. DR-0903 (význam verzie v súbore) – určuje hodnoty v ostatných krokoch; baseline 0.4.0 určil človek.
2. ADR-010 (Accepted/Rejected – človek) a DR-0910 – DR-0915; potom presun requirement-FR1-UI, US-002 a feature na cieľové miesta
   a patche FR1 scope, glossary, US-001.md (DR-17).
3. Redakčné patche 2.1 – 2.4 (V-01, V-04, N-01, N-02b) – nezávisia od iných DR.
4. deployment.yaml, technology-context.yaml (DR-0902, DR-0913).
5. components.yaml (DR-0906), definície IDs (DR-0907), Application Interface v modeli (DR-0914).
6. US-001.feature SC-001-17 + BR-011 (DR-0908), traceability.yaml.
7. ADR-009 (DR-0905).
8. Manifest: verzia 0.4.0, komentáre (2.3), nové views ak DR-0914 = A. Drafty sú v `spec/drafts/` (working) a registráciu
   nepotrebujú. Ak DR-0907 = B, pribudne `authoritative.id_register`.
9. Až potom IMPLEMENT_CHANGE pre FR1-UI (contract 0.2.1 sa nemení, nie je potrebný DESIGN_CONTRACT) a VERIFY_CHANGE.
