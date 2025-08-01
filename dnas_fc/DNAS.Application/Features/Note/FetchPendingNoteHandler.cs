using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.DTO.Note;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace DNAS.Application.Features.Note
{
    public class FetchPendingNoteCommand(NoteModel note): IRequest<PendingNoteModel>
    {
        public NoteModel _note { get; set; } = note;
    }
    internal sealed class FetchPendingNoteHandler(INote iNote, ICustomLogger logger, IEncryption encryption, IHttpContextAccessor haccess) : IRequestHandler<FetchPendingNoteCommand, PendingNoteModel>
    {
        private readonly INote _iNote = iNote;
        public readonly ICustomLogger _logger = logger;
        private readonly IEncryption _encryption= encryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";

        public async Task<PendingNoteModel> Handle(FetchPendingNoteCommand request, CancellationToken cancellationToken)
        {

            PendingNoteModel Response = new();
            try
            {
                var inparam = new
                {
                    @NoteId = Convert.ToInt64(request._note.NoteId),
                    @UserId= Convert.ToInt32(request._note.UserId)
                };
                Response = await _iNote.FetchPendingNote(inparam);                
                if (Response != null)
                {
                    Response.noteModel.NoteId = _encryption.AesEncrypt(Response.noteModel.NoteId.ToString());
                    _logger.LogwriteInfo("Pending note data fetch successfully done", loginUserId);
                    return Response;
                }
                else
                {
                    _logger.LogwriteInfo("Pending note data fetch failed", loginUserId);
                    return new PendingNoteModel();
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("exception occur during FetchPendingNoteCommand execution--- message-"+Environment.NewLine+ex.Message+Environment.NewLine+ex.StackTrace, loginUserId);
                return Response;
            }

        }

    }
}
