# ADR-002 Implementačná platforma .NET 10 (C#) a ASP.NET Core

> Draft pripravený AI v režime PREPARE_SPEC (log 0002). Sekcia Decision zapisuje ľudské rozhodnutie DR-05
> v znení doplňujúceho promptu 0002 (.NET 10 LTS namiesto .NET 8). Stav mení iba človek.

## Status
Accepted

## Owner
Martin Štefanko (Software architect)

## Date
2026-11-25

## Context
IMPLEMENT_CHANGE pre FR1 potrebuje schválenú implementačnú platformu. Docx uvádza „.NET (C#) ako predpokladaná
implementačná platforma“ (kap. 13.2) a v kap. 15.2: „Podklady projektu technológiu neurčujú, preto ide
o predpoklad.“ Enterprise Continuum ju zaraďuje medzi Foundation Solutions (reuse-catalog.yaml) bez verzie
a ownera.

## Options
1. .NET 10 (LTS, C#) s ASP.NET Core.
2. .NET 8 (LTS) s ASP.NET Core. Toto bolo pôvodné rozhodnutie DR-05. Podpora .NET 8 skončila 10. 11. 2026.
3. Iná platforma. V podkladoch pre ňu nie je opora.

## Decision
Implementačná platforma je .NET 10 (LTS, C#) s ASP.NET Core (DR-05, rozhodol Martin Štefanko, 2026-11-25;
pôvodne .NET 8, zmenené doplňujúcim rozhodnutím v 0002).

## Consequences
- Candidate code a tests v IMPLEMENT_CHANGE sa generujú pre .NET 10 / ASP.NET Core.
- Deployable Artifact ART-SoftverIntelliWell je aplikácia ASP.NET Core na .NET 10 (deployment.yaml).
- Tokeny pre prototyp vydáva vývojársky nástroj `dotnet user-jwts`, ktorý je súčasťou .NET SDK (ADR-004).
- V Enterprise Continuum sa položka „.NET (C#) ako predpokladaná implementačná platforma“ mení na rozhodnutý
  Solutions Building Block s verziou 10. Owner a maturity treba doplniť (reuse-catalog.yaml, mimo tohto kroku).
- Výber knižníc (napr. pre prístup k údajom alebo testy) nie je súčasťou tohto rozhodnutia. Významnú
  dependency zaznamená IMPLEMENT_CHANGE ako draft ADR.

## Related elements
FR1, AC-IntelliWell, AC-ModulRezervaciiAKlientov, ART-SoftverIntelliWell, NODE-ServerIntelliWell,
SSW-RuntimeDotNet

## Specification baseline
0.2.0 / architecture baseline B-01
