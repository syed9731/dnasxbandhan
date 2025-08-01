using DNAS.Domain.DTO.Login;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.Login;
using System.Threading.Tasks;

namespace DNAS.Application.IRepository
{
    public interface ILogin
    {
        Task<CommonResponse<UserMasterResponse>> getMasterData(object inparam);
        Task<CommonResponse<UserMasterResponse>> getUserDataByUserId(object inparam);
        Task<CommonResponse<RecoverPasswordResponse>> getRecoverPassword(object inparam);
        Task<CommonResponse<UserMasterResponse>> checkPredefinePassword(object inparam);
        Task<CommonResponse<ChangePasswordModel>> VerifyRecoverPasswordUser(object inparam);
        Task<UserMasterModel> FetchUserData(object inparam);
        Task<UserTrackingModel> CheckUsertracking(object inparam);
        Task<UserTrackingModel> Usertracking(object inparam);
		Task<CommonResponse<UserMasterResponse>> CheckUserExists(object inparam);	
    }
}
