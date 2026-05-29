using CurrencyReconciliation.Clients;
using CurrencyReconciliation.Services;
using Microsoft.Extensions.DependencyInjection;

public class TestFixture
{
    public ServiceProvider ServiceProvider { get; }

    public TestFixture()
    {
        var services = new ServiceCollection();

        services.AddHttpClient(CbrClient.ClientName, client =>
        {
            client.BaseAddress = new Uri("https://cbr.ru/scripts/XML_daily.asp");
        });

        services.AddHttpClient("External", client =>
        {
            client.BaseAddress = new Uri("https://open.er-api.com/v6/latest/USD");
        });

        
        services.AddTransient<ICbrClient, CbrClient>();
        services.AddTransient<IExternalRateClient, ExternalRateClient>();

        
        services.AddTransient<ICurrencyReconciliationService, CurrencyReconciliationService>();

        ServiceProvider = services.BuildServiceProvider();
    }
}