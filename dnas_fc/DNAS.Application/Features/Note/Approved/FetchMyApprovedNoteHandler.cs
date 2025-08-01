using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domain.DTO.Note;
using DNAS.Domian.DTO.Note;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace DNAS.Application.Features.Note.Approved
{
    public class FetchMyApprovedNoteCommand(NoteModel note): IRequest<MyApprovedNoteModel>
    {
        public NoteModel _note { get; set; } = note;
    }
    internal sealed class FetchApproverApprovedNoteHandler(INote iNote, ICustomLogger logger, IEncryption encryption, IHttpContextAccessor haccess) : IRequestHandler<FetchMyApprovedNoteCommand, MyApprovedNoteModel>
    {
        private readonly INote _iNote = iNote;
        public readonly ICustomLogger _logger = logger;
        private readonly IEncryption _encryption= encryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";

        public async Task<MyApprovedNoteModel> Handle(FetchMyApprovedNoteCommand request, CancellationToken cancellationToken)
        {

            MyApprovedNoteModel Response = new();
            try
            {
                var inparam = new
                {
                    @NoteId = Convert.ToInt64(request._note.NoteId),
                    @UserId= Convert.ToInt32(request._note.UserId)
                };
                //Response = await _iNote.FetchMyApprovedNote(inparam);
                Response = await _iNote.FetchMyApprovedNote(request._note.NoteId, request._note.UserId);
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
                    _logger.LogwriteInfo("withdraw note data fetch successfully done", loginUserId);
                    return Response;
                }
                else
                {
                    _logger.LogwriteInfo("Withdraw note data fetch failed", loginUserId);
                    return new MyApprovedNoteModel();
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("exception occur during FetchMyApprovedNoteHandler execution--- message-" + Environment.NewLine+ex.Message+Environment.NewLine+ex.StackTrace, loginUserId);
                return Response;
            }

        }

    }
}
