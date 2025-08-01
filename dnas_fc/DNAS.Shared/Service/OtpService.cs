using DNAS.Application.Common.Interface;
using DNAS.Application.IService;
using DNAS.Domain.DTO.Login;
using DNAS.Domian.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace DNAS.Shared.Service
{
    public class OtpService : IOtpService
    {

        public OtpService(IHttpContextAccessor httpContextAccessor, IOptions<AppConfig> appConfig)
        {
            _httpContextAccessor = httpContextAccessor;
            _appConfig = appConfig;
        }
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<AppConfig> _appConfig;
        public async Task<OtpResponseModel> GenerateOTPAsync(OtpResponseModel requestModel)
        {
            var response = new OtpResponseModel();
            try
            {
                
                string otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
                await Task.Delay(500);
                response.OTPSent = true;
                response.OTP = otp;
                // For session storage
                _httpContextAccessor.HttpContext?.Session.SetString("GeneratedOTP", otp);
            }
            catch (Exception ex)
            {
                response.OTPError = "Error generating OTP. Please try again.";
            }
            return response;
        }
        public async Task<OtpResponseModel> VerifyOTPAsync(OtpResponseModel request)
        {
            try
            {
                // Retrieve the stored OTP from session
                var storedOtp = _httpContextAccessor.HttpContext.Session.GetString("GeneratedOTP");
                string bypassotpval = _appConfig.Value.ByPassOtp;
                if (!string.IsNullOrEmpty(storedOtp) && (storedOtp == request.OTP || bypassotpval== request.OTP))
                {
                    // OTP matches, remove it from session to prevent reuse
                    _httpContextAccessor.HttpContext.Session.Remove("GeneratedOTP");

                    // Return a success response
                    return new OtpResponseModel
                    {
                        Success = true,
                        Message = "OTP verified successfully."
                    };
                }

                // Return failure response for mismatch or missing OTP
                return new OtpResponseModel
                {
                    Success = false,
                    Message = "Invalid OTP. Please try again."
                };
            }
            catch (Exception ex)
            {
                // Return a generic error response
                return new OtpResponseModel
                {
                    Success = false,
                    Message = "An error occurred during OTP verification. Please try again later."
                };
            }
        }

    }
}
