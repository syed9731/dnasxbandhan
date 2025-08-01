using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.DTO.Note;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace DNAS.Application.Features.Note
{
    public class UpdateNoteCapexCommand(NoteModel note):IRequest<bool>
    {
        public NoteModel _note { get; set; } = note;
    }
    internal sealed class UpdateNoteCapexHandler(IUpdate iupdate, ICustomLogger logger,IEncryption iEncryption, IHttpContextAccessor haccess) : IRequestHandler<UpdateNoteCapexCommand, bool>
    {
        private readonly IUpdate _Update = iupdate;
        public readonly ICustomLogger _logger = logger;
        private readonly IEncryption _iEncryption = iEncryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<bool> Handle(UpdateNoteCapexCommand request, CancellationToken cancellationToken)
        {

            bool Response = false;
            try
            {
                NoteModel note=new NoteModel();
                note.NoteId = _iEncryption.AesDecrypt(request._note.NoteId);
                note.CapitalExpenditure = request._note.CapitalExpenditure;
                note.TotalAmount = request._note.TotalAmount;
                Response = await _Update.UpdateNoteCapexData(note);

                if (Response)
                {
                    _logger.LogwriteInfo("Update Note Nature Of Expenses command successfully done", loginUserId);
                    Response = true;
                }
                else
                {
                    _logger.LogwriteInfo("Update Note Nature Of Expenses command failed", loginUserId);
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
