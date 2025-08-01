using DNAS.Application.Business.Interface;
using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Application.IRepository;
using DNAS.Application.IService;
using DNAS.Domain.DTO.Comment;
using DNAS.Domain.DTO.Note;
using DNAS.Domian.Common;
using DNAS.Domian.DAO.DbHelperModels.DeletegateApprover;
using DNAS.Domian.DAO.DbHelperModels.MailNotification;
using DNAS.Domian.DTO.DelegateAsign;
using DNAS.Domian.DTO.MailSend;
using DNAS.Domian.DTO.Notification;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Security.Claims;

namespace DNAS.Application.Features.Note
{
    public class AsignDelegateCommand(RequestApproverNoteModel note) : IRequest<int>
    {
        public RequestApproverNoteModel _note { get; set; } = note;
    }
    internal sealed class AsignDelegateHandler(ISave iSave, ICustomLogger logger, IDapperFactory iDapperFactory, IMailService iMailService, IEncryption encryption, IEmailService _emailService, IHttpContextAccessor haccess) : IRequestHandler<AsignDelegateCommand, int>
    {
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        private readonly ISave _iSave = iSave;
        public readonly ICustomLogger _logger = logger;
        public readonly IMailService _iMailService = iMailService;
        private readonly IEncryption _encryption = encryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<int> Handle(AsignDelegateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request._note.noteModel.NoteId = _encryption.AesDecrypt(request._note.noteModel.NoteId);
                               

                ProcFetchUserAndApproverForDelegateInput InParams = new()
                {
                    @SearchKey = request._note.noteModel.SearchKey,
                    @NoteId = request._note.noteModel.NoteId,
                    @UserId = request._note.noteModel.UserId
                };
                DelegateAsignModel dbuser = await _iDapperFactory.ExecuteSpDapperAsync<FindUserDetails, DelegateApprover, NoteDetails, DelegateAsignModel>(OraStoredProcedureNames.ProcFetchUserAndApproverForDelegate, InParams);

                if (dbuser.noteDetails.UserId == dbuser.findUserDetails.UserId)
                {
                    _logger.LogwriteInfo("Creator and approver can not be same person.", loginUserId);
                    return 2;
                }
                ProcFetchApproverListForDelegateInput InputNoteId = new()
                {                    
                    @NoteId = request._note.noteModel.NoteId
                };
                DelegateAsignListModel listdata= await _iDapperFactory.ExecuteSpDapperAsync<DelegateApproverlist, DelegateAsignListModel>(OraStoredProcedureNames.ProcFetchApproverListForDelegate, InputNoteId);
                var findresult = listdata.delegateApproverlist.Where(d => d.Email == request._note.noteModel.SearchKey);
                if (findresult.Any())
                {
                    _logger.LogwriteInfo("Delegate person already in approver list.", loginUserId);
                    return 3;
                }

                dbuser.delegateAsign.DeligatedUserId = request._note.noteModel.UserId;
                dbuser.delegateAsign.ApproverID = dbuser.delegateApprover.ApproverId;
                dbuser.delegateApprover.UserId = dbuser.findUserDetails.UserId;
                dbuser.delegateApprover.NoteId = request._note.noteModel.NoteId;
                dbuser.delegateAsign.DelegateBy= request._note.noteModel.UserId;
                string str = await _iSave.SaveDelegateAndUpdateApprover(dbuser);
                if (str == "Failed")
                {
                    return 1;
                }

                #region Save Asign Delegate Comment
                AsignDelegateNoteInputModel asignDelegateNoteInputModel = new AsignDelegateNoteInputModel();
                asignDelegateNoteInputModel.NoteId = request._note.noteModel.NoteId;
                asignDelegateNoteInputModel.ApproverId = request._note.noteModel.UserId;
                asignDelegateNoteInputModel.Comment = request._note.querymodel.Comment.Replace("\r\n", "<br/>");
                asignDelegateNoteInputModel.NoteStatus = "DelegateComment";
                if (await _iSave.SaveAsignDelegateComment(asignDelegateNoteInputModel))
                {
                    _logger.LogwriteInfo("Delegate Comment save successfuly", loginUserId);
                }
                else
                {
                    _logger.LogwriteInfo("Delegate Comment save failed", loginUserId);
                }
                #endregion

                #region Notification save and Email send                       

                ProcFetchForDelegateMailInparam InParams1 = new()
                {
                    @NoteId = request._note.noteModel.NoteId,
                    @WhoDelegate = dbuser.delegateAsign.DeligatedUserId,
                    @ToWhome = dbuser.findUserDetails.UserId
                };
                DelegateMailSendModel datauser = await _iDapperFactory.ExecuteSpDapperAsync<NotesCreator, DelegateSender, DelegateReceiver, DelegateMailSendModel>(OraStoredProcedureNames.ProcFetchForDelegateMail, InParams1);
                DeligateMail deligateMail = new()
                {
                    notecreator = datauser.notesCreator.FirstName + (datauser.notesCreator.MiddleName != " " ? " " + datauser.notesCreator.MiddleName + " " : " ") + datauser.notesCreator.LastName,
                    delegateSender = datauser.delegateSender.FirstName + (datauser.delegateSender.MiddleName != "" ? " " + datauser.delegateSender.MiddleName + " " : " ") + datauser.delegateSender.LastName,
                    delegateReceiver = datauser.delegateReceiver.FirstName + (datauser.delegateReceiver.MiddleName != "" ? " " + datauser.delegateReceiver.MiddleName + " " : " ") + datauser.delegateReceiver.LastName,
                    NoteTitle = request._note.noteModel.NoteTitle,
                    noteId= request._note.noteModel.NoteId,
                };
                #region Notification Save
                NotificationModel notificationModel = new()
                {
                    Message =$"{deligateMail.delegateSender} has delegated the approval of your note titled {request._note.noteModel.NoteTitle} to {deligateMail.delegateReceiver}. Please be aware that {deligateMail.delegateReceiver} will now be responsible for reviewing and approving your note.",
                    NoteId = request._note.noteModel.NoteId,
                    Heading = "Note Delegated",
                    ReceiverUserId = datauser.notesCreator.UserId,
                    Action = "None",
                };
                string result = await _iSave.SaveNotificationData(notificationModel);
                #endregion

                if (result == "failed")
                {
                    _logger.LogwriteInfo("Data not save in notification table------", loginUserId);
                }
                #region Delegate Mail Send to Note Creator

                DeligateBodySubject RespopnseBody = await _emailService.GetMailNoteCreatorMailForDelegate(deligateMail);

                _logger.LogwriteInfo("SMTP Configuration details fetched successfully for Delegate------" + Environment.NewLine + "and mail body is----- " + RespopnseBody.Body, loginUserId);
                MailSender objMail = await _emailService.GetMailConfiguration();
                objMail.Receiver = datauser.notesCreator.Email;                
                objMail.Subject = RespopnseBody.Subject.Replace("Note_Name", request._note.noteModel.NoteTitle);
                objMail.Body = RespopnseBody.Body;
                _logger.LogwriteInfo("Before sending the mail for Delegate------", loginUserId);
                bool ISdeligatemailSend = await _iMailService.EmailSend(objMail);
                _logger.LogwriteInfo("After sending the mail for Delegate-" + ISdeligatemailSend, loginUserId);
                #endregion


                #region Delegate Mail Send to Delegated Approver
                deligateMail.noteId = _encryption.AesEncryptForEmail(deligateMail.noteId);
                DeligateBodySubject RespopnseWhomeBody = await _emailService.GetMailNoteCreatorMailForWhome(deligateMail);
                _logger.LogwriteInfo("SMTP Configuration details fetched successfully for Delegate------" + Environment.NewLine + "and mail body is----- " + RespopnseWhomeBody.Body, loginUserId);
                objMail.Receiver = datauser.delegateReceiver.Email;                
                objMail.Subject = RespopnseWhomeBody.Subject.Replace("Note_Name", request._note.noteModel.NoteTitle);
                objMail.Body = RespopnseWhomeBody.Body;
                objMail.CC = datauser.notesCreator.Email;
                _logger.LogwriteInfo("Before sending the mail for Delegate------", loginUserId);
                bool ISWhomedeligatemailSend = await _iMailService.EmailSend(objMail);
                _logger.LogwriteInfo("After sending the mail for Delegate Whom-" + ISWhomedeligatemailSend, loginUserId);
                #endregion

                if (ISWhomedeligatemailSend)
                {
                    _logger.LogwriteInfo("Mail send successfully for Delegate--------", loginUserId);                    
                }
                else
                {
                    _logger.LogwriteInfo("Mail not send to the delegated approver--------", loginUserId);
                }
                return 0;
                #endregion

            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during SaveQueryInitiateEF-------" + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                return 1;
            }
        }
    }
}
