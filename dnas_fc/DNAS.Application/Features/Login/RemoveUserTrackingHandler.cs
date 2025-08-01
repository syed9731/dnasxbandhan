using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.DTO.Login;
using MediatR;

namespace DNAS.Application.Features.Login
{
    public class RemoveUserTrackingCommand(UserMasterModel _userMaster) : IRequest<bool>
    {
        public UserMasterModel UserMaster { get; set; } = _userMaster;
    }
    internal sealed class RemoveUserTrackingHandler(IDelete iDelete, ICustomLogger logger) : IRequestHandler<RemoveUserTrackingCommand, bool>
    {
        private readonly IDelete _iDelete = iDelete;
        public readonly ICustomLogger _logger = logger;
        public async Task<bool> Handle(RemoveUserTrackingCommand Request, CancellationToken cancellationToken)
        {            
            try
            {
                var inparam = new
                {
                    @UserId = Request.UserMaster.UserId
                };

                bool Response = await _iDelete.DeleteUserTracking(inparam);

                if (Response)
                {                    
                    _logger.LogwriteInfo($"UserTracking Deleted Successfully", "User_"+ Request.UserMaster.UserId);
                    return true;
                }
                else
                {
                    _logger.LogwriteInfo($"UserTracking Delete Failed", "User_"+ Request.UserMaster.UserId);
                    return false;
                }
                
            }
            catch (Exception ex)
            {                
                _logger.LogwriteInfo("exception occur during RemoveUserTrackingCommand execution for userid--" + Request.UserMaster.UserId +
                    Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine + ex.StackTrace, "User_"+ Request.UserMaster.UserId);
                return true;
            }
        }
    }
}