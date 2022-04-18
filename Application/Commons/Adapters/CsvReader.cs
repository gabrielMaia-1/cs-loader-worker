using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Application.Commons.Interfaces;

namespace Application.Commons.Adapters;

public class CsvReader : ICsvReader
{
    private readonly IFileSystem fileSystem;
    private readonly CsvConfiguration configuration;

    public CsvReader(IFileSystem fs, CsvConfiguration configuration)
    {
        fileSystem = fs;
        this.configuration = configuration;
    }

    public CsvReader(IFileSystem fs): this(fs, new CsvConfiguration()) {}

    public async Task<DataTable> CreateTableFromFileAsync(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        if(!fileSystem.File.Exists(path)) throw new FileNotFoundException($"The file at path {path} does not exists.");
        var table = new DataTable();

        using var stream = fileSystem.File.OpenText(path);

        if(stream.EndOfStream) return table;

        var firstLine =  (await stream.ReadLineAsync())!;

        if(configuration.HasHeader)
        {
            CreateColumnsFromHeader(firstLine, table);
        }
        else
        {
            CreateDefaultColumns(firstLine, table);
            AddRowFromLine(firstLine, table);
        } 

        var line = (await stream.ReadLineAsync())!;
        while(!stream.EndOfStream)
        {
            AddRowFromLine(line, table);
            line = (await stream.ReadLineAsync())!;
        }

        return table;
    }

    private void AddRowFromLine(string line, DataTable table)
    {
        if (string.IsNullOrWhiteSpace(line)) return;
        var row = table.NewRow();
        var lineItens = line.Split(configuration.Delimiter);

        for (var i = 0; i < lineItens.Length; i++)
        {
            row[i] = lineItens[i];
        }

        table.Rows.Add(row);
    }

    private void CreateDefaultColumns(string firstLine, DataTable table)
    {
        var columnsCount = firstLine.Split(configuration.Delimiter).Count();
        foreach(var index in Enumerable.Range(0, columnsCount))
        {
            table.Columns.Add(new DataColumn(string.Format("Column_{0}", index)));
        }
    }

    private void CreateColumnsFromHeader(string header, DataTable table)
    {
        var columnNames = header.Split(configuration.Delimiter);

        foreach (var column in columnNames)
        {
            table.Columns.Add(new DataColumn(column));
        }
    }

    public async Task LoadFileIntoTableAsync( string path, DataTable table)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(table);

        using var stream = fileSystem.File.OpenText(path);

        if(stream.EndOfStream) return;

        var line =  (await stream.ReadLineAsync())!;

        while(!stream.EndOfStream)
        {
            AddRowFromLine(line, table);
            line = (await stream.ReadLineAsync())!;
        }
    }
}