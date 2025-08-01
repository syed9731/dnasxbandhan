using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.DTO.Category;
using DNAS.Domian.DTO.Note;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Note
{
    public class CreateNoteCommand(NoteModel _note) : IRequest<IEnumerable<CategoryRespModel>>
    {
        public NoteModel note { get; set; } = _note;
    }
    internal sealed class CreateNoteHandler(INote iNote, ICustomLogger logger, IEncryption enc, IHttpContextAccessor haccess) : IRequestHandler<CreateNoteCommand, IEnumerable<CategoryRespModel>>
    {
        private readonly INote _iNote = iNote;
        public readonly ICustomLogger _logger = logger;
        public readonly IEncryption _encryption = enc;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<IEnumerable<CategoryRespModel>> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
        {            
            IEnumerable<CategoryRespModel> Response = [];
            try            
            {                
                Response = await _iNote.FetchCategory();

                if (Response != null)
                {                    
                    _logger.LogwriteInfo("Category fetch successfully done", loginUserId);
                    return Response;
                }
                else
                {
                    _logger.LogwriteInfo("Category fetch failed", loginUserId);
                    return new List<CategoryRespModel>();
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
