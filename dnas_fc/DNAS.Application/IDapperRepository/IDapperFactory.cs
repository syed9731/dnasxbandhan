using System.Data;

namespace DNAS.Application.IDapperRepository
{
    public interface IDapperFactory
    {
        #region Returns a Variety of: Strongly Typed Property
        /// <summary>
        /// Use to Execute Stored Procedure Without any Return Or
        /// Use to Insert, Update or Delete
        /// </summary>
        /// 
        /// <returns>
        /// Affected Rows (-1 or 0 only)
        /// </returns>
        /// 
        /// <sample>
        /// int DbResult = await _iDapperFactory.ExecuteSpDapperAsync(SpName: "Your_SP_Name");                  //Without Params.
        /// int DbResult = await _iDapperFactory.ExecuteSpDapperAsync(SpName: "Your_SP_Name", Params: Params);  //With Params.
        /// </sample>
        public ValueTask<int> ExecuteSpDapperAsync(string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure);

        /// <summary>
        /// Use to Fetch Multiple Tables or Views as DataTable
        /// Use to Insert, Update or Delete
        /// </summary>
        /// 
        /// <returns>
        /// DataSet That Contains All The DataTable
        /// </returns>
        /// 
        /// <sample>
        /// DataSet DbResult = await _iDapperFactory.ExecuteSpDapperAsync(IsDs: true, SpName: "Your_SP_Name");                  //Without Params.
        /// DataSet DbResult = await _iDapperFactory.ExecuteSpDapperAsync(IsDs: true, SpName: "Your_SP_Name", Params: Params);  //With Params.
        /// </sample>
        public ValueTask<DataSet> ExecuteSpDapperAsync(bool IsDs, string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure);
        #endregion

        #region Return Combination Of Single Row and List Rows Type of: T and IEnumerable<T> Model or Models
        /// <summary>
        /// Fetch Single Table or View as MainReturnModel [Can be IEnumerable or Non-IEnumerable]
        /// </summary>
        /// 
        /// <returns>
        /// MainReturnModel That Contains YourModel
        /// </returns>
        /// 
        /// <sample>
        /// public class MainReturnModel
        /// {
        ///     public YourModel YourModelObj { get; set; } = new(); OR public IEnumerable<YourModel> YourModelObj { get; set; } = new();
        /// }
        /// MainReturnModel DbResult = await _iDapperFactory.ExecuteSpDapperAsync<YourModel, MainReturnModel>(SpName: "Your_SP_Name");                  //Without Params.
        /// MainReturnModel DbResult = await _iDapperFactory.ExecuteSpDapperAsync<YourModel, MainReturnModel>(SpName: "Your_SP_Name", Params: Params);  //With Params.
        /// </sample>
        public ValueTask<TReturn> ExecuteSpDapperAsync<T1, TReturn>(string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure)
            where T1 : new()
            where TReturn : new();

        /// <summary>
        /// Fetch Two Tables or Views as MainReturnModel [Can be IEnumerable or Non-IEnumerable]
        /// </summary>
        /// 
        /// <returns>
        /// MainReturnModel That Contains YourModels
        /// </returns>
        /// 
        /// <sample>
        /// public class MainReturnModel
        /// {
        ///     public YourModel1 YourModelObj1 { get; set; } = new(); OR public IEnumerable<YourModel1> YourModelObj1 { get; set; } = new();
        ///     public YourModel2 YourModelObj2 { get; set; } = new(); OR public IEnumerable<YourModel2> YourModelObj2 { get; set; } = new();
        /// }
        /// MainReturnModel DbResult = await _iDapperFactory.ExecuteSpDapperAsync<YourModel1, YourModel2, MainReturnModel>(SpName: "Your_SP_Name");                  //Without Params.
        /// MainReturnModel DbResult = await _iDapperFactory.ExecuteSpDapperAsync<YourModel1, YourModel2, MainReturnModel>(SpName: "Your_SP_Name", Params: Params);  //With Params.
        /// </sample>
        public ValueTask<TReturn> ExecuteSpDapperAsync<T1, T2, TReturn>(string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure)
            where T1 : new()
            where T2 : new()
            where TReturn : new();

        /// <summary>
        /// Fetch Three Tables or Views as MainReturnModel [Can be IEnumerable or Non-IEnumerable]
        /// </summary>
        /// 
        /// <returns>
        /// MainReturnModel That Contains YourModels
        /// </returns>
        /// 
        /// <sample>
        /// For Sample Please Follow The Sample of ExecuteSpDapperAsync<T1, TReturn> OR ExecuteSpDapperAsync<T1, T2, TReturn>
        /// </sample>
        public ValueTask<TReturn> ExecuteSpDapperAsync<T1, T2, T3, TReturn>(string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure)
            where T1 : new()
            where T2 : new()
            where T3 : new()
            where TReturn : new();

        /// <summary>
        /// Fetch Four Tables or Views as MainReturnModel [Can be IEnumerable or Non-IEnumerable]
        /// </summary>
        /// 
        /// <returns>
        /// MainReturnModel That Contains YourModels
        /// </returns>
        /// 
        /// <sample>
        /// For Sample Please Follow The Sample of ExecuteSpDapperAsync<T1, TReturn> OR ExecuteSpDapperAsync<T1, T2, TReturn>
        /// </sample>
        public ValueTask<TReturn> ExecuteSpDapperAsync<T1, T2, T3, T4, TReturn>(string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure)
            where T1 : new()
            where T2 : new()
            where T3 : new()
            where T4 : new()
            where TReturn : new();

        /// <summary>
        /// Fetch Five Tables or Views as MainReturnModel [Can be IEnumerable or Non-IEnumerable]
        /// </summary>
        /// 
        /// <returns>
        /// MainReturnModel That Contains YourModels
        /// </returns>
        /// 
        /// <sample>
        /// For Sample Please Follow The Sample of ExecuteSpDapperAsync<T1, TReturn> OR ExecuteSpDapperAsync<T1, T2, TReturn>
        /// </sample>
        public ValueTask<TReturn> ExecuteSpDapperAsync<T1, T2, T3, T4, T5, TReturn>(string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure)
            where T1 : new()
            where T2 : new()
            where T3 : new()
            where T4 : new()
            where T5 : new()
            where TReturn : new();

        /// <summary>
        /// Fetch Six Tables or Views as MainReturnModel [Can be IEnumerable or Non-IEnumerable]
        /// </summary>
        /// 
        /// <returns>
        /// MainReturnModel That Contains YourModels
        /// </returns>
        /// 
        /// <sample>
        /// For Sample Please Follow The Sample of ExecuteSpDapperAsync<T1, TReturn> OR ExecuteSpDapperAsync<T1, T2, TReturn>
        /// </sample>
        public ValueTask<TReturn> ExecuteSpDapperAsync<T1, T2, T3, T4, T5, T6, TReturn>(string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure)
            where T1 : new()
            where T2 : new()
            where T3 : new()
            where T4 : new()
            where T5 : new()
            where T6 : new()
            where TReturn : new();

        /// <summary>
        /// Fetch Seven Tables or Views as MainReturnModel [Can be IEnumerable or Non-IEnumerable]
        /// </summary>
        /// 
        /// <returns>
        /// MainReturnModel That Contains YourModels
        /// </returns>
        /// 
        /// <sample>
        /// For Sample Please Follow The Sample of ExecuteSpDapperAsync<T1, TReturn> OR ExecuteSpDapperAsync<T1, T2, TReturn>
        /// </sample>
        public ValueTask<TReturn> ExecuteSpDapperAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure)
            where T1 : new()
            where T2 : new()
            where T3 : new()
            where T4 : new()
            where T5 : new()
            where T6 : new()
            where T7 : new()
            where TReturn : new();

        /// <summary>
        /// Fetch Eight Tables or Views as MainReturnModel [Can be IEnumerable or Non-IEnumerable]
        /// </summary>
        /// 
        /// <returns>
        /// MainReturnModel That Contains YourModels
        /// </returns>
        /// 
        /// <sample>
        /// For Sample Please Follow The Sample of ExecuteSpDapperAsync<T1, TReturn> OR ExecuteSpDapperAsync<T1, T2, TReturn>
        /// </sample>
        public ValueTask<TReturn> ExecuteSpDapperAsync<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure)
            where T1 : new()
            where T2 : new()
            where T3 : new()
            where T4 : new()
            where T5 : new()
            where T6 : new()
            where T7 : new()
            where T8 : new()
            where TReturn : new();

        /// <summary>
        /// Fetch Nine Tables or Views as MainReturnModel [Can be IEnumerable or Non-IEnumerable]
        /// </summary>
        /// 
        /// <returns>
        /// MainReturnModel That Contains YourModels
        /// </returns>
        /// 
        /// <sample>
        /// For Sample Please Follow The Sample of ExecuteSpDapperAsync<T1, TReturn> OR ExecuteSpDapperAsync<T1, T2, TReturn>
        /// </sample>
        public ValueTask<TReturn> ExecuteSpDapperAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure)
            where T1 : new()
            where T2 : new()
            where T3 : new()
            where T4 : new()
            where T5 : new()
            where T6 : new()
            where T7 : new()
            where T8 : new()
            where T9 : new()
            where TReturn : new();

        /// <summary>
        /// Fetch Ten Tables or Views as MainReturnModel [Can be IEnumerable or Non-IEnumerable]
        /// </summary>
        /// 
        /// <returns>
        /// MainReturnModel That Contains YourModels
        /// </returns>
        /// 
        /// <sample>
        /// For Sample Please Follow The Sample of ExecuteSpDapperAsync<T1, TReturn> OR ExecuteSpDapperAsync<T1, T2, TReturn>
        /// </sample>
        public ValueTask<TReturn> ExecuteSpDapperAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(string SpName, object? Params = null, CommandType CmdType = CommandType.StoredProcedure)
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
            where TReturn : new();
        #endregion
    }
}
