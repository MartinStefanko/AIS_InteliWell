---
prompt_template_version: "1.0"
mode: "PREPARE_SPEC"
based_on: "SDD_PROJECT_TEMPLATE_v4, kap. 17"
status: "draft"
---

# Prompt šablóna – PREPARE_SPEC

Pred použitím doplň hodnoty v `< >`. Použitý prompt ulož do `spec/prompts/log/`.

```text
Mode: PREPARE_SPEC
Change: FR1 Vytvorenie rezervácie (UC01)
Specification baseline: <0.1.0>
Architecture baseline: B-01
AI rules: AGENTS.md (ai_rules_version 4.0)

Prečítaj spec/manifest.yaml a registrované podklady v rozsahu úlohy.
Obrázky zo Sparx EA (spec/architecture/views/B-01-*.png) použi podľa uvedeného
baseline a stavu kontroly (status, reviewed_by v manifeste).
Pracovné návrhy (spec/drafts/, vrátane id-mapping.yaml) a neoverené údaje
nepovažuj za schválené.
Skontroluj actors, process, Requirement, Service Realization,
Application Structure, Data Objects, rules a scenarios
(povinný kontext podľa AGENTS.md kap. 4).

Priprav iba draft chýbajúcich specifications:
- user story spec/drafts/US-001.md (nie priamo do spec/stories/),
- Gherkin spec/drafts/US-001.feature s pozitívnymi aj negatívnymi scenármi,
- návrh rules, invariants a lifecycle iba ako proposed.
Ak treba ľudské rozhodnutie, vráť DECISION REQUIRED pre závislú časť.
Nezávislé drafty a zoznam otázok môžeš dokončiť. Nič neschvaľuj.

Odpoveď štruktúruj podľa AGENTS.md kap. 15.
```
