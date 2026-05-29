using CurrencyReconciliation.Clients;
using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyReconciliation.Services
{
    public interface ICurrencyReconciliationService
    {
        Task<List<ReconciliationResult>> ReconcileRatesAsync();
    }
    public class CurrencyReconciliationService : ICurrencyReconciliationService
    {
        private readonly CbrClient _cbr;
        private readonly ExternalRateClient _external;

        private static readonly string[] Currencies = ["USD", "EUR", "GBP"];

        public CurrencyReconciliationService(
            CbrClient cbr,
            ExternalRateClient external)
        {
            _cbr = cbr;
            _external = external;
        }

        public async Task<List<ReconciliationResult>> ReconcileRatesAsync()
        {            
            var cbrTask = _cbr.GetRatesAsync();
            var extTask = _external.GetRatesAsync();

            await Task.WhenAll(cbrTask, extTask);

            var cbr = await cbrTask;
            var ext = await extTask;

            var result = new List<ReconciliationResult>();

            foreach (var code in Currencies)
            {
                var cbrRate = cbr.Valutes
                    .FirstOrDefault(v => v.CharCode == code)
                    ?.UnitRate
                    ?? throw new Exception($"Missing CBR rate for {code}");

                if (!ext.Rates.TryGetValue(code, out var usdToCurrency))
                    throw new Exception($"Missing external rate for {code}");

                if (!ext.Rates.TryGetValue("RUB", out var usdToRub))
                    throw new Exception("Missing RUB rate");

                var apiRate = (1 / usdToCurrency) * usdToRub;

                var spread = Math.Abs(cbrRate - apiRate)
                             / ((cbrRate + apiRate) / 2)
                             * 100;

                result.Add(new ReconciliationResult
                {
                    CurrencyCode = code,
                    CbrRate = cbrRate,
                    ApiRate = apiRate,
                    SpreadPercent = spread
                });
            }

            return result;
        }
    }
}
