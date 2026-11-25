# ADR-010 Jednoduché UI prototypu pre FR1 a zmena DR-17

> Draft pripravila AI v režime PREPARE_SPEC (úloha 0009). Sekciu Decision vyplnila AI podľa rozhodnutí človeka
> (prompt 0009 a doplňujúci prompt 0009). Stav ostáva Proposed – Accepted zapisuje iba človek.

## Status
Accepted

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
  Človek to zapísal ako známu medzeru modelu (DR-0914); doplnenie do Sparx EA je mimo kroku 0009.

## Options
1. **Jedna statická stránka HTML + vanilla JavaScript v `wwwroot`**, servírovaná tou istou ASP.NET Core aplikáciou,
   bez nového NuGet balíka a bez frontend buildu (voľba človeka v prompte 0009).
2. Server-side UI v ASP.NET Core (Razor Pages / MVC) – bez nového balíka, ale UI by bolo zviazané so serverovým kódom
   a obišlo by HTTP contract.
3. Samostatná frontendová aplikácia (SPA framework s buildom) – nový build, nové závislosti, samostatný artifact.
4. Bez UI (stav podľa DR-17).

## Decision
Možnosť 1. Rozhodol Martin Štefanko (Software architect), 2026-11-25 – prompt 0009 a doplňujúci prompt 0009.
Zapísala AI; stav Proposed, kým človek nezapíše Accepted.

- DR-17 sa mení: jednoduché UI pre ROLE-SpravcaNavstev na vytvorenie rezervácie (FR1) je v rozsahu prototypu;
  sumarizácia pred uložením je stále mimo rozsahu.
- UI je jedna statická stránka (HTML + vanilla JavaScript) v `src/IntelliWell/wwwroot`, ktorú servíruje tá istá
  ASP.NET Core aplikácia (ART-SoftverIntelliWell). Bez nového NuGet balíka a bez frontend buildu; zoznam ADR-008 sa nemení.
- UI je iba konzument API-Reservations 0.2.1 (`POST /api/v1/reservations`). Contract sa nemení.
- Prihlásenie: používateľ vloží JWT token z `dotnet user-jwts` (DR-23; login endpoint mimo rozsahu). UI ho posiela
  v hlavičke `Authorization: Bearer`.
- DR-0910: token je iba v pamäti stránky; po obnovení stránky sa vkladá znova.
- Klient sa zadáva ako UUID (DR-24); pole je predvyplnené seed klientom `7c9e6679-7425-40de-944b-e07fc1f90ae7`
  (`config/seed-clients.json`).
- DR-0911: používateľ zadá dátum a čas v miestnom čase; UI ho vždy prepočíta podľa Europe/Bratislava (Intl), bez
  ohľadu na nastavenie počítača, na UTC s príponou `Z` (DR-18, DR-C03). Čas, ktorý pri zmene času neexistuje alebo je
  dvojznačný, UI odmietne s hláškou a požiadavku nepošle.
- DR-0912: pri 201 UI zobrazí potvrdenie; pri odmietnutí vlastnú slovenskú hlášku podľa `type` (texty v US-002.feature),
  pri neznámom type `title` z odpovede; pri neodpovedajúcom serveri schválený text zo SC-002-10.
- DR-0913: stránku smie otvoriť aj neprihlásená osoba; údaje chráni API (NFR4).
- DR-0914: Application Interface pre recepčnú v modeli B-01 chýba – známa medzera modelu. FR1-UI sa zatiaľ viaže na
  AC-ModulRezervaciiAKlientov a AS-SpravaRezervaciiAKlientov. Architecture baseline ostáva B-01.
- DR-0915: US-002 sa overí ručným protokolom v `reports/` a automatickými testami bez prehliadača (servírovanie
  stránky, obsah, prepočet času v JS logike, ak je testovateľná).
- Business pravidlá (BR-003 – BR-008, BR-012) vyhodnocuje iba server; UI ich neduplikuje.

## Consequences
- Proces a používateľ: recepčná môže vytvoriť rezerváciu bez nástroja na HTTP požiadavky; vkladanie tokenu je
  prototypové riešenie (DR-23), nie produkčné prihlásenie.
- Contract: bez zmeny; UI je nový konzument API-Reservations 0.2.1. Zmena contractu by vyžadovala DESIGN_CONTRACT.
- Implementácia: `Program.cs` musí servírovať statické súbory (`UseDefaultFiles`, `UseStaticFiles` – súčasť
  ASP.NET Core, bez balíka). Middleware nesmie zmeniť správanie `/api/v1/reservations` (401/403 poradie BR-012).
- Security: token je v prehliadači (iba v pamäti stránky, DR-0910) – riziko XSS; UI musí vkladať texty iba ako text
  (nie HTML). Stránka je dostupná bez prihlásenia (DR-0913), údaje chráni API. Rovnaký pôvod (same origin), CORS sa nepoužíva.
- Privacy / audit: CON-003 – UI nesmie zapisovať poznámku do konzoly prehliadača ani do iného logu (návrh AI).
  Audit QR-001 sa nemení (autor = claim `sub`).
- Testovanie: bez nového NuGet balíka nie je k dispozícii automatizácia prehliadača – US-002 sa overí ručným protokolom
  a automatickými testami bez prehliadača (DR-0915).
- Deployment: UI je súčasť ART-SoftverIntelliWell, žiadny nový artifact (`spec/drafts/0009/deployment.yaml`).

## Related elements
FR1, FR1-UI, NFR1, NFR4, ROLE-SpravcaNavstev, ACT-Recepcna, BP-SpravaNavstev, AS-SpravaRezervaciiAKlientov,
AC-ModulRezervaciiAKlientov, AC-IntelliWell, ART-SoftverIntelliWell, API-Reservations 0.2.1, US-002, SC-002-01 – SC-002-10,
DR-17 (mení sa), DR-0910 – DR-0915, DR-18, DR-23, DR-24, DR-C03, CON-003, ADR-002, ADR-004, ADR-005, ADR-008

## Specification baseline
0.4.0 / architecture baseline B-01
