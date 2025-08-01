using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Domain.DTO.Note.Download;
using DNAS.Domian.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;

namespace DNAS.Application.Features.Note.Download
{
    public class FilesCommand(string docid) : IRequest<CommonResponse<FilesModel>>
    {
        public string Docuid = docid;
    }
    internal sealed class FilesCommandHandler(IDapperFactory iDapperFactory, ICustomLogger logger, IHttpContextAccessor haccess)
        : IRequestHandler<FilesCommand, CommonResponse<FilesModel>>
    {
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        public readonly ICustomLogger _logger = logger;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<CommonResponse<FilesModel>> Handle(FilesCommand Request, CancellationToken cancellationToken)
        {
            CommonResponse<FilesModel> Response = new();
            try
            {
                _logger.LogwriteInfo("before call FilesCommandHandler procedure with parameters------ ", loginUserId);
                GetAttachementDetailsModel DbResult = await _iDapperFactory.ExecuteSpDapperAsync<GetAttachementDetails, GetAttachementDetailsModel>(
                    SpName: OraStoredProcedureNames.getAttachmentDetails,
                    Params: new { @AttachmentId = Request.Docuid });
                _logger.LogwriteInfo("after call getAttachmentDetails procedure with return value------ " + JsonSerializer.Serialize(Response.Data), loginUserId);

                if (!System.IO.File.Exists(DbResult.getAttachementDetails.AttachmentPath)) {
                    _logger.LogwriteInfo("File not found ------ ", loginUserId);
                    return Response;    
                }

                // Read the file into a byte array
                byte[] fileBytes = System.IO.File.ReadAllBytes(DbResult.getAttachementDetails.AttachmentPath);
                return Response = new CommonResponse<FilesModel>()
                {
                    Data = new FilesModel()
                    {
                        FileName = DbResult.getAttachementDetails.DocumentName,
                        fileByte = fileBytes
                    }
                };
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during FilesCommandHandler------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                return new CommonResponse<FilesModel>();
            }
        }
    }
}
