using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using IntelliWell.Hosting;

namespace IntelliWell.Tests.Infrastructure;

/// <summary>Volanie POST /api/v1/reservations a asserty tvaru odpovedí podľa approved contractu 0.2.1.</summary>
public static class ReservationsApi
{
    public const string Path = "/api/v1/reservations";

    /// <summary>Klientka „Eva Nováková“ – v seed dátach (config/seed-clients.json).</summary>
    public static readonly Guid Eva = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7");

    /// <summary>Klient „Peter Horváth“ – nie je v evidencii klientov.</summary>
    public static readonly Guid Peter = Guid.Parse("0b6f2a5e-3c1d-4f8a-9e2b-5d4c3b2a1f00");

    public static CancellationToken Ct => TestContext.Current.CancellationToken;

    public static string Body(Guid clientId, string startTime, string? note = null) =>
        note is null
            ? JsonSerializer.Serialize(new { clientId, startTime })
            : JsonSerializer.Serialize(new { clientId, startTime, note });

    public static Task<HttpResponseMessage> PostAsync(
        HttpClient client, string? token, Guid clientId, string startTime, string? note = null) =>
        PostRawAsync(client, token, Body(clientId, startTime, note));

    public static Task<HttpResponseMessage> PostRawAsync(
        HttpClient client, string? token, string body, string? correlationId = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, Path)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json"),
        };
        if (token is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        if (correlationId is not null)
        {
            request.Headers.TryAddWithoutValidation(CorrelationIdMiddleware.HeaderName, correlationId);
        }
        return client.SendAsync(request, Ct);
    }

    /// <summary>201 + telo Reservation; vráti JSON koreň.</summary>
    public static async Task<JsonElement> AssertCreatedAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync(Ct);
        Assert.True(response.StatusCode == HttpStatusCode.Created, $"Očakávané 201, prišlo {(int)response.StatusCode}: {text}");
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        AssertCorrelationHeader(response);
        return JsonDocument.Parse(text).RootElement.Clone();
    }

    /// <summary>Problem details podľa BR-012 (type URN, title = reason, status = HTTP kód, instance).</summary>
    public static async Task<JsonElement> AssertProblemAsync(HttpResponseMessage response, ProblemKind expected)
    {
        var text = await response.Content.ReadAsStringAsync(Ct);
        Assert.True((int)response.StatusCode == expected.Status,
            $"Očakávané {expected.Status} {expected.Code}, prišlo {(int)response.StatusCode}: {text}");
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        AssertCorrelationHeader(response);

        var root = JsonDocument.Parse(text).RootElement.Clone();
        Assert.Equal("urn:intelliwell:problem:" + expected.Code, root.GetProperty("type").GetString());
        Assert.Equal(expected.Title, root.GetProperty("title").GetString());
        Assert.Equal(expected.Status, root.GetProperty("status").GetInt32());
        Assert.Equal(Path, root.GetProperty("instance").GetString());
        return root;
    }

    public static string AssertCorrelationHeader(HttpResponseMessage response)
    {
        Assert.True(response.Headers.TryGetValues(CorrelationIdMiddleware.HeaderName, out var values),
            "Odpoveď nemá hlavičku X-Correlation-ID.");
        var value = Assert.Single(values);
        Assert.True(CorrelationIdMiddleware.IsValid(value), $"Neplatné X-Correlation-ID: '{value}'.");
        return value;
    }
}
