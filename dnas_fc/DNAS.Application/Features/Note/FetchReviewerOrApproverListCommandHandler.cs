using System.Security.Claims;
using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.DTO.Login;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DNAS.Application.Features.Note;

public record FetchReviewerOrApproverListCommand(string SearchKey, int UserId, int NoteId)
    : IRequest<IEnumerable<UserMasterModel>>
{
    //public UserMasterModel UserModel { get; set; }
}

internal sealed class FetchReviewerOrApproverListCommandHandler(
    INote note,
    ICustomLogger logger,
    IHttpContextAccessor contextAccessor)
    : IRequestHandler<FetchReviewerOrApproverListCommand, IEnumerable<UserMasterModel>>
{
    private readonly INote _note = note;
    private readonly ICustomLogger _logger = logger;
    private readonly string _loginUserId = $"User_{contextAccessor.HttpContext?.User.FindFirstValue("UserId")}";

    public async Task<IEnumerable<UserMasterModel>> Handle(FetchReviewerOrApproverListCommand request,
        CancellationToken cancellationToken)
    {
        IEnumerable<UserMasterModel> response = Enumerable.Empty<UserMasterModel>();
        try
        {
            #region Prepare Request Body

            var inparam = new
            {
                @SearchKey = request.SearchKey,
                @UserId = request.UserId,
                @NoteId = request.NoteId
            };

            #endregion

            #region Interaction with Database

            response = (await _note.FetchReviewerApproverList(inparam)).ToList();

            #endregion

            #region Prepare the Response and Return

            if (!response.Any())
                _logger.LogwriteInfo($"Reviewer Approver list fetch failed",_loginUserId);
            else
                _logger.LogwriteInfo($"Reviewer Approver list fetch successfully done", _loginUserId);

            #endregion
        }
        catch (Exception e)
        {
            _logger.LogwriteInfo(
                $"exception occur during FetchReviewerOrApproverListCommandHandler-----{e.Message}{Environment.NewLine}{e.StackTrace}",
                _loginUserId);
        }

        return response;
    }
}