# 0005 – DESIGN_CONTRACT – revízia contractu FR1 (0.1.0 → 0.2.0)

| Pole | Hodnota |
|---|---|
| Dátum | 2026-11-25 |
| Poradové číslo | 0005 |
| Režim | DESIGN_CONTRACT |
| Verzia šablóny promptu | spec/prompts/design-contract.md, prompt_template_version 1.0 |
| Specification baseline | 0.3.0 (approved) |
| Architecture baseline | B-01 |
| Model | Claude Opus 5.5 (`claude-opus-5-5`), Claude Code |
| Commit | (doplní človek) |

## Prompt

~~~text
# Úloha 0005: revízia contractu FR1

Mode: DESIGN_CONTRACT
Change: FR1
Specification baseline: 0.3.0 (approved)
Prompt template: spec/prompts/design-contract.md v1.0

Prečítaj spec/manifest.yaml a over, že:
- baseline je 0.3.0 so stavom approved,
- ADR-001 až ADR-007 sú Accepted,
- contract-design-profile.yaml je approved,
- v open-decisions.md nie je otvorená žiadna DR.
Ak niečo z toho neplatí, zastav sa s DECISION REQUIRED.

## Úloha
1. Priprav impact analysis podľa kap. 14 AGENTS.md (zmeny oproti contractu 0.1.0).
2. Uprav spec/contracts/proposed/openapi-reservations.yaml a contract-mapping.yaml na verziu 0.2.0 podľa rozhodnutí DR-C01 až DR-C11,
   ktoré sú teraz v approved specification:
   - poradie chýb podľa BR-012 (vrátane 503 service-unavailable na pozícii podľa DR-C10),
   - odpoveď 503 podľa QR-002 a scenára SC-001-15,
   - scenár SC-001-16 (poradie pri viacerých porušeniach) s pozorovateľným výsledkom,
   - X-Correlation-ID podľa DR-C05 a DR-C09,
   - čas iba UTC so „Z“ (DR-C03), maxLength poznámky v Unicode code points (DR-C08),
   - polia a enum hodnoty anglicky (DR-C07, DR-C11), typy problémov ako URN,
   - claim „role“ = SpravcaNavstev a createdBy zo „sub“ (ADR-007).
   Odstráň všetky označenia „(návrh – DR-Cxx)“. Contract nesmie obsahovať nič, čo nemá oporu v approved specification.
3. Over, že každý scenár SC-001-01 až SC-001-16 má v contracte pozorovateľný výsledok a pomenovaný príklad.
4. Vytvor reports/contract-review-0.2.0.md:
   - porovnanie 0.1.0 → 0.2.0,
   - pokrytie scenárov a prevod BR na HTTP,
   - Human contract approval gate checklist (kap. 9 AGENTS.md): pri každom bode PASS / FAIL / UNKNOWN a dôkaz.
5. Over syntax OpenAPI 3.1 (validátor iba v scratchpade, nie v projekte). Ak nie je dostupný, označ UNKNOWN.
6. Nevytváraj approved contract ani approval.yaml, nepíš kód ani testy, nemeň approved specification.
   Ak contract vyžaduje zmenu specification, zastav sa s DECISION REQUIRED.
7. Log podľa CLAUDE.md (0005-2026-11-25-DESIGN_CONTRACT.md). Necommituj.

Odpoveď štruktúruj podľa kapitoly 15 AGENTS.md a na konci jasne napíš, či je contract pripravený na ľudské schválenie.
~~~

## Výstup AI

**Vstupná kontrola:** baseline 0.3.0 approved – PASS; ADR-001 až ADR-007 Accepted – PASS; profil approved – PASS;
žiadna otvorená DR – PASS.

**Zmenené (proposed, neschválené)**

- `spec/contracts/proposed/openapi-reservations.yaml` → 0.2.0: poradie chýb BR-012 v popise operácie
  (401 → 403 → 400 → 503 → 422 ×4 → 409); nová odpoveď 503 `service-unavailable` (QR-002, SC-001-15);
  `status: created` (DR-C11); schéma `UtcDateTime` s `pattern: 'Z$'` (DR-C03); `X-Correlation-ID` podľa DR-C05/C09
  (request bez obmedzenia tvaru, lebo neplatné ID sa neodmieta; response s patternom); 403 aj pri chýbajúcom claime `role`,
  `createdBy` = `sub` (ADR-007); z request schémy poznámky odstránený `maxLength`, aby 501 znakov viedlo k 422 a nie k 400
  (BR-012 poz. 8 vs. 3); pomenované príklady pre všetkých 16 scenárov (request aj response); 0 značiek „(návrh – DR-Cxx)“.
- `spec/contracts/proposed/contract-mapping.yaml` → 0.2.0: nové `error_order`, `response_fields`, `correlation_id`,
  `policy_rules_used`, `examples`; `resolved_decisions` DR-C01 – DR-C11; `open_decisions: [DR-C12]`.

**Vytvorené:** `reports/contract-review-0.2.0.md` – vstupná kontrola, impact analysis, porovnanie 0.1.0 → 0.2.0, pokrytie
16 scenárov, prevod BR → HTTP, approval gate, DR-C12, validácia.

**Validácia (iba scratchpad):** YAML PASS; OpenAPI 3.1 PASS (`openapi-spec-validator` 0.9.0); 35 príkladov vs. schémy PASS
(`jsonschema` 4.26, Draft 2020-12, format checker); pokrytie SC PASS; poradie BR-012 a titulky PASS; IDs PASS (nové iba DR-C12);
lint UNKNOWN.

**Approval gate:** 1, 2, 3, 4, 5, 8 PASS; 6, 7 UNKNOWN (DR-C12); 9 not applicable (potvrdí človek); 10 FAIL (DR-C12).

**Zistenia v podkladoch (neblokujú):** N-01 zastarané komentáre „návrh 0004“ v approved súboroch, N-02 `lifecycles.yaml`
`specification_version: 0.2.0`, N-03 `traceability.yaml` `missing_links` čaká na schválenie contractu. N-04 – N-08 v reporte.

**Otvorené DECISION REQUIRED:** DR-C12 – akú odpoveď vráti systém, ak bearer token nemá neprázdny claim `sub`
(A: 401 – odporúčanie AI, B: 403, C: neriešiť v contracte). Kam zapísať: ADR-007/nové ADR + NFR4/QR-001.

**Pripravený na schválenie:** nie, kým človek nerozhodne DR-C12.

Approved specification, `spec/contracts/approved/`, approval.yaml, kód ani testy sa nemenili. Necommitnuté.

## Ľudské rozhodnutie
- **Rozhodol:** Martin Štefanko (Software architect), 2026-11-25
- **Rozhodnutie:** Contract 0.2.0 zatiaľ neschvaľujem (bod 10 FAIL – DR-C12).
- **DR-C12:** A – 401 unauthenticated. Zapísal som ručne do ADR-007 a BR-011, specification 0.3.1.
- **Potvrdzujem:** odstránenie maxLength zo vstupnej schémy poznámky (zachovanie poradia BR-012) a prijímanie iba veľkého „Z“.

