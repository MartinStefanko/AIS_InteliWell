# ADR-009 Vynútenie invariantu BR-003 a pravidla súbehu BR-009 nad SQLite

> Draft pripravený AI v režime IMPLEMENT_CHANGE (úloha 0007, fáza 1 – plán). Toto nie je rozhodnutie.
> Stav mení iba človek.

## Status
Accepted

## Owner
Martin Štefanko (Software architect)

## Date
2026-11-25

## Context
ADR-001 (Accepted): pri súbehu vyhrá prvá uložená rezervácia, druhá dostane „termín obsadený“; overenie dostupnosti
a uloženie musia byť pre prekrývajúce sa termíny navzájom vylúčené. „Spôsob vynútenia určí IMPLEMENT_CHANGE
v súlade s ADR-003.“ ADR-003: úložisko prototypu je SQLite (knižnica v procese). Termín je polootvorený interval
[začiatok, začiatok + 60 min) s ľubovoľným začiatkom (nie mriežka slotov), preto prekrytie nemožno vynútiť
jednoduchým unikátnym indexom. QR-002: pri nedostupnom úložisku sa uloženie automaticky neopakuje.

## Options
1. **Zápisová transakcia `BEGIN IMMEDIATE`:** kontrola klienta, pravidiel, prekrytia aj vloženie bežia v jednej
   transakcii, ktorá od začiatku drží zápisový zámok databázy. Druhá súbežná požiadavka čaká na zámok
   (busy timeout Microsoft.Data.Sqlite, predvolene 30 s) a potom vidí už uloženú rezerváciu → 409 `slot-occupied`.
   Funguje aj pri viacerých procesoch nad tým istým súborom.
2. Zámok v procese (`SemaphoreSlim`) okolo kontroly a vloženia – jednoduché, ale platí iba pre jeden proces.
3. Trigger v SQLite, ktorý odmietne prekrývajúci sa `INSERT` – vynútenie v databáze, ale logika BR-003 sa rozdelí
   medzi kód a schému.

## Decision
Pozri sekciu „Doplnenie 2026-11-25“ nižšie.

## Consequences
- Pri možnosti 1 sú zápisy rezervácií serializované; pre jednu miestnosť a recepciu je priepustnosť dostatočná
  (meranie výkonu nie je požiadavkou – measure nie je určené).
- Čakanie na zámok nie je opakovanie uloženia v zmysle QR-002 (požiadavka sa vykoná raz). Ak sa zámok nepodarí
  získať v časovom limite, implementácia to posúdi ako nedostupné úložisko (503) – pozri DR-I04 v pláne 0007.
- Poradie kontrol BR-012 sa zachová: 503 (pozícia 4) sa zistí pri otvorení transakcie pred kontrolami 422.
- Prechod na produkčný DBMS (ADR-003) vyžaduje nové overenie tohto ADR.
- SC-001-06 sa overí testom s dvoma súbežnými požiadavkami.

## Related elements
ADR-001, ADR-003, BR-003, BR-009, BR-012, QR-002, DO-Rezervacie, AC-ModulRezervaciiAKlientov, SC-001-06

## Specification baseline
0.3.1 / architecture baseline B-01

## Doplnenie 2026-11-25 (rozhodol Martin Štefanko, Software architect)
- DR-I02: A – BR-003 a BR-009 sa vynucujú transakciou `BEGIN IMMEDIATE` nad SQLite.
- DR-I03: A – audit QR-001 sa ukladá ako stĺpce `created_by` a `created_at` v riadku rezervácie.
- DR-I04: A – každá `SqliteException` vrátane vypršania zámku = úložisko nedostupné → 503 bez opakovania (QR-002).
- DR-I05: A – seed klienti v `config/seed-clients.json` iba s UUID (bez osobných údajov), `INSERT OR IGNORE` pri štarte.