using System.IO.Abstractions;
using Application.Commons.Adapters;
using Application.Commons.Interfaces;
using Application.Commons.Senders;
using Loader;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<CsvLoaderWorker>();
        services.AddTransient<HttpClient>();
        services.AddTransient<IFileSystem, FileSystem>();
        services.AddTransient<ICsvReader, CsvReader>();
        services.AddTransient<IJsonAdapter, JsonAdapter>();
        services.AddTransient<DataTableSender>();
    })
    .Build();

await host.RunAsync();
