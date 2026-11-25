// ART-SoftverIntelliWell – prototyp FR1 (UC01 Vytvor rezerváciu), API-Reservations 0.2.1 (approved).
// Spustenie lokálne cez `dotnet run` (ADR-008 doplnenie); kontajner ani CI nie sú v rozsahu.
using IntelliWell.Hosting;
using IntelliWell.ModulRezervaciiAKlientov;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddIntelliWellAuthentication();
builder.Services.AddReservationModule();

var app = builder.Build();

await ClientSeeder.InitializeStorageAsync(app.Services);

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapReservationEndpoints();

await app.RunAsync();

/// <summary>Vstupný bod; partial kvôli WebApplicationFactory v testoch.</summary>
public partial class Program;
