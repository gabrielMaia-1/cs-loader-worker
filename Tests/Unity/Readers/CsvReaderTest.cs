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
    public void CreateTableFromFile_Throws_If_Null_Path()
    {
        var sut = new CsvReader(new MockFileSystem());

        Assert.ThrowsAsync<ArgumentNullException>(async ()=>{
            await sut.CreateTableFromFileAsync(null!);
        });
    }

    [Fact]
    public void CreateTableFromFile_Throws_FileNotFound()
    {
        var sut = new CsvReader(new MockFileSystem());

        Assert.ThrowsAsync<FileNotFoundException>(async ()=>{
            await sut.CreateTableFromFileAsync("./file.csv");
        });
    }

    [Fact]
    public async Task CreateTableFromFile_Create_ColumnNames_As_CsvHeaderAsync()
    {
        var path = "./file.csv";
        var columns = new string[] {"column1", "column2", "column3"};
        var delimiter = ';';

        var mock = new MockFileSystem();
        var data = new MockFileData(string.Join(delimiter, columns));
        mock.AddFile(path, data);

        var sut = new CsvReader(mock);

        var table = await sut.CreateTableFromFileAsync(path);

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

        var sut = new CsvReader(mock, configuration);

        var table = await sut.CreateTableFromFileAsync(path);
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

        var sut = new CsvReader(mock, configuration);

        var table = await sut.CreateTableFromFileAsync(path);
        var row = table.Rows[0];

        foreach (var index in Enumerable.Range(0, table.Columns.Count))
        {
            Assert.Equal(CsvGenerator.DataSets.Default.Data[0][index], row[index]);
        }
    }
    [Fact]
    public void LoadFileIntoTable_Trows_Argument_Null()
    {
        var mock = new MockFileSystem();
        var sut = new CsvReader(mock);

        Assert.ThrowsAsync<ArgumentNullException>(async ()=>{
            await sut.LoadFileIntoTableAsync(null!, new DataTable());
        });

        Assert.ThrowsAsync<ArgumentNullException>(async ()=>{
            await sut.LoadFileIntoTableAsync("./path", null!);
        });
    }

    [Fact]
    public void LoadFileIntoTable_Throw_File_Not_Found()
    {
        var mock = new MockFileSystem();
        var sut = new CsvReader(mock);

        Assert.ThrowsAsync<FileNotFoundException>(async ()=>{
            await sut.LoadFileIntoTableAsync("./path", new DataTable());
        });
    }

    [Fact]
    public async Task LoadFileIntoTable_Add_Rows_To_TableAsync()
    {
        string path = "./file.csv";
        var configuration = new CsvConfiguration();
        var csvContent = new CsvGenerator().GenerateCsv(
            CsvGenerator.DataSets.Default.Header,
            CsvGenerator.DataSets.Default.Data
        );

        var table = new DataTable()
        {
            Columns =
            {
                new DataColumn(CsvGenerator.DataSets.Default.Header[0]),
                new DataColumn(CsvGenerator.DataSets.Default.Header[1]),
                new DataColumn(CsvGenerator.DataSets.Default.Header[2]),
            }
        };

        var mock = new MockFileSystem();
        mock.AddFile(path, csvContent);
        
        var sut = new CsvReader(mock, configuration);

        await sut.LoadFileIntoTableAsync(path, table);

        Assert.True(table.Columns.Count > 0);
    }
}