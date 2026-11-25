# Glossary – IntelliWell (FR1 Vytvorenie rezervácie)

- status: "approved" (0009; predtým approved)
- approved_by: "Martin Štefanko (Software architect)"
- reviewed_by: "Martin Štefanko (Software architect)"
- architecture_baseline: "B-01"
- specification_version: "0.4.0" (DR-0903; zmena DR-17 a nový pojem v 0009)
- zdroj: `docs/sources/AIS_IntelliWell.docx` (ďalej „docx“) a ľudské rozhodnutia DR-01 až DR-17 (`spec/drafts/open-decisions.md`, log 0002); IDs sú navrhnuté v `spec/drafts/id-mapping.yaml` (status proposed)

Definície sú prevzaté alebo zhrnuté z textu docx alebo z ľudských rozhodnutí (uvedené ako DR-xx). Kde pojem nie je určený, je to uvedené ako otvorená otázka.

## IntelliWell

Interný informačný systém wellness centra NikaWell, ktorý centralizuje plánovanie služieb a diaľkové riadenie inteligentných zariadení. Je určený len pre personál; klient so systémom priamo nepracuje (docx kap. 1, 6).

- IDs: AC-IntelliWell
- Súvisí: CON-002

## Wellness centrum NikaWell

Organizácia, pre ktorú je IntelliWell navrhnutý. Jedno wellness centrum (docx kap. 1, 3).

## Klient

Osoba mimo organizácie, ktorá navštevuje wellness centrum. Požiadavky oznamuje recepčnej telefonicky, e-mailom, SMS alebo osobne (docx kap. 3.2).

- IDs: ACT-Klient
- Rezerváciu možno vytvoriť iba pre klienta, ktorý existuje v evidencii klientov (DR-03, BR-007).

## Recepčná

Interný aktér, ktorý prijíma požiadavky klientov a v systéme vytvára rezervácie (docx kap. 2, 3.2).

- IDs: ACT-Recepcna

## Správca návštev

Rola recepčnej v budúcom stave. V systéme vytvára rezervácie (UC01), registruje nových klientov (UC02) a k rezervácii priraďuje saunového majstra (UC05) (docx kap. 3.3).

- IDs: ROLE-SpravcaNavstev

## Manažér

Interný aktér wellness centra NikaWell (Organization View TO-BE). V rozhodnutiach sa označuje aj ako „Manažér NikaWell“. Je ownerom FR1, pravidiel BR-\*, NFR1, NFR4 a QR-001 (DR-15, DR-21).

- IDs: ACT-Manazer

## Personál

Recepčná, saunový majster a manažér wellness centra NikaWell (docx kap. 3.2).

- Súvisí: NFR4, CON-002

## Prihlásený personál

Personál, ktorý má prístup do IntelliWell. Vďaka prihláseniu je každý zásah priradený konkrétnej prihlásenej osobe (docx kap. 8.1).

- IDs: NFR4, ADR-004
- Personál má lokálny účet v IntelliWell; prihlásenie sa preukazuje JWT bearer tokenom (DR-07). Pre prototyp tokeny vydáva vývojársky nástroj dotnet user-jwts, rola je v claime „role“; prihlasovací endpoint nie je súčasťou slice (DR-23).

## Správa návštev

Biznis proces v budúcom stave. Spúšťa ho požiadavka klienta na rezerváciu. Recepčná v role správcu návštev vyberie alebo zaregistruje klienta, zadá dátum a čas, prípadne priradí saunového majstra; systém overí dostupnosť termínu, uloží rezerváciu a zablokuje termín (docx kap. 8.3.2).

- IDs: BP-SpravaNavstev

## Požiadavka klienta na rezerváciu

Udalosť, ktorou klient (telefonicky, e-mailom, SMS alebo osobne) spúšťa správu návštev (docx kap. 8.3.2).

- IDs: BEV-PoziadavkaNaRezervaciu
- Súvisí: CON-001

## Rezervácia

Záznam o termíne návštevy klienta, ktorý recepčná vytvorí v systéme. Pri vytvorení sa zadáva dátum, čas a poznámka (docx kap. 5). K rezervácii sa môže priradiť saunový majster (docx kap. 9.1.2) a pri príprave prostredia sa do nej uložia parametre zariadení alebo profil (docx kap. 8.3.2).

V rozsahu FR1 obsahuje rezervácia klienta, termín, voliteľnú poznámku, stav a záznam, kto a kedy ju vytvoril (DR-13). Saunový majster a parametre zariadení sú mimo FR1 (DR-10).

- IDs: DO-Rezervacie (údaje „Rezervácie“)
- Stav: rezervácia má iba stav „vytvorená“; zrušenie a zmena sú mimo rozsahu (DR-04).
- V API sa stav „vytvorená“ zapisuje anglickou hodnotou `created` (DR-C11, lifecycles.yaml `api_value`).

## Rezervácia termínu

Biznis služba, ktorá slúži klientovi. Realizuje ju proces Správa návštev (docx kap. 8.2.2, 11.2).

- IDs: BS-RezervaciaTerminu
- Nepoužívať ako synonymum: rezervácia (údaj). „Rezervácia termínu“ je služba, „rezervácia“ je záznam.

## Termín

Docx opisuje návštevu ako „rezerváciu termínu, počas ktorého má [klient] k dispozícii prostredie“ (kap. 8.2.1). Termín je časový úsek s pevnou dĺžkou 60 minút. Jeho začiatok (dátum a čas) zadá recepčná; koniec je začiatok + 60 minút. Sloty sa nepoužívajú. Termín musí celý ležať v otváracích hodinách (DR-01).

- Termín je polootvorený interval [začiatok, koniec): rezervácia môže začať presne v čase konca predchádzajúcej (DR-19).
- IDs: BR-003, BR-004, BR-005

## Overenie dostupnosti termínu

Kontrola, ktorú systém vykoná pred uložením rezervácie (docx kap. 5, 8.3.2).

Termín je dostupný, ak sa neprekrýva so žiadnou uloženou rezerváciou, leží celý v otváracích hodinách a jeho začiatok je v budúcnosti. Ak termín dostupný nie je, systém rezerváciu neuloží, uvedie pomenovaný dôvod a iný termín neponúka (DR-01, DR-11, DR-14).

- IDs: BP-SpravaNavstev-04, BR-001, BR-010

## Zablokovanie termínu

Po uložení rezervácie systém termín zablokuje pre ďalších klientov (docx kap. 5, 8.3.2).

- IDs: BR-002, BR-003, OUT-001

## Termín obsadený

Dôvod odmietnutia rezervácie, ktorej termín sa prekrýva s už uloženou rezerváciou, vrátane súbehu dvoch rezervácií (DR-02).

- IDs: BR-003, BR-009, BR-010, ADR-001

## Súbeh rezervácií

Situácia, keď dvaja správcovia návštev ukladajú rezervácie s prekrývajúcim sa termínom v rovnakom čase. Uloží sa prvá uložená rezervácia; druhá dostane odmietnutie „termín obsadený“ (DR-02).

- IDs: BR-009, ADR-001

## Otváracie hodiny

Čas, v ktorom je wellness centrum NikaWell otvorené: 10:00–22:00 (DR-01).

- Platia v časovom pásme Europe/Bratislava (DR-18) a každý deň rovnako; sviatky a výnimky sú mimo rozsahu (DR-20).
- IDs: BR-005

## Miestnosť NikaWell

Jediná miestnosť wellness centra, v ktorej prebieha návšteva klienta. Preto v jednom čase môže existovať najviac jedna rezervácia (DR-01).

- IDs: BR-003

## Poznámka

Voliteľný text rezervácie, ktorý zadá recepčná; najviac 500 znakov (DR-14, DR-17). Znak = Unicode code point (DR-C08).

- Poznámka je osobný údaj; jej obsah sa nesmie zapisovať do aplikačného logu (DR-27).
- IDs: BR-008, CON-003

## Dôvod odmietnutia

Pomenovanie príčiny, prečo systém rezerváciu neuložil (napr. „termín obsadený“). Iný termín systém neponúka (DR-11).
Pri viacerých porušeniach systém uvedie iba jeden dôvod podľa pevného poradia (DR-C04).

- IDs: BR-010, BR-012

## Evidencia rezervácií

Uložené rezervácie v IntelliWell (Data Object Rezervácie). Pojem sa používa v scenároch, keď úložisko rezervácií
nie je dostupné.

- IDs: DO-Rezervacie, QR-002

## Dočasná nedostupnosť služby

Stav, keď evidencia rezervácií nie je dostupná. Systém rezerváciu neuloží, oznámi dôvod „služba je dočasne
nedostupná“ (znenie návrh AI) a uloženie automaticky neopakuje (DR-C06).

- IDs: QR-002

## Záznam o vytvorení rezervácie

Údaj o tom, kto (prihlásená osoba personálu) a kedy rezerváciu vytvoril (DR-13).

- IDs: QR-001

## Sumarizácia rezervácie

Prehľad, ktorý IntelliWell zobrazí pri vytváraní rezervácie pred overením dostupnosti (docx kap. 5, kap. 15.2 ako „Sumarizácia rezervácie (UC01)“).

- Sumarizácia je súčasť UI, nie contractu, a nie je v rozsahu prototypu (DR-17). Jednoduché UI na vytvorenie rezervácie je v rozsahu (FR1-UI, ADR-010; zmena DR-17 v 0009).

## Rozhranie na vytvorenie rezervácie

Jednoduché používateľské rozhranie IntelliWell, v ktorom správca návštev vytvorí rezerváciu: vloží prihlasovací token, zadá identifikátor klienta, začiatok termínu v miestnom čase Europe/Bratislava a voliteľnú poznámku (prompt 0009, ADR-010).

- IDs: FR1-UI, US-002, ADR-010
- status: "approved" (0009).
- Model B-01 nemá Application Interface pre recepčnú – známa medzera modelu (DR-0914). Rozhranie sa zatiaľ viaže na AC-ModulRezervaciiAKlientov a AS-SpravaRezervaciiAKlientov.
- Miestny čas = čas v Europe/Bratislava; rozhranie ho prepočíta na UTC (DR-18, DR-0911).

## Evidencia klientov

Centrálne uložené údaje o klientoch, ktoré IntelliWell číta aj zapisuje (docx kap. 6, 9.2.2).

- IDs: DO-EvidenciaKlientov

## Profil klienta

Údaje klienta v evidencii klientov: identifikačné a kontaktné údaje a kontraindikácie (docx kap. 5, Registrácia nového klienta). Vzniká registráciou (FR2, UC02).

- IDs: DO-EvidenciaKlientov
- Súvisí: FR2 (mimo rozsahu FR1)

## Kontraindikácie

Súčasť profilu klienta. Docx ich uvádza medzi citlivými údajmi podľa GDPR (kap. 8.1).

- IDs: DO-EvidenciaKlientov

## Saunový majster

Interný aktér, ktorý poskytuje doplnkovú službu. Priradenie k rezervácii potvrdí alebo odmietne vo svojom rozhraní (docx kap. 3.2).

- IDs: ACT-SaunovyMajster, ROLE-PoskytovatelDoplnkovejSluzby

## Priradenie saunového majstra

Krok procesu Správa návštev, ak si to klient želá. Recepčná vyberie saunového majstra, typ arómy a starostlivosti o pokožku (docx kap. 5, 8.3.2). Patrí k UC05 / FR5.

- IDs: BP-SpravaNavstev-03
- Nie je súčasťou FR1 (DR-10).

## Služba správy rezervácií a klientov

Aplikačná služba IntelliWell, ktorá podporuje správu návštev a realizuje FR1, FR2 a NFR1 (docx kap. 8.1, 9.2.2).

- IDs: AS-SpravaRezervaciiAKlientov

## Modul rezervácií a klientov

Modul IntelliWell, ktorý eviduje rezervácie a profily klientov a realizuje službu správy rezervácií a klientov (docx kap. 9.1.2, 9.2.2). Je vlastníkom údajov Rezervácie a Evidencia klientov (DR-12).

- IDs: AC-ModulRezervaciiAKlientov, ADR-006

## Kanál požiadavky

Spôsob, ktorým klient oznamuje požiadavku: telefonicky, e-mailom, SMS alebo osobne. Systém umožňuje zaevidovať rezerváciu bez ohľadu na kanál (docx kap. 6, 8.1).

- IDs: CON-001
