# Glossary – IntelliWell (FR1 Vytvorenie rezervácie)

- status: "draft"
- approved_by: null
- reviewed_by: null
- architecture_baseline: "B-01"
- zdroj: `docs/sources/AIS_IntelliWell.docx` (ďalej „docx“); IDs sú navrhnuté v `spec/drafts/id-mapping.yaml` (status proposed)

Definície sú prevzaté alebo zhrnuté z textu docx. Kde docx pojem nedefinuje, je to uvedené ako otvorená otázka.

## IntelliWell

Interný informačný systém wellness centra NikaWell, ktorý centralizuje plánovanie služieb a diaľkové riadenie inteligentných zariadení. Je určený len pre personál; klient so systémom priamo nepracuje (docx kap. 1, 6).

- IDs: AC-IntelliWell
- Súvisí: CON-002

## Wellness centrum NikaWell

Organizácia, pre ktorú je IntelliWell navrhnutý. Jedno wellness centrum (docx kap. 1, 3).

## Klient

Osoba mimo organizácie, ktorá navštevuje wellness centrum. Požiadavky oznamuje recepčnej telefonicky, e-mailom, SMS alebo osobne (docx kap. 3.2).

- IDs: ACT-Klient
- Otvorená otázka: či rezervácia vyžaduje klienta zaregistrovaného v evidencii klientov.

## Recepčná

Interný aktér, ktorý prijíma požiadavky klientov a v systéme vytvára rezervácie (docx kap. 2, 3.2).

- IDs: ACT-Recepcna

## Správca návštev

Rola recepčnej v budúcom stave. V systéme vytvára rezervácie (UC01), registruje nových klientov (UC02) a k rezervácii priraďuje saunového majstra (UC05) (docx kap. 3.3).

- IDs: ROLE-SpravcaNavstev

## Personál

Recepčná, saunový majster a manažér wellness centra NikaWell (docx kap. 3.2).

- Súvisí: NFR4, CON-002

## Prihlásený personál

Personál, ktorý má prístup do IntelliWell. Vďaka prihláseniu je každý zásah priradený konkrétnej prihlásenej osobe (docx kap. 8.1).

- IDs: NFR4
- Otvorená otázka: spôsob prihlasovania nie je v docx určený.

## Správa návštev

Biznis proces v budúcom stave. Spúšťa ho požiadavka klienta na rezerváciu. Recepčná v role správcu návštev vyberie alebo zaregistruje klienta, zadá dátum a čas, prípadne priradí saunového majstra; systém overí dostupnosť termínu, uloží rezerváciu a zablokuje termín (docx kap. 8.3.2).

- IDs: BP-SpravaNavstev

## Požiadavka klienta na rezerváciu

Udalosť, ktorou klient (telefonicky, e-mailom, SMS alebo osobne) spúšťa správu návštev (docx kap. 8.3.2).

- IDs: BEV-PoziadavkaNaRezervaciu
- Súvisí: CON-001

## Rezervácia

Záznam o termíne návštevy klienta, ktorý recepčná vytvorí v systéme. Pri vytvorení sa zadáva dátum, čas a poznámka (docx kap. 5). K rezervácii sa môže priradiť saunový majster (docx kap. 9.1.2) a pri príprave prostredia sa do nej uložia parametre zariadení alebo profil (docx kap. 8.3.2).

- IDs: DO-Rezervacie (údaje „Rezervácie“)
- Otvorená otázka: stavy rezervácie (napr. zrušenie) docx neuvádza.

## Rezervácia termínu

Biznis služba, ktorá slúži klientovi. Realizuje ju proces Správa návštev (docx kap. 8.2.2, 11.2).

- IDs: BS-RezervaciaTerminu
- Nepoužívať ako synonymum: rezervácia (údaj). „Rezervácia termínu“ je služba, „rezervácia“ je záznam.

## Termín

Docx opisuje návštevu ako „rezerváciu termínu, počas ktorého má [klient] k dispozícii prostredie“ (kap. 8.2.1). Termín sa zadáva dátumom a časom (kap. 5).

- Otvorená otázka: presná definícia termínu (dĺžka, sloty, začiatok/koniec) nie je v docx určená.

## Overenie dostupnosti termínu

Kontrola, ktorú systém vykoná pred uložením rezervácie (docx kap. 5, 8.3.2).

- IDs: BP-SpravaNavstev-04, BR-001
- Otvorená otázka: čo znamená „dostupný“ termín a čo sa stane, ak dostupný nie je.

## Zablokovanie termínu

Po uložení rezervácie systém termín zablokuje pre ďalších klientov (docx kap. 5, 8.3.2).

- IDs: BR-002, OUT-001

## Sumarizácia rezervácie

Prehľad, ktorý IntelliWell zobrazí pri vytváraní rezervácie pred overením dostupnosti (docx kap. 5, kap. 15.2 ako „Sumarizácia rezervácie (UC01)“).

- Otvorená otázka: obsah sumarizácie nie je v docx určený.

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
- Otvorená otázka: či je súčasťou rozsahu FR1.

## Služba správy rezervácií a klientov

Aplikačná služba IntelliWell, ktorá podporuje správu návštev a realizuje FR1, FR2 a NFR1 (docx kap. 8.1, 9.2.2).

- IDs: AS-SpravaRezervaciiAKlientov

## Modul rezervácií a klientov

Modul IntelliWell, ktorý eviduje rezervácie a profily klientov a realizuje službu správy rezervácií a klientov (docx kap. 9.1.2, 9.2.2).

- IDs: AC-ModulRezervaciiAKlientov

## Kanál požiadavky

Spôsob, ktorým klient oznamuje požiadavku: telefonicky, e-mailom, SMS alebo osobne. Systém umožňuje zaevidovať rezerváciu bez ohľadu na kanál (docx kap. 6, 8.1).

- IDs: CON-001
