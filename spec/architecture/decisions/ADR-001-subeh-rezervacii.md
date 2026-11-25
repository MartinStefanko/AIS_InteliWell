# ADR-001 Súbeh dvoch rezervácií na ten istý termín

> Draft pripravený AI v režime PREPARE_SPEC (log 0002). Sekcia Decision zapisuje ľudské rozhodnutie DR-02.
> Stav mení iba človek.

## Status
Accepted

## Owner
Martin Štefanko (Software architect)

## Date
2026-11-25

## Context
FR1 vyžaduje, aby systém pred uložením rezervácie overil dostupnosť termínu (BR-001) a po uložení termín
zablokoval (BR-002). NikaWell má jednu miestnosť, preto v jednom čase môže existovať najviac jedna rezervácia
(DR-01, invariant BR-003). Docx (kap. 5, 8.3.2) neurčuje, čo sa stane, ak dvaja správcovia návštev ukladajú
rezerváciu na prekrývajúci sa termín v rovnakom čase (open question v BR-002, verzia 0.1.0).

## Options
1. Prvá uložená rezervácia vyhrá; druhá je odmietnutá s dôvodom „termín obsadený“.
2. Dočasné podržanie termínu (hold) počas vytvárania rezervácie; ostatní termín vidia ako nedostupný.
3. Ponechať riešenie súbehu na manuálnom postupe recepcie (systém súbeh nerieši).

## Decision
Pri súbehu vyhrá prvá uložená rezervácia. Druhá dostane odmietnutie „termín obsadený“ (DR-02, rozhodol
Martin Štefanko, 2026-11-25). Systém v odmietnutí iný termín neponúka (DR-11).

## Consequences
- Invariant BR-003 (neprekrývanie termínov) musí platiť aj pri súbežnom spracovaní. Overenie dostupnosti
  a uloženie musia byť pre prekrývajúce sa termíny navzájom vylúčené; samotná kontrola pred uložením nestačí.
  Spôsob vynútenia určí IMPLEMENT_CHANGE v súlade s ADR-003.
- Správca návštev, ktorého rezervácia bola odmietnutá, musí s klientom dohodnúť iný termín mimo systému
  a rezerváciu zadať znova.
- Contract musí vyjadriť negatívny výsledok „termín obsadený“ (scenáre SC-001-04, SC-001-05, SC-001-06).
- Vytvorenie rezervácie nie je idempotentné (DR-28). Opakované odoslanie tej istej požiadavky (napr. po výpadku
  spojenia) odmietne invariant BR-003 s dôvodom „termín obsadený“, hoci prvá požiadavka uspela.
- Termín je polootvorený interval [začiatok, koniec). Rezervácia začínajúca presne v čase konca inej rezervácie
  nie je prekrytie (DR-19, SC-001-14).

## Related elements
FR1, BR-001, BR-002, BR-003, BR-009, BP-SpravaNavstev-04, AS-SpravaRezervaciiAKlientov,
AC-ModulRezervaciiAKlientov, DO-Rezervacie, SC-001-04, SC-001-05, SC-001-06

## Specification baseline
0.2.0 (draft) / architecture baseline B-01
