using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.DashBoard;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace DNAS.Application.Features.DashBoard
{
    public class DashBoardsCommand : IRequest<CommonResponse<ApprovalData>>
    {
        public int id { get; set; }
    }

    internal sealed class DashBoardsCommandHandler(IDashboard iDashboard, ICustomLogger logger, IEncryption encryption, IHttpContextAccessor haccess) : IRequestHandler<DashBoardsCommand, CommonResponse<ApprovalData>>
    {
        private readonly IDashboard _iDashboard = iDashboard;
        public readonly ICustomLogger _logger = logger;
        private readonly IEncryption _encryption = encryption;        
        private readonly string _loginUserId = $"user_{haccess.HttpContext?.User.FindFirstValue("UserId")}";

        public async Task<CommonResponse<ApprovalData>> Handle(DashBoardsCommand request, CancellationToken cancellationToken)
        {
            var inparam = new { @UserId = request.id };
            try
            {
                CommonResponse<ApprovalData> Response = await _iDashboard.GetDashboardData(inparam);

                if (Response.Data.ApprovalList != null)
                {
                    Response.Data.ApprovalList = Response.Data.ApprovalList.Select(x => { x.NoteId = _encryption.AesEncrypt(x.NoteId); return x; }).ToList();
                    Response.Data.DraftList = Response.Data.DraftList.Select(x => { x.NoteId = _encryption.AesEncrypt(x.NoteId); return x; }).ToList();
                    Response.ResponseStatus.ResponseCode = 200;
                    Response.ResponseStatus.ResponseMessage = "Data Found";
                    _logger.LogwriteInfo($"Data Found in the User : {request.id}  in the Table", _loginUserId);
                }
                else
                {
                    _logger.LogwriteInfo($"No Data Found in the User: {request.id} in the Table", _loginUserId);
                }
                return Response;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("Exception occur during DashBoardsCommand execution-----message-"+Environment.NewLine +e.Message
                    +Environment.NewLine+e.StackTrace, _loginUserId);
                return new();
            }            
        }
    }
}