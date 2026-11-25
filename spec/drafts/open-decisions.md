# Otvorené rozhodnutia – FR1 (draft, SETUP_FRAMEWORK 0001)

- status: "draft"
- architecture_baseline: "B-01", specification baseline: "0.1.0"
- Žiadne rozhodnutie nie je prijaté. Odpovede zapíše človek do uvedeného miesta (Requirement / Rule / ADR / Project policy).

| ID | Téma | Kam zapísať | Blokuje |
|---|---|---|---|
| DR-01 | Definícia termínu, dĺžka, sloty | Rule + Glossary | PREPARE_SPEC scenáre, contract |
| DR-02 | Súbeh dvoch rezervácií na ten istý termín | Rule + ADR | contract, implementácia |
| DR-03 | Rezervácia bez registrovaného klienta | Requirement | scenáre, contract |
| DR-04 | Stavy rezervácie (zrušenie?) | Rule (lifecycle) | lifecycle, contract |
| DR-05 | Implementačná platforma (.NET iba predpoklad) | ADR | IMPLEMENT_CHANGE |
| DR-06 | Konkrétny DBMS | ADR | IMPLEMENT_CHANGE |
| DR-07 | Prihlasovanie a oprávnenia personálu (NFR4) | Requirement + ADR | contract (security) |
| DR-08 | Štýl rozhrania / API (contract design profile) | Project policy | DESIGN_CONTRACT |
| DR-09 | Technology context applicable / not applicable | Project policy (technology-context.yaml) | DESIGN_CONTRACT, IMPLEMENT_CHANGE |
| DR-10 | Patrí priradenie saunového majstra do FR1? | Requirement | rozsah všetkého |
| DR-11 | Správanie pri nedostupnom termíne | Rule / Scenario | negatívne scenáre, contract |
| DR-12 | Vlastník Data Objects Rezervácie a Evidencia klientov | Requirement / ADR | contract, boundaries |
| DR-13 | Audit vytvorenia rezervácie | Requirement (quality) | contract |
| DR-14 | Platí NFR1 pre údaje rezervácie? Validačné pravidlá | Requirement (quality) | scenáre, contract |
| DR-15 | Owner a znenie FR1 | Requirement | schválenie FR1 |
| DR-16 | Zdrojový dokument: AIS_IntelliWell.docx vs. AIS_IntelliWell_4.docx | Manifest | baseline |
| DR-17 | Poznámka a sumarizácia (kap. 5) vs. kroky v BPC View | Requirement / Scenario | scenáre |

Podrobné znenie vo formáte DECISION REQUIRED je v odpovedi AI k záznamu
`spec/prompts/log/0001-2026-11-25-SETUP_FRAMEWORK.md`.
