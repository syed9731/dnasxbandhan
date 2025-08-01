using DNAS.Application.Business.Interface;
using DNAS.Application.Common.Interface;
using DNAS.Application.IService;
using DNAS.Domain.DTO.Login;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.MailSend;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace DNAS.Application.Features.Login
{
    public class SendOtpCommandHandler(OtpResponseModel user) : IRequest<bool>
    {
        public OtpResponseModel _email { get; set; } = user;

    }
    internal class SendOtpCommand(IOtpService otpService, ICustomLogger logger, IEncryption enc, IMailService iMailService, IEmailService _emailService, IOptions<AppConfig> appConfig, IHttpContextAccessor haccess) : IRequestHandler<SendOtpCommandHandler, bool>
    {
        private readonly IOtpService _otpService = otpService;
        public readonly ICustomLogger _logger = logger;
        public readonly IEncryption _encryption = enc;      
        public readonly IMailService _iMailService = iMailService;
        private readonly string _logpath = "Login";
        public async Task<bool> Handle(SendOtpCommandHandler Request, CancellationToken cancellationToken)
        {
            bool result = false;
            try
            {                
                OtpResponseModel otpRequestModel = new()
                {
                    Email = Request._email.Email.ToString(),
                };
                //Log Here
                OtpResponseModel OtpResponse = await _otpService.GenerateOTPAsync(otpRequestModel);
                _logger.LogwriteInfo("Get Emailid For Sending Mail -- ", _logpath);             
                if (OtpResponse.OTP != null)
                {
                    _logger.LogwriteInfo("SMTP Configuration details fetched successfully for OTP mail send------" +
                                         Environment.NewLine + "and mail id is----- " + otpRequestModel.Email, _logpath);

                    MailSender objMail = await _emailService.GetMailConfiguration();
                    objMail.Receiver = otpRequestModel.Email;

                    // Set the subject for the OTP email
                    objMail.Subject = "Your One-Time Password (OTP)";

                    // Set the body for the OTP email
                    objMail.Body = $"Dear {haccess.HttpContext?.User.FindFirstValue("UserName")},\n\n" +
                                   $"Your OTP for verification is: {OtpResponse.OTP}.\n\n" +
                                   "Please use this OTP to complete your verification process. The OTP is valid for " + Convert.ToInt64(appConfig.Value.MailOtpValidateTime) + " Second.\n\n" +
                                   "If you did not request this, please ignore this email or contact support.\n\n" +
                                   "Thank you,\n";

                    // Send the email
                    result = await _iMailService.EmailSend(objMail);
                    _logger.LogwriteInfo("OTP mail send status---" + result, _logpath);
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during otp send execution" + Environment.NewLine + "exception message-" + e.Message + Environment.NewLine + e.StackTrace, _logpath);
                result = false;
            }            
            return result;
        }
    }
}
