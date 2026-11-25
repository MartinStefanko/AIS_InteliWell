using System.Net;
using System.Text.RegularExpressions;
using IntelliWell.Tests.Infrastructure;

namespace IntelliWell.Tests.Ui;

/// <summary>
/// FR1-UI / US-002 – automatické testy bez prehliadača (DR-0915, úloha 0010): servírovanie stránky z wwwroot
/// a statická kontrola obsahu. Správanie stránky overuje `node --test` (logika, DR-1001) a ručný protokol
/// reports/manual-test-protocol-US-002.md. API časť scenárov pokrývajú testy US-001.
/// </summary>
[Trait("Requirement", "FR1-UI")]
public sealed partial class UiStaticPageTests : IAsyncDisposable
{
    private const string SeedClientId = "7c9e6679-7425-40de-944b-e07fc1f90ae7";

    private static readonly string[] JavaScriptFiles = ["/js/app.js", "/js/messages.js", "/js/bratislava-time.js"];

    private readonly IntelliWellFactory _factory = new();
    private readonly HttpClient _client;

    public UiStaticPageTests()
    {
        _client = _factory.CreateClient();
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    private async Task<string> GetTextAsync(string path, string expectedMediaType)
    {
        using var response = await _client.GetAsync(path, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expectedMediaType, response.Content.Headers.ContentType?.MediaType);
        return await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
    }

    private async Task<string> GetAllJavaScriptAsync()
    {
        var parts = new List<string>();
        foreach (var file in JavaScriptFiles)
        {
            parts.Add(await GetTextAsync(file, "text/javascript"));
        }

        return string.Join("\n", parts);
    }

    // --- Servírovanie stránky (ADR-010, DR-0913) ---

    [Theory]
    [Trait("Scenario", "SC-002-06")]
    [InlineData("/")]
    [InlineData("/index.html")]
    public async Task Stranka_SaOtvoriBezPrihlasenia(string path)
    {
        // Žiadna hlavička Authorization – stránku smie otvoriť aj neprihlásená osoba (DR-0913).
        var html = await GetTextAsync(path, "text/html");

        Assert.Contains("<html lang=\"sk\">", html);
        Assert.Contains("<script type=\"module\" src=\"js/app.js\"></script>", html);
    }

    [Theory]
    [Trait("Scenario", "SC-002-06")]
    [InlineData("/js/app.js")]
    [InlineData("/js/messages.js")]
    [InlineData("/js/bratislava-time.js")]
    public async Task SkriptyStranky_SaServirujuBezPrihlasenia(string path)
    {
        var script = await GetTextAsync(path, "text/javascript");

        Assert.NotEmpty(script);
    }

    // --- SC-002-01: predvolený klient a volanie API ---

    [Fact]
    [Trait("Scenario", "SC-002-01")]
    public async Task Stranka_PonukaSeedKlientkuAkoPredvolenu()
    {
        var html = await GetTextAsync("/", "text/html");

        Assert.Contains($"value=\"{SeedClientId}\"", html);
    }

    [Fact]
    [Trait("Scenario", "SC-002-01")]
    public async Task Skript_PosielaRezervaciuNaApiContractuSBearerTokenom()
    {
        var app = await GetTextAsync("/js/app.js", "text/javascript");

        Assert.Contains("\"/api/v1/reservations\"", app);
        Assert.Contains("method: \"POST\"", app);
        Assert.Contains("`Bearer ${token}`", app);
    }

    [Fact]
    [Trait("Scenario", "SC-002-01")]
    public async Task Skript_ObsahujeSchvaleneZneniePotvrdenia()
    {
        var messages = await GetTextAsync("/js/messages.js", "text/javascript");

        Assert.Contains("`Rezervácia je uložená na termín ${localRange}.`", messages);
    }

    // --- SC-002-04, SC-002-05: hlášky podľa type (DR-0912) ---

    [Theory]
    [Trait("Scenario", "SC-002-05")]
    [InlineData("urn:intelliwell:problem:unauthenticated", "Nie ste prihlásená. Vložte platný prihlasovací token.")]
    [InlineData("urn:intelliwell:problem:forbidden", "Nemáte oprávnenie vytvárať rezervácie. Je potrebná rola správcu návštev.")]
    [InlineData("urn:intelliwell:problem:invalid-request", "Údaje rezervácie nie sú úplné alebo majú nesprávny tvar. Skontrolujte klienta a začiatok.")]
    [InlineData("urn:intelliwell:problem:service-unavailable", "Evidencia rezervácií je dočasne nedostupná. Rezervácia sa neuložila, skúste to neskôr.")]
    [InlineData("urn:intelliwell:problem:client-not-found", "Klient s týmto identifikátorom nie je v evidencii klientov.")]
    [InlineData("urn:intelliwell:problem:start-not-in-future", "Začiatok termínu musí byť v budúcnosti.")]
    [InlineData("urn:intelliwell:problem:outside-opening-hours", "Termín musí celý ležať v otváracích hodinách 10:00–22:00.")]
    [InlineData("urn:intelliwell:problem:note-too-long", "Poznámka môže mať najviac 500 znakov.")]
    [InlineData("urn:intelliwell:problem:slot-occupied", "Termín je obsadený. Rezervácia sa neuložila.")]
    public async Task Skript_MaSchvalenuHlaskuPreTypOdmietnutia(string problemType, string message)
    {
        var messages = await GetTextAsync("/js/messages.js", "text/javascript");

        Assert.Contains($"\"{problemType}\":", messages);
        Assert.Contains($"\"{message}\"", messages);
    }

    [Fact]
    [Trait("Scenario", "SC-002-04")]
    [Trait("Scenario", "SC-002-05")]
    public async Task Skript_PokryvaPresneTypyProblemovZApprovedContractu()
    {
        var contractTypes = ContractProblemTypes();
        var messages = await GetTextAsync("/js/messages.js", "text/javascript");
        var uiTypes = UiProblemType().Matches(messages).Select(m => m.Groups[1].Value).ToHashSet();

        Assert.Equal(9, contractTypes.Count);
        Assert.Equal(contractTypes.Order(), uiTypes.Order());
    }

    // --- SC-002-07: bez automatického opakovania (statický dôkaz; hlavný dôkaz je ručný protokol) ---

    [Fact]
    [Trait("Scenario", "SC-002-07")]
    public async Task Skript_PosielaPoziadavkuIbaRazBezOpakovania()
    {
        var scripts = await GetAllJavaScriptAsync();

        Assert.Single(FetchCall().Matches(scripts));
        Assert.DoesNotContain("setTimeout", scripts);
        Assert.DoesNotContain("setInterval", scripts);
    }

    // --- SC-002-08: CON-003 – nič do konzoly prehliadača ---

    [Fact]
    [Trait("Scenario", "SC-002-08")]
    public async Task StrankaASkripty_NezapisujuDoKonzolyPrehliadaca()
    {
        var content = await GetAllJavaScriptAsync() + await GetTextAsync("/", "text/html");

        Assert.DoesNotContain("console.", content);
    }

    // --- SC-002-09, SC-002-10: hlášky DR-0911, DR-0912, DR-1002 ---

    [Theory]
    [Trait("Scenario", "SC-002-09")]
    [InlineData("Zadaný čas pri zmene času v Europe/Bratislava neexistuje. Zadajte iný čas.")]
    [InlineData("Zadaný čas je pri zmene času v Europe/Bratislava dvojznačný. Zadajte iný čas.")]
    public async Task Skript_MaHlaskuPreCasPriZmeneCasu(string message)
    {
        var messages = await GetTextAsync("/js/messages.js", "text/javascript");

        Assert.Contains($"\"{message}\"", messages);
    }

    [Fact]
    [Trait("Scenario", "SC-002-10")]
    public async Task Skript_MaHlaskuPreServerKtoryNeodpovie()
    {
        var messages = await GetTextAsync("/js/messages.js", "text/javascript");

        Assert.Contains(
            "\"Server neodpovedá. Nie je isté, či sa rezervácia uložila – skontrolujte to pred ďalším pokusom.\"",
            messages);
    }

    [Fact]
    [Trait("Scenario", "SC-002-02")]
    [Trait("Scenario", "SC-002-03")]
    public async Task Skript_PrepocitavaCasVzdyPodlaEuropeBratislava()
    {
        var time = await GetTextAsync("/js/bratislava-time.js", "text/javascript");

        Assert.Contains("timeZone: TIME_ZONE", time);
        Assert.Contains("export const TIME_ZONE = \"Europe/Bratislava\";", time);
    }

    // --- Security ADR-010 a DR-0910 ---

    [Theory]
    [Trait("Decision", "ADR-010")]
    [InlineData("innerHTML")]
    [InlineData("outerHTML")]
    [InlineData("insertAdjacentHTML")]
    [InlineData("document.write")]
    [InlineData("eval(")]
    public async Task Skripty_NevkladajuTextAkoHtml(string forbidden)
    {
        var scripts = await GetAllJavaScriptAsync();

        Assert.DoesNotContain(forbidden, scripts);
    }

    [Theory]
    [Trait("Decision", "DR-0910")]
    [InlineData("localStorage")]
    [InlineData("sessionStorage")]
    [InlineData("indexedDB")]
    [InlineData("document.cookie")]
    public async Task Skripty_NeukladajuTokenMimoPamateStranky(string forbidden)
    {
        var scripts = await GetAllJavaScriptAsync();

        Assert.DoesNotContain(forbidden, scripts);
    }

    [Fact]
    [Trait("Decision", "ADR-010")]
    public async Task Stranka_NenacitavaExterneZdroje()
    {
        var content = await GetTextAsync("/", "text/html") + await GetAllJavaScriptAsync();

        Assert.DoesNotMatch(ExternalResource(), content);
    }

    // --- Pomocné ---

    /// <summary>Enum `type` v ProblemDetails z approved contractu (spec/contracts/approved/openapi-reservations.yaml).</summary>
    private static HashSet<string> ContractProblemTypes()
    {
        var contract = File.ReadAllText(Path.Combine(RepositoryRoot(), "spec", "contracts", "approved", "openapi-reservations.yaml"));
        return ContractEnumItem().Matches(contract).Select(m => m.Groups[1].Value).ToHashSet();
    }

    private static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "IntelliWell.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? throw new InvalidOperationException("Koreň repozitára (IntelliWell.slnx) sa nenašiel.");
    }

    [GeneratedRegex(@"^\s+- (urn:intelliwell:problem:[a-z-]+)\s*$", RegexOptions.Multiline)]
    private static partial Regex ContractEnumItem();

    [GeneratedRegex(@"""(urn:intelliwell:problem:[a-z-]+)"":")]
    private static partial Regex UiProblemType();

    [GeneratedRegex(@"\bfetch\(")]
    private static partial Regex FetchCall();

    [GeneratedRegex(@"(src|href)\s*=\s*[""']?(https?:)?//|\bimport\s+[^;]*from\s+[""']https?:")]
    private static partial Regex ExternalResource();
}
