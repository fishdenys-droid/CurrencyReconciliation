using System.Text.Json;
using CurrencyReconciliation.Models;

namespace CurrencyReconciliation.Clients;

public class ExternalRateClient : IExternalRateClient
{
    public const string ClientName = nameof(ExternalRateClient);

    private readonly IHttpClientFactory _httpClientFactory;

    public ExternalRateClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ExternalRatesResponse> GetRatesAsync()
    {
        var httpClient = _httpClientFactory.CreateClient(ClientName);

        var json = await httpClient.GetStringAsync(
            "https://open.er-api.com/v6/latest/USD"
        );

        var result = JsonSerializer.Deserialize<ExternalRatesResponse>(json);

        if (result is null)
            throw new InvalidOperationException("Failed to parse external API");

        if (result.Result != "success")
            throw new InvalidOperationException("External API error");

        return result;
    }
}
