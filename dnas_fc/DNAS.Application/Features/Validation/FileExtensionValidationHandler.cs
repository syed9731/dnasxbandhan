using DNAS.Application.Common.Interface;
using DNAS.Domian.DTO.Note;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Validation
{
    public class FileExtensionValidationCommand(NoteModel note) : IRequest<bool>
    {
        public NoteModel _note { get; set; } = note;
    }
    internal sealed class FileExtensionValidationHandler(ICustomLogger logger, IHttpContextAccessor haccess, ICheckExtension checkExtension) : IRequestHandler<FileExtensionValidationCommand, bool>
    {

        public readonly ICustomLogger _logger = logger;
        private readonly ICheckExtension _checkExtension = checkExtension;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<bool> Handle(FileExtensionValidationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request._note.AttachFiles?.Count > 0)
                {
                    foreach (var file in request._note.AttachFiles)
                    {
                        if (!_checkExtension.CheckFileExtension(Path.GetExtension(file.FileName)))
                        {
                            _logger.LogwriteInfo("Because of restricted extension " + file.FileName + " can not be uploaded", loginUserId);
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("exception occur during FileExtensionValidationCommand and exception message- " + ex.Message + Environment.NewLine + "exception details- " + ex.StackTrace, loginUserId);
                return false;
            }
            return true;
        }
    }
}
