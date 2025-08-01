using DNAS.Domian.DTO.Template;

namespace DNAS.Application.IRepository
{
    public interface ITemplate
    {
        public Task<TemplateModel> ViewTemplate(object inparam);
    }
}
