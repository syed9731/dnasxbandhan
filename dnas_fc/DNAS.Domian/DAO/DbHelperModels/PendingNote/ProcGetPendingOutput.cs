using DNAS.Domian.DTO.Note;

namespace DNAS.Domian.DAO.DbHelperModels.PendingNote
{
    public class ProcGetPendingOutput
    {
        public IEnumerable<PendingTable> Table { get; set; } = [];
    }
}
