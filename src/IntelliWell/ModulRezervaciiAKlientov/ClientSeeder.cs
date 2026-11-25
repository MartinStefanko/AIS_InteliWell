using System.Text.Json;

namespace IntelliWell.ModulRezervaciiAKlientov;

/// <summary>
/// Pri štarte vytvorí úložisko a naplní DO-EvidenciaKlientov seed dátami (DR-03; DR-I05 / ADR-009 doplnenie):
/// súbor config/seed-clients.json obsahuje iba UUID klientov, vkladá sa INSERT OR IGNORE.
/// </summary>
public static class ClientSeeder
{
    public static async Task InitializeStorageAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        var configuration = services.GetRequiredService<IConfiguration>();
        var fileName = configuration["SeedData:ClientsFile"] ?? "seed-clients.json";
        var path = Path.IsPathRooted(fileName) ? fileName : Path.Combine(AppContext.BaseDirectory, fileName);

        var clientIds = await ReadClientIdsAsync(path, cancellationToken);
        await services.GetRequiredService<SqliteReservationStore>().InitializeAsync(clientIds, cancellationToken);
    }

    private static async Task<IReadOnlyList<Guid>> ReadClientIdsAsync(string path, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(path);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        return document.RootElement.GetProperty("clients")
            .EnumerateArray()
            .Select(element => Guid.ParseExact(element.GetString()!, "D"))
            .ToList();
    }
}
