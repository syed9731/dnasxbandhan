using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ReminderNotification.Common;
using ReminderNotification.Model;
using System.Data;

namespace ReminderNotification.DAL
{
    internal class SaveNotification
    {
        private readonly string _connectionString;
        public SaveNotification(IConfiguration iconfiguration)
        {
            _connectionString = iconfiguration.GetConnectionString("SQLConnection")!;

        }
        public async Task<bool> NotificationSave(SaveNotificationModel Request)
        {
            try
            {
                using IDbConnection con = new SqlConnection(_connectionString);
                var reader = await con.ExecuteReaderAsync(
                   sql: "ProcSaveNotification",
                   param: Request,
                   commandType: CommandType.StoredProcedure
                   );
                if ( reader != null )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                SchedulerLogger.LogwriteInfo("exception occur during NotificationSave-------" + e.Message + Environment.NewLine + e.StackTrace, "Notification");
                return false;
            }            
        }
    }
    
}
