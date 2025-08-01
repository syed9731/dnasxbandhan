using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.Login;
using MediatR;
using Microsoft.Extensions.Options;

namespace DNAS.Application.Features.Login
{
    public class FetchRecoveryPasswordCommand(RecoverPasswordModel _recover) : IRequest<CommonResponse<RecoverPasswordResponse>>
    {
        public RecoverPasswordModel recover { get; set; } = _recover;
    }
    internal sealed class FetchRecoveryPasswordHandler(ILogin iLogin, ICustomLogger logger, IOptions<AppConfig> appConfig) : IRequestHandler<FetchRecoveryPasswordCommand, CommonResponse<RecoverPasswordResponse>>
    {
        private readonly ILogin _iLogin = iLogin;
        public readonly ICustomLogger _logger = logger;
        public async Task<CommonResponse<RecoverPasswordResponse>> Handle(FetchRecoveryPasswordCommand Request, CancellationToken cancellationToken)
        {
            string logfile = "Login";
            CommonResponse<RecoverPasswordResponse> Response = new();
            try
            {                
                var inparam = new
                {
                    @email = Request.recover.Email
                };
                                
                Response = await _iLogin.getRecoverPassword(inparam);
                if(Convert.ToInt64(Response.Data?.TimeDifference) <= Convert.ToInt64(appConfig.Value.RecoveryMailReSendTimeInMinutes))
                {
                    Response.ResponseStatus.ResponseCode = 400;
                    Response.ResponseStatus.ResponseMessage = "Under recovery range";
                    _logger.LogwriteInfo($"Recovery mail try under predefive recovery time :", logfile);
                }
                else if (Response.Data?.Email!=null && Convert.ToInt64(Response.Data?.TimeDifference) > Convert.ToInt64(appConfig.Value.RecoveryMailReSendTimeInMinutes))
                {
                    Response.ResponseStatus.ResponseCode = 200;
                    Response.ResponseStatus.ResponseMessage = "Data Found";
                    _logger.LogwriteInfo($"Data Found for the email : {Request.recover.Email}  in the Table", logfile);
                }
                else
                {
                    _logger.LogwriteInfo($"No Data Found for the email: {Request.recover.Email} in the Table", logfile);
                }
                return Response;
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("exception occur during FetchRecoveryPasswordCommand execution-----message-"+Environment.NewLine+ex.Message+Environment.NewLine+ex.StackTrace, logfile);
                return Response;
            }
        }
    }
}