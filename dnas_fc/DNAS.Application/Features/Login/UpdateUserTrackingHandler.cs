using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domain.DTO.Login;
using DNAS.Domian.DTO.Login;
using MediatR;

namespace DNAS.Application.Features.Login
{
    public class UpdateUserTrackingCommand(UserMasterModel _userMaster) : IRequest<UserMasterModel>
    {
        public UserMasterModel UserMaster { get; set; } = _userMaster;
    }
    internal sealed class UpdateUserTrackingHandler(IUpdate iUpdate, ICustomLogger logger, ILogin iLogin) : IRequestHandler<UpdateUserTrackingCommand, UserMasterModel>
    {
        private readonly IUpdate _iUpdate = iUpdate;
        public readonly ICustomLogger _logger = logger;
        private readonly ILogin _iLogin = iLogin;
        public async Task<UserMasterModel> Handle(UpdateUserTrackingCommand Request, CancellationToken cancellationToken)
        {            
            UserMasterModel userdata = new();
            try
            {
                UserTrackingModel userTrackingModel = new();
                userTrackingModel.UserId = Convert.ToInt32(Request.UserMaster.UserId);
                userTrackingModel.SessionId = Guid.NewGuid().ToString();
                if (await _iUpdate.UpdateUserTracking(userTrackingModel))
                {
                    var inparam = new
                    {
                        @UserId = userTrackingModel.UserId
                    };
                    userdata = await _iLogin.FetchUserData(inparam);
                    userdata.SessionId = userTrackingModel.SessionId;
                    _logger.LogwriteInfo($"UserTracking Created", "User_" + Request.UserMaster.UserId.ToString());
                    return userdata;
                }
                else
                {
                    userdata.SessionId = "";
                    _logger.LogwriteInfo($"Something Went wrong UserTracking not update", "User_" + Request.UserMaster.UserId.ToString());
                    return userdata;
                }
                
            }
            catch (Exception ex)
            {
                userdata.SessionId = "";                
                _logger.LogwriteInfo("exception occur during UpdateUserTrackingCommand execution" +
                    Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine + ex.StackTrace, "User_"+ Request.UserMaster.UserId);
                return userdata;
            }
        }
    }
}