# status: "approved" | approved_by: "Martin Štefanko (Software architect)" | reviewed_by: "Martin Štefanko (Software architect)"
# architecture_baseline: B-01 | specification_version: 0.3.0 (návrh) | pripravila AI v PREPARE_SPEC (log 0002, 0004)
# 0004: doplnené SC-001-15 (QR-002, DR-C06) a SC-001-16 (BR-012, DR-C04).
# Časy sú v časovom pásme Europe/Bratislava (DR-18); API ich prenáša v UTC.
# Znenie dôvodov odmietnutia okrem „termín obsadený“ (DR-02) je návrh AI (BR-010, QR-002).

Feature: Vytvorenie rezervácie
  Recepčná v role správcu návštev vytvorí rezerváciu pre existujúceho klienta
  na 60-minútový termín v miestnosti NikaWell.

  Background:
    Given otváracie hodiny NikaWell sú každý deň 10:00–22:00
    And klientka „Eva Nováková“ je v evidencii klientov
    And aktuálny čas je 25. 11. 2026 12:00

  @SC-001-01 @FR1 @BR-001 @BR-002 @BR-004 @BR-008 @QR-001
  Scenario: Správca návštev vytvorí rezerváciu na voľný termín
    Given recepčná „Jana“ je prihlásená v role správca návštev
    And termín 1. 12. 2026 14:00–15:00 je voľný
    When Jana uloží rezerváciu pre klientku „Eva Nováková“ so začiatkom 1. 12. 2026 14:00 a poznámkou „prvá návšteva“
    Then systém uloží rezerváciu v stave „vytvorená“ s termínom 1. 12. 2026 14:00–15:00
    And termín 1. 12. 2026 14:00–15:00 je zablokovaný pre ďalších klientov
    And systém zaznamená, že rezerváciu vytvorila Jana, a čas jej vytvorenia

  @SC-001-02 @FR1 @BR-008
  Scenario: Rezervácia bez poznámky sa uloží
    Given recepčná „Jana“ je prihlásená v role správca návštev
    And termín 2. 12. 2026 11:00–12:00 je voľný
    When Jana uloží rezerváciu pre klientku „Eva Nováková“ so začiatkom 2. 12. 2026 11:00 bez poznámky
    Then systém uloží rezerváciu v stave „vytvorená“ s termínom 2. 12. 2026 11:00–12:00

  @SC-001-03 @FR1 @BR-005
  Scenario: Termín končiaci presne pri zatvorení sa uloží
    Given recepčná „Jana“ je prihlásená v role správca návštev
    And termín 3. 12. 2026 21:00–22:00 je voľný
    When Jana uloží rezerváciu pre klientku „Eva Nováková“ so začiatkom 3. 12. 2026 21:00
    Then systém uloží rezerváciu v stave „vytvorená“ s termínom 3. 12. 2026 21:00–22:00

  @SC-001-04 @FR1 @BR-003 @BR-010
  Scenario: Rezervácia na obsadený termín je odmietnutá
    Given recepčná „Jana“ je prihlásená v role správca návštev
    And v termíne 1. 12. 2026 14:00–15:00 už existuje rezervácia
    When Jana uloží rezerváciu pre klientku „Eva Nováková“ so začiatkom 1. 12. 2026 14:00
    Then systém rezerváciu neuloží a uvedie dôvod „termín obsadený“
    And systém neponúkne iný termín
    And v termíne 1. 12. 2026 14:00–15:00 existuje iba pôvodná rezervácia

  @SC-001-05 @FR1 @BR-003 @BR-010
  Scenario: Rezervácia na čiastočne prekrývajúci sa termín je odmietnutá
    Given recepčná „Jana“ je prihlásená v role správca návštev
    And v termíne 1. 12. 2026 14:00–15:00 už existuje rezervácia
    When Jana uloží rezerváciu pre klientku „Eva Nováková“ so začiatkom 1. 12. 2026 14:30
    Then systém rezerváciu neuloží a uvedie dôvod „termín obsadený“

  @SC-001-14 @FR1 @BR-003 @BR-004
  Scenario: Rezervácia nadväzujúca presne na koniec predchádzajúcej sa uloží
    Given recepčná „Jana“ je prihlásená v role správca návštev
    And v termíne 1. 12. 2026 14:00–15:00 už existuje rezervácia
    When Jana uloží rezerváciu pre klientku „Eva Nováková“ so začiatkom 1. 12. 2026 15:00
    Then systém uloží rezerváciu v stave „vytvorená“ s termínom 1. 12. 2026 15:00–16:00

  @SC-001-06 @FR1 @BR-003 @BR-009 @ADR-001
  Scenario: Pri súbehu sa uloží iba prvá rezervácia
    Given recepčné „Jana“ a „Mária“ sú prihlásené v role správca návštev
    And termín 4. 12. 2026 16:00–17:00 je voľný
    When Jana a Mária súčasne uložia rezerváciu so začiatkom 4. 12. 2026 16:00
    Then systém uloží iba rezerváciu, ktorá bola uložená ako prvá
    And druhú rezerváciu odmietne s dôvodom „termín obsadený“
    And v termíne 4. 12. 2026 16:00–17:00 existuje práve jedna rezervácia

  @SC-001-07 @FR1 @BR-007 @BR-010 @NFR1
  Scenario: Rezervácia pre klienta, ktorý nie je v evidencii klientov, je odmietnutá
    Given recepčná „Jana“ je prihlásená v role správca návštev
    And klient „Peter Horváth“ nie je v evidencii klientov
    When Jana uloží rezerváciu pre klienta „Peter Horváth“ so začiatkom 1. 12. 2026 16:00
    Then systém rezerváciu neuloží a uvedie dôvod „klient neexistuje v evidencii klientov“

  @SC-001-08 @FR1 @BR-005 @BR-010 @NFR1
  Scenario: Termín presahujúci zatváraciu hodinu je odmietnutý
    Given recepčná „Jana“ je prihlásená v role správca návštev
    When Jana uloží rezerváciu pre klientku „Eva Nováková“ so začiatkom 1. 12. 2026 21:30
    Then systém rezerváciu neuloží a uvedie dôvod „termín mimo otváracích hodín“

  @SC-001-09 @FR1 @BR-005 @BR-010 @NFR1
  Scenario: Termín začínajúci pred otvorením je odmietnutý
    Given recepčná „Jana“ je prihlásená v role správca návštev
    When Jana uloží rezerváciu pre klientku „Eva Nováková“ so začiatkom 1. 12. 2026 09:30
    Then systém rezerváciu neuloží a uvedie dôvod „termín mimo otváracích hodín“

  @SC-001-10 @FR1 @BR-006 @BR-010 @NFR1
  Scenario: Termín so začiatkom v minulosti je odmietnutý
    Given recepčná „Jana“ je prihlásená v role správca návštev
    When Jana uloží rezerváciu pre klientku „Eva Nováková“ so začiatkom 25. 11. 2026 11:00
    Then systém rezerváciu neuloží a uvedie dôvod „začiatok termínu nie je v budúcnosti“

  @SC-001-11 @FR1 @BR-008 @BR-010 @NFR1
  Scenario: Príliš dlhá poznámka je odmietnutá
    Given recepčná „Jana“ je prihlásená v role správca návštev
    When Jana uloží rezerváciu pre klientku „Eva Nováková“ so začiatkom 1. 12. 2026 18:00 a poznámkou dlhou 501 znakov
    Then systém rezerváciu neuloží a uvedie dôvod „poznámka je dlhšia ako 500 znakov“

  @SC-001-12 @FR1 @NFR4 @BR-011 @ADR-004
  Scenario: Osoba bez roly správcu návštev nemôže vytvoriť rezerváciu
    Given saunový majster „Tomáš“ je prihlásený iba v role poskytovateľ doplnkovej služby
    When Tomáš uloží rezerváciu pre klientku „Eva Nováková“ so začiatkom 1. 12. 2026 19:00
    Then systém rezerváciu neuloží pre nedostatočné oprávnenie
    And termín 1. 12. 2026 19:00–20:00 zostane voľný

  @SC-001-13 @FR1 @NFR4 @BR-011 @ADR-004
  Scenario: Neprihlásená osoba nemôže vytvoriť rezerváciu
    Given osoba nie je prihlásená do IntelliWell
    When osoba odošle rezerváciu pre klientku „Eva Nováková“ so začiatkom 1. 12. 2026 20:00
    Then systém rezerváciu neuloží, lebo osoba nie je prihlásená
    And termín 1. 12. 2026 20:00–21:00 zostane voľný

  @SC-001-15 @FR1 @QR-002
  Scenario: Pri nedostupnej evidencii rezervácií sa rezervácia neuloží
    Given recepčná „Jana“ je prihlásená v role správca návštev
    And evidencia rezervácií je dočasne nedostupná
    When Jana uloží rezerváciu pre klientku „Eva Nováková“ so začiatkom 5. 12. 2026 14:00
    Then systém rezerváciu neuloží a uvedie dôvod „služba je dočasne nedostupná“
    And po obnovení dostupnosti je termín 5. 12. 2026 14:00–15:00 voľný

  @SC-001-16 @FR1 @BR-012 @BR-010 @NFR1
  Scenario: Pri viacerých porušeniach systém uvedie iba prvý dôvod podľa poradia
    Given recepčná „Jana“ je prihlásená v role správca návštev
    And klient „Peter Horváth“ nie je v evidencii klientov
    When Jana uloží rezerváciu pre klienta „Peter Horváth“ so začiatkom 25. 11. 2026 09:00 a poznámkou dlhou 501 znakov
    Then systém rezerváciu neuloží a uvedie iba dôvod „klient neexistuje v evidencii klientov“
