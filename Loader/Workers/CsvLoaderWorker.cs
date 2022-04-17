using Application.Commons.Senders;

namespace Loader;

public class CsvLoaderWorker : BackgroundService
{
    private readonly ILogger<CsvLoaderWorker> _logger;
    private readonly DataTableSender _sender;
    private readonly IConfiguration _configuration;

    public CsvLoaderWorker(ILogger<CsvLoaderWorker> logger, DataTableSender sender, IConfiguration configuration)
    {
        _logger = logger;
        _sender = sender;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "{time}\n\tLoading File -> {path}\n\tTo {endpoint}",
                DateTimeOffset.Now,
                _configuration.GetSection("DataLoad:Path").Value,
                _configuration.GetSection("DataLoad:EndPoint").Value
            );
            try
            {
                await _sender.LoadData(_configuration.GetSection("DataLoad:Path").Value, HttpMethod.Post, _configuration.GetSection("DataLoad:EndPoint").Value);
            }
            catch (System.Exception)
            {
                
            }
            await Task.Delay(10000, stoppingToken);
        }
    }
}
