using System.Data;
using System.Text.Json.Nodes;

namespace Application.Commons.Interfaces;

public interface IJsonAdapter
{
    JsonArray ConvertToJson(DataTable table);
}