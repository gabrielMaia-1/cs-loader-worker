using System.Data;

namespace Application.Commons.Interfaces;

public interface ICsvReader
{
    Task<DataTable> CreateTableFromFileAsync(string path);
    Task LoadFileIntoTableAsync(string path, DataTable table);
}