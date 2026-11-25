---
prompt_template_version: "1.0"
mode: "VERIFY_CHANGE"
based_on: "SDD_PROJECT_TEMPLATE_v4, kap. 17"
status: "draft"
---

# Prompt šablóna – VERIFY_CHANGE

Použitý prompt ulož do `spec/prompts/log/`.

```text
Mode: VERIFY_CHANGE
Change: FR1 Vytvorenie rezervácie (UC01)
Specification baseline: <x.y.z>
Architecture baseline: B-01
AI rules: AGENTS.md (ai_rules_version 4.0)
Approved contract: <contract-id> <verzia>

Nemeň specification, contract, code, tests ani rozhodnutia.
Výsledok môžeš zapísať iba ako nový validation report do reports/.
Skontroluj syntax, IDs, traceability, architecture boundaries,
zhodu code s contractom a pokrytie scenarios a invariants.

Každú kontrolu označ PASS, FAIL alebo UNKNOWN a pripoj dôkaz.

Odpoveď štruktúruj podľa AGENTS.md kap. 15.
```
