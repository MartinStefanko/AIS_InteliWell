# ADR-007 Hodnota claimu roly a identita autora rezervácie v JWT

> Draft pripravený AI v režime PREPARE_SPEC (log 0004). Sekcia Decision zapisuje ľudské rozhodnutia DR-C01
> a DR-C02 z contract review 0.1.0 (`reports/contract-review-0.1.0.md`). Dopĺňa ADR-004, ktoré sa nemení.
> Stav mení iba človek.

## Status
Accepted

## Owner
Martin Štefanko (Software architect)

## Date
2026-11-25

## Context
ADR-004 (Accepted) určuje autentifikáciu JWT bearer tokenom a rolu v claime `role`. Presnú hodnotu claimu
pre ROLE-SpravcaNavstev necháva na návrh contractu a schválenie človekom. ADR-004 tiež určuje, že identita
z tokenu je zdrojom údaja „kto rezerváciu vytvoril“ (QR-001), ale neurčuje, ktorý claim túto identitu nesie.
Proposed contract API-Reservations 0.1.0 tieto miesta označil ako otvorené (DR-C01, DR-C02).

## Options
DR-C01 – hodnota claimu `role` pre ROLE-SpravcaNavstev:
1. `SpravcaNavstev` – ID roly bez prefixu.
2. `ROLE-SpravcaNavstev` – identické s ID v špecifikácii.

DR-C02 – claim s identitou autora rezervácie (`createdBy`):
1. `sub` – štandardný identifikátor subjektu tokenu.
2. `unique_name` / `name` – meno osoby; môže sa meniť.

## Decision
- DR-C01: Hodnota claimu `role` pre ROLE-SpravcaNavstev je `SpravcaNavstev`.
- DR-C02: Identita autora rezervácie (`createdBy`, QR-001) sa berie z claimu `sub`.

Rozhodol Martin Štefanko (Software architect), 2026-11-25 (úloha 0004).

## Consequences
- Rezerváciu smie vytvoriť iba token, ktorý má v claime `role` hodnotu `SpravcaNavstev` (BR-011, SC-001-12).
  Iná hodnota alebo chýbajúci claim znamená nedostatočné oprávnenie.
- Záznam o vytvorení rezervácie (QR-001) obsahuje ako autora hodnotu claimu `sub` z tokenu.
- Prototyp vydáva tokeny cez `dotnet user-jwts` (ADR-004, DR-23). Token pre správcu návštev musí niesť rolu
  `SpravcaNavstev`.
- Proposed contract 0.1.0 už tieto hodnoty používa ako návrh. Po prijatí ADR sa v ďalšej verzii contractu
  (krok 0005) odstráni označenie „návrh – DR-C01/DR-C02“.
- Stabilita a formát hodnoty `sub` pri produkčnom vydávaní tokenov sú mimo slice, rovnako ako produkčné vydávanie
  tokenov (ADR-004).
- ADR-004 sa nemení. Toto ADR ho dopĺňa.

## Related elements
ADR-004, NFR4, BR-011, QR-001, ROLE-SpravcaNavstev, AC-ModulRezervaciiAKlientov, DO-Rezervacie, SC-001-01,
SC-001-12, DR-C01, DR-C02, API-Reservations 0.1.0 (proposed)

## Specification baseline
0.3.0 (draft) / architecture baseline B-01
