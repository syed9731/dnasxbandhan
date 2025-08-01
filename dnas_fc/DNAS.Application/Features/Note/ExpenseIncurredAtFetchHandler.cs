using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.DTO.Category;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Note
{
    public class ExpenseIncurredAtFetchCommand(ExpenseIncurredAtModel _subcat) : IRequest<IEnumerable<ExpenseIncurredAtModel>>
    {
        public ExpenseIncurredAtModel subcat { get; set; } = _subcat;
    }
    internal sealed class ExpenseIncurredAtFetchHandler(INote iNote, ICustomLogger logger, IHttpContextAccessor haccess) : IRequestHandler<ExpenseIncurredAtFetchCommand, IEnumerable<ExpenseIncurredAtModel>>
    {
        private readonly INote _iNote = iNote;
        public readonly ICustomLogger _logger = logger;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<IEnumerable<ExpenseIncurredAtModel>> Handle(ExpenseIncurredAtFetchCommand request, CancellationToken cancellationToken)
        {

            IEnumerable<ExpenseIncurredAtModel> Response = [];
            try
            {
                var inparam = new
                {
                    @CategoryId = request.subcat.CategoryId
                };
                Response = await _iNote.FetchExpenseIncurredAt(inparam);

                if (Response != null)
                {
                    _logger.LogwriteInfo("Expense Incurred At fetch successfully done", loginUserId);
                    return Response;
                }
                else
                {
                    _logger.LogwriteInfo("Expense Incurred At fetch failed", loginUserId);
                    return [];
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
