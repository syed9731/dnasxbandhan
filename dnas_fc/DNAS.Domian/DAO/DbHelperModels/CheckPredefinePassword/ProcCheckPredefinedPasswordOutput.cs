using DNAS.Domian.DTO.Login;

namespace DNAS.Domain.DAO.DbHelperModels.CheckPredefinePassword
{
    public class ProcCheckPredefinedPasswordOutput
    {
        public UserMasterResponse UserMaster { get; set; } = new();
    }
}
