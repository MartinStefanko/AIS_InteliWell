using System.Net;
using System.Text.Json;
using IntelliWell.Hosting;
using IntelliWell.Tests.Infrastructure;
using Microsoft.Data.Sqlite;
using static IntelliWell.Tests.Infrastructure.ReservationsApi;

namespace IntelliWell.Tests.Contract;

/// <summary>
/// Contract tests voči spec/contracts/approved/openapi-reservations.yaml (API-Reservations 0.2.1, approved)
/// a contract-mapping.yaml – prípady, ktoré nemajú vlastný scenár v US-001.feature.
/// </summary>
[Trait("Contract", "API-Reservations 0.2.1")]
public sealed class CreateReservationContractTests : IAsyncDisposable
{
    private readonly IntelliWellFactory _factory = new();
    private readonly HttpClient _client;

    public CreateReservationContractTests()
    {
        _client = _factory.CreateClient();
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    // --- Pomenované príklady contractu bez scenára v US-001 ---

    [Theory]
    [Trait("ContractExample", "unauthenticated-missing-sub")]
    [InlineData("bez claimu sub")]
    [InlineData("s prázdnym claimom sub")]
    public async Task UnauthenticatedMissingSub_PlatnyTokenSRolouBezNeprazdnehoSub_Vrati401(string variant)
    {
        var token = variant == "bez claimu sub" ? TestTokens.WithoutSub : TestTokens.WithEmptySub;

        var response = await PostAsync(_client, token, Eva, "2026-12-01T19:00:00Z");

        await AssertProblemAsync(response, ProblemCatalog.Unauthenticated);
        Assert.Equal(0, _factory.Db.CountAll());
    }

    [Fact]
    [Trait("ContractExample", "invalid-request-time-offset")]
    public async Task InvalidRequestTimeOffset_CasSPosunomNamiestoZ_Vrati400()
    {
        var response = await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-01T14:00:00+01:00");

        await AssertProblemAsync(response, ProblemCatalog.InvalidRequest);
        Assert.Equal(0, _factory.Db.CountAll());
    }

    // --- 400 invalid-request: tvar vstupu (NFR1, BR-012 pozícia 3) ---

    [Theory]
    [InlineData("nie je JSON")]
    [InlineData("")]
    [InlineData("[]")]
    [InlineData("{}")]
    [InlineData("""{"startTime":"2026-12-01T13:00:00Z"}""")]
    [InlineData("""{"clientId":"7c9e6679-7425-40de-944b-e07fc1f90ae7"}""")]
    [InlineData("""{"clientId":"abc","startTime":"2026-12-01T13:00:00Z"}""")]
    [InlineData("""{"clientId":123,"startTime":"2026-12-01T13:00:00Z"}""")]
    [InlineData("""{"clientId":"7c9e6679-7425-40de-944b-e07fc1f90ae7","startTime":"1. 12. 2026 14:00"}""")]
    [InlineData("""{"clientId":"7c9e6679-7425-40de-944b-e07fc1f90ae7","startTime":"2026-12-01T13:00:00"}""")]
    [InlineData("""{"clientId":"7c9e6679-7425-40de-944b-e07fc1f90ae7","startTime":"2026-02-30T13:00:00Z"}""")]
    [InlineData("""{"clientId":"7c9e6679-7425-40de-944b-e07fc1f90ae7","startTime":"2026-12-01T13:00:00Z","note":42}""")]
    public async Task NeplatnyTvarVstupu_Vrati400(string body)
    {
        var response = await PostRawAsync(_client, TestTokens.Jana, body);

        await AssertProblemAsync(response, ProblemCatalog.InvalidRequest);
        Assert.Equal(0, _factory.Db.CountAll());
    }

    // --- 401 / 403 (BR-011, ADR-004, ADR-007) ---

    [Fact]
    public async Task TokenPodpisanyNeznamymKlucom_Vrati401()
    {
        var response = await PostAsync(_client, TestTokens.SignedWithUnknownKey, Eva, "2026-12-01T13:00:00Z");

        await AssertProblemAsync(response, ProblemCatalog.Unauthenticated);
        Assert.Equal("Bearer", response.Headers.WwwAuthenticate.ToString());
    }

    [Fact]
    public async Task TokenBezClaimuRole_Vrati403()
    {
        var response = await PostAsync(_client, TestTokens.WithoutRole, Eva, "2026-12-01T13:00:00Z");

        await AssertProblemAsync(response, ProblemCatalog.Forbidden);
        Assert.Equal(0, _factory.Db.CountAll());
    }

    // --- Poradie dôvodov BR-012 (pozície 5–9 v SC-001-16 a nižšie) ---

    [Fact]
    public async Task Poradie1_NeprihlasenyANeplatneTelo_Vrati401()
    {
        var response = await PostRawAsync(_client, token: null, "nie je JSON");
        await AssertProblemAsync(response, ProblemCatalog.Unauthenticated);
    }

    [Fact]
    public async Task Poradie2_InaRolaANeplatneTelo_Vrati403()
    {
        var response = await PostRawAsync(_client, TestTokens.Tomas, "nie je JSON");
        await AssertProblemAsync(response, ProblemCatalog.Forbidden);
    }

    [Fact]
    public async Task Poradie3_NeplatneTeloANedostupneUlozisko_Vrati400()
    {
        _factory.Store.Available = false;
        var response = await PostRawAsync(_client, TestTokens.Jana, "nie je JSON");
        await AssertProblemAsync(response, ProblemCatalog.InvalidRequest);
    }

    [Fact]
    public async Task Poradie4_NedostupneUloziskoANeexistujuciKlient_Vrati503()
    {
        _factory.Store.Available = false;
        var response = await PostAsync(_client, TestTokens.Jana, Peter, "2026-12-01T13:00:00Z");
        await AssertProblemAsync(response, ProblemCatalog.ServiceUnavailable);
    }

    [Fact]
    public async Task Poradie6_MinulostAMimoOtvaracichHodin_VratiStartNotInFuture()
    {
        // 25. 11. 2026 09:00 Europe/Bratislava – v minulosti aj pred otvorením
        var response = await PostAsync(_client, TestTokens.Jana, Eva, "2026-11-25T08:00:00Z");
        await AssertProblemAsync(response, ProblemCatalog.StartNotInFuture);
    }

    [Fact]
    public async Task Poradie7_MimoOtvaracichHodinADlhaPoznamka_VratiOutsideOpeningHours()
    {
        var response = await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-01T20:30:00Z", new string('x', 501));
        await AssertProblemAsync(response, ProblemCatalog.OutsideOpeningHours);
    }

    [Fact]
    public async Task Poradie8_DlhaPoznamkaAObsadenyTermin_VratiNoteTooLong()
    {
        await AssertCreatedAsync(await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-01T13:00:00Z"));

        var response = await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-01T13:00:00Z", new string('x', 501));
        await AssertProblemAsync(response, ProblemCatalog.NoteTooLong);
    }

    // --- QR-002 / DR-I04: reálna SqliteException (vypršanie zámku) → 503 bez uloženia ---

    [Fact]
    public async Task ZamknutaDatabaza_PoVyprsaniZamku_Vrati503()
    {
        await using var factory = new IntelliWellFactory(busyTimeoutSeconds: 1);
        using var client = factory.CreateClient(); // spustí host – schéma a seed sú vytvorené pred zamknutím

        await using var locker = new SqliteConnection(factory.ConnectionString);
        await locker.OpenAsync(Ct);
        await using (var begin = locker.CreateCommand())
        {
            begin.CommandText = "BEGIN EXCLUSIVE;";
            await begin.ExecuteNonQueryAsync(Ct);
        }

        var response = await PostAsync(client, TestTokens.Jana, Eva, "2026-12-01T13:00:00Z");
        await AssertProblemAsync(response, ProblemCatalog.ServiceUnavailable);

        await using (var rollback = locker.CreateCommand())
        {
            rollback.CommandText = "ROLLBACK;";
            await rollback.ExecuteNonQueryAsync(Ct);
        }
        Assert.Equal(0, factory.Db.CountAll());
    }

    // --- DR-28: operácia nie je idempotentná ---

    [Fact]
    public async Task OpakovanaRovnakaPoziadavka_PoUspechuVrati409()
    {
        await AssertCreatedAsync(await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-01T13:00:00Z", "prvá návšteva"));

        var repeated = await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-01T13:00:00Z", "prvá návšteva");

        await AssertProblemAsync(repeated, ProblemCatalog.SlotOccupied);
        Assert.Equal(1, _factory.Db.CountAll());
    }

    // --- BR-008 / DR-C08: dĺžka poznámky v Unicode code points ---

    [Fact]
    public async Task Poznamka500CodePointsMimoBmp_SaUlozi()
    {
        var note = string.Concat(Enumerable.Repeat("😀", 500)); // 500 code points, 1000 UTF-16 jednotiek

        var body = await AssertCreatedAsync(await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-01T13:00:00Z", note));

        Assert.Equal(note, body.GetProperty("note").GetString());
    }

    [Fact]
    public async Task Poznamka501CodePointsMimoBmp_Vrati422()
    {
        var note = string.Concat(Enumerable.Repeat("😀", 501));

        var response = await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-01T13:00:00Z", note);

        await AssertProblemAsync(response, ProblemCatalog.NoteTooLong);
    }

    // --- X-Correlation-ID (DR-31, DR-C05, DR-C09) ---

    [Fact]
    public async Task CorrelationId_PlatnaHodnotaSaVrati()
    {
        var response = await PostRawAsync(_client, TestTokens.Jana, Body(Eva, "2026-12-01T13:00:00Z"), "req-42_ABC:xyz");

        await AssertCreatedAsync(response);
        Assert.Equal("req-42_ABC:xyz", AssertCorrelationHeader(response));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("s medzerou")]
    [InlineData("129")]
    public async Task CorrelationId_ChybajucaAleboNeplatna_SystemVygenerujeUuidANeodmietne(string? value)
    {
        var correlationId = value == "129" ? new string('a', 129) : value;

        var response = await PostRawAsync(_client, TestTokens.Jana, Body(Eva, "2026-12-01T13:00:00Z"), correlationId);

        await AssertCreatedAsync(response);
        Assert.True(Guid.TryParseExact(AssertCorrelationHeader(response), "D", out _));
    }

    [Fact]
    public async Task CorrelationId_JeVKazdejOdpovediContractu()
    {
        var responses = new List<HttpResponseMessage>
        {
            await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-01T13:00:00Z"),         // 201
            await PostRawAsync(_client, TestTokens.Jana, "nie je JSON"),                     // 400
            await PostAsync(_client, token: null, Eva, "2026-12-01T13:00:00Z"),              // 401
            await PostAsync(_client, TestTokens.Tomas, Eva, "2026-12-01T13:00:00Z"),         // 403
            await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-01T13:00:00Z"),          // 409
            await PostAsync(_client, TestTokens.Jana, Peter, "2026-12-01T15:00:00Z"),        // 422
        };
        _factory.Store.Available = false;
        responses.Add(await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-05T13:00:00Z")); // 503

        Assert.Equal([201, 400, 401, 403, 409, 422, 503], responses.Select(r => (int)r.StatusCode));
        Assert.All(responses, r => AssertCorrelationHeader(r));
    }

    // --- Tvar odpovede 201 (schéma Reservation) ---

    [Fact]
    public async Task Odpoved201_MaPovinnePoliaReservation()
    {
        var body = await AssertCreatedAsync(await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-01T13:00:00.5Z", "pozn"));

        Assert.Equal(
            ["id", "clientId", "startTime", "endTime", "note", "status", "createdBy", "createdAt"],
            body.EnumerateObject().Select(p => p.Name));
        Assert.True(Guid.TryParseExact(body.GetProperty("id").GetString(), "D", out _));
        Assert.Equal("2026-12-01T13:00:00.5Z", body.GetProperty("startTime").GetString());
        Assert.Equal("2026-12-01T14:00:00.5Z", body.GetProperty("endTime").GetString());
        Assert.All(new[] { "startTime", "endTime", "createdAt" }, f => Assert.EndsWith("Z", body.GetProperty(f).GetString()));
    }

    // --- CON-003: obsah poznámky sa nezapisuje do aplikačného logu ---

    [Fact]
    public async Task Poznamka_NieJeVAplikacnomLoguAniVProblemDetails()
    {
        const string marker = "OSOBNY-UDAJ-alergia-na-levandulu";

        await AssertCreatedAsync(await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-01T13:00:00Z", marker));
        var rejected = await PostAsync(_client, TestTokens.Jana, Eva, "2026-12-01T15:00:00Z", marker + new string('x', 500));
        var problem = await AssertProblemAsync(rejected, ProblemCatalog.NoteTooLong);

        Assert.NotEmpty(_factory.Logs.Entries);
        Assert.DoesNotContain(_factory.Logs.Entries, entry => entry.Contains(marker, StringComparison.Ordinal));
        Assert.DoesNotContain(marker, problem.GetRawText());
    }
}
