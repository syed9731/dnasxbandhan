using DNAS.Application.Common.Interface;
using DNAS.Application.IEntityRepository;
using DNAS.Application.IRepository;
using DNAS.Domain.Common;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.Login;
using MediatR;
using Microsoft.Extensions.Options;

namespace DNAS.Application.Features.Login
{
    public class CheckLdapLoginCommand(UserMasterModel _userMaster) : IRequest<CommonResponse<UserMasterResponse>>
    {
        public UserMasterModel UserMaster { get; set; } = _userMaster;
    }
    internal sealed class CheckLdapLoginHandler(ILogin iLogin, ICustomLogger logger, IEncryption enc, IEncryptionSha pasenc, IOptions<AppConfig> appConfig, ILdapCheck ildapCheck) : IRequestHandler<CheckLdapLoginCommand, CommonResponse<UserMasterResponse>>
    {
        private readonly ILogin _iLogin = iLogin;
        public readonly ICustomLogger _logger = logger;
        public readonly IEncryption _encryption = enc;
        public readonly IEncryptionSha _encryptionSha = pasenc;
        private readonly ILdapCheck _ildapCheck = ildapCheck;
        private readonly string _logpathPrefix = "User_";
        public async Task<CommonResponse<UserMasterResponse>> Handle(CheckLdapLoginCommand Request, CancellationToken cancellationToken)
        {
            CommonResponse<UserMasterResponse> Response = new();
            try
            {

                #region LDAP Login
                _logger.LogwriteInfo("------Ldap login start-----------" + Request.UserMaster.UserName, "Ldap");
                string ldappath = appConfig.Value.AdPath;
                _logger.LogwriteInfo("AdPath is- " + ldappath, "Ldap");
                string adDomain = appConfig.Value.Ad_Domain;
                _logger.LogwriteInfo("AdDomain is- " + adDomain, "Ldap");
                string domainAndUsername = "";
                _logger.LogwriteInfo("domainAndUsername format- adDomain + @ and single back-slash + Request.UserMaster.UserName", "Ldap");
                domainAndUsername = adDomain + @"\" + Request.UserMaster.UserName;
                _logger.LogwriteInfo("domainAndUsername- " + domainAndUsername, "Ldap");
                bool result = await _ildapCheck.CheckLdapUser(ldappath, Request.UserMaster.UserName, Request.UserMaster.Password, domainAndUsername);
                _logger.LogwriteInfo("LDAP check end and login response-" + result + " for username-" + Request.UserMaster.UserName, "Ldap");
                #endregion

                if (result)
                {
                    var inparam = new
                    {
                        @UserName = Request.UserMaster.UserName
                    };
                    Response = await _iLogin.CheckUserExists(inparam);
                    if (Response.Data?.UserId != 0)
                    {
                        Response.ResponseStatus.ResponseCode = 200;
                        _logger.LogwriteInfo($"Data Found of the User : {Request.UserMaster.UserName}  in the Table", _logpathPrefix + Response.Data?.UserId.ToString());
                    }
                    else
                    {
                        Response.ResponseStatus.ResponseCode = 3;
                        Response.ResponseStatus.ResponseMessage = CommonMsg.LdapSuccessNotInDnas;
                        _logger.LogwriteInfo("Ldap login is success but user not present in DNAS system with username-" + Request.UserMaster.UserName, _logpathPrefix + Response.Data?.UserId.ToString());
                    }
                }
                else
                {
                    Response.ResponseStatus.ResponseCode = 4;
                    Response.ResponseStatus.ResponseMessage = CommonMsg.InvalidLdapCredential;
                    _logger.LogwriteInfo("Ldap login is failed with username-" + Request.UserMaster.UserName, _logpathPrefix + Response.Data?.UserId.ToString());
                }

                return Response;
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("Exception occur during CheckLdapLoginHandler execution-------message-" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace, "Login");
                return Response;
            }
        }
        
    }
}