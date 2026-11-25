# NÁVRH AI (PREPARE_SPEC, úloha 0009) – doplnenie spec/stories/features/US-001.feature o scenár pre N-10.
# Nie je schválený. Význam potvrdzuje owner BR-011 (ACT-Manazer, DR-21). Schválený súbor AI nemenila.
# Zdroj správania: BR-011 (statement, DR-C12), ADR-007 doplnenie 2026-11-25, contract 0.2.1 príklad
# `unauthenticated-missing-sub` (401 unauthenticated). Nové pravidlo nevzniká – scenár iba ilustruje existujúce.
# Termín 1. 12. 2026 20:00 (Europe/Bratislava) = 2026-12-01T19:00:00Z ako v príklade contractu.
# Vložiť za SC-001-13. Background US-001.feature platí bez zmeny.
# Formulácia „prihlásenie neurčuje, kto je prihlásený“ je business opis tokenu bez neprázdneho claimu sub
# (glossary: Prihlásený personál – „každý zásah je priradený konkrétnej prihlásenej osobe“). Znenie potvrdí človek.

  @SC-001-17 @FR1 @NFR4 @BR-011 @ADR-007
  Scenario: Prihlásenie, ktoré neurčuje konkrétnu osobu, sa posudzuje ako neprihlásenie
    Given osoba má prihlásenie s rolou správcu návštev, ktoré neurčuje, kto je prihlásený
    When osoba uloží rezerváciu pre klientku „Eva Nováková“ so začiatkom 1. 12. 2026 20:00
    Then systém rezerváciu neuloží, lebo osoba nie je prihlásená
    And termín 1. 12. 2026 20:00–21:00 zostane voľný

# Súvisiace návrhy (po schválení scenára):
# - business-rules.yaml BR-011: scenarios: ["SC-001-12", "SC-001-13", "SC-001-17"]; oprava chýbajúcej medzery
#   „neuloží.Token“ → „neuloží. Token“ (redakčné, význam sa nemení).
# - US-001.feature hlavička: „0009: doplnený SC-001-17 (BR-011, DR-C12, N-10).“
# - traceability: pozri traceability-realization.yaml (NFR4 → SC-001-17, SC-001-17 → contract test).
# - contract 0.2.1 a test: pozri DR-0908 v navrh-0009.md.
