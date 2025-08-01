using DNAS.Domian.DTO.Note;

namespace DNAS.Domian.DAO.DbHelperModels.SendBack
{
    public class ProcGetSendBackOutput
    {
        public IEnumerable<SendBackTable> Table { get; set; } = [];
    }
}
