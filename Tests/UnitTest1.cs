using CurrencyReconciliation.Clients;
using CurrencyReconciliation.Services;
using System.Diagnostics;

namespace Tests
{
    public class CurrencyReconciliationTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        private static readonly string[] ExpectedCurrencies = ["USD", "EUR", "GBP"];

        public CurrencyReconciliationTests(TestFixture fixture)
        {
            _fixture = fixture;
        }

        private CurrencyReconciliationService CreateService()
        {
            var httpClient = _fixture.HttpClientFactory.CreateClient();

            var cbr = new CbrClient(httpClient);
            var external = new ExternalRateClient(httpClient);

            return new CurrencyReconciliationService(cbr, external);
        }

        private CurrencyReconciliationService Service => CreateService();

        [Fact]
        public async Task Spread_ShouldBeLessThanFivePercent()
        {
            var sw = Stopwatch.StartNew();

            var result = await Service.ReconcileRatesAsync();

            sw.Stop();

            Assert.True(sw.ElapsedMilliseconds < 15000);

            foreach (var r in result)
            {
                Assert.True(r.SpreadPercent <= 5,
                    $"Currency: {r.CurrencyCode}, CBR: {r.CbrRate}, API: {r.ApiRate}, Spread: {r.SpreadPercent}");
            }
        }

        [Fact]
        public async Task ShouldContainOnlyUsdEurGbp()
        {
            var sw = Stopwatch.StartNew();

            var result = await Service.ReconcileRatesAsync();

            sw.Stop();

            Assert.True(sw.ElapsedMilliseconds < 15000);

            var actual = result.Select(x => x.CurrencyCode)
                               .OrderBy(x => x)
                               .ToArray();

            var expected = ExpectedCurrencies.OrderBy(x => x).ToArray();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task Rates_ShouldBeGreaterThanZero()
        {
            var sw = Stopwatch.StartNew();

            var result = await Service.ReconcileRatesAsync();

            sw.Stop();

            Assert.True(sw.ElapsedMilliseconds < 15000);

            Assert.All(result, r =>
            {
                Assert.True(r.CbrRate > 0);
                Assert.True(r.ApiRate > 0);
            });
        }

        [Fact]
        public async Task ParallelCalls_ShouldBeConsistent()
        {
            var sw = Stopwatch.StartNew();

            var results = await ParallelTestHelper.RunParallelAsync(Service, 10);

            sw.Stop();

            Assert.True(sw.ElapsedMilliseconds < 15000);

            var first = results[0];

            foreach (var r in results)
            {
                Assert.Equal(first.Count, r.Count);
                Assert.Equal(
                    first.Select(x => x.CurrencyCode),
                    r.Select(x => x.CurrencyCode)
                );
            }
        }
    }
}