using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Application.IRepository;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.DashBoard;

namespace DNAS.Persistence.Repository
{
    internal class Dashboard(ICustomLogger logger, IDapperFactory iDapperFactory) :IDashboard
    {
        private readonly ICustomLogger _logger = logger;
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        public async Task<CommonResponse<ApprovalData>> GetDashboardData(object inparam)
        {
            CommonResponse<ApprovalData> Response = new();
            try
            {                
                ApprovalData DBResponse = await _iDapperFactory.ExecuteSpDapperAsync<Approval, Draft, Count, ApprovalData>(OraStoredProcedureNames.ProcGetDashboardData, inparam);
                Response.Data = DBResponse;
            }            
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during GetNoteWithApproval------ " + e.Message + Environment.NewLine + e.StackTrace, "Login");
            }
            return Response;
        }
    }
}
