using DNAS.Application.Business.Interface;
using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Application.IRepository;
using DNAS.Application.IService;
using DNAS.Domain.DTO.Note;
using DNAS.Domian.Common;
using DNAS.Domian.DAO.DbHelperModels.FYI;
using DNAS.Domian.DAO.DbHelperModels.MailNotification;
using DNAS.Domian.DTO.DelegateAsign;
using DNAS.Domian.DTO.MailSend;
using DNAS.Domian.DTO.Note;
using DNAS.Domian.DTO.Notification;
using DNAS.Domian.DTO.UserMaster;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Note
{
    public class SaveFyiCommand(RequestApproverNoteModel note) : IRequest<string>
    {
        public RequestApproverNoteModel _note { get; set; } = note;
    }
    internal sealed class SaveFyiHandler(ISave iSave, ICustomLogger logger, IDapperFactory iDapperFactory, IMailService iMailService, IEncryption encryption, IEmailService _emailService, IHttpContextAccessor haccess) : IRequestHandler<SaveFyiCommand, string>
    {
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        private readonly ISave _iSave = iSave;
        public readonly ICustomLogger _logger = logger;
        public readonly IMailService _iMailService = iMailService;
        private readonly IEncryption _iEncryption = encryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<string> Handle(SaveFyiCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool str = false;
                request._note.noteModel.NoteId = _iEncryption.AesDecrypt(request._note.noteModel.NoteId);
                request._note.noteModel.SearchKey=_iEncryption.AesDecrypt(request._note.noteModel.SearchKey);
                PendingNoteModel model = new();
                //ProcFetchUserAsPerEmailOrEmpIdInput InParams = new()
                //{
                //    SearchKey = request._note.noteModel.SearchKey
                //};
                //FyiUserModel dbuser = await _iDapperFactory.ExecuteSpDapperAsync<UsersModel, FyiUserModel>(OraStoredProcedureNames.ProcFetchUserAsPerEmailOrEmpId, InParams);                
                if (request._note.noteModel.CreatorUserId != request._note.noteModel.SearchKey)
                {
                    model.fyiModel.WhoTagged = request._note.noteModel.UserId;
                    model.fyiModel.NoteId = request._note.noteModel.NoteId;
                    model.fyiModel.ToWhome = request._note.noteModel.SearchKey; //dbuser.fyiUsersModel.UserId;
                    str = await _iSave.SaveForYourInformatinData(model);
                    if (!str)
                    {
                        return "Failed";
                    }
                    try
                    {
                        #region Notification save and Email send                       

                        ProcFetchDataForFyiMailSend InParams1 = new()
                        {
                            @NoteId = request._note.noteModel.NoteId,
                            @WhoTagged = request._note.noteModel.UserId,
                            @ToWhome = request._note.noteModel.SearchKey//dbuser.fyiUsersModel.UserId
                        };
                        FyiDataModel datauser = await _iDapperFactory.ExecuteSpDapperAsync<NotesCreator, FyiSender, FyiReceiver, FyiDataModel>(OraStoredProcedureNames.ProcFetchDataForFYIMailSend, InParams1);

                        DeligateMail deligateMail = new();
                        deligateMail.notecreator = datauser.notesCreator.FirstName + (datauser.notesCreator.MiddleName != " " ? " " + datauser.notesCreator.MiddleName + " " : " ") + datauser.notesCreator.LastName;
                        deligateMail.FyiSender = datauser.fyiSender.FirstName + (datauser.fyiSender.MiddleName != "" ? " " + datauser.fyiSender.MiddleName + " " : " ") + datauser.fyiSender.LastName;
                        deligateMail.FyiReceiver = datauser.fyiReceiver.FirstName + (datauser.fyiReceiver.MiddleName != "" ? " " + datauser.fyiReceiver.MiddleName + " " : " ") + datauser.fyiReceiver.LastName;
                        deligateMail.NoteTitle = request._note.noteModel.NoteTitle;
                        deligateMail.noteId = request._note.noteModel.NoteId;
                        #region Notification Save
                        NotificationModel notificationModel = new();
                        notificationModel.Message = "This is to inform you that " + deligateMail.FyiSender +
                                    " has shared the note titled " + request._note.noteModel.NoteTitle + " with you. This note was created by " + deligateMail.notecreator + ".";
                        notificationModel.NoteId = request._note.noteModel.NoteId;
                        notificationModel.Heading = "For your information";
                        notificationModel.ReceiverUserId = request._note.noteModel.SearchKey;//dbuser.fyiUsersModel.UserId;
                        notificationModel.Action = "FYI";
                        string result = await _iSave.SaveNotificationData(notificationModel);
                        #endregion

                        if (result != "success")
                        {
                            _logger.LogwriteInfo("Data not save in notification table------", loginUserId);
                        }
                        #region FYI Mail Send
                        DeligateBodySubject RespopnseBody = await _emailService.GetMailNoteFyiToUser(deligateMail);


                        _logger.LogwriteInfo("SMTP Configuration details fetched successfully for FYI------" + Environment.NewLine + "and mail body is----- " + RespopnseBody.Body, loginUserId);
                        MailSender objMail = await _emailService.GetMailConfiguration();
                        objMail.Receiver = datauser.fyiReceiver.Email;
                        objMail.Subject = RespopnseBody.Subject;
                        objMail.Subject = objMail.Subject.Replace("Note_Name", request._note.noteModel.NoteTitle);
                        objMail.Body = RespopnseBody.Body;
                        objMail.CC = datauser.notesCreator.Email;
                        _logger.LogwriteInfo("Before sending the mail for FYI------", loginUserId);
                        bool result1 = await _iMailService.EmailSend(objMail);
                        #endregion

                        _logger.LogwriteInfo("FYI mail send status--------"+result1, loginUserId);
                        

                        #endregion
                    }
                    catch (Exception e)
                    {
                        _logger.LogwriteInfo("exception occur during mail send from fyi or notification save------" + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                    }

                }
                else
                {
                    return "CreatorSame";
                }
                return "success";

            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during SaveQueryInitiateEF-------" + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                return "Failed";
            }

        }

    }
}
