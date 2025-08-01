using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Domain.DTO.Note;
using DNAS.Domian.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace DNAS.Application.Features.Note
{
    public class SaveAsDraftFromWithdrawCommand(WithdrawNoteDetailsModel note) : IRequest<string>
    {
        public WithdrawNoteDetailsModel _note { get; set; } = note;
    }
    internal sealed class SaveAsDraftFromWithdrawHandler(IDapperFactory iDapperFactory, ICustomLogger logger, IEncryption encryption, IHttpContextAccessor haccess) : IRequestHandler<SaveAsDraftFromWithdrawCommand, string>
    {
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        public readonly ICustomLogger _logger = logger;
        private readonly IEncryption _encryption = encryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<string> Handle(SaveAsDraftFromWithdrawCommand request, CancellationToken cancellationToken)
        {
            try
            {                
                var inparam = new
                {
                    @NoteId = _encryption.AesDecrypt(request._note.noteModel.NoteId)
                };
                int DbResult = await _iDapperFactory.ExecuteSpDapperAsync(
                    SpName: OraStoredProcedureNames.ProcSaveToDraftFromWithdraw,
                    Params: inparam);
                if (DbResult != 0)
                {
                    return "success";
                }
                else
                {
                    return "failed";
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("exception occur during Save As Draft From Withdraw note ------ " + ex.Message + Environment.NewLine + ex.StackTrace, loginUserId);
                return "Failed";
            }

        }

    }
}
