using Reveries.BookEnrichmentWorker;

public class Worker : BackgroundService
{
    private readonly BookEnrichmentRunner _runner;

    public Worker(BookEnrichmentRunner runner)
    {
        _runner = runner;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine("Enriching books...");
                await _runner.RunAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}