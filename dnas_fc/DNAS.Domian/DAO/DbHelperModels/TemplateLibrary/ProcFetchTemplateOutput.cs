using DNAS.Domian.DTO.Template;

namespace DNAS.Domian.DAO.DbHelperModels.TemplateLibrary
{
    public class ProcFetchTemplateOutput
    {
        public IEnumerable<TemplateModel> TemplateList { get; set; } = [];
    }
}
