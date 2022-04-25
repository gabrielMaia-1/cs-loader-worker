using System;
using System.Data;
using System.Threading.Tasks;
using Application.Commons.Senders;
using Moq;
using Xunit;

namespace Tests.Senders;

public class DataSenderTest
{


    [Fact]
    public async Task SendAsync_Calls_FromFuncAsync()
    {
        var mock = new Mock<Func<Task<DataTable>>>();

        await DataSender.Create()
            .FromFunc(mock.Object)
            .ToFunc((table) => Task.CompletedTask)
            .SendAsync();

        mock.Verify(f => f.Invoke(), Times.Once);
    }
    [Fact]
    public void SendAsync_Passes_FromFunc_Result_To_ToFunc()
    {
        var table = new DataTable();

        var mock = new Mock<Func<DataTable, Task>>();
        mock.Setup(f => f.Invoke(It.IsAny<DataTable>()))
            .Callback<DataTable>((t) => {
                Assert.Same(t, table);
            });

        DataSender.Create()
            .FromFunc(() => Task.FromResult(table))
            .ToFunc(mock.Object)
            .Send();

        mock.Verify(f => f.Invoke(It.IsAny<DataTable>()), Times.Once);
    }

    [Fact]
    public async Task SendAsync_Calls_ToFuncAsync()
    {
        var mock = new Mock<Func<DataTable, Task>>();

        await DataSender.Create()
            .FromFunc(() => Task.FromResult(new DataTable()))
            .ToFunc(mock.Object)
            .SendAsync();

        mock.Verify(f => f.Invoke(It.IsAny<DataTable>()), Times.Once);
    }

    [Fact]
    public async Task SendAsync_Trows_If_SenderFunction_Is_NullAsync()
    {
        await Assert.ThrowsAsync<Exception>(async ()=> {
            await DataSender.Create()
                .FromFunc(() => Task.FromResult(new DataTable()))
                .ToFunc(null!)
                .SendAsync();
        });
    }

    [Fact]
    public async Task SendAsync_Trows_If_SourceFunction_Is_NullAsync()
    {
        await Assert.ThrowsAsync<Exception>(async ()=> {
            await DataSender.Create()
                .FromFunc(null!)
                .ToFunc((table) => Task.CompletedTask)
                .SendAsync();
        });
    }
}