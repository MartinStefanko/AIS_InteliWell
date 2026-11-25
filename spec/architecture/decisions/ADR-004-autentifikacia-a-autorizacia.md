# ADR-004 Autentifikácia a autorizácia personálu pre vytvorenie rezervácie

> Draft pripravený AI v režime PREPARE_SPEC (log 0002). Sekcia Decision zapisuje ľudské rozhodnutia DR-07
> a DR-23 (doplňujúci prompt 0002). Stav mení iba človek.

## Status
Accepted

## Owner
Martin Štefanko (Software architect)

## Date
2026-11-25

## Context
NFR4 vyžaduje, aby do IntelliWell mal prístup iba prihlásený personál a aby bol každý zásah priradený konkrétnej
prihlásenej osobe (docx kap. 8.1). CON-002 obmedzuje systém iba na personál. Docx neurčuje spôsob prihlasovania
ani oprávnenia rolí. Deliverable D-003 „Prihlasovanie personálu“ je súčasťou WP-002.

## Options
1. Lokálne účty personálu v IntelliWell, autentifikácia JWT bearer tokenom, autorizácia podľa roly.
2. Externý poskytovateľ identity (napr. adresárová služba organizácie). V podkladoch pre neho nie je opora.
3. Session cookie po prihlásení menom a heslom.

## Decision
- Personál má lokálne účty v IntelliWell. Autentifikácia prebieha JWT bearer tokenom. Rezerváciu smie vytvoriť
  iba rola ROLE-SpravcaNavstev (DR-07).
- Tokeny pre prototyp vydáva vývojársky nástroj `dotnet user-jwts`. Rola je v claime `role`. Prihlasovací
  endpoint nie je súčasťou slice FR1 (DR-23).

Rozhodol Martin Štefanko, 2026-11-25.

## Consequences
- Každá operácia vytvorenia rezervácie vyžaduje platný bearer token. Bez neho systém požiadavku odmietne
  (SC-001-13).
- Prihlásená osoba bez hodnoty zodpovedajúcej ROLE-SpravcaNavstev v claime `role` je odmietnutá (BR-011, SC-001-12).
  Presnú hodnotu claimu navrhne DESIGN_CONTRACT a schváli ju človek.
- Identita z tokenu je zdrojom údaja „kto rezerváciu vytvoril“ pre QR-001.
- `dotnet user-jwts` je vývojársky nástroj. Tokeny a podpisový kľúč existujú iba vo vývojovom prostredí. Produkčné
  vydávanie tokenov, uloženie hesiel lokálnych účtov a prihlasovací endpoint (D-003) sú mimo slice
  a budú potrebovať samostatné rozhodnutie.
- contract-design-profile.yaml: `security.authentication: "JWT bearer token"`,
  `security.authorization: "rola ROLE-SpravcaNavstev pre vytvorenie rezervácie"`,
  `security.token_issuance` podľa DR-23.

## Related elements
NFR4, CON-002, BR-011, QR-001, ROLE-SpravcaNavstev, ACT-Recepcna, AC-IntelliWell, D-003, SC-001-12, SC-001-13

## Specification baseline
0.2.0 (draft) / architecture baseline B-01
