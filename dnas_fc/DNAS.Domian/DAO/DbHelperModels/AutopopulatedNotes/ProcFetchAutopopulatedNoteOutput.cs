using DNAS.Domain.DTO.Note;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNAS.Domain.DAO.DbHelperModels.AutopopulatedNotes
{
    public class ProcFetchAutopopulatedNoteOutput
    {
        public IEnumerable<NoteData> Table { get; set; } = [];
    }
}
