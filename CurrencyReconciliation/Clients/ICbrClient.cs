using CurrencyReconciliation.Models;

namespace CurrencyReconciliation.Clients;

public interface ICbrClient
{
    Task<ValCurs> GetRatesAsync();
}
