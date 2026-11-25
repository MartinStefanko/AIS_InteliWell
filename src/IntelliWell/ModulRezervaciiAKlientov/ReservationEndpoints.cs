using System.Text.Json;
using IntelliWell.Hosting;

namespace IntelliWell.ModulRezervaciiAKlientov;

/// <summary>
/// HTTP rozhranie AS-SpravaRezervaciiAKlientov podľa approved contractu API-Reservations 0.2.1
/// (POST /api/v1/reservations, operationId createReservation).
/// </summary>
public static class ReservationEndpoints
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static IServiceCollection AddReservationModule(this IServiceCollection services)
    {
        services.AddSingleton<SqliteReservationStore>();
        services.AddSingleton<IReservationStore>(sp => sp.GetRequiredService<SqliteReservationStore>());
        services.AddScoped<CreateReservationService>();
        return services;
    }

    public static IEndpointRouteBuilder MapReservationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/v1/reservations", CreateReservationAsync)
            .WithName("createReservation")
            // BR-012 pozície 1–2: 401 / 403 rieši autentifikácia a autorizácia pred čítaním tela.
            .RequireAuthorization(IntelliWellAuthentication.SpravcaNavstevPolicy);
        return endpoints;
    }

    private static async Task<IResult> CreateReservationAsync(
        HttpContext context, CreateReservationService service, CancellationToken cancellationToken)
    {
        // BR-012 pozícia 3: tvar vstupu (400 invalid-request).
        var (command, error) = await CreateReservationRequestParser.ParseAsync(context.Request.Body, cancellationToken);
        if (command is null)
        {
            return Problems.Result(context, ProblemCatalog.InvalidRequest, error);
        }

        // Autor = claim sub (ADR-007, DR-C02); neprázdny sub zaručuje autentifikácia (DR-C12).
        var createdBy = context.User.FindFirst(IntelliWellAuthentication.SubjectClaim)!.Value;

        var result = await service.CreateAsync(command, createdBy, cancellationToken);
        return result.Reservation is { } reservation
            ? Results.Json(ReservationResponse.From(reservation), JsonOptions, "application/json", StatusCodes.Status201Created)
            : Problems.Result(context, result.Rejection!);
    }
}
