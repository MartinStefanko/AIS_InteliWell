# GAP Analysis

- status: "approved"
- approved_by: "Martin Štefanko (Software architect)"
- reviewed_by: "Martin Štefanko (Software architect)"
- architecture_baseline: "B-01"

**Stav:** spracovaná v architektonickom projekte (`docs/sources/AIS_IntelliWell.docx`, kap. 14).
Schválenie nie je v podkladoch zaznamenané. Obsah je prevzatý bez doplnení; AI z neho nevyvodzuje nové requirements.

Porovnávané Plateaus (navrhnuté IDs, `spec/drafts/id-mapping.yaml`):

- AS-IS: PL-Baseline („Baseline architektúra“)
- TO-BE: PL-Ciel („Cieľová architektúra“)

## Tabuľka (prevzatá z docx kap. 14)

| BASELINE ↓ / TARGET →                            | Modul rezervácií a klientov                                               | Modul riadenia a monitorovania zariadení               | Modul koordinácie saunových majstrov                 | Modul prehľadov zásahov                        | Server, databázový systém a sieť                  | E-mail a SMS | Chýbajúce / eliminované |
| ------------------------------------------------ | ------------------------------------------------------------------------- | ------------------------------------------------------ | ---------------------------------------------------- | ---------------------------------------------- | ------------------------------------------------- | ------------ | ----------------------- |
| Papierový formulár klienta                       | Nahradené                                                                 |                                                        |                                                      |                                                | Migrované                                         |              | Eliminované             |
| Záznam rezervácií (ručný)                        | Nahradené                                                                 |                                                        |                                                      |                                                | Migrované                                         |              | Eliminované             |
| Ručná obsluha prostredia                         |                                                                           | Nahradené                                              |                                                      |                                                |                                                   |              | Eliminované             |
| Ovládanie sauny, vírivky, ozvučenia a osvetlenia |                                                                           | Integrované                                            |                                                      |                                                | Pripojené                                         |              |                         |
| Ručná koordinácia saunových majstrov             |                                                                           |                                                        | Nahradené                                            |                                                |                                                   |              | Eliminované             |
| E-mail                                           |                                                                           |                                                        |                                                      |                                                |                                                   | Zachované    |                         |
| SMS                                              |                                                                           |                                                        |                                                      |                                                |                                                   | Zachované    |                         |
| Nové                                             | Overenie dostupnosti, blokovanie termínu, evidencia klientov bez duplicít | Diaľkové nastavovanie prostredia v bezpečných limitoch | Potvrdzovanie priradení v rozhraní saunového majstra | Evidencia zásahov a reporty za zvolené obdobie | Centrálne údaje, prepojenie zariadení a personálu |              |                         |

## Text z docx kap. 14 (relevantný pre FR1)

> V oblasti správy návštev dochádza k úplnému nahradeniu papierových formulárov a ručného záznamu rezervácií
> modulom rezervácií a klientov, ktorý overuje dostupnosť termínu, blokuje obsadené termíny a kontroluje
> existenciu klienta. Existujúce údaje sa migrujú do centrálnej databázy na serveri IntelliWell.

> E-mail a SMS zostávajú zachované ako kanály, ktorými klienti posielajú požiadavky, a recepčná ich zaeviduje
> priamo v systéme IntelliWell.

## Gaps v Implementation and Migration View

Implementation and Migration View (kap. 12) zobrazuje gaps, ktoré podľa docx „vychádzajú z GAP analýzy“:
GAP-001 Manuálna evidencia rezervácií a klientov, GAP-002, GAP-003, GAP-004 (pozri `implementation-context.yaml`).
Priradenie riadkov tabuľky ku gapom v docx nie je uvedené.

## Poznámka

Ocenenie a harmonogram nie sú súčasťou tejto časti SDD.
