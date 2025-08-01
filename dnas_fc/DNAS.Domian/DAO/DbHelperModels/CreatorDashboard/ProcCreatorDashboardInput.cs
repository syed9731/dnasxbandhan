using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNAS.Domain.DAO.DbHelperModels.CreatorDashboard
{
    public class ProcCreatorDashboardInput
    {
        public long @UserId { get; set; } = 0;
        public string @StartDate { get; set; } = string.Empty;
        public string @EndDate { get; set; } = string.Empty;
        public string @Category { get; set; } = string.Empty;
        public string @Status { get; set; } = string.Empty;
        public string @Title { get; set; } = string.Empty;
    }
}
