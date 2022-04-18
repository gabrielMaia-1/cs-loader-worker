using System;
using System.Data;
using System.Text.Json;
using Application.Commons.Adapters;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Adapters;

public class JsonAdapterTest
{
    private readonly ITestOutputHelper Output;

    public JsonAdapterTest(ITestOutputHelper output)
    {
        Output = output;
    }

    [Fact]
    public void ConvertRowToJson_Valid()
    {
        var table = new DataTable();
        table.Columns.AddRange( new DataColumn[] {
            new DataColumn("Coluna1", typeof(decimal)),
            new DataColumn("Coluna2"),
            new DataColumn("Coluna3", typeof(DateTime))
        });
        var row  = table.NewRow();

        row[0] = 2.2;
        row[1] = DateOnly.FromDateTime(DateTime.Now);
        row[2] = DateTime.Now;

        table.Rows.Add(row);

        var sut = new JsonAdapter();
        var obj = sut.ConvertRowToJson(row);

        Assert.True(obj.ContainsKey("Coluna1"));
        Assert.True(obj.ContainsKey("Coluna2"));
        Assert.True(obj.ContainsKey("Coluna3"));

        Assert.Equal(row[0].ToString(),obj["Coluna1"]!.ToString());
        Assert.Equal(row[1].ToString(),obj["Coluna2"]!.ToString());
        Assert.Equal(row[2].ToString(),obj["Coluna3"]!.ToString());

        Output.WriteLine(obj.ToJsonString());
    }

    [Fact]
    public void ConvertToJson_Valid()
    {
        var table = new DataTable();
        table.Columns.AddRange( new DataColumn[] {
            new DataColumn("Coluna1"),
            new DataColumn("Coluna2"),
            new DataColumn("Coluna3")
        });

        var row  = table.NewRow();
        row[0] = "data 1";
        row[1] = "data 2";
        row[2] = "data 3";

        table.Rows.Add(row);

        row  = table.NewRow();
        row[0] = "data 1";
        row[1] = "data 2";
        row[2] = "data 3";

        table.Rows.Add(row);

        var sut = new JsonAdapter();
        var obj = sut.ConvertToJson(table);

        Assert.Equal(2, obj.Count);

        var node = obj[0]!.AsObject();

        Assert.True(node.ContainsKey("Coluna1"));
        Assert.True(node.ContainsKey("Coluna2"));
        Assert.True(node.ContainsKey("Coluna3"));

        Assert.Equal(row[0].ToString(),node["Coluna1"]!.ToString());
        Assert.Equal(row[1].ToString(),node["Coluna2"]!.ToString());
        Assert.Equal(row[2].ToString(),node["Coluna3"]!.ToString());

        Output.WriteLine(obj.ToJsonString());
    }
}