using System;
using System.Data;
using System.Text.Json;
using Application.Commons.Adapters;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Adapters;

public class JsonAdapterTest
{

    public JsonAdapterTest()
    {
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
    }
}