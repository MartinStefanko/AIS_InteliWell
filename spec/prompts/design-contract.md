---
prompt_template_version: "1.0"
mode: "DESIGN_CONTRACT"
based_on: "SDD_PROJECT_TEMPLATE_v4, kap. 17"
status: "draft"
---

# Prompt šablóna – DESIGN_CONTRACT

Použiť až po uzavretí DECISION REQUIRED relevantných pre contract a po schválení
`spec/policies/contract-design-profile.yaml`. Použitý prompt ulož do `spec/prompts/log/`.

```text
Mode: DESIGN_CONTRACT
Change: FR1 Vytvorenie rezervácie (UC01)
Specification baseline: <x.y.z>
Architecture baseline: B-01
AI rules: AGENTS.md (ai_rules_version 4.0)
Contract design profile: spec/policies/contract-design-profile.yaml (verzia <…>, status <approved>)

Navrhni contract podľa approved architecture a contract design profile.
Použi registrované a človekom overené obrázky a textové podklady (spec/manifest.yaml).
Ak podstatnú väzbu z obrázka nemožno overiť, neodhaduj ju.
Každú operation alebo event namapuj na Requirement (FR1), Application Service
(AS-SpravaRezervaciiAKlientov), Component (AC-ModulRezervaciiAKlientov),
rules, Data Objects a scenarios.

Výstup ulož ako proposed do spec/contracts/proposed/, sprievodný report do reports/.
Neschvaľuj ho.
Ak chýba integračné alebo business rozhodnutie, zastav sa.

Odpoveď štruktúruj podľa AGENTS.md kap. 15.
```
