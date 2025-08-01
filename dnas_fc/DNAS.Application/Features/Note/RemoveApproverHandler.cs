using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.DTO.Note;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Note
{
    public class RemoveApproverCommand(ApproverModel appro) : IRequest<string>
    {
        public ApproverModel _approver { get; set; } = appro;
    }
    internal sealed class RemoveApproverHandler(IDelete iDelete, ICustomLogger logger, IEncryption iEncryption, IHttpContextAccessor haccess) : IRequestHandler<RemoveApproverCommand, string>
    {
        private readonly IDelete _iDelete = iDelete;
        public readonly ICustomLogger _logger = logger;
        private readonly IEncryption _iEncryption = iEncryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<string> Handle(RemoveApproverCommand request, CancellationToken cancellationToken)
        {
            string Response = "";
            try
            {
                request._approver.NoteId = _iEncryption.AesDecrypt(request._approver.NoteId);
                Response = await _iDelete.DeleteApproverData(request._approver);
                if (Response == "success")
                {
                    _logger.LogwriteError("Data deleted successfully", loginUserId);
                }
                else
                {
                    _logger.LogwriteError("Data not deleted properly", loginUserId);
                }
            }
            catch (Exception e)
            {                
                _logger.LogwriteInfo("exception occur during RemoveApproverCommand------ " + e.Message + Environment.NewLine + e.StackTrace, string.IsNullOrEmpty(haccess.HttpContext?.User.FindFirstValue("UserId")) ? "Login" : loginUserId);
                return Response;
            }
            return Response;
        }

    }
}
