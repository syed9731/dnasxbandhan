using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.DTO.Login;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace DNAS.Application.Features.Note
{
    public class ApproverFetchFinancialCommand(UserMasterModel user): IRequest<IEnumerable<UserMasterModel>>
    {
        public UserMasterModel _user { get; set; } = user;
    }
    internal sealed class ApproverFetchFinancialHandler(INote iNote, ICustomLogger logger, IHttpContextAccessor haccess) : IRequestHandler<ApproverFetchFinancialCommand, IEnumerable<UserMasterModel>>
    {
        private readonly INote _iNote = iNote;
        public readonly ICustomLogger _logger = logger;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<IEnumerable<UserMasterModel>> Handle(ApproverFetchFinancialCommand request, CancellationToken cancellationToken)
        {

            IEnumerable<UserMasterModel> Response = [];
            try
            { 
                var inparam = new
                {
                    @Expincurd = request._user.ExpenseIncurredAtId,
                    @NetureOfExpense = request._user.NetureOfExpenseId,
                    @TotalAmount = request._user.TotalAmount,
                    @UserId = request._user.UserId
                };
                Response = await _iNote.FetchFinancialApprover(inparam);

                if (Response != null)
                {
                    _logger.LogwriteInfo("Nature Of Expense fetch successfully done", loginUserId);
                    return Response;
                }
                else
                {
                    _logger.LogwriteInfo("Nature Of Expense fetch failed", loginUserId);
                    return new List<UserMasterModel>();
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
