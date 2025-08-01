using DNAS.Domian.DTO.SearchNotes;

namespace DNAS.Domian.DAO.DbHelperModels.SearchNotes
{
    public class ProcGetSearchNoteOutput
    {
        public IEnumerable<SearchNote> SearchNotes { get; set; } = [];
    }
}
