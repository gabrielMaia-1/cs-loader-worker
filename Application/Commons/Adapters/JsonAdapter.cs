using System.Data;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Application.Commons.Adapters;

public class JsonAdapter
{

    public JsonArray ConvertToJson(DataTable table)
    {
        var obj = new JsonArray();

        foreach (var row in table.Rows.Cast<DataRow>())
        {
            obj.Add(ConvertRowToJson(row));
        }

        return obj;
    }

    public JsonObject ConvertRowToJson(DataRow row)
    {
        var obj = new JsonObject();

        foreach (var col in row.Table.Columns.Cast<DataColumn>())
        {
            obj.Add(col.ColumnName, row[col.Ordinal].ToString());
        }

        return obj;
    }

    
}