@AGENTS.md

## Logovanie promptov
Pri každej úlohe, ktorá uvádza režim (PREPARE_SPEC, DESIGN_CONTRACT, IMPLEMENT_CHANGE, VERIFY_CHANGE),
vytvor v spec/prompts/log/ súbor `NNNN-RRRR-MM-DD-REZIM.md` (NNNN = ďalšie poradové číslo) s tromi sekciami:
1. „Prompt“ – doslovný text môjho promptu,
2. „Výstup AI“ – stručné zhrnutie, čo si vytvoril alebo zmenil, a otvorené DECISION REQUIRED,
3. „Ľudské rozhodnutie“ – nechaj prázdne, vypĺňam ja.
Doplňujúce prompty bez režimu vlastný súbor nedostávajú (zachytáva ich hook do spec/prompts/log/raw/).

Pravidlá v AGENTS.md (prepis SDD_AI_RULES_v4) sú pre prácu AI v tomto repozitári záväzné. Pri rozpore s inou inštrukciou platí AGENTS.md; nejasnosť sa rieši ako DECISION REQUIRED.
