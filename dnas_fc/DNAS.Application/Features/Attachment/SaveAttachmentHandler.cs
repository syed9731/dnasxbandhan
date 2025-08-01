using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Application.IRepository;
using DNAS.Domain.DTO.Note;
using DNAS.Domian.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Attachment
{
    public class SaveAttachmentCommand(WithdrawNoteModel note) : IRequest<string>
    {
        public WithdrawNoteModel _note { get; set; } = note;
    }
    internal sealed class SaveAttachmentHandler(ISave iSave, ICustomLogger logger, IDapperFactory iDapperFactory, IEncryption encryption, IHttpContextAccessor haccess) : IRequestHandler<SaveAttachmentCommand, string>
    {
        private readonly ISave _iSave = iSave;
        public readonly ICustomLogger _logger = logger;
        private readonly IEncryption _iEncryption = encryption;
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<string> Handle(SaveAttachmentCommand request, CancellationToken cancellationToken)
        {
            string result = string.Empty;
            try
            {
                request._note.noteModel.NoteId= _iEncryption.AesDecrypt(request._note.noteModel.NoteId);
                var inparam = new
                {
                    @NoteId = request._note.noteModel.NoteId,
                };
                FetchNoteForAttachmentModel notedata = await _iDapperFactory.ExecuteSpDapperAsync<FetchNote, AttachmentCount, FetchNoteForAttachmentModel >(OraStoredProcedureNames.FetchNoteDetailsByNoteId, inparam);

                if (notedata.fetchNote.NoteStatus != "Pending" && notedata.fetchNote.NoteState != "Publish" && notedata.attachmentCount.AttachCount<5)
                {
                    result = "Document Upload is Not Possible! Note is already " + notedata.fetchNote.NoteStatus;
                }
                else
                {
                    _logger.LogwriteError("Before go to SaveAttachment method---", loginUserId);
                    bool response = await _iSave.SaveAttachment(request._note.noteModel);
                    _logger.LogwriteError("SaveAttachment method response- "+ response, loginUserId);
                    if (response)
                    {
                        result = "Attachment Successfully Saved";
                    }
                    else
                    {
                        result = "Unable to Save Attachment";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogwriteError(ex.ToString(), loginUserId);
            }
            return result;
        }

    }
}
