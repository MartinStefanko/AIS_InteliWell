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
// UI FR1-UI (ADR-010): statická stránka z wwwroot, dostupná bez prihlásenia (DR-0913). Obsluhuje iba existujúce
// súbory vo wwwroot, preto /api/v1/reservations prejde ďalej bez zmeny (poradie 401/403 BR-012 ostáva).
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapReservationEndpoints();

await app.RunAsync();

/// <summary>Vstupný bod; partial kvôli WebApplicationFactory v testoch.</summary>
public partial class Program;
