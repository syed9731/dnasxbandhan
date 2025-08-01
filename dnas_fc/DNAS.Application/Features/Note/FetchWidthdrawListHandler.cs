using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Domian.Common;
using DNAS.Domian.DAO.DbHelperModels.Withdraw;
using DNAS.Domian.DTO.WithdrawList;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Note
{
    public class FetchWidthdrawListCommand(FilterWithdrawListNote filterWithdrawListNote) : IRequest<WithdrawListModel>
    {
        public FilterWithdrawListNote InputModel { get; set; } = filterWithdrawListNote;
    }
    internal sealed class FetchWidthdrawListHandler(IDapperFactory iDapperFactory, ICustomLogger logger, IEncryption encryption, IHttpContextAccessor haccess) : IRequestHandler<FetchWidthdrawListCommand, WithdrawListModel>
    {
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        public readonly ICustomLogger _logger = logger;
        private readonly IEncryption _encryption= encryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<WithdrawListModel> Handle(FetchWidthdrawListCommand Request, CancellationToken cancellationToken)
        {
            WithdrawListModel Response = new();
            try
            {
                ProcGetWithdrawListInput InParams = new()
                {
                    @UserId = Request.InputModel.UserId,
                    @StartDate = Request.InputModel.StartDate ?? "",
                    @EndDate = Request.InputModel.EndDate ?? "",
                    @Category = Request.InputModel.Category ?? ""
                };

                Response = await _iDapperFactory.ExecuteSpDapperAsync<WithdrawListOutModel, WithdrawListModel>(
                    SpName: OraStoredProcedureNames.ProcGetWithdrawList,
                    Params: InParams);
                Response.withdrawListOutModels = Response.withdrawListOutModels.Select(x => { x.NoteId = _encryption.AesEncrypt(x.NoteId); return x; }).ToList();
                return Response;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during FetchWidthdrawListHandler------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                return new WithdrawListModel();
            }
        }
    }
}
