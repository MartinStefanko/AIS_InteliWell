# Rozhodnutia – FR1 (draft)

- status: "draft"
- architecture_baseline: "B-01", specification baseline: "0.3.1" (predchádzajúci baseline 0.3.0 approved)
- DR-01 až DR-17 sú ľudské rozhodnutia: rozhodol Martin Štefanko (Software architect), 2026-11-25, úloha 0002.
  Plné znenie je v `spec/prompts/log/0002-2026-11-25-PREPARE_SPEC.md`.
- DR-18 až DR-29 vznikli pri zapracovaní v 0002 a boli rozhodnuté doplňujúcim promptom 0002 (vrátane zmeny DR-05).
- DR-30 a DR-31 boli rozhodnuté doplňujúcim promptom 2 v 0002.
- ADR-001 až ADR-006 sú v stave **Accepted** (zapísal človek). ADR-007 (0004) je v stave **Accepted** (zapísal človek).
- DR-C01 až DR-C08 (contract review 0.1.0) rozhodol človek; zapísané v 0004. DR-C09 až DR-C11 rozhodol človek doplňujúcim promptom 0004. DR-C12 (contract review 0.2.0) rozhodol človek a zapísal ručne. Žiadna DR nie je otvorená.
- DR-V02 (verification 0008): presnosť startTime – rozhodnuté C, známe obmedzenie prototypu (rozhodol Martin Štefanko, 2026-11-25).

## Rozhodnuté k contract review 0.2.0 (DR-C12)

Rozhodol Martin Štefanko (Software architect), 2026-11-25. Znenie otázky: `reports/contract-review-0.2.0.md`. Zapísal človek ručne (AI v režime DESIGN_CONTRACT nesmie meniť specification).

| ID | Rozhodnutie (skrátene) | Zapísané v |
|---|---|---|
| DR-C12 | platný token bez neprázdneho claimu `sub` = neprihlásená osoba → 401 unauthenticated (možnosť A) | ADR-007 (doplnenie 2026-11-25); BR-011 (statement); specification 0.3.1 |

## Rozhodnuté doplňujúcim promptom 0004 (DR-C09 – DR-C11, owneri)

Rozhodol Martin Štefanko (Software architect), 2026-11-25.

| ID | Rozhodnutie (skrátene) | Zapísané v |
|---|---|---|
| DR-C09 | X-Correlation-ID: znaky 0x21–0x7E, dĺžka 1–128; prázdna hodnota je neplatná | contract-design-profile.yaml (http.correlation_id_format) |
| DR-C10 | 503 service-unavailable je v poradí BR-012 po 400 a pred prvým 422 | BR-012 (order, pozícia 4); QR-002 (position_in_reason_order) |
| DR-C11 | hodnoty enum v API anglicky; stav „vytvorená“ = `created` | contract-design-profile.yaml (http.naming.enum_values); lifecycles.yaml (api_value); glossary (Rezervácia) |
| – | owner QR-002 a CON-001 až CON-003 = ACT-Manazer | requirements.yaml; quality-requirements.yaml |

Znenie otázok DR-C09 – DR-C11 (archív):

### DR-C09

```text
DECISION REQUIRED

Kontext:
DR-C05, DR-31; contract-design-profile.yaml http.correlation_id_format; baseline 0.3.0 (návrh) / B-01

Čo specification neurčuje alebo v čom si odporuje:
„Tlačiteľné ASCII znaky“ nemá jednoznačný rozsah (s medzerou 0x20 alebo bez nej) a nie je určené, či je prázdna hodnota neplatná.

Možnosti:
A. Znaky 0x21–0x7E (bez medzery), dĺžka 1–128; prázdna hodnota je neplatná → systém vygeneruje UUID.
B. Znaky 0x20–0x7E (s medzerou), dĺžka 1–128; prázdna hodnota je neplatná → systém vygeneruje UUID.

Odporúčanie AI:
A – ID bez medzier sa jednoduchšie hľadá v logoch. Ide iba o odporúčanie.

Otázka pre človeka:
Ktorý rozsah znakov a akú minimálnu dĺžku má platné X-Correlation-ID?

Kam zapísať rozhodnutie:
Project policy (contract-design-profile.yaml http.correlation_id_format)
```

### DR-C10

```text
DECISION REQUIRED

Kontext:
DR-C04 (BR-012), DR-C06 (QR-002); baseline 0.3.0 (návrh) / B-01

Čo specification neurčuje alebo v čom si odporuje:
Poradie dôvodov 401 → 403 → 400 → 422 → 409 neurčuje, kedy sa vráti 503, ak je úložisko nedostupné a požiadavka zároveň porušuje iné pravidlo.

Možnosti:
A. 503 sa vráti hneď, keď systém potrebuje úložisko: po 401, 403 a 400, pred prvým 422 (client-not-found vyžaduje čítanie evidencie klientov).
B. 503 sa vráti až po všetkých kontrolách, ktoré úložisko nepotrebujú (400, start-not-in-future, outside-opening-hours, note-too-long). Tým sa mení poradie z DR-C04 pri výpadku.

Odporúčanie AI:
A – zachováva poradie z DR-C04 bez výnimky. Ide iba o odporúčanie.

Otázka pre človeka:
Na ktorom mieste poradia BR-012 sa uplatní 503 service-unavailable?

Kam zapísať rozhodnutie:
Rule (BR-012) + Quality requirement (QR-002)
```

### DR-C11

```text
DECISION REQUIRED

Kontext:
DR-C07 (polia anglicky lowerCamelCase), lifecycles.yaml (stav „vytvorená“, id `vytvorena`), contract 0.1.0 (`status: [vytvorena]`); baseline 0.3.0 (návrh)

Čo specification neurčuje alebo v čom si odporuje:
DR-C07 určuje jazyk názvov polí, ale nie jazyk hodnôt enum (napr. stav rezervácie).

Možnosti:
A. Hodnoty enum podľa ID v lifecycles.yaml (`vytvorena`) – zhodné so špecifikáciou.
B. Hodnoty enum anglicky (napr. `created`) – zhodné s jazykom polí; vyžaduje mapovanie na lifecycles.yaml.

Odporúčanie AI:
A – contract 0.1.0 ho už používa a nevyžaduje ďalšie mapovanie. Ide iba o odporúčanie.

Otázka pre človeka:
V akom jazyku a tvare sú hodnoty enum v contracte?

Kam zapísať rozhodnutie:
Project policy (contract-design-profile.yaml http.naming.enum_values)
```

## Rozhodnuté k contract review 0.1.0 (DR-C01 – DR-C08)

Rozhodol Martin Štefanko (Software architect), 2026-11-25, úloha 0004. Znenie otázok: `reports/contract-review-0.1.0.md`, časť 6.

| ID | Rozhodnutie (skrátene) | Zapísané v |
|---|---|---|
| DR-C01 | hodnota claimu `role` pre správcu návštev = `SpravcaNavstev` | ADR-007 (Accepted); quality-requirements.yaml (NFR4 role_claim_value); contract-design-profile.yaml (odkaz) |
| DR-C02 | autor rezervácie (`createdBy`) z claimu `sub` | ADR-007 (Accepted); quality-requirements.yaml (QR-001 identity_source) |
| DR-C03 | API prijíma čas iba v UTC s príponou „Z“; inak 400 | contract-design-profile.yaml (http.date_time_input) |
| DR-C04 | jeden dôvod v poradí 401 → 403 → 400 → 422 (client-not-found → start-not-in-future → outside-opening-hours → note-too-long) → 409 slot-occupied | BR-012 (nové); BR-010 (reason_ordering); SC-001-16 |
| DR-C05 | X-Correlation-ID: reťazec max. 128 tlačiteľných ASCII znakov; ak chýba alebo je neplatný, systém vygeneruje UUID a vráti ho | contract-design-profile.yaml (http.correlation_id_format, correlation_id_invalid); spresnenie DR-C09 |
| DR-C06 | nedostupné úložisko → 503 `service-unavailable`, rezervácia sa neuloží, bez automatického opakovania | QR-002 (nové); SC-001-15; poradie DR-C10 |
| DR-C07 | polia anglicky lowerCamelCase; typy problémov `urn:intelliwell:problem:<kód>` | contract-design-profile.yaml (http.naming.fields, http.problem_types); enum DR-C11 |
| DR-C08 | 500 znakov poznámky = Unicode code points | BR-008 (length_unit); glossary (Poznámka) |

## Rozhodnuté (DR-01 – DR-17)

| ID | Téma | Rozhodnutie (skrátene) | Zapísané v |
|---|---|---|---|
| DR-01 | Definícia termínu | začiatok zadá recepčná, 60 min, otváracie hodiny 10:00–22:00, 1 miestnosť → max. 1 rezervácia v čase, termín celý v otváracích hodinách | BR-003, BR-004, BR-005; glossary (Termín, Otváracie hodiny, Miestnosť NikaWell) |
| DR-02 | Súbeh | prvá uložená vyhrá, druhá dostane „termín obsadený“ | BR-009; ADR-001 |
| DR-03 | Klient bez evidencie | iba existujúci klient; FR2 mimo slice; klienti = seed dáta | BR-007; requirements.yaml (FR1 scope); data-objects.yaml |
| DR-04 | Stavy rezervácie | iba „vytvorená“; zrušenie mimo rozsahu | lifecycles.yaml; BR-002 |
| DR-05 | Platforma | ~~.NET 8~~ → .NET 10 (LTS, C#), ASP.NET Core (zmena v doplňujúcom prompte) | ADR-002; technology-context.yaml; deployment.yaml |
| DR-06 | DBMS | SQLite pre prototyp; produkcia otvorená | ADR-003; technology-context.yaml |
| DR-07 | Prihlasovanie a oprávnenia | lokálne účty, JWT bearer, vytvoriť smie iba ROLE-SpravcaNavstev | ADR-004; BR-011; quality-requirements.yaml (NFR4); contract-design-profile.yaml; actors-processes.yaml |
| DR-08 | Štýl rozhrania | HTTP API, OpenAPI 3.1, RFC 9457, RFC 3339 UTC | ADR-005; contract-design-profile.yaml |
| DR-09 | Technology context | applicable | technology-context.yaml; deployment.yaml (nový) |
| DR-10 | Saunový majster | mimo FR1 | requirements.yaml (scope); components.yaml; interactions.yaml (INT-002, INT-003) |
| DR-11 | Nedostupný termín | odmietnutie s pomenovaným dôvodom, bez ponuky iného termínu | BR-010; actors-processes.yaml (exceptions) |
| DR-12 | Vlastník Data Objects | AC-ModulRezervaciiAKlientov | ADR-006; components.yaml; data-objects.yaml |
| DR-13 | Audit | zaznamenať, kto a kedy rezerváciu vytvoril | QR-001 (quality-requirements.yaml) |
| DR-14 | Validácia | začiatok v budúcnosti, v otváracích hodinách, klient existuje, poznámka voliteľná max. [500] | NFR1 (quality-requirements.yaml); BR-005 – BR-008 |
| DR-15 | Owner a znenie FR1 | owner = Manažér NikaWell (= ACT-Manazer, DR-21); znenie FR1 potvrdené | requirements.yaml (FR1) |
| DR-16 | Zdrojový dokument | AIS_IntelliWell.docx platí pre B-01 (potvrdené v 0001) | manifest.yaml (source_documents) |
| DR-17 | Poznámka, sumarizácia | poznámka voliteľná; sumarizácia je UI, mimo contractu; UI mimo prototypu | BR-008; requirements.yaml (FR1 scope); glossary |

## Rozhodnuté doplňujúcim promptom 0002 (DR-05 zmena, DR-18 – DR-29)

Rozhodol Martin Štefanko (Software architect), 2026-11-25.

| ID | Rozhodnutie (skrátene) | Zapísané v |
|---|---|---|
| DR-05 (zmena) | .NET 10 (LTS) namiesto .NET 8 | ADR-002; technology-context.yaml; deployment.yaml; id-mapping.yaml (SSW-RuntimeDotNet) |
| DR-18 | otváracie hodiny v Europe/Bratislava; API v UTC (RFC 3339), systém prepočíta | BR-005; ADR-005; contract-design-profile.yaml (business_time_zone); glossary |
| DR-19 | termín je polootvorený interval [začiatok, koniec); nadväzujúca rezervácia je povolená | BR-003, BR-004; ADR-001; SC-001-14; glossary |
| DR-20 | otváracie hodiny platia každý deň rovnako; sviatky a výnimky mimo rozsahu | BR-005; requirements.yaml (FR1 out_of_scope); glossary |
| DR-21 | owner FR1, BR-*, NFR1, NFR4, QR-001 = Business Actor Manažér (ACT-Manazer) | requirements.yaml; business-rules.yaml; quality-requirements.yaml; actors-processes.yaml; id-mapping.yaml; glossary |
| DR-22 | 500 znakov je konečná hodnota | BR-008 |
| DR-23 | tokeny pre prototyp vydáva dotnet user-jwts; rola v claime „role“; prihlasovací endpoint mimo slice | ADR-004; NFR4; contract-design-profile.yaml; deployment.yaml |
| DR-24 | contract FR1 neobsahuje vyhľadanie klienta; prijíma identifikátor existujúceho klienta | requirements.yaml (FR1 scope); data-objects.yaml |
| DR-25 | INT-001 nie je súčasťou FR1 | interactions.yaml; requirements.yaml (FR1 scope) |
| DR-26 | audituje sa iba úspešné vytvorenie | QR-001 (scope) |
| DR-27 | poznámka je osobný údaj; obsah sa nesmie zapisovať do aplikačného logu | CON-003 (requirements.yaml); data-objects.yaml; contract-design-profile.yaml (logging) |
| DR-28 | vytvorenie nie je idempotentné; opakovanie odmietne BR-003 („termín obsadený“) | BR-003; ADR-001; ADR-005; contract-design-profile.yaml (idempotency) |
| DR-29 | US-001 ostáva v spec/stories/, úprava manifestu ostáva; postup schválený | manifest.yaml (bez zmeny) |

## Rozhodnuté doplňujúcim promptom 2 (DR-30 – DR-31)

Rozhodol Martin Štefanko (Software architect), 2026-11-25.

| ID | Rozhodnutie (skrátene) | Zapísané v |
|---|---|---|
| DR-30 | contract prijíma systémový identifikátor klienta (UUID), nie je odvodený z osobných údajov | data-objects.yaml; contract-design-profile.yaml (identifiers.client) |
| DR-31 | cesty kebab-case v množnom čísle s prefixom /api/v1; operationId lowerCamelCase; X-Correlation-ID (ak chýba, systém ho vygeneruje a vráti); major verzia v URL, breaking change = rozhodnutie človeka + nová major verzia; prototyp iba v1 | contract-design-profile.yaml (http.naming, correlation_id_*, compatibility) |

Znenie otázok DR-30 a DR-31 (archív):

### DR-30

```text
DECISION REQUIRED

Kontext:
DR-24 (contract prijíma identifikátor existujúceho klienta), BR-007, DO-EvidenciaKlientov (owner AC-ModulRezervaciiAKlientov,
ADR-006), DR-03 (seed dáta); baseline 0.2.0 / B-01

Čo specification neurčuje alebo v čom si odporuje:
Nie je určené, čo je identifikátor klienta. Docx uvádza iba „identifikačné údaje“ a evidencia klientov nemá definovaný kľúč.

Možnosti:
A. Systémový identifikátor bez významu, ktorý pridelí AC-ModulRezervaciiAKlientov (napr. UUID). Neobsahuje osobný údaj.
B. Identifikátor odvodený z osobných údajov (napr. e-mail, telefón). Bol by osobným údajom a potreboval by rovnaké pravidlá ako CON-003.

Odporúčanie AI:
A. Contract tak neprenáša osobný údaj a vyhne sa duplicite pri zmene kontaktu.

Otázka pre človeka:
Akú formu má identifikátor klienta, ktorý contract FR1 prijíma?

Kam zapísať rozhodnutie:
data-objects.yaml (DO-EvidenciaKlientov) + Rule (BR-007)
```

### DR-31

```text
DECISION REQUIRED

Kontext:
contract-design-profile.yaml (http.naming.paths, http.naming.operation_ids, http.correlation_id_header,
compatibility.breaking_change, compatibility.versioning = null); AGENTS.md kap. 5 (compatibility a versioning policy sa nesmie domýšľať)

Čo specification neurčuje alebo v čom si odporuje:
Profil neurčuje pomenovanie ciest a operácií, correlation ID ani politiku verziovania a kompatibility API.

Možnosti:
A. Človek doplní konvencie do profilu pred DESIGN_CONTRACT (napr. kebab-case množné podstatné mená, lowerCamelCase operationId,
   verzia v ceste /v1, breaking change = rozhodnutie človeka).
B. DESIGN_CONTRACT navrhne pomenovanie ako súčasť proposed contractu; verziovanie a kompatibilitu určí človek pri schválení contractu.

Odporúčanie AI:
B pre pomenovanie (je overiteľné pri schválení contractu). Verziovanie a kompatibilitu musí určiť človek (A), lebo AI ich nesmie domýšľať.

Otázka pre človeka:
Doplníš konvencie pomenovania a verziovania do profilu, alebo ich má navrhnúť DESIGN_CONTRACT na tvoje schválenie?

Kam zapísať rozhodnutie:
Project policy (contract-design-profile.yaml)
```

## Znenie otázok DR-18 – DR-29 (archív; rozhodnuté vyššie)

| ID | Téma | Kam zapísať | Pôvodne blokovalo |
|---|---|---|---|
| DR-18 | Časové pásmo otváracích hodín (API v UTC) | Rule (BR-005) + Project policy | DESIGN_CONTRACT (validácia), IMPLEMENT_CHANGE |
| DR-19 | Nadväzujúce termíny (koniec = začiatok) | Rule (BR-003) + Scenario | IMPLEMENT_CHANGE, doplnenie scenára |
| DR-20 | Platia otváracie hodiny každý deň? | Rule (BR-005) | IMPLEMENT_CHANGE |
| DR-21 | ID roly ownera FR1 a owner pravidiel BR-* | Requirement / Rule | schválenie FR1 a rules |
| DR-22 | Potvrdenie hodnoty „[500]“ | Rule (BR-008) | DESIGN_CONTRACT (limit poľa) |
| DR-23 | Vydávanie JWT tokenu (prihlásenie, rola v tokene) | ADR-004 / Requirement | security časť contractu, IMPLEMENT_CHANGE |
| DR-24 | Je vyhľadanie klienta súčasťou contractu FR1? | Requirement (FR1 scope) | DESIGN_CONTRACT (rozsah operácií) |
| DR-25 | Relevancia INT-001 pre FR1 | interactions.yaml | DESIGN_CONTRACT |
| DR-26 | Audit odmietnutých pokusov | Requirement (QR-001) | DESIGN_CONTRACT |
| DR-27 | Citlivosť údajov DO-Rezervacie | data-objects.yaml | DESIGN_CONTRACT (approval gate) |
| DR-28 | Idempotencia / opakované odoslanie | Project policy / Rule | DESIGN_CONTRACT (approval gate) |
| DR-29 | Umiestnenie stories a úprava manifestu AI vs. AGENTS.md kap. 7 | Project policy | nič neblokuje; potvrdenie postupu |

### DR-18

```text
DECISION REQUIRED

Kontext:
FR1, BR-005, DR-01, DR-08, ADR-005; baseline 0.2.0 (draft) / B-01

Čo specification neurčuje alebo v čom si odporuje:
Otváracie hodiny 10:00–22:00 sú miestny čas, ale API prenáša dátumy v RFC 3339 UTC a časové pásmo NikaWell nie je nikde určené.

Možnosti:
A. Pevné časové pásmo NikaWell (napr. Europe/Bratislava s letným časom) v project policy; systém prevádza UTC na miestny čas.
B. Otváracie hodiny sa vyhodnocujú priamo v UTC (10:00–22:00 UTC); v zime aj v lete sa posunú voči miestnemu času.

Odporúčanie AI:
A. Zodpovedá to významu otváracích hodín pre recepciu. Konkrétne pásmo určí človek.

Otázka pre človeka:
V akom časovom pásme sa vyhodnocujú otváracie hodiny 10:00–22:00?

Kam zapísať rozhodnutie:
Rule (BR-005) + Project policy (contract-design-profile.yaml alebo technology-context.yaml)
```

### DR-19

```text
DECISION REQUIRED

Kontext:
FR1, BR-003 (invariant), DR-01; baseline 0.2.0 / B-01

Čo specification neurčuje alebo v čom si odporuje:
Nie je určené, či sa rezervácia 14:00–15:00 prekrýva s rezerváciou začínajúcou o 15:00.

Možnosti:
A. Termín je polootvorený interval [začiatok, začiatok + 60 min) → nadväzujúce termíny sú povolené.
B. Medzi rezerváciami musí byť medzera (napr. na prípravu miestnosti) → treba určiť jej dĺžku.

Odporúčanie AI:
A, ak NikaWell nepotrebuje čas na prípravu miestnosti medzi návštevami.

Otázka pre človeka:
Môže ďalšia rezervácia začať presne v čase, keď končí predchádzajúca?

Kam zapísať rozhodnutie:
Rule (BR-003) + Scenario (doplniť pozitívny alebo negatívny scenár)
```

### DR-20

```text
DECISION REQUIRED

Kontext:
BR-005, DR-01; baseline 0.2.0 / B-01

Čo specification neurčuje alebo v čom si odporuje:
Nie je určené, či otváracie hodiny 10:00–22:00 platia každý deň vrátane víkendov a sviatkov.

Možnosti:
A. Platia každý deň bez výnimky.
B. Existujú zatvorené dni alebo iné hodiny (treba ich zoznam a zdroj).

Otázka pre človeka:
Platia otváracie hodiny 10:00–22:00 každý deň v roku?

Kam zapísať rozhodnutie:
Rule (BR-005)
```

### DR-21

```text
DECISION REQUIRED

Kontext:
FR1 (owner podľa DR-15), BR-001 – BR-011, NFR1, NFR4, QR-001; Organization View TO-BE
(spec/architecture/views/B-01-organization-tobe.png, status draft); baseline 0.2.0 / B-01

Čo specification neurčuje alebo v čom si odporuje:
DR-15 určuje ownera FR1 ako „rolu Manažér NikaWell“. Organization View však zobrazuje Manažéra ako Business Actor s rolami
„Kontrolór zásahov recepcie“ a „Analytik efektivity služieb“. Rola „Manažér NikaWell“ na view nie je, a owner rules a NFR nie je určený.

Možnosti:
A. Owner je Business Actor Manažér (navrhnúť ID ACT-Manazer).
B. Owner je jedna z jeho rolí zobrazených vo view (navrhnúť ID roly).
C. Owner je nová rola „Manažér NikaWell“, ktorú človek doplní do modelu v Sparx EA.

Otázka pre človeka:
Ktorý prvok modelu (a s akým ID) je owner FR1 a je ten istý owner aj pre BR-001 – BR-011, NFR1, NFR4 a QR-001?

Kam zapísať rozhodnutie:
Requirement (FR1.owner) + Rule (owner)
```

### DR-22

```text
DECISION REQUIRED

Kontext:
BR-008, DR-14; baseline 0.2.0 / B-01

Čo specification neurčuje alebo v čom si odporuje:
DR-14 uvádza limit poznámky ako „[500]“; hranaté zátvorky môžu znamenať zástupnú hodnotu.

Možnosti:
A. 500 znakov je konečná hodnota.
B. Ide o zástupnú hodnotu; človek určí inú.

Otázka pre človeka:
Je maximálna dĺžka poznámky definitívne 500 znakov?

Kam zapísať rozhodnutie:
Rule (BR-008)
```

### DR-23

```text
DECISION REQUIRED

Kontext:
NFR4, DR-07, ADR-004 (Proposed), D-003 Prihlasovanie personálu; baseline 0.2.0 / B-01

Čo specification neurčuje alebo v čom si odporuje:
Nie je určené, kto vydáva JWT token (operácia prihlásenia, issuer), ako je v ňom vyjadrená rola ROLE-SpravcaNavstev, ako dlho platí ani či je prihlásenie súčasťou tohto slice.

Možnosti:
A. Prihlásenie (vydanie tokenu z lokálneho účtu) je súčasťou slice FR1 a contractu; pravidlá tokenu určí ADR-004.
B. Prihlásenie je mimo slice; prototyp akceptuje tokeny vydané dohodnutým spôsobom (napr. testovací issuer) a contract FR1 iba vyžaduje bearer token.

Odporúčanie AI:
B drží slice malý. Spôsob vydania tokenu pre prototyp však musí byť zapísaný.

Otázka pre človeka:
Je vydávanie JWT tokenu súčasťou slice FR1, a ak nie, odkiaľ prototyp tokeny dostane a v ktorom claime je rola?

Kam zapísať rozhodnutie:
ADR (ADR-004) + Project policy (contract-design-profile.yaml security.token_issuance)
```

### DR-24

```text
DECISION REQUIRED

Kontext:
FR1 (znenie: „vyhľadať klienta v systéme“), BP-SpravaNavstev-01, DR-03, DR-17; baseline 0.2.0 / B-01

Čo specification neurčuje alebo v čom si odporuje:
Potvrdené znenie FR1 obsahuje vyhľadanie klienta. Nie je určené, či contract FR1 má obsahovať aj operáciu vyhľadania klientov,
alebo iba vytvorenie rezervácie s identifikátorom klienta.

Možnosti:
A. Contract obsahuje vytvorenie rezervácie aj vyhľadanie klientov (čítanie DO-EvidenciaKlientov).
B. Contract obsahuje iba vytvorenie rezervácie. Vyhľadanie je mimo slice, podobne ako UI.

Otázka pre človeka:
Má contract FR1 obsahovať operáciu na vyhľadanie klienta?

Kam zapísať rozhodnutie:
Requirement (FR1.scope)
```

### DR-25

```text
DECISION REQUIRED

Kontext:
INT-001 (Modul rezervácií a klientov → Modul riadenia a monitorovania zariadení, „parametre rezervácie“),
Application Cooperation View TO-BE; baseline 0.2.0 / B-01

Čo specification neurčuje alebo v čom si odporuje:
Nie je určené, či má vytvorenie rezervácie (FR1) odovzdať parametre rezervácie modulu riadenia zariadení.

Možnosti:
A. INT-001 nie je súčasťou FR1 (patrí k príprave prostredia, fáza 2).
B. INT-001 je súčasťou FR1. Potom treba rozhodnúť komunikačný štýl a správanie pri nedostupnosti modulu.

Odporúčanie AI:
A. Docx kap. 8.3.2 viaže parametre na prípravu prostredia a modul riadenia zariadení patrí do PL-Faza2.

Otázka pre človeka:
Je odovzdanie parametrov rezervácie (INT-001) súčasťou FR1?

Kam zapísať rozhodnutie:
interactions.yaml (relevance_for_fr1) + Requirement (FR1.scope)
```

### DR-26

```text
DECISION REQUIRED

Kontext:
QR-001, DR-13, NFR4; baseline 0.2.0 / B-01

Čo specification neurčuje alebo v čom si odporuje:
DR-13 určuje záznam o vytvorenej rezervácii. Nie je určené, či sa zaznamenávajú aj odmietnuté a neoprávnené pokusy.

Možnosti:
A. Iba úspešné vytvorenie (kto, kedy).
B. Aj odmietnuté pokusy (kto, kedy, dôvod) vrátane neoprávnených.

Otázka pre človeka:
Majú sa zaznamenávať aj odmietnuté pokusy o vytvorenie rezervácie?

Kam zapísať rozhodnutie:
Requirement (QR-001)
```

### DR-27

```text
DECISION REQUIRED

Kontext:
DO-Rezervacie (sensitive_data: null), NFR4 (GDPR v docx kap. 8.1), BR-008; baseline 0.2.0 / B-01

Čo specification neurčuje alebo v čom si odporuje:
Nie je určené, či rezervácia obsahuje citlivé údaje. Identifikuje klienta a poznámka je voľný text, do ktorého môže recepčná zapísať aj zdravotný údaj.

Možnosti:
A. DO-Rezervacie je citlivý údaj (rovnaké zaobchádzanie ako evidencia klientov, napr. nelogovať poznámku).
B. DO-Rezervacie nie je citlivý; recepčná nesmie do poznámky zapisovať citlivé údaje (organizačné pravidlo).

Otázka pre človeka:
Sú údaje rezervácie (najmä poznámka) citlivé údaje?

Kam zapísať rozhodnutie:
data-objects.yaml (sensitive_data) + prípadne Constraint
```

### DR-28

```text
DECISION REQUIRED

Kontext:
ADR-001 (súbeh), BR-009, DR-08, contract-design-profile.yaml (http.idempotency: null); AGENTS.md kap. 9; baseline 0.2.0 / B-01

Čo specification neurčuje alebo v čom si odporuje:
Nie je určené, čo sa stane pri opakovanom odoslaní tej istej požiadavky (napr. po výpadku spojenia). Podľa DR-02 by
opakovanie dostalo „termín obsadený“, hoci prvá požiadavka uspela.

Možnosti:
A. Bez idempotencie: opakovanie dostane „termín obsadený“; recepčná si stav overí inak.
B. Idempotentné vytvorenie (napr. klientom zaslaný kľúč požiadavky): opakovanie vráti pôvodný výsledok.

Otázka pre človeka:
Má byť vytvorenie rezervácie idempotentné pri opakovanom odoslaní?

Kam zapísať rozhodnutie:
Project policy (contract-design-profile.yaml http.idempotency) + Rule
```

### DR-29

```text
DECISION REQUIRED

Kontext:
AGENTS.md kap. 7 („Textové doplnenia ukladá ako draft do spec/drafts/… Návrh zmeny manifestu odovzdá človeku“),
spec/prompts/prepare-spec.md v1.0 (US-001 do spec/drafts/), úloha 0002 (US-001 do spec/stories/, úprava manifestu)

Čo specification neurčuje alebo v čom si odporuje:
Úloha 0002 výslovne žiadala zapísať stories do spec/stories/ a upraviť manifest. AGENTS.md kap. 7 a šablóna promptu však
ukladajú drafty do spec/drafts/ a zmenu manifestu iba navrhujú. AI postupovala podľa úlohy, ponechala status draft a zmeny necommitla.

Možnosti:
A. Potvrdiť postup (drafty v cieľových súboroch so status draft, necommitnuté zmeny = návrh na kontrolu) a upraviť prepare-spec.md na v1.1.
B. Presunúť US-001 a feature do spec/drafts/ a manifest vrátiť; zmeny manifestu robí iba človek.

Otázka pre človeka:
Ponechať US-001 v spec/stories/ a úpravu manifestu, alebo ich presunúť do spec/drafts/?

Kam zapísať rozhodnutie:
Project policy (spec/prompts/prepare-spec.md, README)
```