# ADR-005 Štýl rozhrania: HTTP API podľa OpenAPI 3.1

> Draft pripravený AI v režime PREPARE_SPEC (log 0002). Sekcia Decision zapisuje ľudské rozhodnutie DR-08.
> Stav mení iba človek.

## Status
Accepted

## Owner
Martin Štefanko (Software architect)

## Date
2026-11-25

## Context
DESIGN_CONTRACT pre FR1 potrebuje vedieť, či má Application Service AS-SpravaRezervaciiAKlientov sprístupniť
synchronous HTTP API, event alebo iný interface (AGENTS.md kap. 8, krok 3). contract-design-profile.yaml mal
v 0.1.0 iba kostru s hodnotami null.

## Options
1. Synchronous HTTP API popísané v OpenAPI 3.1.
2. Asynchronous events (AsyncAPI).
3. Iný interface (napr. RPC).

## Decision
Rozhranie je HTTP API so špecifikáciou OpenAPI 3.1. Chyby sa vracajú ako problem details (RFC 9457).
Dátumy a časy sú vo formáte RFC 3339 v UTC (DR-08, rozhodol Martin Štefanko, 2026-11-25).

## Consequences
- contract-design-profile.yaml má vyplnené `interface_style`, `http.specification`, `http.errors`
  a `http.date_time`. Ostatné položky (naming, correlation ID, compatibility) zostávajú otvorené.
- Pomenované dôvody odmietnutia (DR-11) sa v contracte vyjadria ako problem details.
- Otváracie hodiny 10:00–22:00 (DR-01) platia v časovom pásme Europe/Bratislava. API prijíma a vracia čas
  v UTC (RFC 3339) a systém ho na overenie otváracích hodín prepočíta (DR-18).
- Vytvorenie rezervácie nie je idempotentné (DR-28). Contract nezavádza kľúč idempotencie.
- Asynchronous komunikácia sa pre FR1 nepoužije. Komunikačný štýl INT-001 až INT-003 tým nie je rozhodnutý.

## Related elements
FR1, AS-SpravaRezervaciiAKlientov, AC-ModulRezervaciiAKlientov, spec/policies/contract-design-profile.yaml,
BR-010

## Specification baseline
0.2.0 / architecture baseline B-01
