using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Note.Amendment
{
    public record ResetPreviousNoteDetailsCommand(string NoteId) : IRequest<bool>
    {
    }
    internal sealed class ResetPreviousNoteDetailsHandler(ICustomLogger logger,IEncryption encryption,IHttpContextAccessor haccess, IUpdate iupdate) : IRequestHandler<ResetPreviousNoteDetailsCommand, bool>
    {        
        private readonly IUpdate _iUpdate = iupdate;
        public readonly ICustomLogger _logger = logger;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";

        public async Task<bool> Handle(ResetPreviousNoteDetailsCommand request, CancellationToken cancellationToken)
        {

            try
            {
                string inparam = encryption.AesDecrypt(request.NoteId);
                bool Response = await _iUpdate.ResetPreviousNoteDetails(inparam);

                if (Response)
                {
                    _logger.LogwriteInfo("Fetch Amendment Note Data command successfully done", loginUserId);
                    return true;
                }
                else
                {
                    _logger.LogwriteInfo("Fetch Amendment Note Data command failed", loginUserId);
                    return false;
                }

            }
            catch (Exception ex)
            {
                _logger.LogwriteError("exception occur during FetchAmendmentHandler execution----message"+ ex.Message+Environment.NewLine+ex.StackTrace, loginUserId);
                return false;
            }

        }
    }
}
