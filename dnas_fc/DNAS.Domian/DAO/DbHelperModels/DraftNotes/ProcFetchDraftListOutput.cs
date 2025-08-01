using DNAS.Domian.DTO.Draft;

namespace DNAS.Domian.DAO.DbHelperModels.DraftNotes
{
    public class ProcFetchDraftListOutput
    {
        public IEnumerable<DraftNote> DraftNotes { get; set; } = [];
    }
}
