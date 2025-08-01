using Dapper;
using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Domian.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Security.Claims;

namespace DNAS.Persistence.DapperRepository
{
    internal sealed class DapperFactory(IOptions<ConnectionStrings> connectionStrings, ICustomLogger logger, IHttpContextAccessor haccess) : IDapperFactory
    {
        private readonly ConnectionStrings _connectionStrings = connectionStrings.Value;
        private readonly ICustomLogger _Logger= logger;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";

        #region Returns a Variety of: Strongly Typed Property
        public async ValueTask<int> ExecuteSpDapperAsync(string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure)
        {
            try
            {
                #region Initialize Properties
                using SqlConnection OracleDbConnection = new(_connectionStrings.SQLConnection?.ToString());
                if (OracleDbConnection.State == ConnectionState.Closed) await OracleDbConnection.OpenAsync();
                int Result;
                #endregion

                #region Execute Stored Procedure
                Result = await OracleDbConnection.ExecuteAsync(
                    sql: SpName,
                    param: Params,
                    commandType: CmdType);
                #endregion

                #region Return
                if (OracleDbConnection.State == ConnectionState.Open) await OracleDbConnection.CloseAsync();                
                return Result;
                #endregion
            }
            catch (Exception e)
            {
                _Logger.LogwriteError("exception occur during dapper ExecuteSpDapperAsync call.----"+e.Message+ Environment.NewLine + e.StackTrace, loginUserId);
                throw;
            }
        }

        public async ValueTask<DataSet> ExecuteSpDapperAsync(bool IsDs, string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure)
        {
            try
            {
                #region Initialize Properties
                using SqlConnection OracleDbConnection = new(_connectionStrings.SQLConnection?.ToString());
                if (OracleDbConnection.State == ConnectionState.Closed) await OracleDbConnection.OpenAsync();
                DataSet Result = new();
                #endregion

                #region Execute Stored Procedure
                DbDataReader Reader = await OracleDbConnection.ExecuteReaderAsync(
                    sql: SpName,
                    param: Params,
                    commandType: CmdType);
                #endregion

                #region Prepare Data
                int Count = 0;
                while (!Reader.IsClosed)
                {
                    Count++;
                    DataTable DataTable = new("Table_" + Count);
                    DataTable.Load(Reader);
                    Result.Tables.Add(DataTable);
                }
                await Reader.CloseAsync();
                await Reader.DisposeAsync();
                #endregion

                #region Return
                if (OracleDbConnection.State == ConnectionState.Open) await OracleDbConnection.CloseAsync();
                
                return Result;
                #endregion
            }
            catch (Exception e)
            {
                _Logger.LogwriteError("exception occur during dapper ExecuteSpDapperAsync call extra parameter.----" + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                throw;
            }
        }
        #endregion

        #region Return Combination Of Single Row and List Rows Type of: T and IEnumerable<T> Model or Models
        public async ValueTask<TReturn> ExecuteSpDapperAsync<T1, TReturn>(string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure)
            where T1 : new()
            where TReturn : new()
        {
            try
            {
                #region Initialize Properties
                using SqlConnection OracleDbConnection = new(_connectionStrings.SQLConnection?.ToString());
                if (OracleDbConnection.State == ConnectionState.Closed) await OracleDbConnection.OpenAsync();
                TReturn Result = new();
                #endregion

                #region Execute Stored Procedure
                PropertyInfo[] Properties = typeof(TReturn).GetProperties();
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[0].PropertyType)))
                {
                    T1 Return = await OracleDbConnection.QueryFirstOrDefaultAsync<T1>(
                    sql: SpName,
                    param: Params,
                    commandType: CommandType.StoredProcedure) ?? new T1();
                    Properties[0].SetValue(Result, Return);
                }
                else
                {
                    IEnumerable<T1> Return = await OracleDbConnection.QueryAsync<T1>(
                        sql: SpName,
                        param: Params,
                        commandType: CommandType.StoredProcedure);
                    Properties[0].SetValue(Result, Return);
                }
                #endregion

                #region Return
                if (OracleDbConnection.State == ConnectionState.Open) await OracleDbConnection.CloseAsync();
                
                return Result;
                #endregion
            }
            catch (Exception e)
            {
                _Logger.LogwriteError("exception occur during dapper ExecuteSpDapperAsync call generic single table type----"+Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                throw;
            }
        }

        public async ValueTask<TReturn> ExecuteSpDapperAsync<T1, T2, TReturn>(string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure)
            where T1 : new()
            where T2 : new()
            where TReturn : new()
        {
            try
            {
                #region Initialize Properties
                using SqlConnection OracleDbConnection = new(_connectionStrings.SQLConnection?.ToString());
                if (OracleDbConnection.State == ConnectionState.Closed) await OracleDbConnection.OpenAsync();
                TReturn Result = new();
                #endregion

                #region Execute Stored Procedure
                SqlMapper.GridReader DbGridReader = await SqlMapper.QueryMultipleAsync(
                    cnn: OracleDbConnection,
                    sql: SpName,
                    param: Params,
                    commandType: CmdType);
                #endregion

                #region Prepare Response Data
                PropertyInfo[] Properties = typeof(TReturn).GetProperties();

                #region Prepare T1
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[0].PropertyType)))
                {
                    T1 Result1 = await DbGridReader.ReadFirstOrDefaultAsync<T1>() ?? new T1();
                    Properties[0].SetValue(Result, Result1);
                }
                else
                {
                    IEnumerable<T1> Result1 = await DbGridReader.ReadAsync<T1>() ?? [];
                    Properties[0].SetValue(Result, Result1);
                }
                #endregion

                #region Prepare T2
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[1].PropertyType)))
                {
                    T2 Result2 = await DbGridReader.ReadFirstOrDefaultAsync<T2>() ?? new T2();
                    Properties[1].SetValue(Result, Result2);
                }
                else
                {
                    IEnumerable<T2> Result2 = await DbGridReader.ReadAsync<T2>() ?? [];
                    Properties[1].SetValue(Result, Result2);
                }
                #endregion

                await DbGridReader.DisposeAsync();
                #endregion

                #region Return
                if (OracleDbConnection.State == ConnectionState.Open) await OracleDbConnection.CloseAsync();
                
                return Result;
                #endregion
            }
            catch (Exception e)
            {
                _Logger.LogwriteError("exception occur during dapper ExecuteSpDapperAsync call generic double table type-------" + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                throw;
            }
        }

        public async ValueTask<TReturn> ExecuteSpDapperAsync<T1, T2, T3, TReturn>(string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure)
            where T1 : new()
            where T2 : new()
            where T3 : new()
            where TReturn : new()
        {
            try
            {
                #region Initialize Properties
                using SqlConnection OracleDbConnection = new(_connectionStrings.SQLConnection?.ToString());
                if (OracleDbConnection.State == ConnectionState.Closed) await OracleDbConnection.OpenAsync();
                TReturn Result = new();
                #endregion

                #region Execute Stored Procedure
                SqlMapper.GridReader DbGridReader = await SqlMapper.QueryMultipleAsync(
                    cnn: OracleDbConnection,
                    sql: SpName,
                    param: Params,
                    commandType: CmdType);
                #endregion

                #region Prepare Response Data
                PropertyInfo[] Properties = typeof(TReturn).GetProperties();

                #region Prepare T1
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[0].PropertyType)))
                {
                    T1 Result1 = await DbGridReader.ReadFirstOrDefaultAsync<T1>() ?? new T1();
                    Properties[0].SetValue(Result, Result1);
                }
                else
                {
                    IEnumerable<T1> Result1 = await DbGridReader.ReadAsync<T1>() ?? [];
                    Properties[0].SetValue(Result, Result1);
                }
                #endregion

                #region Prepare T2
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[1].PropertyType)))
                {
                    T2 Result2 = await DbGridReader.ReadFirstOrDefaultAsync<T2>() ?? new T2();
                    Properties[1].SetValue(Result, Result2);
                }
                else
                {
                    IEnumerable<T2> Result2 = await DbGridReader.ReadAsync<T2>() ?? [];
                    Properties[1].SetValue(Result, Result2);
                }
                #endregion

                #region Prepare T3
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[2].PropertyType)))
                {
                    T3 Result3 = await DbGridReader.ReadFirstOrDefaultAsync<T3>() ?? new T3();
                    Properties[2].SetValue(Result, Result3);
                }
                else
                {
                    IEnumerable<T3> Result3 = await DbGridReader.ReadAsync<T3>() ?? [];
                    Properties[2].SetValue(Result, Result3);
                }
                #endregion

                await DbGridReader.DisposeAsync();
                #endregion

                #region Return
                if (OracleDbConnection.State == ConnectionState.Open) await OracleDbConnection.CloseAsync();
                
                return Result;
                #endregion
            }
            catch (Exception e)
            {
                _Logger.LogwriteError("exception occur during dapper ExecuteSpDapperAsync call generic three table type-------" + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                throw;
            }
        }

        public async ValueTask<TReturn> ExecuteSpDapperAsync<T1, T2, T3, T4, TReturn>(string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure)
            where T1 : new()
            where T2 : new()
            where T3 : new()
            where T4 : new()
            where TReturn : new()
        {
            try
            {
                #region Initialize Properties
                using SqlConnection OracleDbConnection = new(_connectionStrings.SQLConnection?.ToString());
                if (OracleDbConnection.State == ConnectionState.Closed) await OracleDbConnection.OpenAsync();
                TReturn Result = new();
                #endregion

                #region Execute Stored Procedure
                SqlMapper.GridReader DbGridReader = await SqlMapper.QueryMultipleAsync(
                    cnn: OracleDbConnection,
                    sql: SpName,
                    param: Params,
                    commandType: CmdType);
                #endregion

                #region Prepare Response Data
                PropertyInfo[] Properties = typeof(TReturn).GetProperties();

                #region Prepare T1
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[0].PropertyType)))
                {
                    T1 Result1 = await DbGridReader.ReadFirstOrDefaultAsync<T1>() ?? new T1();
                    Properties[0].SetValue(Result, Result1);
                }
                else
                {
                    IEnumerable<T1> Result1 = await DbGridReader.ReadAsync<T1>() ?? [];
                    Properties[0].SetValue(Result, Result1);
                }
                #endregion

                #region Prepare T2
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[1].PropertyType)))
                {
                    T2 Result2 = await DbGridReader.ReadFirstOrDefaultAsync<T2>() ?? new T2();
                    Properties[1].SetValue(Result, Result2);
                }
                else
                {
                    IEnumerable<T2> Result2 = await DbGridReader.ReadAsync<T2>() ?? [];
                    Properties[1].SetValue(Result, Result2);
                }
                #endregion

                #region Prepare T3
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[2].PropertyType)))
                {
                    T3 Result3 = await DbGridReader.ReadFirstOrDefaultAsync<T3>() ?? new T3();
                    Properties[2].SetValue(Result, Result3);
                }
                else
                {
                    IEnumerable<T3> Result3 = await DbGridReader.ReadAsync<T3>() ?? [];
                    Properties[2].SetValue(Result, Result3);
                }
                #endregion

                #region Prepare T4
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[3].PropertyType)))
                {
                    T4 Result4 = await DbGridReader.ReadFirstOrDefaultAsync<T4>() ?? new T4();
                    Properties[3].SetValue(Result, Result4);
                }
                else
                {
                    IEnumerable<T4> Result4 = await DbGridReader.ReadAsync<T4>() ?? [];
                    Properties[3].SetValue(Result, Result4);
                }
                #endregion

                await DbGridReader.DisposeAsync();
                #endregion

                #region Return
                if (OracleDbConnection.State == ConnectionState.Open) await OracleDbConnection.CloseAsync();
                
                return Result;
                #endregion
            }
            catch (Exception e)
            {
                _Logger.LogwriteError("exception occur during dapper ExecuteSpDapperAsync call generic four table type-------" + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                throw;
            }
        }

        public async ValueTask<TReturn> ExecuteSpDapperAsync<T1, T2, T3, T4, T5, TReturn>(string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure)
            where T1 : new()
            where T2 : new()
            where T3 : new()
            where T4 : new()
            where T5 : new()
            where TReturn : new()
        {
            try
            {
                #region Initialize Properties
                using SqlConnection OracleDbConnection = new(_connectionStrings.SQLConnection?.ToString());
                if (OracleDbConnection.State == ConnectionState.Closed) await OracleDbConnection.OpenAsync();
                TReturn Result = new();
                #endregion

                #region Execute Stored Procedure
                SqlMapper.GridReader DbGridReader = await SqlMapper.QueryMultipleAsync(
                    cnn: OracleDbConnection,
                    sql: SpName,
                    param: Params,
                    commandType: CmdType);
                #endregion

                #region Prepare Response Data
                PropertyInfo[] Properties = typeof(TReturn).GetProperties();

                #region Prepare T1
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[0].PropertyType)))
                {
                    T1 Result1 = await DbGridReader.ReadFirstOrDefaultAsync<T1>() ?? new T1();
                    Properties[0].SetValue(Result, Result1);
                }
                else
                {
                    IEnumerable<T1> Result1 = await DbGridReader.ReadAsync<T1>() ?? [];
                    Properties[0].SetValue(Result, Result1);
                }
                #endregion

                #region Prepare T2
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[1].PropertyType)))
                {
                    T2 Result2 = await DbGridReader.ReadFirstOrDefaultAsync<T2>() ?? new T2();
                    Properties[1].SetValue(Result, Result2);
                }
                else
                {
                    IEnumerable<T2> Result2 = await DbGridReader.ReadAsync<T2>() ?? [];
                    Properties[1].SetValue(Result, Result2);
                }
                #endregion

                #region Prepare T3
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[2].PropertyType)))
                {
                    T3 Result3 = await DbGridReader.ReadFirstOrDefaultAsync<T3>() ?? new T3();
                    Properties[2].SetValue(Result, Result3);
                }
                else
                {
                    IEnumerable<T3> Result3 = await DbGridReader.ReadAsync<T3>() ?? [];
                    Properties[2].SetValue(Result, Result3);
                }
                #endregion

                #region Prepare T4
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[3].PropertyType)))
                {
                    T4 Result4 = await DbGridReader.ReadFirstOrDefaultAsync<T4>() ?? new T4();
                    Properties[3].SetValue(Result, Result4);
                }
                else
                {
                    IEnumerable<T4> Result4 = await DbGridReader.ReadAsync<T4>() ?? [];
                    Properties[3].SetValue(Result, Result4);
                }
                #endregion

                #region Prepare T5
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[4].PropertyType)))
                {
                    T5 Result5 = await DbGridReader.ReadFirstOrDefaultAsync<T5>() ?? new T5();
                    Properties[4].SetValue(Result, Result5);
                }
                else
                {
                    IEnumerable<T5> Result5 = await DbGridReader.ReadAsync<T5>() ?? [];
                    Properties[4].SetValue(Result, Result5);
                }
                #endregion

                await DbGridReader.DisposeAsync();
                #endregion

                #region Return
                if (OracleDbConnection.State == ConnectionState.Open) await OracleDbConnection.CloseAsync();
                
                return Result;
                #endregion
            }
            catch (Exception e)
            {
                _Logger.LogwriteError("exception occur during dapper ExecuteSpDapperAsync call generic five table type-------" + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                throw;
            }
        }

        public async ValueTask<TReturn> ExecuteSpDapperAsync<T1, T2, T3, T4, T5, T6, TReturn>(string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure)
            where T1 : new()
            where T2 : new()
            where T3 : new()
            where T4 : new()
            where T5 : new()
            where T6 : new()
            where TReturn : new()
        {
            try
            {
                #region Initialize Properties
                using SqlConnection OracleDbConnection = new(_connectionStrings.SQLConnection?.ToString());
                if (OracleDbConnection.State == ConnectionState.Closed) await OracleDbConnection.OpenAsync();
                TReturn Result = new();
                #endregion

                #region Execute Stored Procedure
                SqlMapper.GridReader DbGridReader = await SqlMapper.QueryMultipleAsync(
                    cnn: OracleDbConnection,
                    sql: SpName,
                    param: Params,
                    commandType: CmdType);
                #endregion

                #region Prepare Response Data
                PropertyInfo[] Properties = typeof(TReturn).GetProperties();

                #region Prepare T1
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[0].PropertyType)))
                {
                    T1 Result1 = await DbGridReader.ReadFirstOrDefaultAsync<T1>() ?? new T1();
                    Properties[0].SetValue(Result, Result1);
                }
                else
                {
                    IEnumerable<T1> Result1 = await DbGridReader.ReadAsync<T1>() ?? [];
                    Properties[0].SetValue(Result, Result1);
                }
                #endregion

                #region Prepare T2
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[1].PropertyType)))
                {
                    T2 Result2 = await DbGridReader.ReadFirstOrDefaultAsync<T2>() ?? new T2();
                    Properties[1].SetValue(Result, Result2);
                }
                else
                {
                    IEnumerable<T2> Result2 = await DbGridReader.ReadAsync<T2>() ?? [];
                    Properties[1].SetValue(Result, Result2);
                }
                #endregion

                #region Prepare T3
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[2].PropertyType)))
                {
                    T3 Result3 = await DbGridReader.ReadFirstOrDefaultAsync<T3>() ?? new T3();
                    Properties[2].SetValue(Result, Result3);
                }
                else
                {
                    IEnumerable<T3> Result3 = await DbGridReader.ReadAsync<T3>() ?? [];
                    Properties[2].SetValue(Result, Result3);
                }
                #endregion

                #region Prepare T4
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[3].PropertyType)))
                {
                    T4 Result4 = await DbGridReader.ReadFirstOrDefaultAsync<T4>() ?? new T4();
                    Properties[3].SetValue(Result, Result4);
                }
                else
                {
                    IEnumerable<T4> Result4 = await DbGridReader.ReadAsync<T4>() ?? [];
                    Properties[3].SetValue(Result, Result4);
                }
                #endregion

                #region Prepare T5
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[4].PropertyType)))
                {
                    T5 Result5 = await DbGridReader.ReadFirstOrDefaultAsync<T5>() ?? new T5();
                    Properties[4].SetValue(Result, Result5);
                }
                else
                {
                    IEnumerable<T5> Result5 = await DbGridReader.ReadAsync<T5>() ?? [];
                    Properties[4].SetValue(Result, Result5);
                }
                #endregion

                #region Prepare T6
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[5].PropertyType)))
                {
                    T6 Result6 = await DbGridReader.ReadFirstOrDefaultAsync<T6>() ?? new T6();
                    Properties[5].SetValue(Result, Result6);
                }
                else
                {
                    IEnumerable<T6> Result6 = await DbGridReader.ReadAsync<T6>() ?? [];
                    Properties[5].SetValue(Result, Result6);
                }
                #endregion

                await DbGridReader.DisposeAsync();
                #endregion

                #region Return
                if (OracleDbConnection.State == ConnectionState.Open) await OracleDbConnection.CloseAsync();
                
                return Result;
                #endregion
            }
            catch (Exception e)
            {
                _Logger.LogwriteError("exception occur during dapper ExecuteSpDapperAsync call generic six table type-------" + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                throw;
            }
        }

        public async ValueTask<TReturn> ExecuteSpDapperAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure)
            where T1 : new()
            where T2 : new()
            where T3 : new()
            where T4 : new()
            where T5 : new()
            where T6 : new()
            where T7 : new()
            where TReturn : new()
        {
            try
            {
                #region Initialize Properties
                using SqlConnection OracleDbConnection = new(_connectionStrings.SQLConnection?.ToString());
                if (OracleDbConnection.State == ConnectionState.Closed) await OracleDbConnection.OpenAsync();
                TReturn Result = new();
                #endregion

                #region Execute Stored Procedure
                SqlMapper.GridReader DbGridReader = await SqlMapper.QueryMultipleAsync(
                    cnn: OracleDbConnection,
                    sql: SpName,
                    param: Params,
                    commandType: CmdType);
                #endregion

                #region Prepare Response Data
                PropertyInfo[] Properties = typeof(TReturn).GetProperties();

                #region Prepare T1
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[0].PropertyType)))
                {
                    T1 Result1 = await DbGridReader.ReadFirstOrDefaultAsync<T1>() ?? new T1();
                    Properties[0].SetValue(Result, Result1);
                }
                else
                {
                    IEnumerable<T1> Result1 = await DbGridReader.ReadAsync<T1>() ?? [];
                    Properties[0].SetValue(Result, Result1);
                }
                #endregion

                #region Prepare T2
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[1].PropertyType)))
                {
                    T2 Result2 = await DbGridReader.ReadFirstOrDefaultAsync<T2>() ?? new T2();
                    Properties[1].SetValue(Result, Result2);
                }
                else
                {
                    IEnumerable<T2> Result2 = await DbGridReader.ReadAsync<T2>() ?? [];
                    Properties[1].SetValue(Result, Result2);
                }
                #endregion

                #region Prepare T3
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[2].PropertyType)))
                {
                    T3 Result3 = await DbGridReader.ReadFirstOrDefaultAsync<T3>() ?? new T3();
                    Properties[2].SetValue(Result, Result3);
                }
                else
                {
                    IEnumerable<T3> Result3 = await DbGridReader.ReadAsync<T3>() ?? [];
                    Properties[2].SetValue(Result, Result3);
                }
                #endregion

                #region Prepare T4
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[3].PropertyType)))
                {
                    T4 Result4 = await DbGridReader.ReadFirstOrDefaultAsync<T4>() ?? new T4();
                    Properties[3].SetValue(Result, Result4);
                }
                else
                {
                    IEnumerable<T4> Result4 = await DbGridReader.ReadAsync<T4>() ?? [];
                    Properties[3].SetValue(Result, Result4);
                }
                #endregion

                #region Prepare T5
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[4].PropertyType)))
                {
                    T5 Result5 = await DbGridReader.ReadFirstOrDefaultAsync<T5>() ?? new T5();
                    Properties[4].SetValue(Result, Result5);
                }
                else
                {
                    IEnumerable<T5> Result5 = await DbGridReader.ReadAsync<T5>() ?? [];
                    Properties[4].SetValue(Result, Result5);
                }
                #endregion

                #region Prepare T6
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[5].PropertyType)))
                {
                    T6 Result6 = await DbGridReader.ReadFirstOrDefaultAsync<T6>() ?? new T6();
                    Properties[5].SetValue(Result, Result6);
                }
                else
                {
                    IEnumerable<T6> Result6 = await DbGridReader.ReadAsync<T6>() ?? [];
                    Properties[5].SetValue(Result, Result6);
                }
                #endregion

                #region Prepare T7
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[6].PropertyType)))
                {
                    T7 Result7 = await DbGridReader.ReadFirstOrDefaultAsync<T7>() ?? new T7();
                    Properties[6].SetValue(Result, Result7);
                }
                else
                {
                    IEnumerable<T7> Result7 = await DbGridReader.ReadAsync<T7>() ?? [];
                    Properties[6].SetValue(Result, Result7);
                }
                #endregion

                await DbGridReader.DisposeAsync();
                #endregion

                #region Return
                if (OracleDbConnection.State == ConnectionState.Open) await OracleDbConnection.CloseAsync();
                
                return Result;
                #endregion
            }
            catch (Exception e)
            {
                _Logger.LogwriteError("exception occur during dapper ExecuteSpDapperAsync call generic seven table type-------" + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                throw;
            }
        }

        public async ValueTask<TReturn> ExecuteSpDapperAsync<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure)
            where T1 : new()
            where T2 : new()
            where T3 : new()
            where T4 : new()
            where T5 : new()
            where T6 : new()
            where T7 : new()
            where T8 : new()
            where TReturn : new()
        {
            try
            {
                #region Initialize Properties
                using SqlConnection OracleDbConnection = new(_connectionStrings.SQLConnection?.ToString());
                if (OracleDbConnection.State == ConnectionState.Closed) await OracleDbConnection.OpenAsync();
                TReturn Result = new();
                #endregion

                #region Execute Stored Procedure
                SqlMapper.GridReader DbGridReader = await SqlMapper.QueryMultipleAsync(
                    cnn: OracleDbConnection,
                    sql: SpName,
                    param: Params,
                    commandType: CmdType);
                #endregion

                #region Prepare Response Data
                PropertyInfo[] Properties = typeof(TReturn).GetProperties();

                #region Prepare T1
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[0].PropertyType)))
                {
                    T1 Result1 = await DbGridReader.ReadFirstOrDefaultAsync<T1>() ?? new T1();
                    Properties[0].SetValue(Result, Result1);
                }
                else
                {
                    IEnumerable<T1> Result1 = await DbGridReader.ReadAsync<T1>() ?? [];
                    Properties[0].SetValue(Result, Result1);
                }
                #endregion

                #region Prepare T2
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[1].PropertyType)))
                {
                    T2 Result2 = await DbGridReader.ReadFirstOrDefaultAsync<T2>() ?? new T2();
                    Properties[1].SetValue(Result, Result2);
                }
                else
                {
                    IEnumerable<T2> Result2 = await DbGridReader.ReadAsync<T2>() ?? [];
                    Properties[1].SetValue(Result, Result2);
                }
                #endregion

                #region Prepare T3
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[2].PropertyType)))
                {
                    T3 Result3 = await DbGridReader.ReadFirstOrDefaultAsync<T3>() ?? new T3();
                    Properties[2].SetValue(Result, Result3);
                }
                else
                {
                    IEnumerable<T3> Result3 = await DbGridReader.ReadAsync<T3>() ?? [];
                    Properties[2].SetValue(Result, Result3);
                }
                #endregion

                #region Prepare T4
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[3].PropertyType)))
                {
                    T4 Result4 = await DbGridReader.ReadFirstOrDefaultAsync<T4>() ?? new T4();
                    Properties[3].SetValue(Result, Result4);
                }
                else
                {
                    IEnumerable<T4> Result4 = await DbGridReader.ReadAsync<T4>() ?? [];
                    Properties[3].SetValue(Result, Result4);
                }
                #endregion

                #region Prepare T5
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[4].PropertyType)))
                {
                    T5 Result5 = await DbGridReader.ReadFirstOrDefaultAsync<T5>() ?? new T5();
                    Properties[4].SetValue(Result, Result5);
                }
                else
                {
                    IEnumerable<T5> Result5 = await DbGridReader.ReadAsync<T5>() ?? [];
                    Properties[4].SetValue(Result, Result5);
                }
                #endregion

                #region Prepare T6
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[5].PropertyType)))
                {
                    T6 Result6 = await DbGridReader.ReadFirstOrDefaultAsync<T6>() ?? new T6();
                    Properties[5].SetValue(Result, Result6);
                }
                else
                {
                    IEnumerable<T6> Result6 = await DbGridReader.ReadAsync<T6>() ?? [];
                    Properties[5].SetValue(Result, Result6);
                }
                #endregion

                #region Prepare T7
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[6].PropertyType)))
                {
                    T7 Result7 = await DbGridReader.ReadFirstOrDefaultAsync<T7>() ?? new T7();
                    Properties[6].SetValue(Result, Result7);
                }
                else
                {
                    IEnumerable<T7> Result7 = await DbGridReader.ReadAsync<T7>() ?? [];
                    Properties[6].SetValue(Result, Result7);
                }
                #endregion

                #region Prepare T8
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[7].PropertyType)))
                {
                    T8 Result8 = await DbGridReader.ReadFirstOrDefaultAsync<T8>() ?? new T8();
                    Properties[7].SetValue(Result, Result8);
                }
                else
                {
                    IEnumerable<T8> Result8 = await DbGridReader.ReadAsync<T8>() ?? [];
                    Properties[7].SetValue(Result, Result8);
                }
                #endregion

                await DbGridReader.DisposeAsync();
                #endregion

                #region Return
                if (OracleDbConnection.State == ConnectionState.Open) await OracleDbConnection.CloseAsync();
                
                return Result;
                #endregion
            }
            catch (Exception e)
            {
                _Logger.LogwriteError("exception occur during dapper ExecuteSpDapperAsync call generic eight table type-------" + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                throw;
            }
        }

        public async ValueTask<TReturn> ExecuteSpDapperAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure)
            where T1 : new()
            where T2 : new()
            where T3 : new()
            where T4 : new()
            where T5 : new()
            where T6 : new()
            where T7 : new()
            where T8 : new()
            where T9 : new()
            where TReturn : new()
        {
            try
            {
                #region Initialize Properties
                using SqlConnection OracleDbConnection = new(_connectionStrings.SQLConnection?.ToString());
                if (OracleDbConnection.State == ConnectionState.Closed) await OracleDbConnection.OpenAsync();

                TReturn Result = new();
                #endregion

                #region Execute Stored Procedure
                SqlMapper.GridReader DbGridReader = await SqlMapper.QueryMultipleAsync(
                    cnn: OracleDbConnection,
                    sql: SpName,
                    param: Params,
                    commandType: CmdType);
                #endregion

                #region Prepare Response Data
                PropertyInfo[] Properties = typeof(TReturn).GetProperties();

                #region Prepare T1
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[0].PropertyType)))
                {
                    T1 Result1 = await DbGridReader.ReadFirstOrDefaultAsync<T1>() ?? new T1();
                    Properties[0].SetValue(Result, Result1);
                }
                else
                {
                    IEnumerable<T1> Result1 = await DbGridReader.ReadAsync<T1>() ?? [];
                    Properties[0].SetValue(Result, Result1);
                }
                #endregion

                #region Prepare T2
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[1].PropertyType)))
                {
                    T2 Result2 = await DbGridReader.ReadFirstOrDefaultAsync<T2>() ?? new T2();
                    Properties[1].SetValue(Result, Result2);
                }
                else
                {
                    IEnumerable<T2> Result2 = await DbGridReader.ReadAsync<T2>() ?? [];
                    Properties[1].SetValue(Result, Result2);
                }
                #endregion

                #region Prepare T3
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[2].PropertyType)))
                {
                    T3 Result3 = await DbGridReader.ReadFirstOrDefaultAsync<T3>() ?? new T3();
                    Properties[2].SetValue(Result, Result3);
                }
                else
                {
                    IEnumerable<T3> Result3 = await DbGridReader.ReadAsync<T3>() ?? [];
                    Properties[2].SetValue(Result, Result3);
                }
                #endregion

                #region Prepare T4
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[3].PropertyType)))
                {
                    T4 Result4 = await DbGridReader.ReadFirstOrDefaultAsync<T4>() ?? new T4();
                    Properties[3].SetValue(Result, Result4);
                }
                else
                {
                    IEnumerable<T4> Result4 = await DbGridReader.ReadAsync<T4>() ?? [];
                    Properties[3].SetValue(Result, Result4);
                }
                #endregion

                #region Prepare T5
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[4].PropertyType)))
                {
                    T5 Result5 = await DbGridReader.ReadFirstOrDefaultAsync<T5>() ?? new T5();
                    Properties[4].SetValue(Result, Result5);
                }
                else
                {
                    IEnumerable<T5> Result5 = await DbGridReader.ReadAsync<T5>() ?? [];
                    Properties[4].SetValue(Result, Result5);
                }
                #endregion

                #region Prepare T6
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[5].PropertyType)))
                {
                    T6 Result6 = await DbGridReader.ReadFirstOrDefaultAsync<T6>() ?? new T6();
                    Properties[5].SetValue(Result, Result6);
                }
                else
                {
                    IEnumerable<T6> Result6 = await DbGridReader.ReadAsync<T6>() ?? [];
                    Properties[5].SetValue(Result, Result6);
                }
                #endregion

                #region Prepare T7
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[6].PropertyType)))
                {
                    T7 Result7 = await DbGridReader.ReadFirstOrDefaultAsync<T7>() ?? new T7();
                    Properties[6].SetValue(Result, Result7);
                }
                else
                {
                    IEnumerable<T7> Result7 = await DbGridReader.ReadAsync<T7>() ?? [];
                    Properties[6].SetValue(Result, Result7);
                }
                #endregion

                #region Prepare T8
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[7].PropertyType)))
                {
                    T8 Result8 = await DbGridReader.ReadFirstOrDefaultAsync<T8>() ?? new T8();
                    Properties[7].SetValue(Result, Result8);
                }
                else
                {
                    IEnumerable<T8> Result8 = await DbGridReader.ReadAsync<T8>() ?? [];
                    Properties[7].SetValue(Result, Result8);
                }
                #endregion

                #region Prepare T9
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[8].PropertyType)))
                {
                    T9 Result9 = await DbGridReader.ReadFirstOrDefaultAsync<T9>() ?? new T9();
                    Properties[8].SetValue(Result, Result9);
                }
                else
                {
                    IEnumerable<T9> Result9 = await DbGridReader.ReadAsync<T9>() ?? [];
                    Properties[8].SetValue(Result, Result9);
                }
                #endregion

                await DbGridReader.DisposeAsync();
                #endregion

                #region Return
                if (OracleDbConnection.State == ConnectionState.Open) await OracleDbConnection.CloseAsync();
                
                return Result;
                #endregion
            }
            catch (Exception e)
            {
                _Logger.LogwriteError("exception occur during dapper ExecuteSpDapperAsync call generic nine table type-------" + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                throw;
            }
        }

        public async ValueTask<TReturn> ExecuteSpDapperAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure)
            where T1 : new()
            where T2 : new()
            where T3 : new()
            where T4 : new()
            where T5 : new()
            where T6 : new()
            where T7 : new()
            where T8 : new()
            where T9 : new()
            where T10 : new()
            where TReturn : new()
        {
            try
            {
                #region Initialize Properties
                using SqlConnection OracleDbConnection = new(_connectionStrings.SQLConnection?.ToString());
                if (OracleDbConnection.State == ConnectionState.Closed) await OracleDbConnection.OpenAsync();

                TReturn Result = new();
                #endregion

                #region Execute Stored Procedure
                SqlMapper.GridReader DbGridReader = await SqlMapper.QueryMultipleAsync(
                    cnn: OracleDbConnection,
                    sql: SpName,
                    param: Params,
                    commandType: CmdType);
                #endregion

                #region Prepare Response Data
                PropertyInfo[] Properties = typeof(TReturn).GetProperties();

                #region Prepare T1
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[0].PropertyType)))
                {
                    T1 Result1 = await DbGridReader.ReadFirstOrDefaultAsync<T1>() ?? new T1();
                    Properties[0].SetValue(Result, Result1);
                }
                else
                {
                    IEnumerable<T1> Result1 = await DbGridReader.ReadAsync<T1>() ?? [];
                    Properties[0].SetValue(Result, Result1);
                }
                #endregion

                #region Prepare T2
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[1].PropertyType)))
                {
                    T2 Result2 = await DbGridReader.ReadFirstOrDefaultAsync<T2>() ?? new T2();
                    Properties[1].SetValue(Result, Result2);
                }
                else
                {
                    IEnumerable<T2> Result2 = await DbGridReader.ReadAsync<T2>() ?? [];
                    Properties[1].SetValue(Result, Result2);
                }
                #endregion

                #region Prepare T3
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[2].PropertyType)))
                {
                    T3 Result3 = await DbGridReader.ReadFirstOrDefaultAsync<T3>() ?? new T3();
                    Properties[2].SetValue(Result, Result3);
                }
                else
                {
                    IEnumerable<T3> Result3 = await DbGridReader.ReadAsync<T3>() ?? [];
                    Properties[2].SetValue(Result, Result3);
                }
                #endregion

                #region Prepare T4
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[3].PropertyType)))
                {
                    T4 Result4 = await DbGridReader.ReadFirstOrDefaultAsync<T4>() ?? new T4();
                    Properties[3].SetValue(Result, Result4);
                }
                else
                {
                    IEnumerable<T4> Result4 = await DbGridReader.ReadAsync<T4>() ?? [];
                    Properties[3].SetValue(Result, Result4);
                }
                #endregion

                #region Prepare T5
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[4].PropertyType)))
                {
                    T5 Result5 = await DbGridReader.ReadFirstOrDefaultAsync<T5>() ?? new T5();
                    Properties[4].SetValue(Result, Result5);
                }
                else
                {
                    IEnumerable<T5> Result5 = await DbGridReader.ReadAsync<T5>() ?? [];
                    Properties[4].SetValue(Result, Result5);
                }
                #endregion

                #region Prepare T6
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[5].PropertyType)))
                {
                    T6 Result6 = await DbGridReader.ReadFirstOrDefaultAsync<T6>() ?? new T6();
                    Properties[5].SetValue(Result, Result6);
                }
                else
                {
                    IEnumerable<T6> Result6 = await DbGridReader.ReadAsync<T6>() ?? [];
                    Properties[5].SetValue(Result, Result6);
                }
                #endregion

                #region Prepare T7
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[6].PropertyType)))
                {
                    T7 Result7 = await DbGridReader.ReadFirstOrDefaultAsync<T7>() ?? new T7();
                    Properties[6].SetValue(Result, Result7);
                }
                else
                {
                    IEnumerable<T7> Result7 = await DbGridReader.ReadAsync<T7>() ?? [];
                    Properties[6].SetValue(Result, Result7);
                }
                #endregion

                #region Prepare T8
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[7].PropertyType)))
                {
                    T8 Result8 = await DbGridReader.ReadFirstOrDefaultAsync<T8>() ?? new T8();
                    Properties[7].SetValue(Result, Result8);
                }
                else
                {
                    IEnumerable<T8> Result8 = await DbGridReader.ReadAsync<T8>() ?? [];
                    Properties[7].SetValue(Result, Result8);
                }
                #endregion

                #region Prepare T9
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[8].PropertyType)))
                {
                    T9 Result9 = await DbGridReader.ReadFirstOrDefaultAsync<T9>() ?? new T9();
                    Properties[8].SetValue(Result, Result9);
                }
                else
                {
                    IEnumerable<T9> Result9 = await DbGridReader.ReadAsync<T9>() ?? [];
                    Properties[8].SetValue(Result, Result9);
                }
                #endregion

                #region Prepare T10
                if (!(typeof(IEnumerable).IsAssignableFrom(Properties[9].PropertyType)))
                {
                    T10 Result10 = await DbGridReader.ReadFirstOrDefaultAsync<T10>() ?? new T10();
                    Properties[9].SetValue(Result, Result10);
                }
                else
                {
                    IEnumerable<T10> Result10 = await DbGridReader.ReadAsync<T10>() ?? [];
                    Properties[9].SetValue(Result, Result10);
                }
                #endregion

                await DbGridReader.DisposeAsync();
                #endregion

                #region Return
                if (OracleDbConnection.State == ConnectionState.Open) await OracleDbConnection.CloseAsync();
                
                return Result;
                #endregion
            }
            catch (Exception e)
            {
                _Logger.LogwriteError("exception occur during dapper ExecuteSpDapperAsync call generic ten table type-------" + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                throw;
            }
        }
        #endregion

    } 
}
