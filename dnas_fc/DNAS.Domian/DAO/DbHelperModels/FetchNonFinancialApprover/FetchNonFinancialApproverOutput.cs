using DNAS.Domian.DTO.Login;

namespace DNAS.Domain.DAO.DbHelperModels.FetchNonFinancialApprover
{
    public class FetchNonFinancialApproverOutput
    {
        public IEnumerable<UserMasterModel> UserMasterModelList { get; set; } = [];
    }
}
