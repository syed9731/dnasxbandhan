using DNAS.Application.Business.Interface;
using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Application.IRepository;
using DNAS.Application.IService;
using DNAS.Domain.DTO.WithdrawList;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.MailSend;
using DNAS.Domian.DTO.Note;
using DNAS.Domian.DTO.Notification;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Security.Claims;


namespace DNAS.Application.Features.Note
{
    public class WithdrawNoteCommand(PendingNoteModel note) : IRequest<bool>
	{
		public PendingNoteModel _note { get; set; } = note;
	}
	internal sealed class WithdrawNoteHandler(INote inote, ICustomLogger logger, IUpdate iupdate, IEncryption encryption, IHttpContextAccessor haccess
		, IEmailService _emailService
		, ISave iSave
		, IMailService iMailService,IDapperFactory _iDapperFactory) : IRequestHandler<WithdrawNoteCommand, bool>
	{
		private readonly INote _iNote = inote;
		public readonly ICustomLogger _logger = logger;
		private readonly IUpdate _Update = iupdate;
		private readonly IEncryption _encryption = encryption;
		public readonly IEmailService _IEmailService = _emailService;
		public readonly IMailService _iMailService = iMailService;
		private readonly ISave _iSave = iSave;
		private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
		public async Task<bool> Handle(WithdrawNoteCommand request, CancellationToken cancellationToken)
		{

			bool Response1 = false;
			try
			{
				PendingNoteModel note = new PendingNoteModel();
				var inparam = new
				{
					@NoteId = _encryption.AesDecrypt(request._note.noteModel.NoteId)
				};
				PendingNoteModel Response = await _iNote.FetchNoteStatus(inparam);

				var Currentapprover = Response.approverModel.FirstOrDefault(a => a.IsCurrentApprover.Equals("True"));


				Domian.DTO.DelegateAsign.MailModel withdrawMail = new()
				{
					notecreator = Response.noteModel?.FirstName + (string.IsNullOrWhiteSpace(Response.noteModel?.MiddleName) ? "" : $" {Response.noteModel?.MiddleName}") + Response.noteModel?.LastName,
					noteId = inparam.NoteId,
					NoteTitle = Response.noteModel.NoteTitle,
					Comment= request._note.querymodel.Comment

				};
				if (Response.noteModel.NoteStatus != "Approved")
				{
					note.noteModel.NoteId = _encryption.AesDecrypt(request._note.noteModel.NoteId);
					note.noteModel.NoteTitle = Response.noteModel.NoteTitle;
					note.noteModel.IsActive = "False";
					note.noteModel.NoteStatus = "Withdraw";
					note.noteModel.WithdrawDate = System.DateTime.Now;
					note.noteModel.UserId= haccess.HttpContext?.User.FindFirstValue("UserId") ?? "";
					note.querymodel.Comment = request._note.querymodel.Comment;
					Response1 = await _Update.UpdateNoteStatusData(note);
				}
				else
				{
					_logger.LogwriteInfo("You can not withdraw when note is approved", loginUserId);
				}
				if (Response1)
				{
					_logger.LogwriteInfo("Update Note command successfully done", loginUserId);

					NotificationModel notificationModel = new()
					{
						Message = $"This is to inform you that {withdrawMail.notecreator} has withdrawn the note titled {Response.noteModel.NoteTitle}. The reason provided for withdraw is: {request._note.querymodel.Comment}",
						NoteId = inparam.NoteId,
						Heading = "Note Withdraw",
						ReceiverUserId = Currentapprover?.UserId ?? string.Empty,
						Action = "None"
					};
					string result = await _iSave.SaveNotificationData(notificationModel);
					if (result == "failed")
					{
						_logger.LogwriteInfo("Data not save in Skip notification table------", loginUserId);
					}

                    //#endregion
                    #region Delegate Mail Send to Note Creator
                    var inputparameter = new
                    {
                        @Userid = note.noteModel.UserId
                    };
                    CreatorModel DBResponse = await _iDapperFactory.ExecuteSpDapperAsync<ApproversModel, CreatorModel>(OraStoredProcedureNames.FetchCreaterForCCMail, inputparameter);

                    withdrawMail.All_Approvers = string.Join(", ",Response.approverModel.Select(x => x.FirstName).Concat(DBResponse.approversModel.Select(x => x.FirstName)));
                    //               withdrawMail.All_Approvers = string.Join(", ", Response.approverModel
                    //	.Select(x => x.FirstName));
                    //withdrawMail.All_Approvers=string.Join(", ", DBResponse.approversModel.FirstName);
                    Domian.DTO.DelegateAsign.MailBodySubject RespopnseBody = await _IEmailService.GetMailConfigurationData(withdrawMail, "WithdrawNote");

					_logger.LogwriteInfo("SMTP Configuration details fetched successfully for Delegate------" + Environment.NewLine + "and mail body is----- " + RespopnseBody.Body, loginUserId);
					MailSender objMail = await _emailService.GetMailConfiguration();
					objMail.Receiver = Currentapprover?.Email ?? string.Empty;
					objMail.Subject = RespopnseBody.Subject.Replace("Note_Name", Response.noteModel.NoteTitle);
					objMail.Body = RespopnseBody.Body;
					objMail.CC = string.Join(',', Response.approverModel
						.Where(item => item.UserId != Currentapprover?.UserId)
						.Select(x => x.Email).Concat(DBResponse.approversModel.Select(x=>x.Email)));

                    _logger.LogwriteInfo("Before sending the mail for Creator Delegate------", loginUserId);
					bool ISdeligatemailSend = await _iMailService.EmailSend(objMail);
					_logger.LogwriteInfo("After sending the mail for Creator Delegate-" + ISdeligatemailSend, loginUserId);
					#endregion

				}
				else
				{
					_logger.LogwriteInfo("Update Note command failed", loginUserId);
				}
			}
			catch (Exception ex)
			{
				_logger.LogwriteInfo("exception occur during withdraw note ------ " + ex.Message + Environment.NewLine + ex.StackTrace, loginUserId);
			}
			return Response1;
		}

	}
}
