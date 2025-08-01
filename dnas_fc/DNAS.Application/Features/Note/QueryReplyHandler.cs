using DNAS.Application.Business.Interface;
using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Application.IRepository;
using DNAS.Application.IService;
using DNAS.Domain.DTO.Note;
using DNAS.Domian.Common;
using DNAS.Domian.DAO.DbHelperModels.Approver;
using DNAS.Domian.DTO.DelegateAsign;
using DNAS.Domian.DTO.MailSend;
using DNAS.Domian.DTO.Note;
using DNAS.Domian.DTO.Notification;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Note
{
    public class QueryReplyCommand(WithdrawNoteModel note) : IRequest<bool>
    {
        public WithdrawNoteModel _note { get; set; } = note;
    }
    internal sealed class QueryReplyHandler(ISave iSave, ICustomLogger logger, IDapperFactory iDapperFactory, IMailService iMailService, IEncryption encryption, IEmailService _emailService, IHttpContextAccessor haccess) : IRequestHandler<QueryReplyCommand, bool>
    {
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        private readonly ISave _iSave = iSave;
        public readonly ICustomLogger _logger = logger;
        public readonly IMailService _iMailService = iMailService;
        private readonly IEncryption _encryption = encryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<bool> Handle(QueryReplyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request._note.noteModel.NoteId = _encryption.AesDecrypt(request._note.noteModel.NoteId);
                WithdrawNoteModel model = new();
                ProcFetchApproverAndCreatorInput InParams = new()
                {
                    @NoteId = request._note.noteModel.NoteId,
                    @UserId = request._note.noteModel.UserId
                };
                QueryReplyModel approver = await _iDapperFactory.ExecuteSpDapperAsync<ApproverForQuery, UserForQuery, QueryReplyModel>(OraStoredProcedureNames.ProcFetchApproverAndCreator, InParams);
                model.querymodel.ApproverId = request._note.noteModel.UserId;
                model.querymodel.Comment = request._note.querymodel.Comment.Replace("\r\n", "<br/>");
                model.noteModel.NoteId = request._note.noteModel.NoteId;
                bool str = await _iSave.SaveQueryReply(model);
                if (!str)
                {
                    return false;
                }
                try
                {
                    #region Notification save and Email send Query Reply 

                    DeligateMail deligateMail = new();
                    deligateMail.notecreator = approver.userForQuery.FirstName + (approver.userForQuery.MiddleName != " " ? " " + approver.userForQuery.MiddleName + " " : " ") + approver.userForQuery.LastName;
                    deligateMail.noteApprover = approver.approverForQuery.FirstName + (approver.approverForQuery.MiddleName != "" ? " " + approver.approverForQuery.MiddleName + " " : " ") + approver.approverForQuery.LastName;
                    deligateMail.notecomment = request._note.querymodel.Comment;
                    deligateMail.NoteTitle = request._note.noteModel.NoteTitle;
                    deligateMail.noteId = _encryption.AesEncryptForEmail(request._note.noteModel.NoteId);

                    #region Notification Save
                    NotificationModel notificationModel = new();
                    notificationModel.Message = deligateMail.notecreator + " has replied to your query regarding the note titled " + request._note.noteModel.NoteTitle + ". Please review the response provided by the creator.";
                    notificationModel.NoteId = request._note.noteModel.NoteId;
                    notificationModel.Heading = "Note Query Reply";
                    notificationModel.ReceiverUserId = approver.approverForQuery.UserId;
                    notificationModel.Action = "PendingApproval";
                    string result = await _iSave.SaveNotificationData(notificationModel);
                    #endregion

                    if (result == "success")
                    {
                        #region Query Reply Mail Send

                        DeligateBodySubject RespopnseBody = await _emailService.GetMailNoteQueryReply(deligateMail);
                        _logger.LogwriteInfo("SMTP Configuration details fetched successfully for QueryInitiate------" + Environment.NewLine + "and mail body is----- " + RespopnseBody.Body, loginUserId);
                        MailSender objMail = await _emailService.GetMailConfiguration();
                        objMail.Receiver = approver.approverForQuery.Email;
                        objMail.Subject = RespopnseBody.Subject;
                        objMail.Subject = objMail.Subject.Replace("Note_Name", request._note.noteModel.NoteTitle);
                        objMail.Body = RespopnseBody.Body;
                        _logger.LogwriteInfo("Before sending the mail for QueryInitiate------", loginUserId);
                        bool result1 = await _iMailService.EmailSend(objMail);
                        #endregion
                        _logger.LogwriteInfo("QueryInitiate mail send status--------"+result1, loginUserId);

                    }
                    else
                    {
                        _logger.LogwriteInfo("Data not save in notification table------", loginUserId);
                    }

                    #endregion
                    return true;
                }
                catch (Exception e)
                {
                    _logger.LogwriteInfo("exception occur during QueryReplyHandler inner try block-------" + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                    return true;
                }

            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during QueryReplyHandler-------" + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                return false;
            }
        }
    }
}
