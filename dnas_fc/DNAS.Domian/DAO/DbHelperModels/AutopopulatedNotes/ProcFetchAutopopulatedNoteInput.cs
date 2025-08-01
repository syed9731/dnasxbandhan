using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNAS.Domain.DAO.DbHelperModels.AutopopulatedNotes
{
    public class ProcFetchAutopopulatedNoteInput
    {
        public long @UserId { get; set; } = 0;
        public string @SearchKey { get; set; } = string.Empty;
        public string @DashboardType { get; set; }= string.Empty;

    }
}
