using CurrencyReconciliation.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CurrencyReconciliation.Clients
{
    public class CbrClient
    {
        private readonly HttpClient _httpClient;
        private const string Url = "https://www.cbr.ru/scripts/XML_daily.asp";

        public CbrClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ValCurs> GetRatesAsync()
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using var response = await _httpClient.GetAsync(Url);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();

            var serializer = new XmlSerializer(typeof(ValCurs));

            var result = serializer.Deserialize(stream) as ValCurs;

            if (result is null)
                throw new InvalidOperationException("Не удалось распарсить ответ ЦБ");

            return result;
        }
    }
}
