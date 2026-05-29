using CurrencyReconciliation.Models;

namespace CurrencyReconciliation.Clients;

public interface IExternalRateClient
{
    Task<ExternalRatesResponse> GetRatesAsync();
}
