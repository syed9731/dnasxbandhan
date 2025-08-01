using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Domain.DAO.DbHelperModels.ApproverHistory;
using DNAS.Domain.DTO.Note;
using DNAS.Domian.Common;

using MediatR;

using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace DNAS.Application.Features.DashBoard
{
    public record ApproverHistoryCommand : IRequest<CommonResponse<ApproverHistoryNotes>>
    {
        public FilterApproverHistory FilterData { get; set; } = new();
    }

    internal sealed class ApproverHistoryCommandHandler(ICheckExtension checkExtension, IDapperFactory dapperFactory, IHttpContextAccessor httpContextAccessor, ICustomLogger customLogger, IEncryption encryption) : IRequestHandler<ApproverHistoryCommand, CommonResponse<ApproverHistoryNotes>>
    {
        private readonly ICheckExtension _checkExtension = checkExtension;
        private readonly IDapperFactory _dapperFactory = dapperFactory;
        private readonly ICustomLogger _logger = customLogger;
        private readonly IEncryption _encryption = encryption;
        private readonly string _loginUserId = $"user_{httpContextAccessor.HttpContext?.User.FindFirst("UserId")}";

        public async Task<CommonResponse<ApproverHistoryNotes>> Handle(ApproverHistoryCommand request, CancellationToken cancellationToken)
        {
            CommonResponse<ApproverHistoryNotes> response = new();
            try
            {
                if (!string.IsNullOrWhiteSpace(request.FilterData.DateRange))
                {
                    (string, string) dates = _checkExtension.DateRangeToDate(request.FilterData.DateRange, request.FilterData.StartDate, request.FilterData.EndDate);

                    request.FilterData.StartDate = dates.Item1;
                    request.FilterData.EndDate = dates.Item2;

                }
                else
                {
                    var defaultFilter = new FilterApproverHistory();
                    request.FilterData.StartDate = defaultFilter.StartDate;
                    request.FilterData.EndDate = defaultFilter.EndDate;
                }


                //set SqlParameter for stored procedure
                ProcGetApproverHistoryInput procParams = new()
                {
                    @Category = request.FilterData.Category,
                    @StartDate = request.FilterData.StartDate,
                    @EndDate = request.FilterData.EndDate,
                    @UserId = request.FilterData.UserId,
                    @Status = request.FilterData.Status,
                    @Title = request.FilterData.Title,
                };

                //call the stored procedure
                ProcGetApproverHistoryOutput dbResult = await _dapperFactory.ExecuteSpDapperAsync<ApproverHistoryData, ProcGetApproverHistoryOutput>(
                    SpName: OraStoredProcedureNames.ProcApproverDashboard,
                    Params: procParams);

                //assign stored procedure result into response object
                response.Data.ApproverHistoryData = dbResult.Table;

                //encrypt the noteId
                response.Data.ApproverHistoryData = response.Data.ApproverHistoryData.Select(e =>
                {
                    e.NoteId = _encryption.AesEncrypt(e.NoteId);
                    var dateTime = DateTime.ParseExact(e.DateOfCreation, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    e.DateOfCreation = dateTime.ToString("dd MMM yyyy");

                    return e;
                }).ToList();

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo($"Exception occur during ApproverHistoryCommandHandler ---------------- {ex.Message}{Environment.NewLine} {ex.StackTrace} ", _loginUserId);

                return new CommonResponse<ApproverHistoryNotes>();
            }
        }
    }
}
