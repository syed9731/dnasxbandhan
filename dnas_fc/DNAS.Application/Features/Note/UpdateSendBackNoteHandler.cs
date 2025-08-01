using DNAS.Application.Business.Interface;
using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Application.IRepository;
using DNAS.Application.IService;
using DNAS.Domain.DAO.DbHelperModels.Attachment;
using DNAS.Domain.DTO.Attachment;
using DNAS.Domain.DTO.Note;
using DNAS.Domain.DTO.SendBack;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.DelegateAsign;
using DNAS.Domian.DTO.MailSend;
using DNAS.Domian.DTO.Notification;
using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DNAS.Application.Features.Note
{
    public record UpdateSendBackNoteCommand(SendBackNoteDto request) : IRequest<CommonResponse<bool>>
    {
        public SendBackNoteDto SendBackNoteDto { get; set; } = request;
    }
    internal sealed class UpdateSendBackNoteHandler(IUpdate update, ICustomLogger logger, IMailService iMailService, ICheckExtension checkExtension, IHttpContextAccessor haccess, IOptions<AppConfig> appConfig, IFileValidation fileValidation, IDapperFactory iDapperFactory, IEmailService _emailService, ISave _iSave) : IRequestHandler<UpdateSendBackNoteCommand, CommonResponse<bool>>
    {
        #region properties initialization
        private readonly IUpdate _iUpdate = update;
        public readonly ICustomLogger _logger = logger;
        public readonly IMailService _iMailService = iMailService;
        private readonly ICheckExtension _checkExtension = checkExtension;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        private readonly IFileValidation _fileValidation = fileValidation;
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        #endregion

        public async Task<CommonResponse<bool>> Handle(UpdateSendBackNoteCommand request, CancellationToken cancellationToken)
        {
            CommonResponse<bool> response = new();
            try
            {
                List<AttachmentDto> newAttachmentList = new();

                #region deserialize the AttachmentListJson
                var existingAttachmentList = JsonSerializer.Deserialize<List<AttachmentDto>>(request.SendBackNoteDto.AttachmentListJson);

                if (existingAttachmentList?.Count > 0)
                {
                    newAttachmentList.AddRange(existingAttachmentList);
                }

                #endregion

                #region save the new attachment files in the folder and get the file path
                
                try
                {
                    string path = appConfig.Value.FileUploadPath;
                    //create folder if not exist
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    if (request.SendBackNoteDto.NoteModel?.AttachFiles?.Count > 0)
                    {                        
                        foreach (var file in request.SendBackNoteDto.NoteModel!.AttachFiles)
                        {
                            bool isFileValid = CheckFileValidation(file);
                            bool isFileExistsInDB = await CheckFileExistsInDB(file, request.SendBackNoteDto.NoteModel.NoteId.ToString());

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
                                AttachmentDto attachment = new();
                                attachment.NoteId = Convert.ToInt64(request.SendBackNoteDto.NoteModel.NoteId);

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
                        request.SendBackNoteDto.AttachmentList = newAttachmentList;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogwriteInfo("Upload file exception : " + ex.StackTrace, loginUserId);
                }


                #endregion

                #region update the database , prepare the response and Mail send
                ApproverDtlModel result = await _iUpdate.UpdateSendBackNote(request.SendBackNoteDto);

                if (!string.IsNullOrWhiteSpace(result.approverDtl.Email))
                {
                    response.Data = true;
                    response.ResponseStatus.ResponseCode = StatusCodes.Status200OK;
                    response.ResponseStatus.ResponseMessage = $"NoteId:{request.SendBackNoteDto.NoteModel?.NoteId} successfully republish.";
                    _logger.LogwriteInfo($"NoteId:{request.SendBackNoteDto.NoteModel?.NoteId} successfully republish.", loginUserId);

                    #region Notification Save
                    NotificationModel notificationModel = new();
                    notificationModel.Message = "This is to inform you that the note titled"+ result.noteDtl.NoteTitle + " created by "+result.creatorDtl.FirstName+", re-submitted the note after making the necessary changes.";
                    notificationModel.NoteId = result.noteDtl.NoteId;
                    notificationModel.Heading = "Note re-submitted after revision";
                    notificationModel.ReceiverUserId = result.approverDtl.UserId;
                    notificationModel.Action = "PendingNote";
                    await _iSave.SaveNotificationData(notificationModel);
                    #endregion

                    #region Send Re-Publish Note Email to the Creator and Approver

                    DeligateBodySubject RespopnseBody = await _emailService.GetMailToApproverForReSubmitNote(result);

                    _logger.LogwriteInfo("SMTP Configuration details fetched successfully for note approver mail send------" + Environment.NewLine + "and mail body is----- " + RespopnseBody.Body, loginUserId);
                    MailSender objMail = await _emailService.GetMailConfiguration();
                    objMail.Receiver = result.approverDtl.Email;
                    objMail.Subject = RespopnseBody.Subject.Replace("Note_title", result.noteDtl.NoteTitle);
                    objMail.Body = RespopnseBody.Body;
                    objMail.CC = result.creatorDtl.Email;
                    _logger.LogwriteInfo("before resubmit note mail send to current approver with creater in cc------", loginUserId);
                    bool result1 = await _iMailService.EmailSend(objMail);

                    _logger.LogwriteInfo("Note resubmit mail send status--------" + result1, loginUserId);
                    #endregion
                }
                else
                {
                    response.Data = false;
                    response.ResponseStatus.ResponseCode = StatusCodes.Status304NotModified;
                    response.ResponseStatus.ResponseMessage = $"Issue in republish the NoteId:{request.SendBackNoteDto.NoteModel?.NoteId}.";
                    _logger.LogwriteInfo($"Issue in republish the NoteId:{request.SendBackNoteDto.NoteModel?.NoteId}.", loginUserId);
                }
                #endregion

                return response;

            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("exception occur during UpdateSendBackNoteHandler execution" + Environment.NewLine + "exception is-----" + ex.StackTrace, loginUserId);

                response.Data = false;
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
