using Dapper;
using DNAS.Domian.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace DNAS.Persistence.SqlFactory
{
    internal sealed class SqlDapper(IOptions<ConnectionStrings> connectionStrings) : ISqlDapper
    {
        private readonly ConnectionStrings _connectionStrings = connectionStrings.Value;

        public async ValueTask<int> Execute(string sp, object parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            int rowsAffected = 0;
            using IDbConnection db = new SqlConnection(_connectionStrings.SQLConnection?.ToString());
            try
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                using var tran = db.BeginTransaction();
                try
                {
                    rowsAffected = await db.ExecuteAsync(sp, param: parameters, commandType: CommandType.StoredProcedure);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    tran.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }

            return rowsAffected;
        }

        public async ValueTask<T> Get<T>(string sp, object? parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection db = new SqlConnection(_connectionStrings.SQLConnection?.ToString());
            return await db.QueryFirstOrDefaultAsync<T>(sp, parameters, commandType: commandType) ?? throw new InvalidOperationException("Data Not Found");
        }

        public async ValueTask<IEnumerable<T>> GetAll<T>(string sp, object? parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionStrings.SQLConnection?.ToString());
                return await db.QueryAsync<T>(sp, parameters, commandType: commandType);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async ValueTask<T> Insert<T>(string sp, object? parameters = null, CommandType commandType = CommandType.StoredProcedure) where T : new()
        {
            T result = new();
            using IDbConnection db = new SqlConnection(_connectionStrings.SQLConnection?.ToString());
            try
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                using var tran = db.BeginTransaction();
                try
                {
                    result = await db.QueryFirstOrDefaultAsync<T>(sp, param: parameters, commandType: commandType, transaction: tran) ?? new();
                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                    throw;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }

            return result;
        }

        public T Update<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            T result;
            using IDbConnection db = new SqlConnection(_connectionStrings.SQLConnection?.ToString());
            try
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                using var tran = db.BeginTransaction();
                try
                {
                    result = db.Query<T>(sp, parms, commandType: commandType, transaction: tran).FirstOrDefault();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    tran.Rollback();
                    throw;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }

            return result;
        }

        public async ValueTask<bool> QuerySPMultiple(string sp, Action<SqlMapper.GridReader> callback, object? parameters = null)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionStrings.SQLConnection?.ToString());
                var multi = await SqlMapper.QueryMultipleAsync(db, sp, param: parameters, commandType: CommandType.StoredProcedure);
                callback(multi);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }
    }
}