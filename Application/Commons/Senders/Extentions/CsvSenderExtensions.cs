using System.IO.Abstractions;
using Application.Commons.Adapters;
using static Application.Commons.Senders.DataSender;

namespace Application.Commons.Senders.Extensions;

public static class CsvSenderExtensions
{
    public static IDataSenderFromResult FromCsvFileAsync(this IDataSender sender, string path, IFileSystem filesystem, CsvConfiguration configuration)
    {
        return CreateIDataSenderFromFunc(sender, path, filesystem, configuration);
    }
    public static IDataSenderFromResult FromCsvFileAsync(this IDataSender sender, string path, IFileSystem filesystem)
    {
        return CreateIDataSenderFromFunc(sender, path, filesystem, new CsvConfiguration());
    }

    private static IDataSenderFromResult CreateIDataSenderFromFunc(IDataSender sender, string path, IFileSystem filesystem, CsvConfiguration configuration)
    {
        if(!filesystem.Directory.Exists(path)) throw new DirectoryNotFoundException($"Diretorio {path} não encontrado.");

        return sender.FromFunc(async () =>
        {
            using var stream = filesystem.File.OpenText(path);
            var reader = new CsvReader(stream, configuration);
            return await reader.ReadAllAsync();
        }); 
    }
} 