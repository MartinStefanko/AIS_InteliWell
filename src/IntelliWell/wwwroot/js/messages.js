// FR1-UI (ADR-010) – hlášky rozhrania. Texty schválil človek (DR-0912; SC-002-09 v 0009; potvrdenie 201 v 0010).
// Kľúče = enum `type` v ProblemDetails z API-Reservations 0.2.1 (approved, DR-C07). Modul nepracuje s DOM.

export const PROBLEM_MESSAGES = Object.freeze({
  "urn:intelliwell:problem:unauthenticated": "Nie ste prihlásená. Vložte platný prihlasovací token.",
  "urn:intelliwell:problem:forbidden": "Nemáte oprávnenie vytvárať rezervácie. Je potrebná rola správcu návštev.",
  "urn:intelliwell:problem:invalid-request":
    "Údaje rezervácie nie sú úplné alebo majú nesprávny tvar. Skontrolujte klienta a začiatok.",
  "urn:intelliwell:problem:service-unavailable":
    "Evidencia rezervácií je dočasne nedostupná. Rezervácia sa neuložila, skúste to neskôr.",
  "urn:intelliwell:problem:client-not-found": "Klient s týmto identifikátorom nie je v evidencii klientov.",
  "urn:intelliwell:problem:start-not-in-future": "Začiatok termínu musí byť v budúcnosti.",
  "urn:intelliwell:problem:outside-opening-hours": "Termín musí celý ležať v otváracích hodinách 10:00–22:00.",
  "urn:intelliwell:problem:note-too-long": "Poznámka môže mať najviac 500 znakov.",
  "urn:intelliwell:problem:slot-occupied": "Termín je obsadený. Rezervácia sa neuložila.",
});

// SC-002-09 (DR-0911)
export const MESSAGE_NONEXISTENT_TIME =
  "Zadaný čas pri zmene času v Europe/Bratislava neexistuje. Zadajte iný čas.";
export const MESSAGE_AMBIGUOUS_TIME =
  "Zadaný čas je pri zmene času v Europe/Bratislava dvojznačný. Zadajte iný čas.";

// SC-002-10 (DR-0912); aj pre odpoveď bez problem details (DR-1002 = A)
export const MESSAGE_NO_RESPONSE =
  "Server neodpovedá. Nie je isté, či sa rezervácia uložila – skontrolujte to pred ďalším pokusom.";

/** Potvrdenie pri 201 (schválené v 0010), napr. „Rezervácia je uložená na termín 1. 12. 2026 14:00–15:00.“ */
export function confirmationMessage(localRange) {
  return `Rezervácia je uložená na termín ${localRange}.`;
}

/**
 * Hláška pre odmietnutie: podľa `type`; pri neznámom `type` text `title` z odpovede (DR-0912);
 * bez použiteľných problem details text zo SC-002-10 (DR-1002).
 */
export function messageForProblem(problem) {
  if (problem !== null && typeof problem === "object") {
    if (typeof problem.type === "string" && Object.hasOwn(PROBLEM_MESSAGES, problem.type)) {
      return PROBLEM_MESSAGES[problem.type];
    }
    if (typeof problem.title === "string" && problem.title.trim() !== "") {
      return problem.title;
    }
  }
  return MESSAGE_NO_RESPONSE;
}
