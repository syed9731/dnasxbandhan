using DNAS.Domian.DTO.Category;

namespace DNAS.Domain.DAO.DbHelperModels.FetchCategory
{
    public class FetchCategoryOutput
    {
        public IEnumerable<CategoryRespModel> CategoryRespModelList { get; set; } = [];
    }
}
