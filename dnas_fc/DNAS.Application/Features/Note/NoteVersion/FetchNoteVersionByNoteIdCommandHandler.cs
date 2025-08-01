using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Domain.DAO.DbHelperModels.NoteVersion;
using DNAS.Domain.DTO.Note;
using DNAS.Domian.Common;

using MediatR;

using Microsoft.AspNetCore.Http;

using System.Security.Claims;
using System.Text.Json;

namespace DNAS.Application.Features.Note.NoteVersion
{
    public record FetchNoteVersionByNoteIdCommand(string NoteId) : IRequest<CommonResponse<NoteVersionModel>> { }
    internal sealed class FetchNoteVersionByNoteIdCommandHandler(ICustomLogger logger, IDapperFactory iDapperFactory, IEncryption iEncryption, IHttpContextAccessor haccess)
: IRequestHandler<FetchNoteVersionByNoteIdCommand, CommonResponse<NoteVersionModel>>
    {
        #region properties initialization

        public readonly ICustomLogger _logger = logger;
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        private readonly IEncryption _iEncryption = iEncryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        #endregion

        public async Task<CommonResponse<NoteVersionModel>> Handle(FetchNoteVersionByNoteIdCommand request, CancellationToken cancellationToken)
        {
            CommonResponse<NoteVersionModel> response = new();

            try
            {
                #region Prepare procedure params initialization
                ProcFetchNoteVersionListByNoteIdInparam inparam = new()
                {
                    NoteId = request.NoteId
                };
                #endregion

                _logger.LogwriteInfo("before call ProcFetchNoteVersionListByNoteId procedure with parameters------ " + JsonSerializer.Serialize(inparam), loginUserId);

                #region Database interaction

                var dbResult = await _iDapperFactory.ExecuteSpDapperAsync<NoteApprovedDto, NoteApprovedVersionDto, NoteVersionDto, NoteVersionModel>(
                        OraStoredProcedureNames.ProcFetchNoteVersionListByNoteId, inparam);

                #endregion

                #region Prepare response and return

                if (dbResult is not null)
                {
                    response.Data.NoteApproved = dbResult.NoteApproved;

                    response.Data.NoteApproved.NoteId = (response.Data.NoteApproved.NoteId != "" || response.Data.NoteApproved.NoteId != null) ? _iEncryption.AesEncrypt(response.Data.NoteApproved.NoteId) : "";

                    response.Data.NoteApprovedVersion = (dbResult.NoteApprovedVersion is not null && dbResult.NoteApprovedVersion.Count() > 0) 
                        ? dbResult.NoteApprovedVersion.Select(e =>
                          {
                              e.NoteId = _iEncryption.AesEncrypt(e.NoteId);
                              e.NoteApproved_VersionId = _iEncryption.AesEncrypt(e.NoteApproved_VersionId);
                              return e;
                          }).ToList() 
                         : new List<NoteApprovedVersionDto>();

                    response.Data.NoteVersion = (dbResult.NoteVersion is not null && dbResult.NoteVersion.Count() > 0) 
                        ? dbResult.NoteVersion.Select(e =>
                            {
                                e.NoteId = _iEncryption.AesEncrypt(e.NoteId);
                                e.Note_VersionId = _iEncryption.AesEncrypt(e.Note_VersionId);
                                return e;
                            }).ToList() 
                        : new List<NoteVersionDto>();
                }

                return response;

                #endregion

            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("Upload file exception : " + ex.StackTrace, loginUserId);
                return response;
            }


        }
    }
}
