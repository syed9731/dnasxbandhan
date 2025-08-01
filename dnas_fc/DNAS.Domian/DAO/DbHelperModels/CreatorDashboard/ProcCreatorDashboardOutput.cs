using DNAS.Domain.DTO.DashBoard;
using DNAS.Domain.DTO.Note;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNAS.Domain.DAO.DbHelperModels.CreatorDashboard
{
    public class ProcCreatorDashboardOutput
    {
        public IEnumerable<CreatorData> Table { get; set; } = [];
    }
}
