using DNAS.Application.Common.Interface;
using DNAS.Application.IService;
using DNAS.Domain.DTO.Login;
using DNAS.Domian.Common;
using MediatR;

namespace DNAS.Application.Features.Login
{
    public class VerifyOtpCommandHandler(OtpResponseModel otp) : IRequest<CommonResponse<OtpResponseModel>>
    {
        public OtpResponseModel _otp { get; set; } = otp;
    }
    internal class VerifyOtpCommand(IOtpService otpService, ICustomLogger logger, IEncryption enc) : IRequestHandler<VerifyOtpCommandHandler, CommonResponse<OtpResponseModel>>
    {
        private readonly IOtpService _otpService = otpService;
        public readonly ICustomLogger _logger = logger;
        public readonly IEncryption _encryption = enc;
        private readonly string _logpathPrefix = "Login";
        public async Task<CommonResponse<OtpResponseModel>> Handle(VerifyOtpCommandHandler request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogwriteInfo($"OTP Generated for the user.{Environment.NewLine}" +
                                     $"OTP: {request._otp.OTP}", _logpathPrefix);

                OtpResponseModel otpRequestModel = new()
                {
                    OTP = request._otp.OTP,
                };

                OtpResponseModel response = await _otpService.VerifyOTPAsync(otpRequestModel);

                // Check if the OTP verification was successful
                if (!response.Success)
                {
                    _logger.LogwriteInfo($"OTP verification failed.{Environment.NewLine}" +
                                         $"Message: {response.Message}{Environment.NewLine}" +
                                         $"OTP: {request._otp.OTP}", _logpathPrefix);

                    return new CommonResponse<OtpResponseModel>
                    {
                        Data = response,
                        Success = false,
                        Message = response.Message // Return the specific failure message from the response
                    };
                }

                _logger.LogwriteInfo($"OTP verification successful.{Environment.NewLine}" +
                                     $"OTP: {request._otp.OTP}", _logpathPrefix);

                return new CommonResponse<OtpResponseModel>
                {
                    Data = response,
                    Success = true,
                    Message = "OTP verification successful."
                };
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo($"Exception occurred during OTP verification.{Environment.NewLine}" +
                                     $"Exception message: {e.Message}{Environment.NewLine}{e.StackTrace}", _logpathPrefix);

                return new CommonResponse<OtpResponseModel>
                {
                    Data = new(),
                    Success = false,
                    Message = "An error occurred during OTP verification. Please try again later."
                };
            }
        }

    }
}

