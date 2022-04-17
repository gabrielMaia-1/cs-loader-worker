using System.IO.Abstractions;
using Application.Commons.Adapters;
using Application.Commons.Senders;
using Loader;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<CsvLoaderWorker>();
        services.AddTransient<HttpClient>();
        services.AddTransient<IFileSystem, FileSystem>();
        services.AddTransient<ICsvAdapter, CsvAdapter>();
        services.AddTransient<IJsonAdapter, JsonAdapter>();
        services.AddTransient<DataTableSender>();
    })
    .Build();

await host.RunAsync();
