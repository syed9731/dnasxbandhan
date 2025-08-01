using DNAS.Domian.Common;
using DNAS.Domian.DTO.DashBoard;

namespace DNAS.Application.IRepository
{
    public interface IDashboard
    {
        public Task<CommonResponse<ApprovalData>> GetDashboardData(object inparam);
    }
}
