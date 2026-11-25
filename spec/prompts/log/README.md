# Log promptov

Každé spustenie AI nad týmto repozitárom má jeden záznam. Log zabezpečuje reprodukovateľnosť:
z rovnakej verzie šablóny promptu a rovnakého specification baseline sa dá prácu zopakovať.

## Pomenovanie súboru

`NNNN-YYYY-MM-DD-<REŽIM>.md`, napr. `0002-2026-11-26-PREPARE_SPEC.md`.
`NNNN` je poradové číslo (rastie, nikdy sa neopakuje).

## Formát záznamu

```markdown
# NNNN – <REŽIM> – <stručný názov>

| Pole | Hodnota |
|---|---|
| Dátum | YYYY-MM-DD |
| Poradové číslo | NNNN |
| Režim | PREPARE_SPEC / DESIGN_CONTRACT / IMPLEMENT_CHANGE / VERIFY_CHANGE (alebo SETUP_FRAMEWORK) |
| Verzia šablóny promptu | napr. spec/prompts/prepare-spec.md, prompt_template_version 1.0 |
| Specification baseline | napr. 0.1.0 |
| Architecture baseline | napr. B-01 |
| Model | názov a ID modelu AI |
| Commit | hash commitu, v ktorom bol výstup zapísaný (doplní človek) |

## Presný text promptu
<doslovne, bez úprav>

## Výstup AI (stručne)
<zoznam vytvorených/zmenených súborov, kľúčové zistenia, otvorené DECISION REQUIRED>

## Ľudské rozhodnutie
<vypĺňa iba človek: kto, kedy, čo rozhodol (prijaté / zamietnuté / upravené),
 odkaz na ADR alebo súbor, kde je rozhodnutie zapísané>
```

## Pravidlá

- Text promptu sa ukladá doslovne.
- Sekciu „Ľudské rozhodnutie“ AI nevypĺňa.
- Záznam sa po commite nemení; oprava sa robí novým záznamom.
