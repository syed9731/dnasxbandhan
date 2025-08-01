using DNAS.Application.Common.Filter;
using DNAS.Application.Common.Interface;
using DNAS.Application.Features.Note.NoteVersion;
using DNAS.Domain.DTO.Note;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

using System.Security.Claims;
using System.Text.Json;

namespace DNAS.WEB.Controllers
{
    [Authorize]
	[TypeFilter(typeof(UserCurrentAuth))]
	public class VersionController(ISender iSender,
		ICustomLogger iCustomLogger, IHttpContextAccessor haccess, IEncryption iEncryption) : Controller
	{
		private readonly ISender _iSender = iSender;
		private readonly ICustomLogger _iCustomLogger = iCustomLogger;
		private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
		private readonly IEncryption _iEncryption = iEncryption;
		private readonly string _commonlogpath = "Login";
		private readonly string _approverDashboardUrl = "/Note/ApproverDashboard";
		private readonly string _approvalRequestUrl = "/Note/ApproverDashboard";

		public async ValueTask<IActionResult> Index()
		{
			NoteVersionModel noteVersionModel = new();
			try
			{
				string noteId = string.Empty;

				#region Get the NoteId from the previous request

				if (Request.Headers.Referer.ToString() != "")
				{
					string queryString = QueryHelpers.ParseQuery(new Uri(Request.Headers.Referer.ToString()).Query).TryGetValue("p", out var pValue)
											? pValue.ToString()
											: QueryHelpers.ParseQuery(new Uri(Request.Headers.Referer.ToString()).Query).TryGetValue("NoteId", out var noteIdValue)
											? noteIdValue.ToString()
											: string.Empty;

					noteId = (queryString != null) ? _iEncryption.AesDecrypt(queryString) : string.Empty;
				}
				else
				{
					_iCustomLogger.LogwriteInfo(
						"note id not found in Approval Request page ------ ", _commonlogpath);
					return Redirect(_approvalRequestUrl);
				}
				#endregion

				//noteId = "896";

				string userId = User.FindFirstValue("UserId") ?? "";

				if (userId != "")
				{
					_iCustomLogger.LogwriteInfo(
						"Get version list method initiated------ and request---" + JsonSerializer.Serialize(noteId), loginUserId);

					var response = await _iSender.Send(new FetchNoteVersionByNoteIdCommand(noteId));

					if (response.Data != null)
						noteVersionModel = response.Data;
				}
				return View(noteVersionModel);
			}
			catch (Exception e)
			{
				_iCustomLogger.LogwriteInfo("exception occur during Create Note page ------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
				return Redirect(_approverDashboardUrl);
			}

		}

		public async ValueTask<IActionResult> NoteVersionDetails(string NoteId = "", string NoteVersionId = "", string NoteVersionType = "")
		{
			RequestApproverNoteModel responseModel = new();

			try
			{
				string userId = User.FindFirstValue("UserId") ?? "";
				if (string.IsNullOrWhiteSpace(userId))
				{
					_iCustomLogger.LogwriteInfo($"Userid not found in NoteVersionDetails page ------ ", _commonlogpath);
					//HttpContext.Session.SetString("RedirectFromApprovalRequest","/ApprovalRequest?p=" + _iEncryption.AesEncrypt(noteid));
					return Redirect(_approverDashboardUrl);
				}

				if (string.IsNullOrWhiteSpace(NoteId))
				{
					_iCustomLogger.LogwriteInfo($"NoteId not found in NoteVersionDetails page ------ ", _commonlogpath);
					return Redirect(_approvalRequestUrl);

				}
				if (NoteVersionType == "previous" && string.IsNullOrWhiteSpace(NoteVersionId))
				{
					_iCustomLogger.LogwriteInfo($"NoteVersionId not found in NoteVersionDetails page ------ ", _commonlogpath);
					return Redirect(_approvalRequestUrl);
				}

				if (NoteVersionType == "child" && string.IsNullOrWhiteSpace(NoteVersionId))
				{
					_iCustomLogger.LogwriteInfo($"NoteVersionId not found in NoteVersionDetails page ------ ", _commonlogpath);
					return Redirect(_approvalRequestUrl);
				}

				if (NoteVersionType == "child" && !string.IsNullOrWhiteSpace(NoteVersionId))
				{
					NoteVersionId = _iEncryption.AesDecrypt(NoteVersionId);
				}
				else if (NoteVersionType == "previous" && !string.IsNullOrWhiteSpace(NoteVersionId))
				{
					NoteVersionId = _iEncryption.AesDecrypt(NoteVersionId);
				}

				_iCustomLogger.LogwriteInfo("ApprovalRequest method initiated ---",
					   loginUserId);

				ViewBag.UserId = userId;
				NoteId = _iEncryption.AesDecrypt(NoteId);

				_iCustomLogger.LogwriteInfo($"NoteVersionDetails method request NoteId:{NoteId} NoteVersionId:{NoteVersionId} NoteVersionType:{NoteVersionType}", loginUserId);

				var response = await _iSender.Send(new NoteVersionDetailsCommand(NoteId, NoteVersionId, NoteVersionType));

				if (response.Data != null)
				{
					_iCustomLogger.LogwriteInfo($"ApprovalRequest method response-------NoteTitle:{response.Data.noteModel.NoteTitle} ", loginUserId);

					responseModel = response.Data;
				}

				return View(responseModel);
			}
			catch (Exception e)
			{
				_iCustomLogger.LogwriteInfo($"exception occur during Approval Request ------ {e.Message} {Environment.NewLine} {e.StackTrace}", _commonlogpath);
				return Redirect(_approverDashboardUrl);
			}
		}
	}
}
