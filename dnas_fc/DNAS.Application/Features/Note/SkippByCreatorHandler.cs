using DNAS.Application.Business.Interface;
using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Application.IRepository;
using DNAS.Application.IService;
using DNAS.Domain.DAO.DbHelperModels.Approver;
using DNAS.Domain.DTO.Approver;
using DNAS.Domain.DTO.Comment;
using DNAS.Domain.DTO.Note;
using DNAS.Domian.Common;
using DNAS.Domian.DAO.DbHelperModels.MailNotification;
using DNAS.Domian.DTO.DelegateAsign;
using DNAS.Domian.DTO.MailSend;
using DNAS.Domian.DTO.Notification;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Note
{
    public class SkippByCreatorCommand(WithdrawNoteModel note) : IRequest<int>
    {
        public WithdrawNoteModel _note { get; set; } = note;
    }
    internal sealed class SkippByCreatorHandler(ISave iSave, ICustomLogger logger, IDapperFactory iDapperFactory, IMailService iMailService, IEmailService _emailService, IHttpContextAccessor haccess, IUpdate iUpdate) : IRequestHandler<SkippByCreatorCommand, int>
    {
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        private readonly ISave _iSave = iSave;
        public readonly ICustomLogger _logger = logger;
        public readonly IMailService _iMailService = iMailService;
        public readonly IUpdate _iUpdate = iUpdate;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<int> Handle(SkippByCreatorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                ProcFetchForSkippByCreatorInput InParams = new()
                {                   
                    @NoteId = request._note.skippApprover.NoteId
                };
                SkippByCreatorModel dbuser = await _iDapperFactory.ExecuteSpDapperAsync<LastApprover, CurrentApprover, NextApprover, SkippByCreatorModel>(OraStoredProcedureNames.ProcFetchForSkippByCreator, InParams);

                if (dbuser.lastApprover.UserId == dbuser.currentApprover.UserId)
                {
                    _logger.LogwriteInfo("You can not skipp last approver", loginUserId);
                    return 1;
                }

                #region Make Next Approver Current Approver
                if(!await _iUpdate.UpdateForNextApprover(dbuser))
                {
                    _logger.LogwriteInfo("Next approver not update as Current Approver", loginUserId);
                    return 2;
                }
                else
                {
                    _logger.LogwriteInfo("Next approver update as Current Approver", loginUserId);
                }
                #endregion

                #region Make Current Approver skipp
                dbuser.UserId=request._note.skippApprover.UserId;
                if (!await _iUpdate.UpdateCurrentApproverAsSkippedApprover(dbuser))
                {
                    _logger.LogwriteInfo("Current Approver not skipped properly.", loginUserId);
                    await _iUpdate.UpdateRollBackNextApprover(dbuser);
                    return 3;
                }
                else
                {
                    _logger.LogwriteInfo("Current Approver skipped successfully.", loginUserId);
                }
                #endregion

                #region Save Asign Skip Comment
                AsignDelegateNoteInputModel asignDelegateNoteInputModel = new AsignDelegateNoteInputModel();
                asignDelegateNoteInputModel.NoteId = request._note.skippApprover.NoteId;
                asignDelegateNoteInputModel.ApproverId = request._note.skippApprover.UserId;
                asignDelegateNoteInputModel.Comment = request._note.querymodel.Comment.Replace("\r\n", "<br/>");
                asignDelegateNoteInputModel.NoteStatus = "SkipComment";
                if (await _iSave.SaveAsignDelegateComment(asignDelegateNoteInputModel))
                {
                    _logger.LogwriteInfo("Skip Comment save successfuly", loginUserId);
                }
                else
                {
                    _logger.LogwriteInfo("Skip Comment save failed", loginUserId);
                }
                #endregion

                //#region Notification save and Email send                       

                ProcFetchForDelegateMailInparam InParams1 = new()
                {
                    @NoteId = InParams.NoteId,
                    @WhoDelegate = dbuser.UserId,
                    @ToWhome = dbuser.currentApprover.UserId,
                };
                DelegateMailSendModel datauser = await _iDapperFactory.ExecuteSpDapperAsync<NotesCreator, DelegateSender, DelegateReceiver, DelegateMailSendModel>(OraStoredProcedureNames.ProcFetchForDelegateMail, InParams1);
                Domian.DTO.DelegateAsign.DeligateMail deligateMail = new()
                {
                    notecreator = datauser.notesCreator.FirstName + (datauser.notesCreator.MiddleName != " " ? " " + datauser.notesCreator.MiddleName + " " : " ") + datauser.notesCreator.LastName,
                    delegateSender = datauser.delegateSender.FirstName + (datauser.delegateSender.MiddleName != "" ? " " + datauser.delegateSender.MiddleName + " " : " ") + datauser.delegateSender.LastName,
                    delegateReceiver = datauser.delegateReceiver.FirstName + (datauser.delegateReceiver.MiddleName != "" ? " " + datauser.delegateReceiver.MiddleName + " " : " ") + datauser.delegateReceiver.LastName,
                    nextApprover= dbuser.nextApprover.FirstName + (dbuser.nextApprover.MiddleName != "" ? " " + dbuser.nextApprover.MiddleName + " " : " ") + dbuser.nextApprover.LastName,
                    NoteTitle = datauser.notesCreator.NoteTitle,
                    noteId = InParams.NoteId,
				};
				//#region Notification Save
				//#region Notification Save
				NotificationModel notificationModel = new()
				{
					Message = $"This is to inform you that the creator of the note titled {request._note.noteModel.NoteTitle}, {deligateMail.notecreator} has skipped your approval for this note.",
					NoteId = InParams.NoteId,
					Heading = "Note Delegated",
					ReceiverUserId = datauser.notesCreator.UserId,
					Action = "None"
				};
				string result = await _iSave.SaveNotificationData(notificationModel);
				if (result == "failed")
				{
					_logger.LogwriteInfo("Data not save in Skip notification table------", loginUserId);
				}
				//#endregion
				#region Skip Mail Send to Current Approver and Note Creator

				Domian.DTO.DelegateAsign.DeligateBodySubject RespopnseBody = await _emailService.GetMailToSkip(deligateMail);

				_logger.LogwriteInfo("SMTP Configuration details fetched successfully for Delegate------" + Environment.NewLine + "and mail body is----- " + RespopnseBody.Body, loginUserId);
				MailSender objMail = await _emailService.GetMailConfiguration();
				objMail.Receiver = datauser.delegateReceiver.Email;
				objMail.Subject = RespopnseBody.Subject.Replace("Note_Name", request._note.noteModel.NoteTitle);
				objMail.Body = RespopnseBody.Body;
                objMail.CC=datauser.notesCreator.Email;
				_logger.LogwriteInfo("Before sending the mail for Creator Delegate------", loginUserId);
				bool ISdeligatemailSend = await _iMailService.EmailSend(objMail);
				_logger.LogwriteInfo("After sending the mail for Creator Delegate-" + ISdeligatemailSend, loginUserId);
                #endregion

                #region mail Send to Next Approver
                #region Notification Save
                NotificationModel notificationModel1 = new();
                notificationModel1.Message = "This is to inform you that note titled " + request._note.noteModel.NoteTitle + " has been skip by creator.This note was created by " + deligateMail.notecreator + ". The note is now in your approval queue.";
                notificationModel1.NoteId = InParams.NoteId;
                notificationModel1.Heading = "Note in approval queue";
                notificationModel1.ReceiverUserId = dbuser.nextApprover.UserId;
                notificationModel1.Action = "PendingNote";
                string resultForNextApprover = await _iSave.SaveNotificationData(notificationModel1);
                #endregion
                if (resultForNextApprover != "success")
                {
                    _logger.LogwriteInfo("Data not save in notification table for next approver------", loginUserId);
                }
                #region NextApprover Mail Send

                DeligateBodySubject ApproverRespopnseBody = await _emailService.GetMailNextNoteApproverAfterSkip(deligateMail);

                _logger.LogwriteInfo("SMTP Configuration details fetched successfully for Next Approver after skip------" + Environment.NewLine + "and mail body is----- " + RespopnseBody.Body, loginUserId);
                MailSender skipobjMail = await _emailService.GetMailConfiguration();
                skipobjMail.Receiver = dbuser.nextApprover.Email;
                skipobjMail.Subject = ApproverRespopnseBody.Subject;
                skipobjMail.Subject = skipobjMail.Subject.Replace("Note_Name", datauser.notesCreator.NoteTitle);
                skipobjMail.Body = ApproverRespopnseBody.Body;
                skipobjMail.CC = datauser.notesCreator.Email;
                _logger.LogwriteInfo("Before sending the mail for Next Approver after skip------", loginUserId);
                bool result2 = await _iMailService.EmailSend(skipobjMail);
                #endregion
                _logger.LogwriteInfo("sending mail to next approver mail-send status------" + result2, loginUserId);
                #endregion
                return 0;
                //#endregion

            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during SaveQueryInitiateEF-------" + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                return 4;
            }
        }
    }
}
