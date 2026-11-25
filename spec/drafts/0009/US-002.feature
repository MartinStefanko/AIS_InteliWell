# status: "draft" | návrh AI, PREPARE_SPEC, úloha 0009 | cieľ: spec/stories/features/US-002.feature
# architecture_baseline: B-01 | specification_version: 0.4.0 (draft)
# Časy sú v Europe/Bratislava (DR-18). Rozhranie posiela API čas v UTC s príponou Z (DR-C03).
# Texty hlášok sú návrh AI (DR-0912). Scenáre neopisujú ovládacie prvky obrazovky (AGENTS.md kap. 12).

Feature: Vytvorenie rezervácie cez používateľské rozhranie
  Recepčná v role správcu návštev vytvorí rezerváciu v jednoduchom rozhraní IntelliWell
  a dozvie sa, či sa rezervácia uložila, alebo prečo nie.

  Background:
    Given otváracie hodiny NikaWell sú každý deň 10:00–22:00
    And klientka „Eva Nováková“ je v evidencii klientov a rozhranie ponúka jej identifikátor ako predvolený
    And aktuálny čas je 25. 11. 2026 12:00

  @SC-002-01 @FR1-UI @FR1 @BR-001 @BR-002
  Scenario: Správkyňa návštev vytvorí rezerváciu v rozhraní
    Given recepčná Jana je prihlásená v rozhraní v role správcu návštev
    When Jana v rozhraní uloží rezerváciu pre klientku „Eva Nováková“ so začiatkom 1. 12. 2026 14:00 a poznámkou „prvá návšteva“
    Then rozhranie potvrdí uloženú rezerváciu na termín 1. 12. 2026 14:00–15:00
    And termín 1. 12. 2026 14:00–15:00 je zablokovaný pre ďalších klientov

  @SC-002-02 @FR1-UI @DR-18 @DR-C03
  Scenario: Začiatok zadaný v letnom čase sa uloží ako ten istý okamih
    Given recepčná Jana je prihlásená v rozhraní v role správcu návštev
    When Jana v rozhraní uloží rezerváciu pre klientku „Eva Nováková“ so začiatkom 1. 7. 2027 14:00 miestneho času
    Then systém uloží rezerváciu so začiatkom 1. 7. 2027 12:00 UTC
    And rozhranie potvrdí termín 1. 7. 2027 14:00–15:00 miestneho času

  @SC-002-03 @FR1-UI @DR-18 @DR-C03
  Scenario: Začiatok zadaný v zimnom čase sa uloží ako ten istý okamih
    Given recepčná Jana je prihlásená v rozhraní v role správcu návštev
    When Jana v rozhraní uloží rezerváciu pre klientku „Eva Nováková“ so začiatkom 1. 12. 2026 16:00 miestneho času
    Then systém uloží rezerváciu so začiatkom 1. 12. 2026 15:00 UTC
    And rozhranie potvrdí termín 1. 12. 2026 16:00–17:00 miestneho času

  @SC-002-04 @FR1-UI @BR-003 @BR-010
  Scenario: Rozhranie zobrazí dôvod odmietnutia pri obsadenom termíne
    Given recepčná Jana je prihlásená v rozhraní v role správcu návštev
    And termín 1. 12. 2026 18:00–19:00 je už rezervovaný
    When Jana v rozhraní uloží rezerváciu pre klientku „Eva Nováková“ so začiatkom 1. 12. 2026 18:00
    Then rozhranie zobrazí hlášku „Termín je obsadený. Rezervácia sa neuložila.“
    And nová rezervácia sa neuloží

  @SC-002-05 @FR1-UI @BR-010 @BR-012
  Scenario Outline: Rozhranie zobrazí zrozumiteľný dôvod pre každý typ odmietnutia
    Given recepčná Jana sa pokúsi vytvoriť rezerváciu v rozhraní
    And systém rezerváciu odmietne s dôvodom „<dôvod>“
    When Jana odošle rezerváciu
    Then rozhranie zobrazí hlášku „<hláška>“
    And rezervácia sa neuloží

    Examples:
      | dôvod                                   | hláška                                                                                   |
      | osoba nie je prihlásená                 | Nie ste prihlásená. Vložte platný prihlasovací token.                                    |
      | nedostatočné oprávnenie                 | Nemáte oprávnenie vytvárať rezervácie. Je potrebná rola správcu návštev.                 |
      | neplatná požiadavka                     | Údaje rezervácie nie sú úplné alebo majú nesprávny tvar. Skontrolujte klienta a začiatok. |
      | služba je dočasne nedostupná            | Evidencia rezervácií je dočasne nedostupná. Rezervácia sa neuložila, skúste to neskôr.   |
      | klient neexistuje v evidencii klientov  | Klient s týmto identifikátorom nie je v evidencii klientov.                              |
      | začiatok termínu nie je v budúcnosti    | Začiatok termínu musí byť v budúcnosti.                                                  |
      | termín mimo otváracích hodín            | Termín musí celý ležať v otváracích hodinách 10:00–22:00.                                |
      | poznámka je dlhšia ako 500 znakov       | Poznámka môže mať najviac 500 znakov.                                                    |
      | termín obsadený                         | Termín je obsadený. Rezervácia sa neuložila.                                             |

  @SC-002-06 @FR1-UI @NFR4 @BR-011
  Scenario: Neprihlásená osoba sa v rozhraní dozvie, že nie je prihlásená
    Given osoba otvorila rozhranie a nie je prihlásená
    When osoba uloží rezerváciu pre klientku „Eva Nováková“ so začiatkom 1. 12. 2026 20:00
    Then rozhranie zobrazí hlášku „Nie ste prihlásená. Vložte platný prihlasovací token.“
    And termín 1. 12. 2026 20:00–21:00 zostane voľný

  @SC-002-07 @FR1-UI @QR-002
  Scenario: Rozhranie po nedostupnosti evidencie neopakuje uloženie samo
    Given recepčná Jana je prihlásená v rozhraní v role správcu návštev
    And evidencia rezervácií je nedostupná
    When Jana v rozhraní uloží rezerváciu pre klientku „Eva Nováková“ so začiatkom 1. 12. 2026 13:00
    Then rozhranie zobrazí hlášku o dočasnej nedostupnosti
    And rozhranie uloženie automaticky nezopakuje

  @SC-002-08 @FR1-UI @CON-003
  Scenario: Obsah poznámky sa pri práci v rozhraní nezapíše do logu
    Given recepčná Jana je prihlásená v rozhraní v role správcu návštev
    When Jana v rozhraní uloží rezerváciu s poznámkou „alergia na eukalyptus“
    Then text „alergia na eukalyptus“ sa neobjaví v aplikačnom logu ani v logu prehliadača
