using System.Text.Json;
using CurrencyReconciliation.Models;

namespace CurrencyReconciliation.Clients;

public class ExternalRateClient
{
    private readonly HttpClient _httpClient;
    private const string Url = "https://open.er-api.com/v6/latest/USD";

    public ExternalRateClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ExternalRatesResponse> GetRatesAsync()
    {
        var json = await _httpClient.GetStringAsync(Url);

        var result = JsonSerializer.Deserialize<ExternalRatesResponse>(json);

        if (result is null)
            throw new InvalidOperationException("Failed to parse external API");

        if (result.Result != "success")
            throw new InvalidOperationException("The external API returned an error.");

        return result;
    }
}