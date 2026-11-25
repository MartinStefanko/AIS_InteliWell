# ADR-008 Knižnice (NuGet balíky) prototypu FR1

> Draft pripravila AI v režime IMPLEMENT_CHANGE (úloha 0007, fáza 1 – plán). Rozhodnutie zapísal človek
> v sekcii „Doplnenie 2026-11-25“ (Martin Štefanko, Software architect). Redakčná úprava hlavičky: 0009 (V-04).

## Status
Accepted

## Owner
Martin Štefanko (Software architect)

## Date
2026-11-25

## Context
ADR-002 (Accepted) určuje platformu .NET 10 / ASP.NET Core a výslovne uvádza, že výber knižníc (prístup k údajom,
testy) nie je jeho súčasťou a významnú dependency zaznamená IMPLEMENT_CHANGE ako draft ADR. ADR-003 určuje SQLite,
ADR-004 JWT bearer s tokenmi z `dotnet user-jwts`. Človek v úlohe 0007 rozhodol: testy xUnit + WebApplicationFactory,
čas cez `TimeProvider`. Konkrétne NuGet balíky a verzie nie sú v žiadnom ADR.

Overené dostupné verzie (`dotnet package search`, 2026-11-25): runtime Microsoft.AspNetCore.App 10.0.15,
SDK 10.0.404. Balíky radu 11.x patria .NET 11 a pre ADR-002 sa nepoužijú.

## Options
1. **Minimálna sada prvostranových balíkov (Microsoft) + xUnit:**
   - `src/IntelliWell`: `Microsoft.AspNetCore.Authentication.JwtBearer` 10.0.15, `Microsoft.Data.Sqlite` 10.0.15
     (ADO.NET, bez ORM; transitívne `SQLitePCLRaw.bundle_e_sqlite3` – natívna knižnica SQLite).
   - `tests/IntelliWell.Tests`: `Microsoft.AspNetCore.Mvc.Testing` 10.0.15, `Microsoft.NET.Test.Sdk` 18.12.0,
     `xunit.v3` 4.0.3, `xunit.runner.visualstudio` 4.0.1.
   - Bez ďalších balíkov: `TimeProvider` je súčasť .NET (testy použijú vlastnú podtriedu, nie
     `Microsoft.Extensions.TimeProvider.Testing`); tokeny v testoch podpíše `Microsoft.IdentityModel.JsonWebTokens`,
     ktorý je transitívnou závislosťou JwtBearer; problem details a JSON sú v ASP.NET Core / System.Text.Json.
2. Ako 1, ale prístup k údajom cez **Entity Framework Core** (`Microsoft.EntityFrameworkCore.Sqlite`) – väčšia
   dependency, migrácie, bližšie k „databázovému návrhu“, ktorý IMPLEMENT_CHANGE nemá robiť.
3. Ako 1 + pomocné testovacie knižnice tretích strán (napr. FluentAssertions, validátor JSON Schema/OpenAPI,
   YamlDotNet na načítanie príkladov z approved contractu) – pohodlnejšie testy, ďalšie dependencies a licencie.

## Decision
Pozri sekciu „Doplnenie 2026-11-25“ nižšie.

## Consequences
- Pri možnosti 1 má prototyp dve priame runtime dependencies (JwtBearer, Microsoft.Data.Sqlite) a natívnu SQLite
  knižnicu (transitívne). Verzie sú pripnuté v `.csproj`.
- `xunit.v3` robí z testovacieho projektu spustiteľný projekt (`OutputType Exe`); `dotnet test` ho spúšťa cez
  `xunit.runner.visualstudio`.
- Contract tests sa píšu ako explicitné asserty voči approved contractu 0.2.1 (bez automatickej validácie
  schémy). Automatická validácia odpovedí voči OpenAPI by vyžadovala možnosť 3.
- Zmena verzie runtime (napr. prechod na .NET 11) vyžaduje nové ADR alebo zmenu ADR-002.

## Related elements
ADR-002, ADR-003, ADR-004, ART-SoftverIntelliWell, SSW-RuntimeDotNet, SSW-DatabazovySystem, TS-DatabazovaSluzba,
AC-ModulRezervaciiAKlientov, FR1, QR-001

## Specification baseline
0.3.1 / architecture baseline B-01

## Doplnenie 2026-11-25 (rozhodol Martin Štefanko, Software architect)
- DR-I01: A – minimálna sada balíkov podľa tabuľky v tomto ADR. Nový balík vyžaduje nové rozhodnutie.
- Prostredie prototypu: lokálne spustenie cez `dotnet run`; kontajner ani CI nie sú v rozsahu (rieši I-04).

## Doplnenie 2026-11-25 (DR-1001, rozhodol Martin Štefanko, Software architect)
JS logika UI sa smie testovať cez Node.js (`node --test`, bez npm balíkov a bez buildu). Node.js je iba vývojový nástroj,
nie je súčasťou ART-SoftverIntelliWell. Testy JS sa spúšťajú samostatným príkazom mimo `dotnet test`.