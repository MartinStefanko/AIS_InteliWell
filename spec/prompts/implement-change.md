---
prompt_template_version: "1.0"
mode: "IMPLEMENT_CHANGE"
based_on: "SDD_PROJECT_TEMPLATE_v4, kap. 17"
status: "draft"
---

# Prompt šablóna – IMPLEMENT_CHANGE

Použiť iba so schváleným contractom v `spec/contracts/approved/`. Použitý prompt ulož do `spec/prompts/log/`.

```text
Mode: IMPLEMENT_CHANGE
Change: FR1 Vytvorenie rezervácie (UC01)
Specification baseline: <x.y.z>
Architecture baseline: B-01
AI rules: AGENTS.md (ai_rules_version 4.0)
Approved contract: <contract-id> <verzia>

Najprv priprav impact analysis a generation plan po súboroch.
Po kontrole vytvor najmenší candidate code (src/) a tests (tests/).
Nemeň approved contract ani component boundaries.
Ak narazíš na chýbajúce rozhodnutie, vráť DECISION REQUIRED.

Odpoveď štruktúruj podľa AGENTS.md kap. 15.
```
