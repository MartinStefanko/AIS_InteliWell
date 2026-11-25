# ADR-006 Vlastníctvo Data Objects Rezervácie a Evidencia klientov

> Draft pripravený AI v režime PREPARE_SPEC (log 0002). Sekcia Decision zapisuje ľudské rozhodnutie DR-12.
> Stav mení iba človek.

## Status
Accepted

## Owner
Martin Štefanko (Software architect)

## Date
2026-11-25

## Context
Application Usage a Layered View TO-BE zobrazujú prístup k údajom Rezervácie a Evidencia klientov
zo systému AC-IntelliWell ako celku (docx kap. 9.2.2: IntelliWell ich číta aj zapisuje). Vlastníka na úrovni
modulu docx neurčuje. Modul rezervácií a klientov ich podľa docx kap. 9.1.2 „eviduje“. Ostatné moduly s nimi
súvisia: INT-001 odovzdáva parametre rezervácie, INT-003 zapisuje priradenie späť k rezervácii.

## Options
1. Vlastníkom oboch Data Objects je AC-ModulRezervaciiAKlientov; ostatné moduly k nim pristupujú cez jeho rozhranie.
2. Vlastníkom je AC-IntelliWell ako celok (spoločné údaje, priamy prístup všetkých modulov).
3. Rozdelené vlastníctvo (napr. evidencia klientov samostatne).

## Decision
Vlastníkom DO-Rezervacie a DO-EvidenciaKlientov je AC-ModulRezervaciiAKlientov (DR-12, rozhodol Martin
Štefanko, 2026-11-25).

## Consequences
- components.yaml: `owns_data_objects` modulu rezervácií a klientov obsahuje DO-Rezervacie
  a DO-EvidenciaKlientov. data-objects.yaml: `owner_component` je AC-ModulRezervaciiAKlientov.
- Ostatné moduly (riadenia a monitorovania zariadení, koordinácie saunových majstrov) nesmú tieto údaje meniť
  priamo, iba cez rozhranie vlastníka. To sa týka INT-001 a INT-003, ktoré sú mimo FR1.
- Väzba prístupu v modeli Sparx EA (AC-IntelliWell → Data Objects) je s rozhodnutím zlučiteľná, lebo modul
  je súčasťou IntelliWell. Ak sa má vlastníctvo zobraziť aj v modeli, upraví ho človek.
- Allowed dependencies modulu zostávajú nerozhodnuté (components.yaml).

## Related elements
DO-Rezervacie, DO-EvidenciaKlientov, AC-ModulRezervaciiAKlientov, AC-IntelliWell, INT-001, INT-003, FR1

## Specification baseline
0.2.0 (draft) / architecture baseline B-01
