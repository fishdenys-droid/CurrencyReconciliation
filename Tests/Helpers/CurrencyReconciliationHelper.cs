using CurrencyReconciliation.Services;

public static class ParallelTestHelper
{
    public static async Task<List<List<ReconciliationResult>>> RunParallelAsync(
        ICurrencyReconciliationService service,
        int count)
    {
        var tasks = new List<Task<List<ReconciliationResult>>>();

        for (int i = 0; i < count; i++)
        {
            tasks.Add(service.ReconcileRatesAsync());
        }

        var results = await Task.WhenAll(tasks);

        return results.ToList();
    }
}