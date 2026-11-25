// FR1-UI (ADR-010) – obsluha stránky. UI je iba konzument API-Reservations 0.2.1 (POST /api/v1/reservations).
// - Pravidlá termínu (BR-003 – BR-008, BR-012) vyhodnocuje server; UI ich neduplikuje.
// - Token je iba v pamäti stránky (DR-0910); nič sa neukladá do úložiska prehliadača.
// - Požiadavka sa posiela raz, UI ju samo neopakuje (QR-002).
// - Texty sa vkladajú iba ako text, nie HTML (ADR-010 Security); nič sa nezapisuje do konzoly (CON-003).

import { formatLocalRange, toUtcIso } from "./bratislava-time.js";
import {
  MESSAGE_AMBIGUOUS_TIME,
  MESSAGE_NONEXISTENT_TIME,
  MESSAGE_NO_RESPONSE,
  confirmationMessage,
  messageForProblem,
} from "./messages.js";

const RESERVATIONS_URL = "/api/v1/reservations";

const form = document.getElementById("reservation-form");
const tokenInput = document.getElementById("token");
const clientInput = document.getElementById("client-id");
const startInput = document.getElementById("start-time");
const noteInput = document.getElementById("note");
const submitButton = document.getElementById("submit");
const result = document.getElementById("result");

function showResult(text, kind) {
  result.textContent = text;
  result.className = `result ${kind}`;
}

function buildRequest(startUtc) {
  const body = { clientId: clientInput.value.trim() };
  if (startUtc !== null) {
    body.startTime = startUtc;
  }
  if (noteInput.value !== "") {
    body.note = noteInput.value;
  }
  const headers = { "Content-Type": "application/json", Accept: "application/json" };
  const token = tokenInput.value.trim();
  if (token !== "") {
    headers.Authorization = `Bearer ${token}`;
  }
  return { method: "POST", headers, body: JSON.stringify(body) };
}

async function readJson(response) {
  try {
    return await response.json();
  } catch {
    return null;
  }
}

async function submitReservation(event) {
  event.preventDefault();
  if (submitButton.disabled) {
    return;
  }

  // Prázdny alebo neúplný začiatok sa neposiela – rozhodne server (400 invalid-request).
  let startUtc = null;
  const converted = toUtcIso(startInput.value);
  if (converted.ok) {
    startUtc = converted.utc;
  } else if (converted.error === "nonexistent") {
    showResult(MESSAGE_NONEXISTENT_TIME, "error");
    return;
  } else if (converted.error === "ambiguous") {
    showResult(MESSAGE_AMBIGUOUS_TIME, "error");
    return;
  }

  submitButton.disabled = true;
  showResult("Ukladám rezerváciu…", "pending");
  try {
    let response;
    try {
      response = await fetch(RESERVATIONS_URL, buildRequest(startUtc));
    } catch {
      showResult(MESSAGE_NO_RESPONSE, "error");
      return;
    }

    const payload = await readJson(response);
    if (response.status === 201 && payload !== null) {
      const range = formatLocalRange(payload.startTime, payload.endTime);
      showResult(range === null ? MESSAGE_NO_RESPONSE : confirmationMessage(range), range === null ? "error" : "success");
      return;
    }

    const contentType = response.headers.get("Content-Type") ?? "";
    const problem = contentType.includes("application/problem+json") ? payload : null;
    showResult(messageForProblem(problem), "error");
  } finally {
    submitButton.disabled = false;
  }
}

form.addEventListener("submit", submitReservation);
