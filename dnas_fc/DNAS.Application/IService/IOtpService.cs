using DNAS.Domain.DTO.Login;

namespace DNAS.Application.IService
{
    public interface IOtpService
    {
        Task<OtpResponseModel> GenerateOTPAsync(OtpResponseModel requestModel);      
        Task<OtpResponseModel> VerifyOTPAsync(OtpResponseModel request);
    }
}
