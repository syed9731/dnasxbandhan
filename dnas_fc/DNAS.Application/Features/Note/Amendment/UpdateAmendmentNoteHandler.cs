using DNAS.Application.Business.Interface;
using DNAS.Application.Common.Implementation;
using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Application.IRepository;
using DNAS.Application.IService;
using DNAS.Domain.DAO.DbHelperModels.Attachment;
using DNAS.Domain.DTO.Amendment;
using DNAS.Domain.DTO.Attachment;
using DNAS.Domain.DTO.CommonModel;
using DNAS.Domain.DTO.SendBack;
using DNAS.Domian.Common;
using DNAS.Domian.DAO.DbHelperModels.MailNotification;
using DNAS.Domian.DTO.DelegateAsign;
using DNAS.Domian.DTO.Login;
using DNAS.Domian.DTO.MailSend;
using DNAS.Domian.DTO.Note;
using DNAS.Domian.DTO.Notification;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

using System.Drawing;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DNAS.Application.Features.Note.Amendment
{

    public record UpdateAmendmentNoteCommand(AmendmentNoteModel request) : IRequest<CommonResponse<AmendmentNoteModel>>
    {
        public AmendmentNoteModel AmendmentNoteModel { get; set; } = request;
    }
    internal sealed class UpdateAmendmentNoteHandler(ICustomLogger logger, IMailService iMailService, ICheckExtension checkExtension, IHttpContextAccessor haccess, IOptions<AppConfig> appConfig, IFileValidation fileValidation, IDapperFactory iDapperFactory, IEmailService _emailService, ISave _iSave, ILogin iLogin, IEncryption encryption) : IRequestHandler<UpdateAmendmentNoteCommand, CommonResponse<AmendmentNoteModel>>
    {
        #region properties initialization
        public readonly ICustomLogger _logger = logger;
        public readonly IMailService _iMailService = iMailService;
        private readonly ICheckExtension _checkExtension = checkExtension;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        private readonly IFileValidation _fileValidation = fileValidation;
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        private readonly ILogin _iLogin = iLogin;
        private readonly IEncryption _iEncryption = encryption;
        #endregion
        public async Task<CommonResponse<AmendmentNoteModel>> Handle(UpdateAmendmentNoteCommand request, CancellationToken cancellationToken)
        {
            CommonResponse<AmendmentNoteModel> response = new();
            try
            {
                List<CommonAttachmentModel> newAttachmentList = new();

                request.AmendmentNoteModel.NoteId = !string.IsNullOrWhiteSpace(request.AmendmentNoteModel.NoteId) ? _iEncryption.AesDecrypt(request.AmendmentNoteModel.NoteId) : "";

                #region deserialize the AttachmentListJson
                if (!string.IsNullOrWhiteSpace(request.AmendmentNoteModel.AttachmentListJson))
                {
                    var existingAttachmentList = JsonSerializer.Deserialize<List<CommonAttachmentModel>>(request.AmendmentNoteModel.AttachmentListJson);

                    if (existingAttachmentList?.Count > 0)
                    {
                        newAttachmentList.AddRange(existingAttachmentList);
                    }
                }
                #endregion

                #region save the new attachment files in the folder and get the file path

                try
                {
                    string path = appConfig.Value.FileUploadPath;
                    //create folder if not exist
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    if (request.AmendmentNoteModel.AttachFiles?.Count > 0)
                    {
                        foreach (var file in request.AmendmentNoteModel.AttachFiles)
                        {
                            bool isFileValid = CheckFileValidation(file);
                            bool isFileExistsInDB = await CheckFileExistsInDB(file, request.AmendmentNoteModel.NoteId.ToString());

                            #region Duplicate Filename Exists
                            bool isDuplicateFileExists = false;
                            List<string> filenamelist = new List<string>();

                            if (filenamelist.Count > 0)
                            {
                                if (filenamelist.Contains(file.FileName))
                                {
                                    isDuplicateFileExists = true;
                                }
                                else
                                {
                                    filenamelist.Add(file.FileName);
                                    isDuplicateFileExists = false;
                                }
                            }
                            else
                            {
                                filenamelist.Add(file.FileName);
                                isDuplicateFileExists = false;
                            }
                            #endregion

                            if (isFileValid && !isFileExistsInDB && !isDuplicateFileExists)
                            {
                                CommonAttachmentModel attachment = new();
                                attachment.NoteId = request.AmendmentNoteModel.NoteId;

                                #region Random number generation

                                int sixDigitNumber = RandomNumberGenerator.GetInt32(100000, 1000000);

                                #endregion

                                string fileNameWithPath = Path.Combine(path, attachment.NoteId + "_Attachment_" + sixDigitNumber + Path.GetExtension(file.FileName));

                                using (var stream = new FileStream(fileNameWithPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                                {
                                    await file.CopyToAsync(stream);
                                    attachment.AttachmentPath = fileNameWithPath;
                                    attachment.DocumentName = file.FileName;
                                    _logger.LogwriteInfo("Upload file success : " + fileNameWithPath, loginUserId);
                                    newAttachmentList.Add(attachment);
                                }
                            }
                            else
                            {
                                _logger.LogwriteInfo("Because of restricted extension " + file.FileName + " can not be uploaded", loginUserId);
                            }
                        }
                        
                    }
                    request.AmendmentNoteModel.AttachmentList = newAttachmentList;
                }
                catch (Exception ex)
                {
                    _logger.LogwriteInfo("Upload file exception : " + ex.StackTrace, loginUserId);
                }


                #endregion

                #region fetch user data

                var inparam = new
                {
                    @UserId = request.AmendmentNoteModel.UserId,
                };
                UserMasterModel user = await _iLogin.FetchUserData(inparam);
                if (user.Department != "")
                {
                    _logger.LogwriteError("Creator department id fetch successfully done", loginUserId);
                    request.AmendmentNoteModel.CreatorDepartment = user.Department;
                }
                else
                {
                    _logger.LogwriteError("Creator department id not found", loginUserId);
                }

                #endregion

                #region total amount calculation based on category

                if (request.AmendmentNoteModel.CategoryId == "1")
                {
                    request.AmendmentNoteModel.TotalAmount = (Convert.ToDecimal(request.AmendmentNoteModel.OperationalExpenditure) + Convert.ToDecimal(request.AmendmentNoteModel.CapitalExpenditure)).ToString();
                }

                #endregion

                #region  database interaction and prepare response data
                AmendmentNoteModel result = await _iSave.SaveAmendNote(request.AmendmentNoteModel);
                #endregion

                if (result != null)
                {
                    #region send mail section

                    #region fetch approver for mail send

                    ProcFetchApproverForMailSendInparam InParams3 = new()
                    {
                        @NoteId = result.NoteId
                    };
                    ApproverMailSendModel datauser = await _iDapperFactory.ExecuteSpDapperAsync<NotesCreator, NotesApprover1, ApproverMailSendModel>(OraStoredProcedureNames.ProcFetchApproverForMailSend, InParams3);

                    DeligateMail deligateMail = new();
                    deligateMail.notecreator = datauser.notesCreator.FirstName + (datauser.notesCreator.MiddleName != " " ? " " + datauser.notesCreator.MiddleName + " " : " ") + datauser.notesCreator.LastName;
                    deligateMail.noteApprover = datauser.notesApprover.FirstName + (datauser.notesApprover.MiddleName != "" ? " " + datauser.notesApprover.MiddleName + " " : " ") + datauser.notesApprover.LastName;
                    deligateMail.NoteTitle = request.AmendmentNoteModel.NoteTitle;
                    deligateMail.noteId = _iEncryption.AesEncryptForEmail(result.NoteId);

                    #endregion

                    #region notification Save

                    NotificationModel notificationModel = new();
                    notificationModel.Message = "A new note titled " + request.AmendmentNoteModel.NoteTitle + " has been assigned to you for approval. This note was created by " + deligateMail.notecreator + ".";
                    notificationModel.NoteId = result.NoteId;
                    notificationModel.Heading = "A new note assigned";
                    notificationModel.ReceiverUserId = datauser.notesApprover.UserId;
                    notificationModel.Action = "PendingNote";
                    string notificationResponse = await _iSave.SaveNotificationData(notificationModel);

                    #endregion

                    #region if notification save success then send mail to approver

                    if (notificationResponse == "success")
                    {
                        DeligateBodySubject RespopnseBody = await _emailService.GetMailToApproverForNoteAsign(deligateMail);

                        _logger.LogwriteInfo("SMTP Configuration details fetched successfully for note approver mail send------" + Environment.NewLine + "and mail body is----- " + RespopnseBody.Body, loginUserId);
                        MailSender objMail = await _emailService.GetMailConfiguration();
                        objMail.Receiver = datauser.notesApprover.Email;
                        objMail.Subject = RespopnseBody.Subject;
                        objMail.Subject = objMail.Subject.Replace("Note_Name", request.AmendmentNoteModel.NoteTitle);
                        objMail.Body = RespopnseBody.Body;
                        objMail.CC = datauser.notesCreator.Email;
                        _logger.LogwriteInfo("Before sending the mail for note approver mail send------", loginUserId);
                        bool result1 = await _iMailService.EmailSend(objMail);

                        _logger.LogwriteInfo("Note approver mail send status--------" + result1, loginUserId);

                    }
                    #endregion

                    #endregion

                    response.Data = result;
                    response.ResponseStatus.ResponseCode = StatusCodes.Status200OK;
                    response.ResponseStatus.ResponseMessage = $"NoteId:{request.AmendmentNoteModel.NoteId} successfully republish.";
                    _logger.LogwriteInfo($"NoteId:{request.AmendmentNoteModel.NoteId} successfully republish.", loginUserId);
                }
                else
                {
                    response.Data = new AmendmentNoteModel();
                    response.ResponseStatus.ResponseCode = StatusCodes.Status304NotModified;
                    response.ResponseStatus.ResponseMessage = $"Issue in republish the NoteId:{request.AmendmentNoteModel.NoteId}.";
                    _logger.LogwriteInfo($"Issue in republish the NoteId:{request.AmendmentNoteModel.NoteId}.", loginUserId);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("exception occur during UpdateAmendmentNoteHandler execution" + Environment.NewLine + "exception is-----" + ex.StackTrace, loginUserId);

                response.Data = new AmendmentNoteModel();
                response.ResponseStatus.ResponseCode = StatusCodes.Status500InternalServerError;
                response.ResponseStatus.ResponseMessage = $"Something went wrong.";

                return response;
            }
        }

        public bool CheckFileValidation(IFormFile file)
        {
            #region File Extension Check
            if (!_checkExtension.CheckFileExtension(Path.GetExtension(file.FileName)))
            {
                return false;
            }
            #endregion

            #region MIME Type check
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(file.FileName, out string? contentType) || contentType == null)
            {
                return false;
            }
            if (!_fileValidation.CheckMimeType(contentType))
            {
                return false;
            }
            #endregion

            #region File Size Check
            long fileSizeInBytes = file.Length;
            if (fileSizeInBytes > 5242880)
            {
                return false;
            }
            #endregion

            #region Check Eicar Content
            const string eicarPattern = @"X5[0O]!P%@AP\[4\\PZX54\(P\^\)7CC\)7}\$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!\$H\+H\*";
            using (var streamReader = new StreamReader(file.OpenReadStream()))
            {
                string content = streamReader.ReadToEnd();
                if (Regex.IsMatch(content, eicarPattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(300)))
                {
                    return false;
                }

            }
            #endregion
            return true;
        }
        public async Task<bool> CheckFileExistsInDB(IFormFile file, string noteid)
        {
            FetchAttachmentByNoteIdModel InParams = new()
            {
                @NoteId = noteid,
                @FileName = file.FileName
            };
            FetchAttachmentModel Result = await _iDapperFactory.ExecuteSpDapperAsync<AttachmentModel, FetchAttachmentModel>(
                        SpName: OraStoredProcedureNames.FetchAttachmentByNoteId, Params: InParams);
            if (Result.attachmentModel.DocumentName.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
