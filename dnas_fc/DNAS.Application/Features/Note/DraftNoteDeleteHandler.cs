using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Note
{
    public class DraftNoteDeleteCommand(int Noteid) : IRequest<bool>
    {
        public int Noteid { get; } = Noteid;
    }

    internal sealed class DraftNoteDeleteCommandHandler(INote iNote, ICustomLogger logger, IHttpContextAccessor haccess) : IRequestHandler<DraftNoteDeleteCommand, bool>
    {
        private readonly INote _iNote = iNote;
        public readonly ICustomLogger _logger = logger;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<bool> Handle(DraftNoteDeleteCommand request, CancellationToken cancellationToken)
        {
            bool result = false;
            try
            {

                int response = await _iNote.DeleteDraftNote(request.Noteid);

                if (response > 0)
                {
                    _logger.LogwriteInfo("Draft Note fetch successfullye", loginUserId);
                    result = true;
                }
                else
                {
                    _logger.LogwriteInfo("Draft Note fetch failed", loginUserId);
                    result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogwriteError(ex.ToString(), loginUserId);
                return result;
            }
        }
    }
}
