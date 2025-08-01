using Dapper;
using System.Data;

namespace DNAS.Persistence.SqlFactory
{
    internal interface ISqlDapper
    {
        ValueTask<T> Get<T>(string sp, object? parameters = null, CommandType commandType = CommandType.StoredProcedure);
        ValueTask<IEnumerable<T>> GetAll<T>(string sp, object? parameters = null, CommandType commandType = CommandType.StoredProcedure);
        ValueTask<int> Execute(string sp, object parameters, CommandType commandType = CommandType.StoredProcedure);
        ValueTask<T> Insert<T>(string sp, object? parameters = null, CommandType commandType = CommandType.StoredProcedure) where T : new();
        T Update<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        ValueTask<bool> QuerySPMultiple(string sp, Action<SqlMapper.GridReader> callback, object? parameters = null);
    }
}