using System.Security.Claims;
using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domain.DTO.Approver;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DNAS.Application.Features.Note;

public record InsertReviewerApproverCommand(AppproverReviewerRequestModelDto Model)
    : IRequest<ApproverReviewerResponseModelDto>
{
}

internal sealed class InsertReviewerApproverCommandHandler(
    ISave save,
    ICustomLogger logger,
    IHttpContextAccessor contextAccessor)
    : IRequestHandler<InsertReviewerApproverCommand, ApproverReviewerResponseModelDto>
{
    private readonly ISave _save = save;
    private readonly ICustomLogger _logger = logger;

    private readonly string _loginUserId =
        $"User_{contextAccessor.HttpContext?.User.FindFirstValue("UserId")}";
public async Task<ApproverReviewerResponseModelDto> Handle(InsertReviewerApproverCommand request,
        CancellationToken cancellationToken)

    {
        ApproverReviewerResponseModelDto response = new();
        try
        {
            #region Interaction with Database

            response = await _save.SaveReviewerApprover(request.Model);

            #endregion

            #region Prepare the Response and Return

            _logger.LogwriteInfo($"data inserted successfully SaveReviewerApprover----",_loginUserId);
            return response;

            #endregion
            
        }
        catch (Exception e)
        {
            _logger.LogwriteInfo(
                $"exception occur during InsertReviewerApproverCommandHandler-----{e.Message}{Environment.NewLine}{e.StackTrace}",
                _loginUserId);

            return response;
        }
    }
}