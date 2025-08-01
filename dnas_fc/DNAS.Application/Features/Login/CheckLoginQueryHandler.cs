using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.Login;
using MediatR;

namespace DNAS.Application.Features.Login
{
    public class CheckLoginQueryCommand(UserMasterModel _userMaster) : IRequest<CommonResponse<UserMasterResponse>>
    {
        public UserMasterModel UserMaster { get; set; } = _userMaster;
    }
    internal sealed class CheckLoginQueryCommandHandler(ILogin iLogin, ICustomLogger logger, IEncryption enc, IEncryptionSha pasenc) : IRequestHandler<CheckLoginQueryCommand, CommonResponse<UserMasterResponse>>
    {
        private readonly ILogin _iLogin = iLogin;
        public readonly ICustomLogger _logger = logger;
        public readonly IEncryption _encryption = enc;
        public readonly IEncryptionSha _encryptionSha = pasenc;
        private readonly string _logpathPrefix = "User_";
        public async Task<CommonResponse<UserMasterResponse>> Handle(CheckLoginQueryCommand Request, CancellationToken cancellationToken)
        {
            CommonResponse<UserMasterResponse> Response = new();
            try
            {
                var inparam = new
                {
                    @Password = _encryptionSha.EncryptionSha256Hash(Request.UserMaster.Password),
                    @UserName = Request.UserMaster.UserName
                };

                Response = await _iLogin.getMasterData(inparam);


                if (Response.Data?.UserId != 0)
                {                    
                    _logger.LogwriteInfo($"Data Found in the User : {Request.UserMaster.UserName}  in the Table", _logpathPrefix + Response.Data?.UserId.ToString());
                    Response.ResponseStatus.ResponseCode = 200;









                    //var userinparam = new
                    //{
                    //    @UserId = Response.Data?.UserId
                    //};
                    //UserTrackingModel resp = await _iLogin.Usertracking(userinparam);

                    //if (resp.UserId==0)
                    //{
                    //    Response.ResponseStatus.ResponseCode = 200;
                    //    Response.ResponseStatus.ResponseMessage = "Data Not Exists in Usertraking";
                    //    Response.Data!.LastloginTime = resp.LastLoginTime;
                    //    Response.Data.UserId = resp.UserId;
                    //    _logger.LogwriteInfo($"Data Found in the User : {Request.UserMaster.UserName}  in the Table", _logpathPrefix + Response.Data?.UserId.ToString());
                    //}
                    //else
                    //{
                    //    Response.ResponseStatus.ResponseCode = 300;
                    //    Response.ResponseStatus.ResponseMessage = "Data exists";
                    //    Response.Data!.LastloginTime = resp.LastLoginTime;
                    //    Response.Data.UserId = resp.UserId;
                    //    _logger.LogwriteInfo($"Data Found in the User : {Request.UserMaster.UserName}  in the Table", _logpathPrefix + Response.Data?.UserId.ToString());
                    //}
                }
                else
                {
                    Response.ResponseStatus.ResponseCode = 404;
                    _logger.LogwriteInfo($"No Data Found in the User: {Request.UserMaster.UserName} in the Table", "Login");
                }
                return Response;
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("Exception occur during FetchLoginQueryCommand execution-------message-"+Environment.NewLine+ex.Message+Environment.NewLine+ex.StackTrace, "Login");
                return Response;
            }
        }
    }
}