using CurrencyReconciliation.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Tests;

public class CurrencyReconciliationTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    private static readonly string[] ExpectedCurrencies = ["USD", "EUR", "GBP"];
    private const int MaxExecutionMs = 15000;

    public CurrencyReconciliationTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    private ICurrencyReconciliationService CreateService()
    {
        return _fixture.ServiceProvider
            .GetRequiredService<ICurrencyReconciliationService>();
    }

    private async Task<(List<ReconciliationResult> Result, long Ms)> ExecuteAsync()
    {
        var service = CreateService();

        var sw = Stopwatch.StartNew();
        var result = await service.ReconcileRatesAsync();
        sw.Stop();

        return (result, sw.ElapsedMilliseconds);
    }

    [Fact]
    public async Task Spread_ShouldNotExceedFivePercent()
    {
        var (result, ms) = await ExecuteAsync();

        Assert.True(ms < MaxExecutionMs);

        Assert.All(result, r =>
        {
            Assert.True(
                r.SpreadPercent <= 5,
                $"Currency: {r.CurrencyCode}, CBR: {r.CbrRate}, API: {r.ApiRate}, Spread: {r.SpreadPercent:F2}%"
            );
        });
    }

    [Fact]
    public async Task ShouldContainOnlyExpectedCurrencies()
    {
        var (result, ms) = await ExecuteAsync();

        Assert.True(ms < MaxExecutionMs);

        var actual = result
            .Select(x => x.CurrencyCode)
            .OrderBy(x => x)
            .ToList();

        Assert.Equal(ExpectedCurrencies.OrderBy(x => x), actual);
    }

    [Fact]
    public async Task Rates_ShouldBeGreaterThanZero()
    {
        var (result, ms) = await ExecuteAsync();

        Assert.True(ms < MaxExecutionMs);

        Assert.All(result, r =>
        {
            Assert.True(r.CbrRate > 0);
            Assert.True(r.ApiRate > 0);
        });
    }

    [Fact]
    public async Task ParallelCalls_ShouldBeConsistent()
    {
        var service = CreateService();

        var sw = Stopwatch.StartNew();

        var tasks = Enumerable.Range(0, 10)
            .Select(_ => service.ReconcileRatesAsync());

        var results = await Task.WhenAll(tasks);

        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < MaxExecutionMs);

        var first = results[0];

        Assert.All(results, r =>
        {
            Assert.Equal(first.Count, r.Count);

            Assert.Equal(
                first.Select(x => x.CurrencyCode),
                r.Select(x => x.CurrencyCode)
            );

            for (int i = 0; i < r.Count; i++)
            {
                Assert.Equal(first[i].SpreadPercent, r[i].SpreadPercent);
            }
        });
    }
}