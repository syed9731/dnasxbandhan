using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Domain.DTO.Note;
using DNAS.Domian.Common;

using MediatR;

using Microsoft.AspNetCore.Http;

using System.Security.Claims;

namespace DNAS.Application.Features.Note.NoteVersion
{
	public record NoteVersionDetailsCommand(string NoteId, string NoteVersionId, string NoteVersionType) : IRequest<CommonResponse<RequestApproverNoteModel>> { }
	internal sealed class NoteVersionDetailsHandler(IDapperFactory iDapperFactory, ICustomLogger logger, IEncryption encryption, IHttpContextAccessor haccess) : IRequestHandler<NoteVersionDetailsCommand, CommonResponse<RequestApproverNoteModel>>
	{
		#region Properties initialization

		public readonly IDapperFactory _iDapperFactory = iDapperFactory;
		public readonly ICustomLogger _logger = logger;
		private readonly IEncryption _encryption = encryption;
		private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";

		#endregion
		public async Task<CommonResponse<RequestApproverNoteModel>> Handle(NoteVersionDetailsCommand request, CancellationToken cancellationToken)
		{
			CommonResponse<RequestApproverNoteModel> response = new();
			try
			{
				#region Database interaction

				if (request.NoteVersionType.ToLower() == "current")
				{
					var noteIdParam = new
					{
						@NoteId = Convert.ToInt64(request.NoteId)
					};
					response.Data = await _iDapperFactory
				   .ExecuteSpDapperAsync<ReqNotesModel, ReqApproversModel, ReqNoteComment, ReqAttachment,
					   ReqRecomendedApproverModel, RequestApproverNoteModel>(
					   OraStoredProcedureNames.ProcFetchCurrentNoteVersion, noteIdParam);
				}
				else if (request.NoteVersionType.ToLower() == "previous")
				{
					var previousParam = new
					{
						@NoteVersionId = Convert.ToInt64(request.NoteVersionId)
					};
					response.Data = await _iDapperFactory
				   .ExecuteSpDapperAsync<ReqNotesModel, ReqApproversModel, ReqNoteComment, ReqAttachment,
					   ReqRecomendedApproverModel, RequestApproverNoteModel>(
					   OraStoredProcedureNames.ProcFetchPreviousNoteVersion, previousParam);
				}
				else if (request.NoteVersionType.ToLower() == "child")
				{
					var noteVersionIdParam = new
					{
						@NoteVersionId = Convert.ToInt64(request.NoteVersionId)
					};

					response.Data = await _iDapperFactory
				   .ExecuteSpDapperAsync<ReqNotesModel, ReqApproversModel, ReqNoteComment, ReqAttachment,
					   ReqRecomendedApproverModel, RequestApproverNoteModel>(
					   OraStoredProcedureNames.ProcFetchChildNoteVersion, noteVersionIdParam);
				}

				#endregion

				#region Prepare response and return

				if (response.Data != null)
				{
					_logger.LogwriteInfo("Note version data fetch successfully done", loginUserId);

					if (response.Data.attachmentsModel.Any())
					{
						response.Data.attachmentsModel = response.Data.attachmentsModel.Select(e =>
						{
							e.AttachmentId = _encryption.AesEncrypt(e.AttachmentId);
							e.AttachmentPath = e.AttachmentPath.Replace("wwwroot", "");
							return e;
						}).ToList();
					}

					response.Data.noteModel.NoteId = _encryption.AesEncrypt(response.Data.noteModel.NoteId.ToString());
					response.Data.noteModel.NatureOfExpenseCode = string.IsNullOrWhiteSpace(response.Data.noteModel.NatureOfExpenseCode) ? "" : string.Concat(response.Data.noteModel.NatureOfExpenseCode, ")");
				}
				else
				{
					_logger.LogwriteInfo("Note version data fetch failed", loginUserId);
				}

				return response;

				#endregion

			}
			catch (Exception ex)
			{
				_logger.LogwriteError(ex.ToString(), loginUserId);
				return response;
			}
		}
	}
}
