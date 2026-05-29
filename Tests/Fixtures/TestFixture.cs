using Microsoft.Extensions.DependencyInjection;

public class TestFixture
{
    public IHttpClientFactory HttpClientFactory { get; }

    public TestFixture()
    {
        var services = new ServiceCollection();

        services.AddHttpClient();

        var provider = services.BuildServiceProvider();

        HttpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    }
}