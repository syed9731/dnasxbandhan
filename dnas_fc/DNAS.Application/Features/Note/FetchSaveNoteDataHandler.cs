using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.DTO.Note;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Note
{
    public class FetchSaveNoteDataCommand(NoteModel noteModel) : IRequest<DraftNoteModel>
    {
        public NoteModel _note { get; set; } = noteModel;
    }
    internal sealed class FetchSaveNoteDataHandler(INote iNote, ICustomLogger logger, IHttpContextAccessor haccess) : IRequestHandler<FetchSaveNoteDataCommand, DraftNoteModel>
    {
        private readonly INote _iNote = iNote;
        public readonly ICustomLogger _logger = logger;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";

        public async Task<DraftNoteModel> Handle(FetchSaveNoteDataCommand request, CancellationToken cancellationToken)
        {

            DraftNoteModel Response = new();
            try
            {
                var inparam = new
                {
                    @NoteId = request._note.NoteId,
                };
                Response = await _iNote.FetchSaveNoteData(inparam);

                if (Response != null)
                {
                    _logger.LogwriteInfo("Save Note Data command successfully done", loginUserId);
                    return Response;
                }
                else
                {
                    _logger.LogwriteInfo("Save Note Data command failed", loginUserId);
                    return Response = new();
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
