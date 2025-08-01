using DNAS.Domian.DTO.ApprovedNotes;

namespace DNAS.Domian.DAO.DbHelperModels.ApprovedNotes
{
    public class ProcGetApprovedNoteOutput
    {
        public IEnumerable<ApprovedNote> Table { get; set; } = [];
    }
}
