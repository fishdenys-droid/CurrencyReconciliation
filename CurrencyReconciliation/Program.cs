using CurrencyReconciliation.Clients;

var httpClient = new HttpClient();

var cbrClient = new CbrClient(httpClient);
var externalClient = new ExternalRateClient(httpClient);

var cbrRates = await cbrClient.GetRatesAsync();
var externalRates = await externalClient.GetRatesAsync();

Console.WriteLine($"CBR currencies: {cbrRates.Valutes.Count}");
Console.WriteLine($"External currencies: {externalRates.Rates.Count}");

Console.WriteLine($"USD -> RUB (External): {externalRates.Rates["RUB"]}");