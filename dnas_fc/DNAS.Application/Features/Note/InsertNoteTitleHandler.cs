using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.DTO.Note;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace DNAS.Application.Features.Note
{
    public class InsertNoteTitleCommand(NoteModel note):IRequest<NoteModel>
    {
        public NoteModel _note { get; set; } = note;
    }
    internal sealed class InsertNoteTitleHandler(ISave iISave, ICustomLogger logger, IEncryption iEncryption, IHttpContextAccessor haccess) : IRequestHandler<InsertNoteTitleCommand, NoteModel>
    {
        private readonly ISave _iISave = iISave;
        public readonly ICustomLogger _logger = logger;
        private readonly IEncryption _iEncryption = iEncryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<NoteModel> Handle(InsertNoteTitleCommand request, CancellationToken cancellationToken)
        {

            NoteModel Response = new();
            try
            {
                NoteModel note=new NoteModel();
                note.NoteTitle=request._note.NoteTitle;
                note.UserId=request._note.UserId;
                Response = await _iISave.InsertNoteTitleData(note);
                
                if (Response !=null)
                {
                    Response.NoteId = _iEncryption.AesEncrypt(Response.NoteId);
                    _logger.LogwriteInfo("Update Note command successfully done", loginUserId);
                    return Response;
                }
                else
                {
                    _logger.LogwriteInfo("Update Note command failed", loginUserId);
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
