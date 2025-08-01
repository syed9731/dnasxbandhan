using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.DTO.Note;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace DNAS.Application.Features.Note
{
    public class UpdateNoteTitleCommand(NoteModel note):IRequest<bool>
    {
        public NoteModel _note { get; set; } = note;
    }
    internal sealed class UpdateNoteTitleHandler(IUpdate iupdate, ICustomLogger logger, IEncryption iEncryption, IHttpContextAccessor haccess) : IRequestHandler<UpdateNoteTitleCommand, bool>
    {
        private readonly IUpdate _Update = iupdate;
        public readonly ICustomLogger _logger = logger;
        private readonly IEncryption _iEncryption = iEncryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<bool> Handle(UpdateNoteTitleCommand request, CancellationToken cancellationToken)
        {

            bool Response = false;
            try
            {
                NoteModel note=new NoteModel();
                note.NoteId= _iEncryption.AesDecrypt(request._note.NoteId);
                note.NoteTitle=request._note.NoteTitle;
                Response = await _Update.UpdateNoteTitleData(note);

                if (Response)
                {
                    _logger.LogwriteInfo("Update Note command successfully done", loginUserId);
                    Response=true;
                }
                else
                {
                    _logger.LogwriteInfo("Update Note command failed", loginUserId);
                }
                return Response;
            }
            catch (Exception ex)
            {
                _logger.LogwriteError(ex.ToString(), loginUserId);
                return Response;
            }

        }

    }
}
