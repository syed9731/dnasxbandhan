using DNAS.Domain.DTO.Note;
using DNAS.Domian.DTO.Approver;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNAS.Domain.DAO.DbHelperModels.ApproverHistory
{
    public class ProcGetApproverHistoryOutput
    {
        public IEnumerable<ApproverHistoryData> Table { get; set; } = [];

    }
}
