using DNAS.Domian.DTO.Category;

namespace DNAS.Domain.DAO.DbHelperModels.FetchExpenseIncurredAt
{
    public class FetchExpenseIncurredAtOutput
    {
        public IEnumerable<ExpenseIncurredAtModel> ExpenseIncurredAtModelList { get; set; } = [];
    }
}
