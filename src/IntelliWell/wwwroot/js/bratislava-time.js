// FR1-UI (ADR-010) – prepočet miestneho času Europe/Bratislava na UTC a späť (DR-18, DR-C03, DR-0911).
// Výpočet vždy používa pásmo Europe/Bratislava cez Intl, nezávisle od pásma počítača.
// Modul nepracuje s DOM, aby sa dal testovať cez `node --test` (DR-1001, ADR-008 doplnenie).

export const TIME_ZONE = "Europe/Bratislava";

const wallClockFormat = new Intl.DateTimeFormat("en-GB", {
  timeZone: TIME_ZONE,
  hourCycle: "h23",
  year: "numeric",
  month: "2-digit",
  day: "2-digit",
  hour: "2-digit",
  minute: "2-digit",
  second: "2-digit",
});

const DAY_MS = 24 * 60 * 60 * 1000;

/** Miestny čas (Europe/Bratislava) daného okamihu ako čísla. */
function wallClockOf(epochMs) {
  const parts = {};
  for (const part of wallClockFormat.formatToParts(new Date(epochMs))) {
    if (part.type !== "literal") {
      parts[part.type] = Number(part.value);
    }
  }
  return {
    year: parts.year,
    month: parts.month,
    day: parts.day,
    hour: parts.hour,
    minute: parts.minute,
    second: parts.second,
  };
}

/** Posun Europe/Bratislava voči UTC v milisekundách v danom okamihu. */
function offsetAt(epochMs) {
  const w = wallClockOf(epochMs);
  const asUtc = Date.UTC(w.year, w.month - 1, w.day, w.hour, w.minute, w.second);
  return asUtc - Math.floor(epochMs / 1000) * 1000;
}

/** Rozloží hodnotu „YYYY-MM-DDTHH:MM[:SS]“ (vstup datetime-local); inak null. */
export function parseWallTime(value) {
  if (typeof value !== "string") {
    return null;
  }
  const match = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2})(?::(\d{2}))?$/.exec(value.trim());
  if (!match) {
    return null;
  }
  const [year, month, day, hour, minute] = match.slice(1, 6).map(Number);
  const second = match[6] === undefined ? 0 : Number(match[6]);
  const check = new Date(Date.UTC(year, month - 1, day, hour, minute, second));
  if (
    check.getUTCFullYear() !== year ||
    check.getUTCMonth() !== month - 1 ||
    check.getUTCDate() !== day ||
    check.getUTCHours() !== hour ||
    check.getUTCMinutes() !== minute ||
    check.getUTCSeconds() !== second
  ) {
    return null;
  }
  return { year, month, day, hour, minute, second };
}

function sameWallClock(a, b) {
  return (
    a.year === b.year &&
    a.month === b.month &&
    a.day === b.day &&
    a.hour === b.hour &&
    a.minute === b.minute &&
    a.second === b.second
  );
}

/** RFC 3339 v UTC s príponou Z, bez zlomkov sekundy (DR-C03). */
function toIsoZ(epochMs) {
  return new Date(epochMs).toISOString().replace(/\.\d{3}Z$/, "Z");
}

/**
 * Prepočíta miestny čas Europe/Bratislava na UTC.
 * Výsledok: { ok: true, utc: "2026-12-01T15:00:00Z" }
 *        alebo { ok: false, error: "invalid" | "nonexistent" | "ambiguous" } (DR-0911).
 */
export function toUtcIso(value) {
  const wall = parseWallTime(value);
  if (wall === null) {
    return { ok: false, error: "invalid" };
  }
  const naive = Date.UTC(wall.year, wall.month - 1, wall.day, wall.hour, wall.minute, wall.second);
  const offsets = new Set([offsetAt(naive - DAY_MS), offsetAt(naive), offsetAt(naive + DAY_MS)]);
  const matches = new Set();
  for (const offset of offsets) {
    const candidate = naive - offset;
    if (sameWallClock(wallClockOf(candidate), wall)) {
      matches.add(candidate);
    }
  }
  if (matches.size === 0) {
    return { ok: false, error: "nonexistent" };
  }
  if (matches.size > 1) {
    return { ok: false, error: "ambiguous" };
  }
  const [instant] = matches;
  return { ok: true, utc: toIsoZ(instant) };
}

function pad2(n) {
  return String(n).padStart(2, "0");
}

function formatDate(w) {
  return `${w.day}. ${w.month}. ${w.year}`;
}

function formatTime(w) {
  return `${pad2(w.hour)}:${pad2(w.minute)}`;
}

/**
 * Termín v miestnom čase Europe/Bratislava, napr. „1. 12. 2026 14:00–15:00“ (US-002 AK 3).
 * Ak koniec padne na iný deň, uvedie aj jeho dátum. Neplatný vstup → null.
 */
export function formatLocalRange(startIso, endIso) {
  const start = Date.parse(startIso);
  const end = Date.parse(endIso);
  if (Number.isNaN(start) || Number.isNaN(end)) {
    return null;
  }
  const s = wallClockOf(start);
  const e = wallClockOf(end);
  const sameDay = s.year === e.year && s.month === e.month && s.day === e.day;
  return sameDay
    ? `${formatDate(s)} ${formatTime(s)}–${formatTime(e)}`
    : `${formatDate(s)} ${formatTime(s)} – ${formatDate(e)} ${formatTime(e)}`;
}
