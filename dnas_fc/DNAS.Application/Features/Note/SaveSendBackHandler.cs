using DNAS.Application.Business.Interface;
using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Application.IRepository;
using DNAS.Application.IService;
using DNAS.Domain.DTO.Note;
using DNAS.Domian.Common;
using DNAS.Domian.DAO.DbHelperModels.Approver;
using DNAS.Domian.DAO.DbHelperModels.MailNotification;
using DNAS.Domian.DTO.Approver;
using DNAS.Domian.DTO.DelegateAsign;
using DNAS.Domian.DTO.MailSend;
using DNAS.Domian.DTO.Note;
using DNAS.Domian.DTO.Notification;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Note
{
    public class SaveSendBackCommand(RequestApproverNoteModel note) : IRequest<bool>
    {
        public RequestApproverNoteModel _note { get; set; } = note;
    }
    internal sealed class SaveSendBackHandler(ISave iSave, ICustomLogger logger, IDapperFactory iDapperFactory, IMailService iMailService, IEncryption encryption, IEmailService _emailService, IHttpContextAccessor haccess) : IRequestHandler<SaveSendBackCommand, bool>
    {
        private readonly ISave _iSave = iSave;
        public readonly ICustomLogger _logger = logger;
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        public readonly IMailService _iMailService = iMailService;
        private readonly IEncryption _iEncryption = encryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<bool> Handle(SaveSendBackCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request._note.noteModel.NoteId = _iEncryption.AesDecrypt(request._note.noteModel.NoteId);
                PendingNoteModel model = new();
                ProcFetchApproverInput InParams = new()
                {
                    @NoteId = request._note.noteModel.NoteId,
                    @UserId = request._note.querymodel.ApproverId
                };
                await _iDapperFactory.ExecuteSpDapperAsync<DNAS.Domian.DTO.Approver.ApproverModel, ApproverData>(OraStoredProcedureNames.ProcFetchApprover, InParams);
                model.querymodel.ApproverId = request._note.querymodel.ApproverId;
                model.querymodel.Comment = request._note.querymodel.Comment.Replace("\r\n", "<br/>");
                model.noteModel.NoteId = request._note.noteModel.NoteId;
                bool str = await _iSave.SaveSendBack(model);
                if (!str)
                {
                    return false;
                }
                #region Notification save and Email send                       


                ProcFetchCreatorApproverNoteInparam InParams1 = new()
                {
                    @NoteId = request._note.noteModel.NoteId,
                    @ApproverUserId = request._note.querymodel.ApproverId
                };
                SendBackDataModel datauser = await _iDapperFactory.ExecuteSpDapperAsync<NotesCreator, NotesApprover, SendBackDataModel>(OraStoredProcedureNames.ProcFetchCreatorApproverNoteForQuery, InParams1);

                DeligateMail deligateMail = new();
                deligateMail.notecreator = datauser.notesCreator.FirstName + (datauser.notesCreator.MiddleName != " " ? " " + datauser.notesCreator.MiddleName + " " : " ") + datauser.notesCreator.LastName;
                deligateMail.noteApprover = datauser.notesApprover.FirstName + (datauser.notesApprover.MiddleName != "" ? " " + datauser.notesApprover.MiddleName + " " : " ") + datauser.notesApprover.LastName;
                deligateMail.notecomment = request._note.querymodel.Comment;
                deligateMail.NoteTitle = request._note.noteModel.NoteTitle;
                deligateMail.noteId = request._note.noteModel.NoteId;

                #region Notification Save
                NotificationModel notificationModel = new();
                notificationModel.Message = "Your note titled " + request._note.noteModel.NoteTitle + " has been sent back for revision by " + deligateMail.nextApprover + ". The reason provided for sending it back is: " + request._note.querymodel.Comment + ".";
                notificationModel.NoteId = request._note.noteModel.NoteId;
                notificationModel.Heading = "Note Send Back";
                notificationModel.ReceiverUserId = datauser.notesCreator.UserId;
                notificationModel.Action = "SendBack";
                await _iSave.SaveNotificationData(notificationModel);
                #endregion

                #region SendBack Mail Send
                DeligateBodySubject RespopnseBody = await _emailService.GetMailSendBackToCreator(deligateMail);


                _logger.LogwriteInfo("SMTP Configuration details fetched successfully for SendBack------" + Environment.NewLine + "and mail body is----- " + RespopnseBody.Body, loginUserId);
                MailSender objMail = await _emailService.GetMailConfiguration();
                objMail.Receiver = datauser.notesCreator.Email;
                objMail.Subject = RespopnseBody.Subject;
                objMail.Subject = objMail.Subject.Replace("Note_Name", request._note.noteModel.NoteTitle);
                objMail.Body = RespopnseBody.Body;
                _logger.LogwriteInfo("Before sending the mail for SendBack------", loginUserId);
                bool result1 = await _iMailService.EmailSend(objMail);
                #endregion
                _logger.LogwriteInfo("SendBack mail send status--------"+ result1, loginUserId);
                
                #endregion
                return true;

            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during SaveQueryInitiateEF-------" + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                return false;
            }

        }

    }
}
