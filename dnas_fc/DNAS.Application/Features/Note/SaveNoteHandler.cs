using DNAS.Application.Business.Interface;
using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Application.IRepository;
using DNAS.Application.IService;
using DNAS.Domian.Common;
using DNAS.Domian.DAO.DbHelperModels.MailNotification;
using DNAS.Domian.DTO.DelegateAsign;
using DNAS.Domian.DTO.Login;
using DNAS.Domian.DTO.MailSend;
using DNAS.Domian.DTO.Note;
using DNAS.Domian.DTO.Notification;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Note
{
    public class SaveNoteCommand(NoteModel note) : IRequest<NoteModel>
    {
        public NoteModel _note { get; set; } = note;        
    }
    internal sealed class SaveNoteHandler(ISave iSave, ICustomLogger logger, ILogin iLogin, IDapperFactory iDapperFactory, IMailService iMailService, IEmailService _emailService, IEncryption encryption, IHttpContextAccessor haccess) : IRequestHandler<SaveNoteCommand, NoteModel>
    {
        private readonly ISave _iSave = iSave;
        public readonly ICustomLogger _logger = logger;
        private readonly ILogin _iLogin = iLogin;
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        public readonly IMailService _iMailService = iMailService;
        private readonly IEncryption _iEncryption = encryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<NoteModel> Handle(SaveNoteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var inparam = new
                {
                    @UserId = request._note.UserId,
                };
                UserMasterModel user = await _iLogin.FetchUserData(inparam);
                if (user.Department != "")
                {
                    _logger.LogwriteError("Creator department id fetch successfully done", loginUserId);
                    request._note.CreatorDepartment = user.Department;
                }
                else
                {
                    _logger.LogwriteError("Creator department id not found", loginUserId);
                }

                if (request._note.CategoryId == "1")
                {
                    request._note.TotalAmount = (Convert.ToDecimal(request._note.OperationalExpenditure) + Convert.ToDecimal(request._note.CapitalExpenditure)).ToString();
                }

                NoteModel result = await _iSave.SaveNote(request._note);

                #region Approver mail send
                ProcFetchApproverForMailSendInparam InParams3 = new()
                {
                    @NoteId = result.NoteId
                };
                ApproverMailSendModel datauser = await _iDapperFactory.ExecuteSpDapperAsync<NotesCreator, NotesApprover1, ApproverMailSendModel>(OraStoredProcedureNames.ProcFetchApproverForMailSend, InParams3);

                DeligateMail deligateMail = new();
                deligateMail.notecreator = datauser.notesCreator.FirstName + (datauser.notesCreator.MiddleName != " " ? " " + datauser.notesCreator.MiddleName + " " : " ") + datauser.notesCreator.LastName;
                deligateMail.noteApprover = datauser.notesApprover.FirstName + (datauser.notesApprover.MiddleName != "" ? " " + datauser.notesApprover.MiddleName + " " : " ") + datauser.notesApprover.LastName;
                deligateMail.NoteTitle = request._note.NoteTitle;
                deligateMail.noteId = _iEncryption.AesEncryptForEmail(result.NoteId);
                #region Notification Save
                NotificationModel notificationModel = new();
                notificationModel.Message = "A new note titled " + request._note.NoteTitle + " has been assigned to you for approval. This note was created by " + deligateMail.notecreator + ".";
                notificationModel.NoteId = result.NoteId;
                notificationModel.Heading = "A new note assigned";
                notificationModel.ReceiverUserId = datauser.notesApprover.UserId;
                notificationModel.Action = "PendingNote";
                string result3 = await _iSave.SaveNotificationData(notificationModel);
                #endregion
                if (result3 == "success")
                {
                    DeligateBodySubject RespopnseBody = await _emailService.GetMailToApproverForNoteAsign(deligateMail);

                    _logger.LogwriteInfo("SMTP Configuration details fetched successfully for note approver mail send------" + Environment.NewLine + "and mail body is----- " + RespopnseBody.Body, loginUserId);
                    MailSender objMail = await _emailService.GetMailConfiguration();
                    objMail.Receiver = datauser.notesApprover.Email;
                    objMail.Subject = RespopnseBody.Subject;
                    objMail.Subject = objMail.Subject.Replace("Note_Name", request._note.NoteTitle);
                    objMail.Body = RespopnseBody.Body;
                    objMail.CC = datauser.notesCreator.Email;
                    _logger.LogwriteInfo("Before sending the mail for note approver mail send------", loginUserId);
                    bool result1 = await _iMailService.EmailSend(objMail);

                    _logger.LogwriteInfo("Note approver mail send status--------" + result1, loginUserId);

                }

                #endregion
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogwriteError(ex.ToString(), loginUserId);
                return new NoteModel();
            }

        }

    }
}
