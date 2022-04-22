using System.Data;

namespace Application.Commons.Interfaces;

public interface ICsvReader
{
    Task<DataTable> ReadAllAsync();
}