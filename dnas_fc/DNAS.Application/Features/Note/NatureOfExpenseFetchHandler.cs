using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.DTO.Category;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Note
{
    public class NatureOfExpenseFetchCommand(NatureOfExpensesModel natureOfExpenses): IRequest<IEnumerable<NatureOfExpensesModel>>
    {
        public NatureOfExpensesModel _natureOfExpenses { get; set; } = natureOfExpenses;
    }
    internal sealed class NatureOfExpenseFetchHandler(INote iNote, ICustomLogger logger, IHttpContextAccessor haccess) : IRequestHandler<NatureOfExpenseFetchCommand, IEnumerable<NatureOfExpensesModel>>
    {
        private readonly INote _iNote = iNote;
        public readonly ICustomLogger _logger = logger;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<IEnumerable<NatureOfExpensesModel>> Handle(NatureOfExpenseFetchCommand request, CancellationToken cancellationToken)
        {

            IEnumerable<NatureOfExpensesModel> Response = [];
            try
            {
                var inparam = new
                {
                    @ExpensesIncurredAtId = request._natureOfExpenses.ExpensesIncurredAtId
                };
                Response = await _iNote.FetchNatureOfExpenses(inparam);

                if (Response != null)
                {
                    _logger.LogwriteInfo("Nature Of Expense fetch successfully done", loginUserId);
                    return Response;
                }
                else
                {
                    _logger.LogwriteInfo("Nature Of Expense fetch failed", loginUserId);
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
