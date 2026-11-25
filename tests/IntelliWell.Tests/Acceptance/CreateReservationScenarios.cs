using System.Net;
using IntelliWell.Hosting;
using IntelliWell.Tests.Infrastructure;
using static IntelliWell.Tests.Infrastructure.ReservationsApi;

namespace IntelliWell.Tests.Acceptance;

/// <summary>
/// Acceptance tests – spec/stories/features/US-001.feature (FR1). Jeden test = jeden scenár, trait Scenario = ID.
/// Background: otváracie hodiny 10:00–22:00; „Eva Nováková“ je v evidencii klientov; teraz = 25. 11. 2026 12:00.
/// Časy v scenároch sú Europe/Bratislava (UTC+1 v decembri); API ich prenáša v UTC (DR-18).
/// </summary>
public sealed class CreateReservationScenarios : IAsyncDisposable
{
    private readonly IntelliWellFactory _factory = new();
    private readonly HttpClient _client;

    public CreateReservationScenarios()
    {
        _client = _factory.CreateClient();
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    private async Task GivenExistingReservationAsync(string startTime) =>
        await AssertCreatedAsync(await PostAsync(_client, TestTokens.Jana, Eva, startTime));

    [Fact]
    [Trait("Scenario", "SC-001-01")]
    public async Task SC_001_01_SpravcaNavstevVytvoriRezervaciuNaVolnyTermin()
    {
        // Given termín 1. 12. 2026 14:00–15:00 je voľný
        Assert.Equal(0, _factory.Db.CountInSlot(DateTimeOffset.Parse("2026-12-01T13:00:00Z")));

        // When Jana uloží rezerváciu pre Evu so začiatkom 1. 12. 2026 14:00 a poznámkou „prvá návšteva“
        var response = await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-01T13:00:00Z", "prvá návšteva");

        // Then systém uloží rezerváciu v stave „vytvorená“ s termínom 14:00–15:00
        var body = await AssertCreatedAsync(response);
        Assert.Equal("created", body.GetProperty("status").GetString());
        Assert.Equal(Eva.ToString(), body.GetProperty("clientId").GetString());
        Assert.Equal("2026-12-01T13:00:00Z", body.GetProperty("startTime").GetString());
        Assert.Equal("2026-12-01T14:00:00Z", body.GetProperty("endTime").GetString());
        Assert.Equal("prvá návšteva", body.GetProperty("note").GetString());

        // And systém zaznamená, že rezerváciu vytvorila Jana, a čas jej vytvorenia (QR-001)
        Assert.Equal("jana", body.GetProperty("createdBy").GetString());
        Assert.Equal("2026-11-25T11:00:00Z", body.GetProperty("createdAt").GetString());
        var audit = _factory.Db.AuditOf(Guid.Parse(body.GetProperty("id").GetString()!));
        Assert.Equal("jana", audit.CreatedBy);
        Assert.Equal(IntelliWellFactory.Now, audit.CreatedAt);

        // And termín 14:00–15:00 je zablokovaný pre ďalších klientov (BR-002)
        Assert.Equal(1, _factory.Db.CountInSlot(DateTimeOffset.Parse("2026-12-01T13:00:00Z")));
        var another = await PostAsync(_client, TestTokens.Maria, Eva, "2026-12-01T13:00:00Z");
        await AssertProblemAsync(another, ProblemCatalog.SlotOccupied);
    }

    [Fact]
    [Trait("Scenario", "SC-001-02")]
    public async Task SC_001_02_RezervaciaBezPoznamkySaUlozi()
    {
        var response = await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-02T10:00:00Z");

        var body = await AssertCreatedAsync(response);
        Assert.Equal("created", body.GetProperty("status").GetString());
        Assert.Equal("2026-12-02T10:00:00Z", body.GetProperty("startTime").GetString());
        Assert.Equal("2026-12-02T11:00:00Z", body.GetProperty("endTime").GetString());
        Assert.False(body.TryGetProperty("note", out _), "Pri rezervácii bez poznámky pole note chýba.");
    }

    [Fact]
    [Trait("Scenario", "SC-001-03")]
    public async Task SC_001_03_TerminKonciaciPresnePriZatvoreniSaUlozi()
    {
        // 3. 12. 2026 21:00–22:00 Europe/Bratislava
        var response = await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-03T20:00:00Z");

        var body = await AssertCreatedAsync(response);
        Assert.Equal("created", body.GetProperty("status").GetString());
        Assert.Equal("2026-12-03T21:00:00Z", body.GetProperty("endTime").GetString());
    }

    [Fact]
    [Trait("Scenario", "SC-001-04")]
    public async Task SC_001_04_RezervaciaNaObsadenyTerminJeOdmietnuta()
    {
        await GivenExistingReservationAsync("2026-12-01T13:00:00Z");

        var response = await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-01T13:00:00Z");

        // Then systém rezerváciu neuloží a uvedie dôvod „termín obsadený“; iný termín neponúkne
        var problem = await AssertProblemAsync(response, ProblemCatalog.SlotOccupied);
        Assert.Equal(["type", "title", "status", "instance"], problem.EnumerateObject().Select(p => p.Name));
        // And v termíne existuje iba pôvodná rezervácia
        Assert.Equal(1, _factory.Db.CountInSlot(DateTimeOffset.Parse("2026-12-01T13:00:00Z")));
        Assert.Equal(1, _factory.Db.CountAll());
    }

    [Fact]
    [Trait("Scenario", "SC-001-05")]
    public async Task SC_001_05_RezervaciaNaCiastocnePrekryvajuciSaTerminJeOdmietnuta()
    {
        await GivenExistingReservationAsync("2026-12-01T13:00:00Z");

        var response = await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-01T13:30:00Z");

        await AssertProblemAsync(response, ProblemCatalog.SlotOccupied);
        Assert.Equal(1, _factory.Db.CountAll());
    }

    [Fact]
    [Trait("Scenario", "SC-001-06")]
    public async Task SC_001_06_PriSubehuSaUloziIbaPrvaRezervacia()
    {
        // When Jana a Mária súčasne uložia rezerváciu so začiatkom 4. 12. 2026 16:00
        using var start = new ManualResetEventSlim(false);
        var jana = Task.Run(() => { start.Wait(Ct); return PostAsync(_client, TestTokens.Jana, Eva, "2026-12-04T15:00:00Z"); }, Ct);
        var maria = Task.Run(() => { start.Wait(Ct); return PostAsync(_client, TestTokens.Maria, Eva, "2026-12-04T15:00:00Z"); }, Ct);
        start.Set();
        var responses = await Task.WhenAll(jana, maria);

        // Then systém uloží iba prvú; druhú odmietne s dôvodom „termín obsadený“
        var statuses = responses.Select(r => r.StatusCode).OrderBy(s => (int)s).ToArray();
        Assert.Equal([HttpStatusCode.Created, HttpStatusCode.Conflict], statuses);
        await AssertCreatedAsync(responses.Single(r => r.StatusCode == HttpStatusCode.Created));
        await AssertProblemAsync(responses.Single(r => r.StatusCode == HttpStatusCode.Conflict), ProblemCatalog.SlotOccupied);

        // And v termíne 4. 12. 2026 16:00–17:00 existuje práve jedna rezervácia
        Assert.Equal(1, _factory.Db.CountInSlot(DateTimeOffset.Parse("2026-12-04T15:00:00Z")));
    }

    [Fact]
    [Trait("Scenario", "SC-001-07")]
    public async Task SC_001_07_RezervaciaPreKlientaMimoEvidencieJeOdmietnuta()
    {
        var response = await PostAsync(_client, TestTokens.Jana, Peter, "2026-12-01T15:00:00Z");

        await AssertProblemAsync(response, ProblemCatalog.ClientNotFound);
        Assert.Equal(0, _factory.Db.CountAll());
    }

    [Fact]
    [Trait("Scenario", "SC-001-08")]
    public async Task SC_001_08_TerminPresahujuciZatvaraciuHodinuJeOdmietnuty()
    {
        // začiatok 1. 12. 2026 21:30 Europe/Bratislava
        var response = await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-01T20:30:00Z");

        await AssertProblemAsync(response, ProblemCatalog.OutsideOpeningHours);
        Assert.Equal(0, _factory.Db.CountAll());
    }

    [Fact]
    [Trait("Scenario", "SC-001-09")]
    public async Task SC_001_09_TerminZacinajuciPredOtvorenimJeOdmietnuty()
    {
        // začiatok 1. 12. 2026 09:30 Europe/Bratislava
        var response = await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-01T08:30:00Z");

        await AssertProblemAsync(response, ProblemCatalog.OutsideOpeningHours);
        Assert.Equal(0, _factory.Db.CountAll());
    }

    [Fact]
    [Trait("Scenario", "SC-001-10")]
    public async Task SC_001_10_TerminSoZaciatkomVMinulostiJeOdmietnuty()
    {
        // začiatok 25. 11. 2026 11:00 Europe/Bratislava; teraz 12:00
        var response = await PostAsync(_client, TestTokens.Jana, Eva, "2026-11-25T10:00:00Z");

        await AssertProblemAsync(response, ProblemCatalog.StartNotInFuture);
        Assert.Equal(0, _factory.Db.CountAll());
    }

    [Fact]
    [Trait("Scenario", "SC-001-11")]
    public async Task SC_001_11_PrilisDlhaPoznamkaJeOdmietnuta()
    {
        var note = new string('x', 501);

        var response = await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-01T17:00:00Z", note);

        var problem = await AssertProblemAsync(response, ProblemCatalog.NoteTooLong);
        Assert.DoesNotContain(note, problem.GetRawText()); // CON-003
        Assert.Equal(0, _factory.Db.CountAll());
    }

    [Fact]
    [Trait("Scenario", "SC-001-12")]
    public async Task SC_001_12_OsobaBezRolySpravcuNavstevNemozeVytvoritRezervaciu()
    {
        // Given saunový majster „Tomáš“ je prihlásený iba v role poskytovateľ doplnkovej služby
        var response = await PostAsync(_client, TestTokens.Tomas, Eva, "2026-12-01T18:00:00Z");

        // Then systém rezerváciu neuloží pre nedostatočné oprávnenie; termín 19:00–20:00 zostane voľný
        await AssertProblemAsync(response, ProblemCatalog.Forbidden);
        Assert.Equal(0, _factory.Db.CountInSlot(DateTimeOffset.Parse("2026-12-01T18:00:00Z")));
    }

    [Fact]
    [Trait("Scenario", "SC-001-13")]
    public async Task SC_001_13_NeprihlasenaOsobaNemozeVytvoritRezervaciu()
    {
        var response = await PostAsync(_client, token: null, Eva, "2026-12-01T19:00:00Z");

        await AssertProblemAsync(response, ProblemCatalog.Unauthenticated);
        Assert.Equal("Bearer", response.Headers.WwwAuthenticate.ToString());
        Assert.Equal(0, _factory.Db.CountInSlot(DateTimeOffset.Parse("2026-12-01T19:00:00Z")));
    }

    [Fact]
    [Trait("Scenario", "SC-001-14")]
    public async Task SC_001_14_RezervaciaNadvazujucaNaKoniecPredchadzajucejSaUlozi()
    {
        await GivenExistingReservationAsync("2026-12-01T13:00:00Z");

        var response = await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-01T14:00:00Z");

        var body = await AssertCreatedAsync(response);
        Assert.Equal("created", body.GetProperty("status").GetString());
        Assert.Equal("2026-12-01T14:00:00Z", body.GetProperty("startTime").GetString());
        Assert.Equal("2026-12-01T15:00:00Z", body.GetProperty("endTime").GetString());
        Assert.Equal(2, _factory.Db.CountAll());
    }

    [Fact]
    [Trait("Scenario", "SC-001-15")]
    public async Task SC_001_15_PriNedostupnejEvidenciiRezervaciiSaRezervaciaNeulozi()
    {
        // Given evidencia rezervácií je dočasne nedostupná
        _factory.Store.Available = false;

        var response = await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-05T13:00:00Z");

        // Then systém rezerváciu neuloží a uvedie dôvod „služba je dočasne nedostupná“
        await AssertProblemAsync(response, ProblemCatalog.ServiceUnavailable);

        // And po obnovení dostupnosti je termín 5. 12. 2026 14:00–15:00 voľný
        _factory.Store.Available = true;
        Assert.Equal(0, _factory.Db.CountInSlot(DateTimeOffset.Parse("2026-12-05T13:00:00Z")));
        await AssertCreatedAsync(await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-05T13:00:00Z"));
    }

    [Fact]
    [Trait("Scenario", "SC-001-16")]
    public async Task SC_001_16_PriViacerychPoruseniachSystemUvedieIbaPrvyDovod()
    {
        // Peter nie je v evidencii; začiatok 25. 11. 2026 09:00 (minulosť aj pred otvorením); poznámka 501 znakov
        var response = await PostAsync(_client, TestTokens.Jana, Peter, "2026-11-25T08:00:00Z", new string('x', 501));

        await AssertProblemAsync(response, ProblemCatalog.ClientNotFound);
        Assert.Equal(0, _factory.Db.CountAll());
    }
}
