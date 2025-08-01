using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.Login;
using MediatR;


namespace DNAS.Application.Features.Login
{
    public class UserVerificationCommand(UserVerivicationReqModel _userverify) : IRequest<CommonResponse<ChangePasswordModel>>
    {
        public UserVerivicationReqModel userVerify { get; set; } = _userverify;
    }
    internal sealed class UserVerificationHandler(ICustomLogger logger, IEncryption enc, IEncryptionSha encryptionSha, IDapperFactory dapperFactory) : IRequestHandler<UserVerificationCommand, CommonResponse<ChangePasswordModel>>
    {        
        public readonly ICustomLogger _logger = logger;
        public readonly IEncryption _encryption = enc;
        private readonly IEncryptionSha _encryptionSha = encryptionSha;
        private readonly IDapperFactory _dapperFactory = dapperFactory;
        public async Task<CommonResponse<ChangePasswordModel>> Handle(UserVerificationCommand Request, CancellationToken cancellationToken)
        {
            CommonResponse<ChangePasswordModel> Response = new();
            try
            {
                string[] valSplit = _encryption.AesDecrypt(Request.userVerify.UserId).Split('/');
                var inparam = new
                {
                    @userid = valSplit[0],
                    @otp = _encryptionSha.EncryptionSha256Hash(valSplit[1]),
                };
                
                Response = await _dapperFactory.ExecuteSpDapperAsync<ChangePasswordModel, CommonResponse<ChangePasswordModel>>(OraStoredProcedureNames.ProcVerifyRecoverPasswordUser, inparam);

                if (!string.IsNullOrEmpty(Response.Data?.UserId))
                {
                    Response.Data.UserId = _encryption.AesEncrypt(Response.Data.UserId);
                    Response.ResponseStatus.ResponseCode = 200;
                    Response.ResponseStatus.ResponseMessage = "Data Found";
                    _logger.LogwriteInfo("User verification successfully done", "Login");
                }
                else
                {
                    _logger.LogwriteInfo("User verification failed", "Login");
                }
                return Response;
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("exception occur during UserVerificationCommand execution" +
                    Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine + ex.StackTrace, "Login");
                return Response;
            }
        }
    }
}