using System;
using System.Data;
using System.Linq;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Application.Commons.Adapters;
using Moq;
using Xunit;
using System.Threading.Tasks;
using Tests.Data;

namespace Tests.Unity.Readers;

public class CsvReaderTest
{

    [Fact]
    public async Task CreateTableFromFile_Create_ColumnNames_As_CsvHeaderAsync()
    {
        var path = "./file.csv";
        var columns = new string[] {"column1", "column2", "column3"};
        var delimiter = ';';

        var mock = new MockFileSystem();
        var data = new MockFileData(string.Join(delimiter, columns));
        mock.AddFile(path, data);

        using var stream = mock.File.OpenText(path);

        var sut = new CsvReader(stream);

        var table = await sut.ReadAllAsync();

        foreach (var column in columns)
        {
            Assert.Contains(column, table.Columns.Cast<DataColumn>().Select(c=> c.ColumnName));
        }
    }

    [Fact]
    public async void CreateTableFromFile_Create_Default_ColumnNames()
    {
        var path = "./file.csv";
        var delimiter = ';';
        var data = CsvGenerator.DataSets.Default.Data[0];
        var configuration = new CsvConfiguration()
        {
            HasHeader = false,
            Delimiter = delimiter
        };

        var mock = new MockFileSystem();
        var fileData = new MockFileData(string.Join(delimiter, data));

        mock.AddFile(path, fileData); 

        using var stream = mock.File.OpenText(path);

        var sut = new CsvReader(stream, configuration);

        var table = await sut.ReadAllAsync();
        var columnNames = table.Columns.Cast<DataColumn>().Select(c => c.ColumnName);

        foreach (var index in Enumerable.Range(0, columnNames.Count()))
        {
            Assert.Contains(string.Format("Column_{0}", index), columnNames);
        }
    }
    [Fact]
    public async void CreateTableFromFile_Load_Csv_Data()
    {
        var path = "./file.csv";
        var csvGen = new CsvGenerator();

        var csvContent = csvGen.GenerateCsv(CsvGenerator.DataSets.Default.Header, CsvGenerator.DataSets.Default.Data);
        var configuration = new CsvConfiguration()
        {
            Delimiter = csvGen.Delimiter
        };

        var mock = new MockFileSystem();
        var fileData = new MockFileData(csvContent);
        mock.AddFile(path, fileData);

        using var stream = mock.File.OpenText(path);

        var sut = new CsvReader(stream);

        var table = await sut.ReadAllAsync();
        var row = table.Rows[0];

        foreach (var index in Enumerable.Range(0, table.Columns.Count))
        {
            Assert.Equal(CsvGenerator.DataSets.Default.Data[0][index], row[index]);
        }
    }
}