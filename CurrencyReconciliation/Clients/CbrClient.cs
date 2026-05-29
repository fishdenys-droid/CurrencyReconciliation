using System.Text;
using System.Xml.Serialization;
using CurrencyReconciliation.Models;

namespace CurrencyReconciliation.Clients;

public class CbrClient : ICbrClient
{
    public const string ClientName = nameof(CbrClient);

    private readonly IHttpClientFactory _httpClientFactory;

    public CbrClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ValCurs> GetRatesAsync()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var httpClient = _httpClientFactory.CreateClient(ClientName);

        using var response = await httpClient.GetAsync("/scripts/XML_daily.asp");
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();

        var serializer = new XmlSerializer(typeof(ValCurs));

        var result = serializer.Deserialize(stream) as ValCurs;

        if (result is null)
            throw new InvalidOperationException("Не удалось распарсить ответ ЦБ");

        return result;
    }
}
