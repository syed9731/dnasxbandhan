using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domain.DTO.Login;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.Login;
using MediatR;
using System.Text.Json;

namespace DNAS.Application.Features.Login
{
    public class FetchLoginQueryCommand(UserMasterModel _userMaster) : IRequest<CommonResponse<UserMasterResponse>>
    {
        public UserMasterModel UserMaster { get; set; } = _userMaster;
    }
    internal sealed class FetchLoginQueryCommandHandler(ILogin iLogin, ICustomLogger logger, IEncryption enc, IEncryptionSha pasenc, ISave iSave, IUpdate iUpdate) : IRequestHandler<FetchLoginQueryCommand, CommonResponse<UserMasterResponse>>
    {
        private readonly ILogin _iLogin = iLogin;
        public readonly ICustomLogger _logger = logger;
        public readonly IEncryption _encryption = enc;
        public readonly IEncryptionSha _encryptionSha = pasenc;
        private readonly ISave _iSave = iSave;
        private readonly IUpdate _iUpdate = iUpdate;
        private readonly string _logpathPrefix = "User_";
        private readonly string _logpath="Login";
        public async Task<CommonResponse<UserMasterResponse>> Handle(FetchLoginQueryCommand Request, CancellationToken cancellationToken)
        {
            CommonResponse<UserMasterResponse> Response = new();
            try
            {
                //Login type GROUP means users who have no LDAP credential. Login based on Portal username and Password
                
                    var inparam = new
                    {                        
                        @UserName = Request.UserMaster.UserName
                    };
                    Response = await _iLogin.getUserDataByUserId(inparam);
                    if (Response.Data?.UserId != 0)
                    {
                        _logger.LogwriteInfo($"Data Found in the User : {Request.UserMaster.UserName}  in the Table", _logpathPrefix + Response.Data?.UserId.ToString());
                        Response = await CheckAndModifyUserTracking(Response, Request);
                    }
                    else
                    {
                        _logger.LogwriteInfo($"No Data Found in the User: {Request.UserMaster.UserName} in the Table", _logpath);
                    }
                
                return Response;
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("Exception occur during FetchLoginQueryCommand execution-------message-" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace, "Login");
                return Response;
            }
        }
        public async Task<CommonResponse<UserMasterResponse>> CheckAndModifyUserTracking(CommonResponse<UserMasterResponse> Response, FetchLoginQueryCommand Request)
        {
            var userinparam = new
            {
                @UserId = Response.Data?.UserId
            };
            UserTrackingModel resp = await _iLogin.CheckUsertracking(userinparam);

            if (resp.SessionId == "")
            {
                UserTrackingModel userTrackingModel = new();
                userTrackingModel.UserId = Convert.ToInt32(Response.Data?.UserId);
                userTrackingModel.SessionId = Guid.NewGuid().ToString();
                _logger.LogwriteInfo($"SaveUserTracking pass datamodel : {JsonSerializer.Serialize(userTrackingModel)}", _logpathPrefix + Response.Data?.UserId.ToString());
                if (await _iSave.SaveUserTracking(userTrackingModel))
                {
                    await _iUpdate.UpdateLatestLoginTime(userTrackingModel.UserId);
                    Response.ResponseStatus.ResponseCode = 200;
                    Response.ResponseStatus.ResponseMessage = "Data Found";
                    Response.Data!.SessionId = userTrackingModel.SessionId;
                    _logger.LogwriteInfo($"UserTracking Created for the user : {Request.UserMaster.UserName}  in the Table", _logpathPrefix + Response.Data?.UserId.ToString());
                }
                else
                {
                    Response.Data!.SessionId = "";
                    _logger.LogwriteInfo($"Something Went wrong UserTracking not created for the user : {Request.UserMaster.UserName}  in the Table", _logpathPrefix + Response.Data?.UserId.ToString());
                }
            }
            else
            {
                Response.ResponseStatus.ResponseCode = 300;
                Response.ResponseStatus.ResponseMessage = "Data exists";
                Response.Data!.LastloginTime = resp.LastLoginTime;
                Response.Data.UserId = resp.UserId;
                _logger.LogwriteInfo($"Data Found in the User : {Request.UserMaster.UserName}  in the Table", _logpathPrefix + Response.Data?.UserId.ToString());
            }
            return Response;
        }
	}
}