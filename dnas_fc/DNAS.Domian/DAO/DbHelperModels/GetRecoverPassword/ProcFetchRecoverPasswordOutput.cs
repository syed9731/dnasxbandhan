using DNAS.Domian.DTO.Login;

namespace DNAS.Domain.DAO.DbHelperModels.GetRecoverPassword
{
    public class ProcFetchRecoverPasswordOutput
    {
        public RecoverPasswordResponse RecoverPassword { get; set; } = new();
    }
}
