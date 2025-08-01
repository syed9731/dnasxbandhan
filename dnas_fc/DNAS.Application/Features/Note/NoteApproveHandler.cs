using DNAS.Application.Business.Interface;
using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Application.IRepository;
using DNAS.Application.IService;
using DNAS.Domain.DTO.Comment;
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
    public class NoteApproveCommand(RequestApproverNoteModel note) : IRequest<bool>
    {
        public RequestApproverNoteModel _note { get; set; } = note;
    }
    internal sealed class NoteApproveHandler(ISave iSave, IUpdate iupdate, ICustomLogger logger, IDapperFactory iDapperFactory, IMailService iMailService, IEncryption encryption, IEmailService _emailService, IHttpContextAccessor haccess) : IRequestHandler<NoteApproveCommand, bool>
    {
        private readonly ISave _iSave = iSave;
        private readonly IUpdate _Update = iupdate;
        public readonly ICustomLogger _logger = logger;
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        public readonly IMailService _iMailService = iMailService;
        private readonly IEncryption _encryption = encryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<bool> Handle(NoteApproveCommand request, CancellationToken cancellationToken)
        {
            bool Response = false;
            try
            {
                request._note.noteModel.NoteId = _encryption.AesDecrypt(request._note.noteModel.NoteId);
                ProcFetchApproverInput InParams = new()
                {
                    @NoteId = request._note.noteModel.NoteId,
                    @UserId = request._note.noteModel.UserId
                };
                CommentReqModel commentReqModel = new();
                commentReqModel.NoteId = request._note.noteModel.NoteId;
                commentReqModel.Comment= request._note.querymodel.Comment.Replace("\r\n", "<br/>");
                commentReqModel.ApproverId= request._note.noteModel.UserId;
                bool SaveApproveResult = await _iSave.SaveApproveComment(commentReqModel);
                if (!SaveApproveResult)
                {
                    _logger.LogwriteInfo("Approve comment not save in databse response return-"+ SaveApproveResult, loginUserId);
                    return Response;
                }
                ApproverData dbapprover = await _iDapperFactory.ExecuteSpDapperAsync<Domian.DTO.Approver.ApproverModel, ApproverData>(OraStoredProcedureNames.ProcFetchApprover, InParams);
                Domian.DTO.Approver.ApproverModel approver = new();
                approver.NoteId = request._note.noteModel.NoteId;
                approver.UserId = request._note.noteModel.UserId;
                approver.IsApproved = "True";
                approver.IsCurrentApprover = "False";
                approver.ApprovedTime = "ForApprovedUser";
                approver.ApproverId = dbapprover.approver.ApproverId;
                Response = await _Update.UpdateApproverData(approver);
                if (!Response)
                {
                    _logger.LogwriteInfo("Update Note command failed", loginUserId);
                    return Response;
                }
                _logger.LogwriteInfo("Update approver successfully done", loginUserId);
                ProcFetchTopApproverInput InParams1 = new()
                {
                    @NoteId = request._note.noteModel.NoteId
                };
                ApproverData approver1 = await _iDapperFactory.ExecuteSpDapperAsync<DNAS.Domian.DTO.Approver.ApproverModel, ApproverData>(OraStoredProcedureNames.ProcFetchTopApprover, InParams1);

                if (approver1.approver.UserId != "")
                {
                    //send mail to next approver
                    DeligateMail deligateMail = new();
                    deligateMail.nextApprover = approver1.approver.FirstName + (approver1.approver.MiddleName != " " ? " " + approver1.approver.MiddleName + " " : " ") + approver1.approver.LastName;
                    Domian.DTO.Approver.ApproverModel approver2 = new();
                    approver2.NoteId = request._note.noteModel.NoteId;
                    approver2.UserId = approver1.approver.UserId;
                    approver2.IsApproved = "False";
                    approver2.IsCurrentApprover = "True";
                    approver2.ApproverId = approver1.approver.ApproverId;
                    approver2.ApprovedTime = "UpdateForNextApprover";
                    bool Response1 = await _Update.UpdateApproverData(approver2);

                    #region Notification save and Email send                   


                    ProcFetchCreatorApproverNoteInparam InParams3 = new()
                    {
                        @NoteId = request._note.noteModel.NoteId,
                        @ApproverUserId = request._note.noteModel.UserId
                    };
                    SendBackDataModel datauser = await _iDapperFactory.ExecuteSpDapperAsync<NotesCreator, NotesApprover, SendBackDataModel>(OraStoredProcedureNames.ProcFetchCreatorApproverNote, InParams3);

                    if (datauser == null)
                    {
                        _logger.LogwriteInfo("ProcFetchDataForFYIMailSend return null value------", loginUserId);
                        return false;
                    }
                    deligateMail.notecreator = datauser.notesCreator.FirstName + (datauser.notesCreator.MiddleName != " " ? " " + datauser.notesCreator.MiddleName + " " : " ") + datauser.notesCreator.LastName;
                    deligateMail.noteApprover = datauser.notesApprover.FirstName + (datauser.notesApprover.MiddleName != "" ? " " + datauser.notesApprover.MiddleName + " " : " ") + datauser.notesApprover.LastName;
                    deligateMail.NoteTitle = datauser.notesCreator.NoteTitle;
                    deligateMail.noteId = _encryption.AesEncryptForEmail(request._note.noteModel.NoteId);
                    #region Next Approver
                    if (Response1)
                    {
                        //mail and notification send to next approver
                        #region Notification Save
                        NotificationModel notificationModel = new();
                        notificationModel.Message = "This is to inform you that note titled " + request._note.noteModel.NoteTitle + " has been approved by " + deligateMail.noteApprover + ".This note was created by " + deligateMail.notecreator + ". The note is now in your approval queue.";
                        notificationModel.NoteId = request._note.noteModel.NoteId;
                        notificationModel.Heading = "Note in approval queue";
                        notificationModel.ReceiverUserId = approver1.approver.UserId;
                        notificationModel.Action = "PendingNote";
                        string result = await _iSave.SaveNotificationData(notificationModel);
                        #endregion
                        if (result != "success")
                        {
                            _logger.LogwriteInfo("Data not save in notification table for next approver------", loginUserId);
                            return false;
                        }
                        #region NextApprover Mail Send

                        DeligateBodySubject RespopnseBody = await _emailService.GetMailNextNoteApprover(deligateMail);

                        _logger.LogwriteInfo("SMTP Configuration details fetched successfully for Next Approver------" + Environment.NewLine + "and mail body is----- " + RespopnseBody.Body, loginUserId);
                        MailSender objMail = await _emailService.GetMailConfiguration();
                        objMail.Receiver = approver1.approver.Email;
                        objMail.Subject = RespopnseBody.Subject;
                        objMail.Subject = objMail.Subject.Replace("Note_Name", datauser.notesCreator.NoteTitle);
                        objMail.Body = RespopnseBody.Body;
                        objMail.CC = datauser.notesCreator.Email;
                        _logger.LogwriteInfo("Before sending the mail for Next Approver------", loginUserId);
                        bool result2 = await _iMailService.EmailSend(objMail);
                        #endregion
                        _logger.LogwriteInfo("sending mail to next approver mail-send status------"+ result2, loginUserId);
                        return true;
                        
                    }
                    #endregion


                    #region Note Creator

                    #region Notification Save
                    NotificationModel notificationModel1 = new();
                    notificationModel1.Message = "This is to inform you that note titled " + request._note.noteModel.NoteTitle + " has been approved by " + deligateMail.noteApprover + ".";
                    notificationModel1.NoteId = request._note.noteModel.NoteId;
                    notificationModel1.Heading = "Note Approved By Approver";
                    notificationModel1.ReceiverUserId = datauser.notesCreator.UserId;
                    notificationModel1.Action = "PendingNote";
                    await _iSave.SaveNotificationData(notificationModel1);
                    #endregion

                    deligateMail.noteId = _encryption.AesDecrypt(deligateMail.noteId);
                    DeligateBodySubject RespopnseBody1 = await _emailService.GetMailApproverApprovedToCreator(deligateMail);

                    _logger.LogwriteInfo("SMTP Configuration details fetched successfully for note creator mail send------" + Environment.NewLine + "and mail body is----- " + RespopnseBody1.Body, loginUserId);
                    MailSender objMail1 = await _emailService.GetMailConfiguration();
                    objMail1.Receiver = datauser.notesCreator.Email;

                    objMail1.Subject = RespopnseBody1.Subject;
                    objMail1.Subject = objMail1.Subject.Replace("Note_Name", request._note.noteModel.NoteTitle);

                    objMail1.Body = RespopnseBody1.Body;
                    _logger.LogwriteInfo("Before sending the mail for note creator mail send------", loginUserId);
                    bool result1 = await _iMailService.EmailSend(objMail1);

                    _logger.LogwriteInfo("Mail send to approver mail-send status------"+ result1, loginUserId);
                    
                    return true;

                    #endregion


                    #endregion

                }
                else
                {
                    //no approver left note now approved
                    bool Response1 = false;
                    NoteModel note = new NoteModel();
                    note.NoteId = request._note.noteModel.NoteId;
                    note.NoteStatus = "Approved";
                    Response1 = await _Update.UpdateNoteData(note);
                    if (!Response1)
                    {
                        _logger.LogwriteInfo("Note data not updated for final note approved", loginUserId);
                        return false;
                    }
                    else
                    {
                        #region Transfer Data To Note_Approved Table
                        bool noteApprovedResponse = await _iSave.TransferToNoteApproved(note.NoteId);
                        if (!noteApprovedResponse) 
                        {
                            _logger.LogwriteInfo("Data Note saved in Note_Approved table. We are revart back note status and final approver status.", loginUserId);
                            if(!await _Update.RevartBackNoteStatus(note.NoteId, approver.ApproverId))
                            {
                                _logger.LogwriteInfo("Revart back failed for noteid- "+ note.NoteId, loginUserId);
                            }
                            else
                            {
                                _logger.LogwriteInfo("Revart back successfuly done for noteid- " + note.NoteId, loginUserId);
                            }
                            return false;
                        }
                        #endregion
                    }
                    //mail send for note finally approved
                    _logger.LogwriteInfo("Note Finally Approved", loginUserId);
                    ProcFetchNoteApproversAndCreatorInparam InParams2 = new()
                    {
                        @NoteId = request._note.noteModel.NoteId
                    };
                    NoteApprovedModel Approvedmailsend = await _iDapperFactory.ExecuteSpDapperAsync<NoteCreator, NoteApprover, NoteApprovedModel>(OraStoredProcedureNames.ProcFetchNoteApproversAndCreator, InParams2);
                    if (Approvedmailsend.noteCreator.Email != "")
                    {
                        //mail send to creator

                        #region Notification save and Email send Note Creator                      


                        ProcFetchCreatorApproverNoteInparam InParams3 = new()
                        {
                            @NoteId = request._note.noteModel.NoteId,
                            @ApproverUserId = request._note.noteModel.UserId
                        };
                        SendBackDataModel datauser = await _iDapperFactory.ExecuteSpDapperAsync<NotesCreator, NotesApprover, SendBackDataModel>(OraStoredProcedureNames.ProcFetchCreatorApproverNote, InParams3);

                        DeligateMail deligateMail = new();
                        deligateMail.notecreator = Approvedmailsend.noteCreator.FirstName + (Approvedmailsend.noteCreator.MiddleName != " " ? " " + Approvedmailsend.noteCreator.MiddleName + " " : " ") + Approvedmailsend.noteCreator.LastName;
                        deligateMail.noteApprover = datauser.notesApprover.FirstName + (datauser.notesApprover.MiddleName != "" ? " " + datauser.notesApprover.MiddleName + " " : " ") + datauser.notesApprover.LastName;
                        deligateMail.NoteTitle = Approvedmailsend.noteCreator.NoteTitle;
                        deligateMail.noteId = request._note.noteModel.NoteId;

                        #region Note Creator
                        #region Notification Save
                        NotificationModel notificationModel = new();
                        notificationModel.Message = "We are pleased to inform you that your note titled " + request._note.noteModel.NoteTitle + " has been approved by " + deligateMail.noteApprover + ".";
                        notificationModel.NoteId = request._note.noteModel.NoteId;
                        notificationModel.Heading = "Note Approved";
                        notificationModel.ReceiverUserId = Approvedmailsend.noteCreator.UserId;
                        notificationModel.Action = "NoteApproved";
                        await _iSave.SaveNotificationData(notificationModel);
                        #endregion

                        #region Note Creator Final Approver Mail Send

                        string ccapprover = string.Empty;

                        ccapprover = string.Join(",", Approvedmailsend.noteApprovers.Select(item => item.Email));

                        DeligateBodySubject RespopnseBody = await _emailService.GetMailApproverApprovedToCreatorFinal(deligateMail);

                        _logger.LogwriteInfo("SMTP Configuration details fetched successfully for note approve------" + Environment.NewLine + "and mail body is----- " + RespopnseBody.Body, loginUserId);
                        MailSender objMail = await _emailService.GetMailConfiguration();
                        objMail.Receiver = Approvedmailsend.noteCreator.Email;
                        objMail.CC = ccapprover;

                        objMail.Subject = RespopnseBody.Subject;
                        objMail.Subject = objMail.Subject.Replace("Note_Name", deligateMail.NoteTitle);

                        objMail.Body = RespopnseBody.Body;
                        _logger.LogwriteInfo("Before sending the mail for note approve------", loginUserId);
                        bool result1 = await _iMailService.EmailSend(objMail);
                        #endregion
                        _logger.LogwriteInfo("final note approve mail send status------" + result1, loginUserId);
                        return true;
                        #endregion

                        #endregion
                    }
                    else
                    {
                        _logger.LogwriteInfo("Note Approved but Creator fetch from database not done. Mail not send", loginUserId);
                    }
                }
                return Response;
            }
            catch (Exception ex)
            {
                _logger.LogwriteError(ex.ToString(), loginUserId);
                return Response;
            }

        }

    }
}
