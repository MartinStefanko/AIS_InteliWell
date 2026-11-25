# Ručný protokol – US-002 Vytvorenie rezervácie cez používateľské rozhranie (FR1-UI)

| Pole | Hodnota |
|---|---|
| Stav | vyplnený čiastočne – tester vykonal vybrané scenáre (DR-0915) |
| Pripravila | AI v úlohe 0010 (IMPLEMENT_CHANGE), log `spec/prompts/log/0010-2026-11-25-IMPLEMENT_CHANGE.md` |
| Specification baseline | 0.4.0 / architecture baseline B-01 |
| Contract | API-Reservations 0.2.1 (approved, bez zmeny) |
| Scenáre | `spec/stories/features/US-002.feature` – SC-002-01 až SC-002-10 |
| Automatické testy | `tests/IntelliWell.Tests/Ui/UiStaticPageTests.cs` (dotnet test), `tests/ui/*.test.mjs` (node --test) |
| Tester | Martin Štefanko (Software architect) |
| Dátum | 2026-11-25 |
| Commit | pozri commit, v ktorom je tento protokol pridaný |
| Prehliadač a verzia | Chrome 131 |
| Operačný systém a časové pásmo počítača | Windows, Europe/Bratislava |

Tento protokol overuje správanie stránky v prehliadači. Pravidlá termínu vyhodnocuje systém a pokrývajú ich
acceptance testy US-001 (SC-001-xx). Protokol overuje iba, že rozhranie výsledok správne odovzdá recepčnej.

## 1. Príprava

Aplikácia spustená cez `dotnet run --project src/IntelliWell --launch-profile http`, token T-JANA vytvorený cez
`dotnet user-jwts create --project src/IntelliWell --name jana --role SpravcaNavstev --output token`,
stránka otvorená na `http://localhost:5080/`. Identifikátor klienta bol predvyplnený `7c9e6679-7425-40de-944b-e07fc1f90ae7`.

Test prebehol v iný deň ako Background feature, preto bol použitý termín 27. 11. 2026 11:00 (zimný čas, v budúcnosti, v otváracích hodinách).

## 2. Scenáre

Výsledok: PASS / FAIL / UNKNOWN (chýba dôkaz – nepísať odhad).

| Scenár | Očakávaný výsledok | Skutočný výsledok | Výsledok |
|---|---|---|---|
| SC-002-01 | 1. potvrdenie o uložení 2. druhý pokus: „Termín je obsadený. Rezervácia sa neuložila.“ | Rezervácia na 27. 11. 2026 11:00 sa uložila (aplikačný log: „Rezervácia a9578a94-… bola vytvorená“). Druhý pokus na ten istý termín zobrazil „Termín je obsadený. Rezervácia sa neuložila.“ | PASS |
| SC-002-02 | `startTime` = `2027-07-01T12:00:00Z` | nevykonané | UNKNOWN |
| SC-002-03 | `startTime` = `2026-12-01T15:00:00Z` | nevykonané | UNKNOWN |
| SC-002-02/03 (DR-0911) | prepočet nezávisí od pásma počítača | nevykonané | UNKNOWN |
| SC-002-04 | druhý pokus: „Termín je obsadený. Rezervácia sa neuložila.“ | overené v rámci SC-002-01 (rovnaký termín 27. 11. 2026 11:00) | PASS |
| SC-002-05 / unauthenticated | „Nie ste prihlásená. Vložte platný prihlasovací token.“ | nevykonané | UNKNOWN |
| SC-002-05 / forbidden | „Nemáte oprávnenie vytvárať rezervácie…“ | nevykonané | UNKNOWN |
| SC-002-05 / invalid-request | „Údaje rezervácie nie sú úplné…“ | nevykonané | UNKNOWN |
| SC-002-05 / client-not-found | „Klient s týmto identifikátorom nie je v evidencii klientov.“ | nevykonané | UNKNOWN |
| SC-002-05 / start-not-in-future | „Začiatok termínu musí byť v budúcnosti.“ | nevykonané | UNKNOWN |
| SC-002-05 / outside-opening-hours | „Termín musí celý ležať v otváracích hodinách 10:00–22:00.“ | nevykonané | UNKNOWN |
| SC-002-05 / note-too-long | „Poznámka môže mať najviac 500 znakov.“ | nevykonané | UNKNOWN |
| SC-002-05 / slot-occupied | „Termín je obsadený. Rezervácia sa neuložila.“ | overené v SC-002-01 | PASS |
| SC-002-05 / service-unavailable | „Evidencia rezervácií je dočasne nedostupná…“ | nevykonané | UNKNOWN |
| SC-002-06 | 1. hláška o neprihlásení 2. rezervácia sa uloží | nevykonané | UNKNOWN |
| SC-002-07 | hláška o nedostupnosti, jediná požiadavka | nevykonané | UNKNOWN |
| SC-002-08 | text poznámky nie je v konzole ani v logu | nevykonané (pri teste bola poznámka prázdna) | UNKNOWN |
| SC-002-09 / neexistujúci | hláška o neexistujúcom čase, žiadna požiadavka | nevykonané | UNKNOWN |
| SC-002-09 / dvojznačný | hláška o dvojznačnom čase, žiadna požiadavka | nevykonané | UNKNOWN |
| SC-002-10 | „Server neodpovedá…“, žiadne opakovanie | nevykonané | UNKNOWN |

Nevykonané scenáre pokrývajú automatické testy (dotnet test 86/86, node --test 44/44) a acceptance testy SC-001-xx na úrovni API.
Ručný dôkaz v prehliadači pre ne chýba.

## 3. Doplnkové kontroly (ADR-010, DR-0910, DR-0913)

| Kontrola | Očakávaný výsledok | Skutočný výsledok | Výsledok |
|---|---|---|---|
| DR-0913 | stránka sa zobrazí bez tokenu | stránka sa zobrazila pred vložením tokenu | PASS |
| DR-0910 | token nie je v žiadnom úložisku prehliadača | nevykonané | UNKNOWN |
| ADR-010 Security | HTML sa nevykreslí | nevykonané | UNKNOWN |

## 4. Záver

| Pole | Hodnota |
|---|---|
| Počet PASS / FAIL / UNKNOWN | 4 / 0 / 19 |
| Zistené odchýlky (s odkazom na SC-002-xx) | žiadne vo vykonaných scenároch |
| Potvrdenie business významu (owner FR1-UI) | Potvrdzujem, že UI umožňuje správcovi návštev vytvoriť rezerváciu a zrozumiteľne zobrazí odmietnutie obsadeného termínu. Martin Štefanko (Software architect, v mene ACT-Manazer), 2026-11-25 |