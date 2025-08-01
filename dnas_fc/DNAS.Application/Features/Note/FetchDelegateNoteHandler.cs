using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domain.DTO.Note;
using DNAS.Domian.DTO.Note;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Note
{
    public class FetchDelegateNoteCommand(NoteModel note) : IRequest<DelegateNoteModel>
    {
        public NoteModel _note { get; set; } = note;
    }
    internal sealed class FetchDelegateNoteHandler(INote iNote, ICustomLogger logger, IEncryption encryption, IHttpContextAccessor haccess) : IRequestHandler<FetchDelegateNoteCommand, DelegateNoteModel>
    {
        private readonly INote _iNote = iNote;
        public readonly ICustomLogger _logger = logger;
        private readonly IEncryption _encryption = encryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";

        public async Task<DelegateNoteModel> Handle(FetchDelegateNoteCommand request, CancellationToken cancellationToken)
        {

            DelegateNoteModel Response = new();
            try
            {
                var inparam = new
                {
                    @NoteId = Convert.ToInt64(request._note.NoteId),
                    @UserId = Convert.ToInt32(request._note.UserId)
                };
                Response = await _iNote.FetchDelegateNoteDetails(inparam);
                if (Response != null)
                {
                    Response.noteModel.NoteId = _encryption.AesEncrypt(Response.noteModel.NoteId.ToString());
                    if (Response.attachmentsModel.Any())
                    {
                        Response.attachmentsModel = Response.attachmentsModel.Select(e =>
                        {
                            e.AttachmentId = _encryption.AesEncrypt(e.AttachmentId);
                            return e;
                        }).ToList();
                    }
                    _logger.LogwriteInfo("Delegated note data fetch successfully done", loginUserId);
                    return Response;
                }
                else
                {
                    _logger.LogwriteInfo("Delegated note data fetch failed", loginUserId);
                    return new DelegateNoteModel();
                }

            }
            catch (Exception ex)
            {
                _logger.LogwriteError(ex.ToString(), loginUserId);
                return Response;
            }

        }

    }
}
