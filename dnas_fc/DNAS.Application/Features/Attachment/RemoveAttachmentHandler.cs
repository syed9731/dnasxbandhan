using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.Common;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace DNAS.Application.Features.Attachment
{
    public record RemoveAttachmentCommand(string NoteId,string AttachmentId) : IRequest<CommonResponse<int>>
    {

    }
    internal sealed class RemoveAttachmentHandler(IDelete iDelete, ICustomLogger logger, IHttpContextAccessor httpContextAccessor) : IRequestHandler<RemoveAttachmentCommand, CommonResponse<int>>
    {
        private readonly IDelete _iDelete = iDelete;
        public readonly ICustomLogger _logger = logger;
        private readonly string _loginUserId = $"User_{httpContextAccessor.HttpContext?.User?.FindFirst("UserId")}";

        public async Task<CommonResponse<int>> Handle(RemoveAttachmentCommand request, CancellationToken cancellationToken)
        {
            CommonResponse<int> response = new();

            try
            {
                #region database interaction and prepare data
                response.Data = await _iDelete.DeleteAttachment(Convert.ToInt32(request.NoteId), Convert.ToInt32(request.AttachmentId));
                #endregion

                #region create response
                if(response.Data ==1)
                {
                    response.ResponseStatus.ResponseCode = 200;
                    response.ResponseStatus.ResponseMessage = "Data Found";
                }
                else if (response.Data == 0)
                {
                    response.ResponseStatus.ResponseCode = 404;
                    response.ResponseStatus.ResponseMessage = "Data Not Found";
                }
                else
                {
                    response.ResponseStatus.ResponseCode = 400;
                    response.ResponseStatus.ResponseMessage = "Something went wrong.";
                }

                #endregion
            }
            catch (Exception e)
            {
                _logger.LogwriteError($"{e.Message}{Environment.NewLine}{e.StackTrace}", _loginUserId);

                response.ResponseStatus.ResponseCode = 400;
                response.ResponseStatus.ResponseMessage = "Something went wrong.";
            }
            return response;
        }

       
    }
}
