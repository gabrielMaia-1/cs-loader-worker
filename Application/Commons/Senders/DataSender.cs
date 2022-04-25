using System.Data;
using System.IO.Abstractions;
using Application.Commons.Adapters;

namespace Application.Commons.Senders;

public class DataSender : 
    DataSender.IDataSender
    , DataSender.IDataSenderToResult
    , DataSender.IDataSenderFromResult
{
    private DataTable? data;
    private Func<Task<DataTable>>? sourceFunction;
    private Func<DataTable, Task>? senderFunction;

    private DataSender()
    { }

    public static IDataSender Create()
    {
        return new DataSender() as IDataSender;
    }

    public IDataSenderFromResult FromFunc(Func<Task<DataTable>> func)
    {
        sourceFunction = func;
        return this;
    }
    public IDataSenderToResult ToFunc(Func<DataTable, Task> func)
    {
        senderFunction = func;
        return this;
    }

    public async Task SendAsync()
    {
        if(sourceFunction is null) throw new Exception();
        if(senderFunction is null) throw new Exception();

        var data = await sourceFunction.Invoke();
        await senderFunction.Invoke(data);
    }

    public void Send()
    {
        SendAsync().GetAwaiter().GetResult();
    }

    public interface IDataSenderFromResult
    {
        IDataSenderToResult ToFunc(Func<DataTable, Task> func);
    }

    public interface IDataSender
    {
        IDataSenderFromResult FromFunc(Func<Task<DataTable>> func);
    }

    public interface IDataSenderToResult
    {
        Task SendAsync();
        void Send();
    }
}