using DNAS.Application.Business.Interface;
using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Application.IRepository;
using DNAS.Application.IService;
using DNAS.Domain.DAO.DbHelperModels.DelegateByCreator;
using DNAS.Domain.DTO.Comment;
using DNAS.Domain.DTO.DelegateByCreator;
using DNAS.Domain.DTO.Note;
using DNAS.Domian.Common;
using DNAS.Domian.DAO.DbHelperModels.MailNotification;
using DNAS.Domian.DTO.MailSend;
using DNAS.Domian.DTO.Notification;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Security.Claims;

namespace DNAS.Application.Features.Note
{
    public class DelegateByCreatorCommand(WithdrawNoteModel note) : IRequest<int>
    {
        public WithdrawNoteModel _note { get; set; } = note;
    }
    internal sealed class DelegateByCreatorHandler(ISave iSave, ICustomLogger logger, IDapperFactory iDapperFactory, IMailService iMailService, IEncryption encryption, IEmailService _emailService, IHttpContextAccessor haccess) : IRequestHandler<DelegateByCreatorCommand, int>
    {
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        private readonly ISave _iSave = iSave;
        public readonly ICustomLogger _logger = logger;
        public readonly IMailService _iMailService = iMailService;
        private readonly IEncryption _encryption = encryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<int> Handle(DelegateByCreatorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request._note.noteModel.NoteId = _encryption.AesDecrypt(request._note.noteModel.NoteId);


                ProcFetchDelegateByCreatorInparam InParams = new()
                {
                    @SearchKey =_encryption.AesDecrypt(request._note.noteModel.SearchKey),
                    @NoteId = request._note.noteModel.NoteId,
                    @OldApprover = request._note.creatorDelegate.oldapproverid,
                    @CreatorId = request._note.creatorDelegate.creatorid
                };
                DelegateByCreatorModel dbuser = await _iDapperFactory.ExecuteSpDapperAsync<NewApprover, OldApprover, ApproverList, CreatorDetails, NoteDetails, DelegateByCreatorModel>(OraStoredProcedureNames.ProcFetchDelegateByCreator, InParams);
                dbuser.newApprover.UserId = InParams.SearchKey;
                if (string.IsNullOrWhiteSpace(dbuser.newApprover.UserId))
                {
					return 4;
				}
                if (dbuser.newApprover.UserId == request._note.creatorDelegate.creatorid)
                {
                    _logger.LogwriteInfo("Creator and approver can not be same person.", loginUserId);
                    return 2;
                }                
                var findresult = dbuser.approverlist.Where(d => d.UserId == dbuser.newApprover.UserId);
                if (findresult.Any())
                {
                    _logger.LogwriteInfo("Delegate person already in approver list.", loginUserId);
                    return 3;
                }
                if (! await _iSave.SaveDelegateByCreator(dbuser))
                {
                    return 1;
                }

                #region Save Asign Delegate Comment
                AsignDelegateNoteInputModel asignDelegateNoteInputModel = new AsignDelegateNoteInputModel();
                asignDelegateNoteInputModel.NoteId = dbuser.noteDetails.NoteId;
                asignDelegateNoteInputModel.ApproverId = dbuser.creatorDetails.UserId;
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

                //#region Notification save and Email send                       

                ProcFetchForDelegateMailInparam InParams1 = new()
                {
                    @NoteId = request._note.noteModel.NoteId,
                    @WhoDelegate = request._note.creatorDelegate.creatorid,
                    @ToWhome = dbuser.newApprover.UserId,
				};
                DelegateMailSendModel datauser = await _iDapperFactory.ExecuteSpDapperAsync<NotesCreator, DelegateSender, DelegateReceiver, DelegateMailSendModel>(OraStoredProcedureNames.ProcFetchForDelegateMail, InParams1);
                Domian.DTO.DelegateAsign.DeligateMail deligateMail = new()
                {
                    notecreator = datauser.notesCreator.FirstName + (datauser.notesCreator.MiddleName != " " ? " " + datauser.notesCreator.MiddleName + " " : " ") + datauser.notesCreator.LastName,
                    delegateSender = datauser.delegateSender.FirstName + (datauser.delegateSender.MiddleName != "" ? " " + datauser.delegateSender.MiddleName + " " : " ") + datauser.delegateSender.LastName,
                    delegateReceiver = datauser.delegateReceiver.FirstName + (datauser.delegateReceiver.MiddleName != "" ? " " + datauser.delegateReceiver.MiddleName + " " : " ") + datauser.delegateReceiver.LastName,
                    NoteTitle = request._note.noteModel.NoteTitle,
                    noteId = request._note.noteModel.NoteId,
					Comment = asignDelegateNoteInputModel.Comment
				};
                //#region Notification Save
                NotificationModel notificationModel = new()
                {
                    Message = $"{deligateMail.delegateSender} has delegated the approval of your note titled {request._note.noteModel.NoteTitle} to {deligateMail.delegateReceiver}. Please be aware that {deligateMail.delegateReceiver} will now be responsible for reviewing and approving your note.",
                    NoteId = request._note.noteModel.NoteId,
                    Heading = "Note Delegated",
                    ReceiverUserId = datauser.notesCreator.UserId,
                    Action = "None"
				};
                string result = await _iSave.SaveNotificationData(notificationModel);
                //#endregion

                if (result == "failed")
                {
                    _logger.LogwriteInfo("Data not save in notification table------", loginUserId);
                }
                #region Delegate Mail Send to Note Creator

                Domian.DTO.DelegateAsign.DeligateBodySubject RespopnseBody = await _emailService.GetMailToCreatorDelegate(deligateMail);

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

				return 0;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during SaveQueryInitiateEF-------" + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                return 1;
            }
        }
    }
}
