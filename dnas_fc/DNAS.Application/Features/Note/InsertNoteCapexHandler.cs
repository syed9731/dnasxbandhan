using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.DTO.Note;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace DNAS.Application.Features.Note
{
    public class InsertNoteCapexCommand(NoteModel note):IRequest<NoteModel>
    {
        public NoteModel _note { get; set; } = note;
    }
    internal sealed class InsertNoteCapexHandler(ISave iISave, ICustomLogger logger, IEncryption iEncryption, IHttpContextAccessor haccess) : IRequestHandler<InsertNoteCapexCommand, NoteModel>
    {
        private readonly ISave _iISave = iISave;
        public readonly ICustomLogger _logger = logger;
        private readonly IEncryption _iEncryption = iEncryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<NoteModel> Handle(InsertNoteCapexCommand request, CancellationToken cancellationToken)
        {

            NoteModel Response = new();
            try
            {
                NoteModel note=new NoteModel();
                note.CapitalExpenditure=request._note.CapitalExpenditure;
                note.UserId=request._note.UserId;
                Response = await _iISave.InsertNoteCapexData(note);
                
                if (Response !=null)
                {
                    Response.NoteId = _iEncryption.AesEncrypt(Response.NoteId);
                    _logger.LogwriteInfo("Update Note command successfully done", loginUserId);
                    return Response;
                }
                else
                {
                    _logger.LogwriteInfo("Update Note command failed", loginUserId);
                    return Response=new();
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
