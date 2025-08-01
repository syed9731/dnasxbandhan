using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.Login;
using MediatR;

namespace DNAS.Application.Features.Login
{
    public class LoginChangePasswordCommand(ChangePasswordModel user) : IRequest<CommonResponse<UserMasterResponse>>
    {
        public ChangePasswordModel usermaster { get; set; } = user;
    }
    internal class LoginChangePasswordHandler(ILogin iLogin, ICustomLogger logger, IEncryption enc, IEncryptionSha pasenc, IUpdate iupdate) : IRequestHandler<LoginChangePasswordCommand, CommonResponse<UserMasterResponse>>
    {
        private readonly ILogin _iLogin = iLogin;
        public readonly ICustomLogger _logger = logger;
        public readonly IEncryption _encryption = enc;
        public readonly IEncryptionSha _encryptionSha = pasenc;
        public readonly IUpdate _updatedata = iupdate;
        private readonly string _logpathPrefix = "User_";
        public async Task<CommonResponse<UserMasterResponse>> Handle(LoginChangePasswordCommand Request, CancellationToken cancellationToken)
        {
            CommonResponse<UserMasterResponse> Response = new();
            try
            {
                Request.usermaster.UserId = _encryption.AesDecrypt(Request.usermaster.UserId);


                var inparam = new
                {
                    @Password = _encryptionSha.EncryptionSha256Hash(Request.usermaster.predefinedpassword),
                    @UserId = Request.usermaster.UserId
                };

                Response = await _iLogin.checkPredefinePassword(inparam);

                if (Response.Data?.UserId != 0)
                {
                    _logger.LogwriteInfo("Predefined Password matched for the user -- ", _logpathPrefix + Request.usermaster.UserId);
                    UserMasterModel userMasterModel = new();
                    userMasterModel.Password = _encryptionSha.EncryptionSha256Hash(Request.usermaster.Password);
                    userMasterModel.UserId = Convert.ToInt32(Request.usermaster.UserId);                    
                    var changeresult = await _updatedata.UpdatePassword(userMasterModel);
                    if (changeresult.ResponseStatus.ResponseCode == 200)
                    {
                        _logger.LogwriteInfo("Password update successfully for the user -- ", _logpathPrefix + Request.usermaster.UserId);
                        Response.ResponseStatus.ResponseCode = 200;
                        Response.ResponseStatus.ResponseMessage = "Password Changes successfully";
                        _logger.LogwriteInfo($"Password Changes successfully", _logpathPrefix + Request.usermaster.UserId);
                    }
                    else
                    {
                        Response.ResponseStatus.ResponseCode = changeresult.ResponseStatus.ResponseCode;
                        Response.ResponseStatus.ResponseMessage = changeresult.ResponseStatus.ResponseMessage;
                        _logger.LogwriteInfo("Password not update for the user ---", _logpathPrefix + Request.usermaster.UserId);
                    }
                }
                else
                {
                    _logger.LogwriteInfo("Predefined Password not matched for the user ---", _logpathPrefix + Request.usermaster.UserId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("exception occur during LoginChangePasswordCommand execution for userid--" + _logpathPrefix + Request.usermaster.UserId+
                    Environment.NewLine+ "exception message-"+ex.Message+Environment.NewLine+ex.StackTrace, "Login");
                return new CommonResponse<UserMasterResponse>();
            }
            return Response;
        }
    }
}
