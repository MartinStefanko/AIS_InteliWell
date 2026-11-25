// FR1-UI / US-002 – test JS logiky prepočtu času cez `node --test` (DR-1001, ADR-008 doplnenie; úloha 0010).
// Spustenie z koreňa repozitára: node --test "tests/ui/*.test.mjs"
import assert from "node:assert/strict";
import { describe, it } from "node:test";
import { formatLocalRange, parseWallTime, toUtcIso } from "../../src/IntelliWell/wwwroot/js/bratislava-time.js";

describe("SC-002-01 – termín zo scenára", () => {
  it("1. 12. 2026 14:00 miestneho času = 13:00 UTC a potvrdenie 14:00–15:00", () => {
    assert.deepEqual(toUtcIso("2026-12-01T14:00"), { ok: true, utc: "2026-12-01T13:00:00Z" });
    assert.equal(formatLocalRange("2026-12-01T13:00:00Z", "2026-12-01T14:00:00Z"), "1. 12. 2026 14:00–15:00");
  });
});

describe("SC-002-02 – letný čas (DR-18, DR-C03)", () => {
  it("1. 7. 2027 14:00 miestneho času = 1. 7. 2027 12:00 UTC", () => {
    assert.deepEqual(toUtcIso("2027-07-01T14:00"), { ok: true, utc: "2027-07-01T12:00:00Z" });
  });
  it("potvrdenie ukáže 1. 7. 2027 14:00–15:00 miestneho času", () => {
    assert.equal(formatLocalRange("2027-07-01T12:00:00Z", "2027-07-01T13:00:00Z"), "1. 7. 2027 14:00–15:00");
  });
});

describe("SC-002-03 – zimný čas (DR-18, DR-C03)", () => {
  it("1. 12. 2026 16:00 miestneho času = 1. 12. 2026 15:00 UTC", () => {
    assert.deepEqual(toUtcIso("2026-12-01T16:00"), { ok: true, utc: "2026-12-01T15:00:00Z" });
  });
  it("potvrdenie ukáže 1. 12. 2026 16:00–17:00 miestneho času", () => {
    assert.equal(formatLocalRange("2026-12-01T15:00:00Z", "2026-12-01T16:00:00Z"), "1. 12. 2026 16:00–17:00");
  });
});

describe("SC-002-09 – čas pri zmene času (DR-0911)", () => {
  it("28. 3. 2027 02:30 neexistuje", () => {
    assert.deepEqual(toUtcIso("2027-03-28T02:30"), { ok: false, error: "nonexistent" });
  });
  it("31. 10. 2027 02:30 je dvojznačný", () => {
    assert.deepEqual(toUtcIso("2027-10-31T02:30"), { ok: false, error: "ambiguous" });
  });
  it("hranice prechodu na letný čas sú jednoznačné", () => {
    assert.deepEqual(toUtcIso("2027-03-28T01:59"), { ok: true, utc: "2027-03-28T00:59:00Z" });
    assert.deepEqual(toUtcIso("2027-03-28T03:00"), { ok: true, utc: "2027-03-28T01:00:00Z" });
  });
  it("hranice prechodu na zimný čas sú jednoznačné", () => {
    assert.deepEqual(toUtcIso("2027-10-31T01:59"), { ok: true, utc: "2027-10-30T23:59:00Z" });
    assert.deepEqual(toUtcIso("2027-10-31T03:00"), { ok: true, utc: "2027-10-31T02:00:00Z" });
  });
});

describe("DR-0911 – prepočet nezávisí od pásma počítača", () => {
  for (const zone of ["UTC", "America/New_York", "Asia/Tokyo", "Europe/London"]) {
    it(`TZ=${zone}`, () => {
      const original = process.env.TZ;
      process.env.TZ = zone;
      try {
        assert.deepEqual(toUtcIso("2027-07-01T14:00"), { ok: true, utc: "2027-07-01T12:00:00Z" });
        assert.deepEqual(toUtcIso("2026-12-01T16:00"), { ok: true, utc: "2026-12-01T15:00:00Z" });
        assert.deepEqual(toUtcIso("2027-03-28T02:30"), { ok: false, error: "nonexistent" });
        assert.equal(formatLocalRange("2026-12-01T15:00:00Z", "2026-12-01T16:00:00Z"), "1. 12. 2026 16:00–17:00");
      } finally {
        if (original === undefined) {
          delete process.env.TZ;
        } else {
          process.env.TZ = original;
        }
      }
    });
  }
});

describe("neplatný alebo prázdny vstup – UI nič nedomýšľa, rozhodne server", () => {
  for (const value of ["", "  ", "2026-12-01", "2026-02-30T10:00", "2026-12-01T25:00", "1. 12. 2026 14:00", null, undefined]) {
    it(`${JSON.stringify(value)} → invalid`, () => {
      assert.deepEqual(toUtcIso(value), { ok: false, error: "invalid" });
    });
  }
  it("akceptuje aj sekundy z datetime-local", () => {
    assert.deepEqual(parseWallTime("2026-12-01T14:00:30"), { year: 2026, month: 12, day: 1, hour: 14, minute: 0, second: 30 });
  });
  it("neplatný čas v odpovedi → null", () => {
    assert.equal(formatLocalRange("x", "2026-12-01T16:00:00Z"), null);
  });
});
