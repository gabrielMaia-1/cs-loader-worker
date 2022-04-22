using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Application.Commons.Adapters;
using Application.Commons.Interfaces;

namespace Application.Commons.Senders;

public class DataTableSender
{
    private readonly IJsonAdapter jsonAdapter;
    private readonly ICsvReader csvAdapter;
    private readonly HttpClient httpClient;

    public DataTableSender(IJsonAdapter jsonAdapter, ICsvReader csvAdapter, HttpClient httpClient)
    {
        this.jsonAdapter = jsonAdapter;
        this.csvAdapter = csvAdapter;
        this.httpClient = httpClient;
    }

    public async Task LoadData(string path, HttpMethod httpMethod, string url)
    {
        var table = await csvAdapter.ReadAllAsync();

        //Add Transformation Pipeline

        var json = jsonAdapter.ConvertToJson(table);

        var response = await SendData(httpMethod, url, json);

        //Handle(Response)
    }

    private async Task<HttpResponseMessage> SendData(HttpMethod httpMethod, string url, JsonNode json)
    {
        var httpMessage = new HttpRequestMessage(httpMethod, url);
        httpMessage.Content = JsonContent.Create(json);
        return await httpClient.SendAsync(httpMessage);
    }
}