using IntelliWell.ModulRezervaciiAKlientov;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IntelliWell.Tests.Infrastructure;

/// <summary>
/// Testovací host IntelliWell: vlastná SQLite databáza pre každý test, „teraz“ = 25. 11. 2026 12:00 Europe/Bratislava
/// (Background US-001.feature), testovací podpisový kľúč JWT, zachytávanie logov a prepínač dostupnosti úložiska.
/// </summary>
public sealed class IntelliWellFactory : WebApplicationFactory<Program>
{
    /// <summary>Aktuálny čas 25. 11. 2026 12:00 Europe/Bratislava = 11:00Z.</summary>
    public static readonly DateTimeOffset Now = new(2026, 11, 25, 11, 0, 0, TimeSpan.Zero);

    private readonly string _dbPath;

    /// <param name="busyTimeoutSeconds">Busy timeout SQLite pripojenia (Default Timeout); predvolene 30 s.</param>
    public IntelliWellFactory(int busyTimeoutSeconds = 30)
    {
        var directory = Path.Combine(Path.GetTempPath(), "intelliwell-tests");
        Directory.CreateDirectory(directory);
        _dbPath = Path.Combine(directory, $"{Guid.NewGuid():N}.db");
        ConnectionString = $"Data Source={_dbPath};Pooling=False;Default Timeout={busyTimeoutSeconds}";
    }

    public string ConnectionString { get; }

    public ManualTimeProvider Clock { get; } = new(Now);

    public CapturingLoggerProvider Logs { get; } = new();

    public ReservationDbProbe Db => new(ConnectionString);

    public ToggleableReservationStore Store => (ToggleableReservationStore)Services.GetRequiredService<IReservationStore>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:Reservations", ConnectionString);
        builder.ConfigureLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Trace);
            logging.AddFilter(_ => true);
            logging.AddProvider(Logs);
        });
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<TimeProvider>(Clock);
            services.AddSingleton<IReservationStore>(sp =>
                new ToggleableReservationStore(sp.GetRequiredService<SqliteReservationStore>()));
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters.ValidIssuer = TestTokens.Issuer;
                options.TokenValidationParameters.ValidAudience = TestTokens.Audience;
                options.TokenValidationParameters.IssuerSigningKey = TestTokens.SigningKey;
            });
        });
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        SqliteConnection.ClearAllPools();
        try
        {
            File.Delete(_dbPath);
        }
        catch (IOException)
        {
            // Dočasný súbor v %TEMP% – ak je ešte zamknutý, zostane tam.
        }
    }
}
