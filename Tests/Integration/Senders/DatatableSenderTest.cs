using System.Net;
using System.Net.Http;
using System.Data;
using System.IO.Abstractions;
using Application.Commons.Adapters;
using Application.Commons.Senders;
using Moq;
using Xunit;
using System.Threading.Tasks;
using System.Text.Json.Nodes;
using Moq.Protected;
using System.Threading;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using System;
using System.Linq;

namespace Tests.Senders;

public class DatatableSenderTest
{
    [Fact]
    public async void SendFile()
    {
        var path = "./file.csv";
        var url = "http://localhost";

        var handlerMock = new Mock<HttpMessageHandler>();

        var fileSystem = new MockFileSystem();
        var fileData = new MockFileData("column1;column2;column3\n1;2;3\n4;5;6");
        fileSystem.AddFile(path, fileData);

        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync((HttpRequestMessage request, CancellationToken token) => {
                var response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.OK;
                return response;
            })
            .Callback<HttpRequestMessage, CancellationToken>(async (message, token)=> {
                Assert.NotNull(message.Content);
                Assert.True(HttpMethod.Post == message.Method);

                var body = await message.Content!.ReadAsStringAsync();
                var bodyJson = JsonArray.Parse(body)!;

                var element = bodyJson[0]!;

                Assert.True(element.AsObject().ContainsKey("column1"));
                Assert.True(element.AsObject().ContainsKey("column2"));
                Assert.True(element.AsObject().ContainsKey("column3"));
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var jsonAdapter = new JsonAdapter();
        var csvAdapter = new CsvReader(fileSystem.FileSystem);

        var sut = new DataTableSender(jsonAdapter, csvAdapter, httpClient);

        await sut.LoadData(path, HttpMethod.Post, url: url);
    }
}