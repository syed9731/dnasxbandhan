using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domain.DTO.Note;
using DNAS.Domian.DTO.Note;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Note
{
    public class FetchApprovalRequestCommand(NoteModel note) : IRequest<RequestApproverNoteModel>
    {
        public NoteModel _note { get; set; } = note;
    }
    internal sealed class FetchApprovalRequestHandler(INote iNote, ICustomLogger logger, IEncryption encryption, IHttpContextAccessor haccess) : IRequestHandler<FetchApprovalRequestCommand, RequestApproverNoteModel>
    {
        private readonly INote _iNote = iNote;
        public readonly ICustomLogger _logger = logger;
        private readonly IEncryption _encryption = encryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";

        public async Task<RequestApproverNoteModel> Handle(FetchApprovalRequestCommand request, CancellationToken cancellationToken)
        {

            RequestApproverNoteModel Response = new();
            try
            {
                var inparam = new
                {
                    @NoteId = Convert.ToInt64(request._note.NoteId),
                    @UserId = Convert.ToInt32(request._note.UserId),
                    @IsNoteApprovedDataExist= false
                };
                //Response = await _iNote.FetchApprovalRequestNote(inparam);
                Response = await _iNote.FetchApprovalRequestNote(request._note.NoteId, request._note.UserId);
                if (Response != null)
                {
                    //AttachmentId encryption start
                    if (Response.attachmentsModel.Any())
                    {
                        Response.attachmentsModel = Response.attachmentsModel.Select(e =>
                        {
                            e.AttachmentId = _encryption.AesEncrypt(e.AttachmentId);
                            return e;
                        }).ToList();
                    }
                    //AttachmentId encryption end
                    Response.noteModel.NoteId = _encryption.AesEncrypt(Response.noteModel.NoteId.ToString());
                    _logger.LogwriteInfo("Pending note data fetch successfully done", loginUserId);
                    return Response;
                }
                else
                {
                    _logger.LogwriteInfo("Pending note data fetch failed", loginUserId);
                    return new RequestApproverNoteModel();
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
