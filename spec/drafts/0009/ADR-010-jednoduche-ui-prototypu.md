# ADR-010 Jednoduché UI prototypu pre FR1 a zmena DR-17

> Draft pripravila AI v režime PREPARE_SPEC (úloha 0009). Toto nie je rozhodnutie. Stav mení iba človek.
> Cieľové umiestnenie po schválení: `spec/architecture/decisions/ADR-010-jednoduche-ui-prototypu.md`.
> Voľby v časti Options 1 zadal človek v prompte 0009; AI ich iba zapísala. Otvorené časti sú označené DR-09xx.

## Status
Proposed

## Owner
Martin Štefanko (Software architect)

## Date
2026-11-25

## Context
- DR-17 (0002) určilo: „Sumarizácia je UI, mimo contractu; UI nie je v rozsahu prototypu.“
- V prompte 0009 (change experiment, bod E) človek rozhodol, že prototyp dostane jednoduché UI pre
  ROLE-SpravcaNavstev na vytvorenie rezervácie (FR1). Sumarizácia pred uložením ostáva mimo rozsahu.
- Approved contract API-Reservations 0.2.1 sa nemení. UI je iba jeho konzument.
- Platforma je .NET 10 / ASP.NET Core (ADR-002); knižnice prototypu sú obmedzené na zoznam ADR-008.
- Tokeny vydáva `dotnet user-jwts` a prihlasovací endpoint je mimo rozsahu (DR-23, ADR-004).
- Klient sa zadáva identifikátorom UUID, vyhľadanie klienta je mimo rozsahu (DR-24, DR-30).
- Otváracie hodiny platia v Europe/Bratislava; API prijíma iba UTC s príponou `Z` (DR-18, DR-C03).
- Views B-01 nezobrazujú Application Interface pre recepčnú. Jediný zobrazený Application Interface je
  „Rozhranie saunového majstra“ (B-01-application-usage-tobe.png, B-01-implementation-migration-tobe.png).
  Prvok v modeli doplní človek (DR-0914).

## Options
1. **Jedna statická stránka HTML + vanilla JavaScript v `wwwroot`**, servírovaná tou istou ASP.NET Core aplikáciou,
   bez nového NuGet balíka a bez frontend buildu (voľba človeka v prompte 0009).
2. Server-side UI v ASP.NET Core (Razor Pages / MVC) – bez nového balíka, ale UI by bolo zviazané so serverovým kódom
   a obišlo by HTTP contract.
3. Samostatná frontendová aplikácia (SPA framework s buildom) – nový build, nové závislosti, samostatný artifact.
4. Bez UI (stav podľa DR-17).

## Decision
Rozhodnutie doplní zodpovedný človek.

Návrh AI na zápis (iba ak človek zvolí možnosť 1):
- DR-17 sa mení: UI pre vytvorenie rezervácie je v rozsahu prototypu; sumarizácia pred uložením je stále mimo rozsahu.
- UI je jedna statická stránka (HTML + vanilla JavaScript) v `src/IntelliWell/wwwroot`, ktorú servíruje
  ART-SoftverIntelliWell. Nepridáva sa NuGet balík ani npm/frontend build. Zoznam ADR-008 sa nemení.
- UI volá iba `POST /api/v1/reservations` podľa API-Reservations 0.2.1. Contract sa nemení.
- Prihlásenie: používateľ vloží JWT token z `dotnet user-jwts`; UI ho posiela v hlavičke `Authorization: Bearer`.
  Uloženie tokenu v prehliadači – DR-0910.
- Klient sa zadáva ako UUID; pole je predvyplnené seed klientom `7c9e6679-7425-40de-944b-e07fc1f90ae7`
  (`config/seed-clients.json`).
- Používateľ zadá dátum a čas v miestnom čase Europe/Bratislava; UI ho prepočíta na UTC s príponou `Z`
  (DR-18, DR-C03). Spôsob prepočtu – DR-0911.
- UI zobrazí potvrdenie pri 201 a slovenskú hlášku pre každý typ problému contractu (texty – DR-0912).
- Business pravidlá (BR-003 – BR-008, BR-012) vyhodnocuje iba server; UI ich neduplikuje (návrh AI, potvrdí owner).

## Consequences
- Proces a používateľ: recepčná môže vytvoriť rezerváciu bez nástroja na HTTP požiadavky; vkladanie tokenu je
  prototypové riešenie (DR-23), nie produkčné prihlásenie.
- Contract: bez zmeny; UI je nový konzument API-Reservations 0.2.1. Zmena contractu by vyžadovala DESIGN_CONTRACT.
- Implementácia: `Program.cs` musí servírovať statické súbory (`UseDefaultFiles`, `UseStaticFiles` – súčasť
  ASP.NET Core, bez balíka). Middleware nesmie zmeniť správanie `/api/v1/reservations` (401/403 poradie BR-012).
- Security: token je v prehliadači – riziko XSS; UI musí vkladať texty iba ako text (nie HTML). Prístup k stránke
  bez prihlásenia – DR-0913. Rovnaký pôvod (same origin), CORS sa nepoužíva.
- Privacy / audit: CON-003 – UI nesmie zapisovať poznámku do konzoly prehliadača ani do iného logu (návrh AI).
  Audit QR-001 sa nemení (autor = claim `sub`).
- Testovanie: bez nového NuGet balíka nie je k dispozícii automatizácia prehliadača – spôsob overenia US-002 rieši DR-0915.
- Deployment: UI je súčasť ART-SoftverIntelliWell, žiadny nový artifact (`spec/drafts/0009/deployment.yaml`).

## Related elements
FR1, FR1-UI (draft 0009), NFR1, NFR4, ROLE-SpravcaNavstev, ACT-Recepcna, BP-SpravaNavstev, AS-SpravaRezervaciiAKlientov,
AC-ModulRezervaciiAKlientov, AC-IntelliWell, ART-SoftverIntelliWell, API-Reservations 0.2.1, US-002, SC-002-01 – SC-002-08,
DR-17 (mení sa), DR-18, DR-23, DR-24, DR-C03, CON-003, ADR-002, ADR-004, ADR-005, ADR-008

## Specification baseline
0.4.0 (draft) / architecture baseline B-01
