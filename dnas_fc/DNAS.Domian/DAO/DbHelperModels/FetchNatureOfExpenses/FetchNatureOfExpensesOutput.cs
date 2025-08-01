using DNAS.Domian.DTO.Category;
using DNAS.Domian.DTO.Login;

namespace DNAS.Domain.DAO.DbHelperModels.FetchNatureOfExpenses
{
    public class FetchNatureOfExpensesOutput
    {
        public IEnumerable<NatureOfExpensesModel> NatureOfExpensesModelList { get; set; } = [];
    }
}
