// FR1-UI / US-002 – test mapovania hlášok cez `node --test` (DR-1001; DR-0912, DR-1002; úloha 0010).
// Spustenie z koreňa repozitára: node --test "tests/ui/*.test.mjs"
import assert from "node:assert/strict";
import { describe, it } from "node:test";
import {
  MESSAGE_AMBIGUOUS_TIME,
  MESSAGE_NONEXISTENT_TIME,
  MESSAGE_NO_RESPONSE,
  PROBLEM_MESSAGES,
  confirmationMessage,
  messageForProblem,
} from "../../src/IntelliWell/wwwroot/js/messages.js";

// Examples zo SC-002-05 (US-002.feature, schválené DR-0912) × enum `type` z API-Reservations 0.2.1.
const approved = [
  ["urn:intelliwell:problem:unauthenticated", "Nie ste prihlásená. Vložte platný prihlasovací token."],
  ["urn:intelliwell:problem:forbidden", "Nemáte oprávnenie vytvárať rezervácie. Je potrebná rola správcu návštev."],
  ["urn:intelliwell:problem:invalid-request", "Údaje rezervácie nie sú úplné alebo majú nesprávny tvar. Skontrolujte klienta a začiatok."],
  ["urn:intelliwell:problem:service-unavailable", "Evidencia rezervácií je dočasne nedostupná. Rezervácia sa neuložila, skúste to neskôr."],
  ["urn:intelliwell:problem:client-not-found", "Klient s týmto identifikátorom nie je v evidencii klientov."],
  ["urn:intelliwell:problem:start-not-in-future", "Začiatok termínu musí byť v budúcnosti."],
  ["urn:intelliwell:problem:outside-opening-hours", "Termín musí celý ležať v otváracích hodinách 10:00–22:00."],
  ["urn:intelliwell:problem:note-too-long", "Poznámka môže mať najviac 500 znakov."],
  ["urn:intelliwell:problem:slot-occupied", "Termín je obsadený. Rezervácia sa neuložila."],
];

describe("SC-002-04, SC-002-05, SC-002-06, SC-002-07 – hláška podľa type (DR-0912)", () => {
  for (const [type, message] of approved) {
    it(type, () => {
      assert.equal(messageForProblem({ type, title: "iný text zo servera", status: 400 }), message);
    });
  }
  it("mapa obsahuje presne 9 typov z contractu", () => {
    assert.deepEqual(Object.keys(PROBLEM_MESSAGES).sort(), approved.map(([type]) => type).sort());
  });
});

describe("neznámy type → title z odpovede (DR-0912)", () => {
  it("vráti title", () => {
    assert.equal(messageForProblem({ type: "urn:intelliwell:problem:new", title: "nový dôvod", status: 422 }), "nový dôvod");
  });
  it("zdedené vlastnosti objektu nie sú typ problému", () => {
    assert.equal(messageForProblem({ type: "toString", title: "nový dôvod" }), "nový dôvod");
  });
});

describe("odpoveď bez problem details → text SC-002-10 (DR-1002 = A)", () => {
  for (const problem of [null, undefined, "text", {}, { title: "   " }, { type: 42 }]) {
    it(JSON.stringify(problem) ?? "undefined", () => {
      assert.equal(messageForProblem(problem), MESSAGE_NO_RESPONSE);
    });
  }
});

describe("schválené texty", () => {
  it("SC-002-10 server neodpovedá", () => {
    assert.equal(MESSAGE_NO_RESPONSE, "Server neodpovedá. Nie je isté, či sa rezervácia uložila – skontrolujte to pred ďalším pokusom.");
  });
  it("SC-002-09 neexistujúci a dvojznačný čas", () => {
    assert.equal(MESSAGE_NONEXISTENT_TIME, "Zadaný čas pri zmene času v Europe/Bratislava neexistuje. Zadajte iný čas.");
    assert.equal(MESSAGE_AMBIGUOUS_TIME, "Zadaný čas je pri zmene času v Europe/Bratislava dvojznačný. Zadajte iný čas.");
  });
  it("SC-002-01 potvrdenie pri 201 (schválené v 0010)", () => {
    assert.equal(confirmationMessage("1. 12. 2026 14:00–15:00"), "Rezervácia je uložená na termín 1. 12. 2026 14:00–15:00.");
  });
});
