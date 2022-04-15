using System.Data;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using Application.Commons.Adapters;
using Moq;
using Xunit;

namespace Tests.Loaders;

public class CsvAdapterTest
{
    [Fact]
    public void CreateTableFromHeader_Valid_Input()
    {
        var mock = new Mock<IFileSystem>();
        var sut = new CsvAdapter(mock.Object);
        var columnNames = new string[] { "column1", "column2", "column3" };

        var table = sut.CreateTableFromHeader(string.Join(";", columnNames));

        Assert.Equal(columnNames.Length, table.Columns.Count);
        Assert.Equal(columnNames[0], table.Columns[0].ColumnName);
        Assert.Equal(columnNames[1], table.Columns[1].ColumnName);
        Assert.Equal(columnNames[2], table.Columns[2].ColumnName);
    }

    [Fact]
    public void CreateTableFromHeader_Null_Input()
    {
        var mock = new Mock<IFileSystem>();
        var sut = new CsvAdapter(mock.Object);

        var table = sut.CreateTableFromHeader(null);

        Assert.Equal(0, table.Columns.Count);
    }

    [Fact]
    public void CreateTableFromHeader_Empty_Input()
    {
        var mock = new Mock<IFileSystem>();
        var sut = new CsvAdapter(mock.Object);

        var table = sut.CreateTableFromHeader("");

        Assert.Equal(0, table.Columns.Count);
    }

    [Fact]
    public void AddLineToTable_Valid_Line()
    {
        var mock = new Mock<IFileSystem>();
        var sut = new CsvAdapter(mock.Object);

        var table = new DataTable();
        var columns = new DataColumn[]
        {
            new DataColumn(),
            new DataColumn(),
            new DataColumn(),
        };

        table.Columns.AddRange(columns);

        var data = new object[] { 1, "str", 2.2m };
        var line = string.Join(";", data);

        sut.AddLineToTable(line, table);

        Assert.Equal(data[0].ToString(), table.Rows[0][0]);
        Assert.Equal(data[1].ToString(), table.Rows[0][1]);
        Assert.Equal(data[2].ToString(), table.Rows[0][2]);
    }

    [Fact]
    public void AddLineToTable_NullOrEmptyLine()
    {
        var mock = new Mock<IFileSystem>();
        var sut = new CsvAdapter(mock.Object);

        var table = new DataTable();
        var columns = new DataColumn[]
        {
            new DataColumn(),
            new DataColumn(),
            new DataColumn(),
        };

        table.Columns.AddRange(columns);

        sut.AddLineToTable("", table);
        sut.AddLineToTable(null, table);
        sut.AddLineToTable("  ", table);
        
        Assert.Equal(0, table.Rows.Count);
    }

    [Fact]
    public async void LoadCsvToTable()
    {
        var path = "./pasta/arquivo.csv";
        var csvContent = "coluna1;coluna2;coluna3\n1;2;3";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        using var reader = new StreamReader(stream);

        var mock = new Mock<IFileSystem>();
        mock.Setup(m => m.File.OpenText(path)).Returns(reader);

        var sut = new CsvAdapter(mock.Object);


        var table = await sut.LoadTableAsync(path);

        Assert.Equal(3, table.Columns.Count);
        Assert.Equal(1, table.Rows.Count);
    }
}