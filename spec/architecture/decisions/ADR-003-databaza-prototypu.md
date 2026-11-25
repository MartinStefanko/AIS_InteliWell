# ADR-003 Databázový systém prototypu: SQLite

> Draft pripravený AI v režime PREPARE_SPEC (log 0002). Sekcia Decision zapisuje ľudské rozhodnutie DR-06.
> Stav mení iba človek.

## Status
Accepted

## Owner
Martin Štefanko (Software architect)

## Date
2026-11-25

## Context
Layered View TO-BE a docx kap. 10.1.2 uvádzajú System Software „Databázový systém“ (SSW-DatabazovySystem).
Je nasadený na serveri IntelliWell a uchováva rezervácie, evidenciu klientov a evidenciu zásahov. Enterprise
Continuum uvádza iba „Centrálna relačná databáza“ alebo „Relačná databáza“ (kap. 13). Konkrétny DBMS docx neurčuje.
IMPLEMENT_CHANGE pre FR1 potrebuje úložisko pre DO-Rezervacie a DO-EvidenciaKlientov.

## Options
1. SQLite pre prototyp; produkčný DBMS rozhodnúť neskôr.
2. Rovno zvoliť produkčný relačný DBMS (server).
3. Úložisko iba v pamäti (bez perzistencie) pre prototyp.

## Decision
Pre prototyp sa použije SQLite. DBMS pre produkciu zostáva otvorený (DR-06, rozhodol Martin Štefanko,
2026-11-25).

## Consequences
- SSW-DatabazovySystem má pre prototyp produkt SQLite; pre produkciu `product: null`
  (technology-context.yaml).
- SQLite je knižnica v procese aplikácie, nie samostatný databázový server. Layered View zobrazuje Databázový
  systém ako samostatný System Software na NODE-ServerIntelliWell. Pre prototyp ide o zjednodušenie, ktoré model
  nemení.
- Invariant BR-003 a pravidlo súbehu (ADR-001) musia byť vynútené aj nad SQLite.
- Prechod na produkčný DBMS bude vyžadovať nové ADR a overenie BR-003 a ADR-001 na novom DBMS.
- Klienti pre prototyp sú seed dáta (DR-03). Spôsob ich naplnenia určí IMPLEMENT_CHANGE.

## Related elements
SSW-DatabazovySystem, TS-DatabazovaSluzba, NODE-ServerIntelliWell, DO-Rezervacie, DO-EvidenciaKlientov,
BR-003, ADR-001, ADR-006

## Specification baseline
0.2.0 (draft) / architecture baseline B-01
