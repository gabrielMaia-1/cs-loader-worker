using System.Data;
using System.IO.Abstractions;

namespace Application.Commons.Adapters;

public interface ICsvAdapter
{
    void AddLineToTable(string? line, DataTable table);
    DataTable CreateTableFromHeader(string? header);
    Task<DataTable> LoadTableAsync(string path);
}

public class CsvAdapter : ICsvAdapter
{
    private readonly IFileSystem fileSystem;

    public CsvAdapter(IFileSystem fs)
    {
        fileSystem = fs;
    }

    public async Task<DataTable> LoadTableAsync(string path)
    {
        using var stream = fileSystem.File.OpenText(path);

        var header = await stream.ReadLineAsync();
        var table = CreateTableFromHeader(header);

        while (!stream.EndOfStream)
        {
            AddLineToTable(await stream.ReadLineAsync(), table);
        }

        return table;
    }

    public void AddLineToTable(string? line, DataTable table)
    {
        if (string.IsNullOrWhiteSpace(line)) return;
        var row = table.NewRow();
        var lineItens = line.Split(';');

        for (var i = 0; i < lineItens.Length; i++)
        {
            row[i] = lineItens[i];
        }

        table.Rows.Add(row);
    }

    public DataTable CreateTableFromHeader(string? header)
    {
        if (string.IsNullOrWhiteSpace(header)) return new DataTable();
        var table = new DataTable();
        var columnNames = header.Split(';');

        foreach (var column in columnNames)
        {
            table.Columns.Add(new DataColumn(column));
        }

        return table;
    }

}