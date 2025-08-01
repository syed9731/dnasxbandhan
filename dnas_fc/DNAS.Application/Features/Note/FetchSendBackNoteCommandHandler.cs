using DNAS.Application.Common.Interface;
using DNAS.Application.IEntityRepository;
using DNAS.Domain.DTO.Note;
using DNAS.Domian.Common;

using MediatR;

using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;

namespace DNAS.Application.Features.Note
{
    public record FetchSendBackNoteCommand(string NoteId) : IRequest<CommonResponse<SendBackNoteDto>>
    {
    }
    internal sealed class FetchSendBackNoteCommandHandler(
        ICustomLogger logger,
        IEncryption encryption,
        IHttpContextAccessor httpContextAccessor,
        IFetch iFetch
        ) : IRequestHandler<FetchSendBackNoteCommand, CommonResponse<SendBackNoteDto>>
    {
        private readonly ICustomLogger _logger = logger;
        private readonly IFetch _iFetch = iFetch;
        private readonly IEncryption _encryption = encryption;
        private readonly string _loginUserId = $"user_{httpContextAccessor.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<CommonResponse<SendBackNoteDto>> Handle(FetchSendBackNoteCommand request, CancellationToken cancellationToken)
        {
            CommonResponse<SendBackNoteDto> response = new();
            try
            {
                #region database interaction and prepare response data
                response.Data = await _iFetch.FetchSendBackNoteByNoteId(Convert.ToInt64(request.NoteId));

                response.Data.NoteModel!.NoteId = _encryption.AesEncrypt(response.Data.NoteModel!.NoteId);
                response.Data.ApproverListJson = JsonSerializer.Serialize(response.Data.ApproverList);
                response.Data.AttachmentListJson = JsonSerializer.Serialize(response.Data.AttachmentList);

                response.ResponseStatus.ResponseCode = 200;
                response.ResponseStatus.ResponseMessage = "Data Found";
                #endregion

                #region log and return the response
                _logger.LogwriteInfo($"Data Found in fetching the SendBackNote : {request.NoteId} ", _loginUserId);
                #endregion
            }
            catch (Exception ex)
            {
                #region log the error and send the default value
                _logger.LogwriteInfo($"Exception occur during FetchSendBackNoteCommandHandler ---------------- NoteId:{request.NoteId} {Environment.NewLine} UserId:{_loginUserId} {Environment.NewLine} {ex.Message}{Environment.NewLine} {ex.StackTrace} ", request.NoteId);
                response.ResponseStatus.ResponseCode = 404;
                response.ResponseStatus.ResponseMessage = "Data Not Found";
                #endregion
            }
            return response;
        }
    }
}
