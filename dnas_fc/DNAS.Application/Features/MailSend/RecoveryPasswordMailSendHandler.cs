using DNAS.Application.Business.Interface;
using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Application.IService;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.Login;
using DNAS.Domian.DTO.MailSend;
using MediatR;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;


namespace DNAS.Application.Features.MailSend
{
    public class RecoveryPasswordMailSend(RecoverPasswordResponse _mailrequest) : IRequest<CommonResponse<RecoverPasswordResponse>>
    {
        public RecoverPasswordResponse mailrequest { get; set; } = _mailrequest;
    }
    internal sealed class RecoveryPasswordMailSendHandler(IConfiguration iConfiguration, ICustomLogger logger, IEncryption enc, IEncryptionSha pasenc, IMailService iMailService, IUpdate iupdate, IOptions<AppConfig> appConfig, IEmailService _emailService) : IRequestHandler<RecoveryPasswordMailSend, CommonResponse<RecoverPasswordResponse>>
    {
        private readonly IConfiguration _iConfiguration = iConfiguration;
        private readonly ICustomLogger _logger = logger;
        private readonly IEncryption _encryption = enc;
        private readonly IEncryptionSha _encryptionSha = pasenc;
        private readonly IMailService _iMailService = iMailService;
        private readonly IUpdate _updatedata = iupdate;
        private readonly AppConfig _appConfig = appConfig.Value;
        private readonly string _logfilename = "RecoverPassword";

        public async Task<CommonResponse<RecoverPasswordResponse>> Handle(RecoveryPasswordMailSend Request, CancellationToken cancellationToken)
        {
            CommonResponse<RecoverPasswordResponse> resp = new();
            try
            {

                var inparam = new
                {
                    @ConfigurationFor = "SMTPCredential"
                };
                await _iConfiguration.FetchConfiguration(inparam);

                #region making mail body
                #region OTP Generation
                int otp = RandomNumberGenerator.GetInt32(0, 1000000);

                #endregion
                string baseurl = string.Concat(_appConfig.BaseUrl, "/login/changepassword");
                string userid = Request.mailrequest.UserId.ToString();
                string url = baseurl + "?st=" + _encryption.AesEncrypt(userid + "/" + otp);
                string body = "Hello " + Request.mailrequest.FirstName + ", <br/>Please be calm. To recover your password click on the below link- <br/><a href=" + url + ">Click Here</a> <br/>And your predefined password is-" + otp;
                #endregion

                _logger.LogwriteInfo("SMTP Configuration details fetched successfully------" + Environment.NewLine + "and mail body is----- " + body, _logfilename);
                MailSender objMail = await _emailService.GetMailConfiguration();
                objMail.Receiver = Request.mailrequest.Email;
                objMail.Subject = "Recover Password";
                objMail.Body = body;
                _logger.LogwriteInfo("Before sending the mail for Recover Password------", _logfilename);
                bool result = await _iMailService.EmailSend(objMail);
                if (result)
                {
                    _logger.LogwriteInfo("Mail successfully send------", _logfilename);
                    UserMasterModel userMasterModel = new();
                    userMasterModel.UserId = Request.mailrequest.UserId;
                    userMasterModel.Password = _encryptionSha.EncryptionSha256Hash(otp.ToString());
                    var changeresult = await _updatedata.UpdatePassword(userMasterModel);
                    if (changeresult.ResponseStatus.ResponseCode == 200)
                    {
                        _logger.LogwriteInfo("user predefined password update successfully--------", _logfilename);
                        resp.ResponseStatus.ResponseCode = 200;
                        resp.ResponseStatus.ResponseMessage = "Data Found";
                    }
                    else
                    {
                        resp.ResponseStatus.ResponseCode = changeresult.ResponseStatus.ResponseCode;
                        resp.ResponseStatus.ResponseMessage = changeresult.ResponseStatus.ResponseMessage;
                        _logger.LogwriteInfo("mail send successfully but Predefined password not updated" + Environment.NewLine, _logfilename);
                    }
                }
                else
                {
                    _logger.LogwriteInfo("There is a problem in sending mail------", _logfilename);
                }

                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("exception occur during RecoveryPasswordMailSend command execution" +
                    Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine + ex.StackTrace, _logfilename);
                return resp;
            }
        }
    }
}