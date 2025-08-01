using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.DTO.Note;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace DNAS.Application.Features.Note
{
    public class UpdateNoteNetureOfExpenseCommand(NoteModel note):IRequest<bool>
    {
        public NoteModel _note { get; set; } = note;
    }
    internal sealed class UpdateNoteNetureOfExpenseHandler(IUpdate iupdate, ICustomLogger logger, IEncryption iEncryption, IHttpContextAccessor haccess) : IRequestHandler<UpdateNoteNetureOfExpenseCommand, bool>
    {
        private readonly IUpdate _Update = iupdate;
        public readonly ICustomLogger _logger = logger;
        private readonly IEncryption _iEncryption = iEncryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<bool> Handle(UpdateNoteNetureOfExpenseCommand request, CancellationToken cancellationToken)
        {
            bool Response = false;
            try
            {
                NoteModel note=new NoteModel();
                note.NoteId= _iEncryption.AesDecrypt(request._note.NoteId);
                note.NatureOfExpensesId = request._note.NatureOfExpensesId;
                Response = await _Update.UpdateNoteNatureOfExpensesData(note);

                if (Response)
                {
                    _logger.LogwriteInfo("Update Note Nature Of Expenses command successfully done", loginUserId);
                    Response=true;
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
