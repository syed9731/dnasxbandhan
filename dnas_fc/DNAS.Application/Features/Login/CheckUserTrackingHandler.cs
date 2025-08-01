using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domain.DTO.Login;
using DNAS.Domian.DTO.Login;
using MediatR;

namespace DNAS.Application.Features.Login
{
    public class CheckUserTrackingCommand(UserMasterModel _userMaster) : IRequest<UserTrackingModel>
    {
        public UserMasterModel UserMaster { get; set; } = _userMaster;
    }
    internal sealed class CheckUserTrackingHandler(ILogin iLogin, ICustomLogger logger, IEncryption enc, IEncryptionSha pasenc) : IRequestHandler<CheckUserTrackingCommand, UserTrackingModel>
    {
        private readonly ILogin _iLogin = iLogin;
        public readonly ICustomLogger _logger = logger;
        public readonly IEncryption _encryption = enc;
        public readonly IEncryptionSha _encryptionSha = pasenc;
        public async Task<UserTrackingModel> Handle(CheckUserTrackingCommand Request, CancellationToken cancellationToken)
        {
            UserTrackingModel Response = new();
            try
            {
                var inparam = new
                {
                    @Password = _encryptionSha.EncryptionSha256Hash(Request.UserMaster.Password),
                    @UserName = Request.UserMaster.UserName
                };

                Response = await _iLogin.CheckUsertracking(inparam);
                Response.TimeDifference= Convert.ToInt32(DateTime.Now.Subtract(Response.LastLoginTime).TotalMinutes);
                if (Response.UserId!=0)
                {                    
                    _logger.LogwriteInfo($"UserTracking exists against : {Request.UserMaster.UserName}  in the Table", Request.UserMaster.UserName);
                }
                else
                {
                    _logger.LogwriteInfo($"UserTracking not exists against: {Request.UserMaster.UserName} in the Table. So user can enter for login", Request.UserMaster.UserName);                    
                }
                return Response;
            }
            catch (Exception ex)
            {
                _logger.LogwriteError(ex.ToString(), Request.UserMaster.UserName);
                return Response;
            }
        }
    }
}