using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Application.Commons.Interfaces;

namespace Application.Commons.Adapters;

public class CsvReader : ICsvReader
{
    private readonly StreamReader reader;
    private readonly CsvConfiguration configuration;

    public CsvReader(StreamReader reader, CsvConfiguration configuration)
    {
        this.reader = reader;
        this.configuration = configuration;
    }

    public CsvReader(StreamReader reader): this(reader, new CsvConfiguration()) {}

    public async Task<DataTable> ReadAllAsync()
    {
        var table = await CreateTable();

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            AddRowFromLine(line!, table);
        }

        return table;
    }

    private async Task<DataTable> CreateTable()
    {
        var table = new DataTable();

        if (configuration.HasHeader)
        {
            var firstLine = await reader.ReadLineAsync();
            CreateColumnsFromHeader(firstLine!, table);
        }
        else
        {
            var position = reader.BaseStream.Position;
            var line = await reader.ReadLineAsync();
            reader.BaseStream.Position = position;
            CreateDefaultColumns(line!.Split(configuration.Delimiter).Count(), table);
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

    private void CreateDefaultColumns(int columns, DataTable table)
    {
        foreach(var index in Enumerable.Range(0, columns))
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
}