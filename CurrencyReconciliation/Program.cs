using CurrencyReconciliation.Clients;
using CurrencyReconciliation.Services;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddHttpClient(CbrClient.ClientName, client =>
{
    client.BaseAddress = new Uri("https://www.cbr.ru");
});

services.AddHttpClient(ExternalRateClient.ClientName, client =>
{
    client.BaseAddress = new Uri("https://open.er-api.com");
});

services.AddTransient<ICbrClient, CbrClient>();
services.AddTransient<IExternalRateClient, ExternalRateClient>();
services.AddTransient<ICurrencyReconciliationService, CurrencyReconciliationService>();

await using var provider = services.BuildServiceProvider();

var cbrClient = provider.GetRequiredService<ICbrClient>();
var externalClient = provider.GetRequiredService<IExternalRateClient>();

var cbrRates = await cbrClient.GetRatesAsync();
var externalRates = await externalClient.GetRatesAsync();

Console.WriteLine($"CBR currencies: {cbrRates.Valutes.Count}");
Console.WriteLine($"External currencies: {externalRates.Rates.Count}");

Console.WriteLine($"USD -> RUB (External): {externalRates.Rates["RUB"]}");
