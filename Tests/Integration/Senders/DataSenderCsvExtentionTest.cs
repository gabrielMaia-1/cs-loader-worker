using System.IO;
using System;
using System.Data;
using System.Threading.Tasks;
using Application.Commons.Senders;
using Application.Commons.Senders.Extensions;
using Moq;
using Xunit;
using System.IO.Abstractions.TestingHelpers;
using Application.Commons.Adapters;

namespace Tests.Senders.Extensions;

public class DataSenderCsvExtentionTest
{
    [Fact]
    public void FromCsvFileAsync_Throws_If_Directory_Not_Exists()
    {
        var filesystem = new MockFileSystem();
        Assert.Throws<DirectoryNotFoundException>(()=> {
            DataSender.Create()
                .FromCsvFileAsync("/not-found-directory", filesystem)
            ;
        });

        Assert.Throws<DirectoryNotFoundException>(()=> {
            DataSender.Create()
                .FromCsvFileAsync("/not-found-directory", filesystem, new CsvConfiguration())
            ;
        });
    }
}