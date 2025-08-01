using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ReminderNotification.Model;
using System.Data;

namespace ReminderNotification.DAL
{
    internal class FetchUsers
    {

        private readonly string _connectionString;
        
        public FetchUsers(IConfiguration iconfiguration)
        {
            _connectionString = iconfiguration.GetConnectionString("SQLConnection")!;
           
        }

        public async Task<NotificationModel> GetList()
        {
            NotificationModel NotificationList = new();
            try
            {
                using IDbConnection con = new SqlConnection(_connectionString);
                SqlMapper.GridReader reader = await con.QueryMultipleAsync(
                   sql: "GetNotRespondApprover",
                   commandType: CommandType.StoredProcedure
                   );
                NotificationList.NotificationUserDetails = await reader.ReadAsync<NotificationUserDetail>();
                NotificationList.EmailConfigs = await reader.ReadAsync<EmailConfig>();
                NotificationList.MailBodyContents = await reader.ReadFirstOrDefaultAsync<MailBodyContent>() ?? new MailBodyContent();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }            
            return NotificationList;
        }
    }

}
