using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domain.DTO.Amendment;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Note.Amendment
{
    public record FetchAmendmentCommand(string NoteId) : IRequest<NoteAmendmentModel>
    {
    }
    internal sealed class FetchAmendmentHandler(ICustomLogger logger,IEncryption encryption,IHttpContextAccessor haccess, INote inote) : IRequestHandler<FetchAmendmentCommand, NoteAmendmentModel>
    {        
        private readonly INote _iNote = inote;
        public readonly ICustomLogger _logger = logger;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";

        public async Task<NoteAmendmentModel> Handle(FetchAmendmentCommand request, CancellationToken cancellationToken)
        {

            NoteAmendmentModel Response = new();
            try
            {
                var inparam = new
                {
                    @NoteId = request.NoteId
                };
                Response = await _iNote.FetchAmendmentData(inparam);

                if (Response != null)
                {
                    _logger.LogwriteInfo("Fetch Amendment Note Data command successfully done", loginUserId);
                    return Response;
                }
                else
                {
                    _logger.LogwriteInfo("Fetch Amendment Note Data command failed", loginUserId);
                    return Response = new();
                }

            }
            catch (Exception ex)
            {
                _logger.LogwriteError("exception occur during FetchAmendmentHandler execution----message"+ ex.Message+Environment.NewLine+ex.StackTrace, loginUserId);
                return Response;
            }

        }
    }
}
