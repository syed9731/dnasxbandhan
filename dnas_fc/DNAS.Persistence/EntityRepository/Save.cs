using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Application.IRepository;
using DNAS.Domain.DAO.DbHelperModels.Attachment;
using DNAS.Domain.DTO.Approver;
using DNAS.Domain.DTO.Comment;
using DNAS.Domain.DTO.DelegateByCreator;
using DNAS.Domain.DTO.Login;
using DNAS.Domain.DTO.Note;
using DNAS.Domian.Common;
using DNAS.Domian.DAO.DbHelperModels.Approver;
using DNAS.Domian.DTO.Approver;
using DNAS.Domian.DTO.CommonResponse;
using DNAS.Domian.DTO.DelegateAsign;
using DNAS.Domian.DTO.Note;
using DNAS.Domian.DTO.Notification;
using DNAS.Domian.DTO.Template;
using DNAS.Persistence.DataAccessContents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using System.Data;
using DNAS.Domian.DAO.DbHelperModels.ApprovedNotes;
using DNAS.Domian.DTO.ApprovedNotes;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DNAS.Domain.DTO.Amendment;
using Azure.Core;
using MediatR;
using DNAS.Domain.DAO.DbHelperModels.Login;


namespace DNAS.Persistence.Repository
{
    internal class Save(
        DataContext dataContext,
        ICustomLogger logger,
        IEncryption encryption,
        IDapperFactory iDapperFactory,
        ICheckExtension checkExtension,
        IHttpContextAccessor haccess,
        IFileValidation fileValidation,
        IOptions<AppConfig> appConfig,
        IDelete iDelete) : ISave
    {
        private readonly DataContext _dbContext = dataContext;
        public readonly ICustomLogger _logger = logger;
        private readonly IEncryption _encryption = encryption;
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        private readonly ICheckExtension _checkExtension = checkExtension;
        private readonly IDelete _iDelete = iDelete;
        private readonly IFileValidation _fileValidation = fileValidation;

        private readonly string loginUserId =
            $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";

        private readonly string _pendingNoteText = "Pending";
        private readonly string _draftNoteText = "Draft";

        public async Task<CommonResponse<CommonResp>> SavedData(
            Domian.DTO.FYI.FyiModel Request)
        {
            CommonResponse<CommonResp> Response = new();
            try
            {
                DataAccessContents.Fyi fyi = new()
                {
                    NoteId = Request.NoteId,
                    WhoTagged = Request.WhoTagged,
                    TaggedTime = Request.TaggedTime,
                    ToWhome = Request.ToWhome
                };
                _logger.LogwriteInfo("Before save the data", loginUserId);
                await _dbContext.Fyis.AddAsync(fyi);
                var result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogwriteInfo("data save successfully", loginUserId);
                    Response.ResponseStatus.ResponseCode = 200;
                    Response.ResponseStatus.ResponseMessage = "Data Saved";
                }
                else
                {
                    _logger.LogwriteInfo("data not save successfully",
                        loginUserId);
                    Response.ResponseStatus.ResponseCode = 500;
                    Response.ResponseStatus.ResponseMessage = "Data Not Saved";
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during SavedDataEF-------" + e.Message +
                    Environment.NewLine + e.StackTrace,
                    loginUserId);
            }

            return Response;
        }

        public async Task<NoteModel> SaveNote(NoteModel Request)
        {
            NoteModel Response = new();
            try
            {
                DataAccessContents.Note note =
                    _dbContext.Notes.FirstOrDefault(x =>
                        x.NoteId.ToString() ==
                        _encryption.AesDecrypt(Request.NoteId)) ?? new();

                note.CategoryId = Convert.ToInt16(Request.CategoryId);
                note.UserId = Convert.ToInt32(Request.UserId);
                note.CreatorDepartment = Request.CreatorDepartment;

                if (Request.CategoryId == "1")
                {
                    note.ExpenseIncurredAtId =
                        Convert.ToInt32(Request.ExpenseIncurredAtId);
                    note.NatureOfExpensesId =
                        Convert.ToInt32(Request.NatureOfExpensesId);
                    note.CapitalExpenditure =
                        Convert.ToDecimal(Request.CapitalExpenditure);
                    note.OperationalExpenditure =
                        Convert.ToDecimal(Request.OperationalExpenditure);
                    note.TotalAmount = Convert.ToDecimal(Request.TotalAmount);
                }
                else
                {
                    note.ExpenseIncurredAtId = null;
                    note.NatureOfExpensesId = null;
                    note.CapitalExpenditure = null;
                    note.OperationalExpenditure = null;
                    note.TotalAmount = null;
                }

                note.NoteTitle = Request.NoteTitle;
                note.NoteBody = Request.NoteBody;
                note.DateOfCreation = System.DateTime.Now;
                note.NoteStatus = _pendingNoteText;
                note.IsActive = true;
                note.NoteState = Request.NoteState;
                note.DateOfCreation = System.DateTime.Now;
                if (Request.TemplateId != null && Request.TemplateId != "")
                {
                    note.TemplateId = Convert.ToUInt32(Request.TemplateId);
                }
                #region Fetch Note Version

                VersionModel versionResult = await _iDapperFactory.ExecuteSpDapperAsync<AmendVersion, VersionModel>(
                    SpName: OraStoredProcedureNames.FetchNoteVersion,
                    Params: new { NoteId = note.NoteId, IsAmend = Request.IsAmend });

                note.MajorRevision = versionResult.version.MajorRevision;
                note.MinorRevision = versionResult.version.MinorRevision == 0 ? 1 : versionResult.version.MinorRevision;
                #endregion
                int result = 0;
                if (note.NoteId == 0)
                {
                    await _dbContext.Notes.AddAsync(note);
                    result = await _dbContext.SaveChangesAsync();
                }
                else
                {
                    result = await _dbContext.SaveChangesAsync();
                }

                if (result > 0)
                {
                    //#region Note version update
                    //int updateResult = await _iDapperFactory.ExecuteSpDapperAsync(
                    //    SpName: OraStoredProcedureNames.UpdateNoteVersion,
                    //    Params: new { NoteId = note.NoteId }
                    //);

                    //if (updateResult > 0)
                    //{
                    //    _logger.LogwriteInfo("Note Version Update Successfully Done", loginUserId);
                    //}
                    //else
                    //{
                    //    _logger.LogwriteInfo("Note Version Update failed", loginUserId);
                    //}
                    //#endregion

                    #region Approver Add
                    ProcFetchApproverByNoteIdInput InParams = new()
                    {
                        @NoteId = note.NoteId.ToString(),
                        @Approval = Request.ApproverIdList,
                        @RecomendedApproval =
                            Request.RecomendedApproverIdList ?? ""
                    };

                    string[] values = Request.ApproverIdList is not null
                        ? Request.ApproverIdList.Split(',')
                        : [];
                    ApproverForDraftModel DbResult =
                        await _iDapperFactory
                            .ExecuteSpDapperAsync<ApproverForDraft,
                                ApproverForDraftModel>(
                                SpName: OraStoredProcedureNames
                                    .ProcFetchApproverByNoteId,
                                Params: InParams);
                    if (DbResult != null && values.Length ==
                        DbResult.approverForDraft.ToArray().Length)
                    {
                        _logger.LogwriteInfo("Note Save Successfully Done",
                            loginUserId);
                    }
                    else
                    {
                        _logger.LogwriteInfo("Note Approver Update failed", loginUserId);
                    }
                    #endregion

                    #region Attachment Save
                    try
                    {
                        string path = appConfig.Value.FileUploadPath;
                        //create folder if not exist
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        if (Request.AttachFiles?.Count > 0)
                        {
                            List<DataAccessContents.Attachment> Attachmentlist =
                                new();
                            foreach (var file in Request.AttachFiles)
                            {
                                bool isFileValid = CheckFileValidation(file);
                                bool isFileExistsInDB =
                                    await CheckFileExistsInDB(file,
                                        note.NoteId.ToString());

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

                                if (isFileValid && !isFileExistsInDB &&
                                    !isDuplicateFileExists)
                                {
                                    DataAccessContents.Attachment attachment =
                                        new();
                                    attachment.NoteId = note.NoteId;

                                    #region Random number generation

                                    int sixDigitNumber =
                                        RandomNumberGenerator.GetInt32(100000,
                                            1000000);

                                    #endregion

                                    string fileNameWithPath = Path.Combine(path,
                                        note.NoteId + "_Attachment_" +
                                        sixDigitNumber +
                                        Path.GetExtension(file.FileName));

                                    using (var stream =
                                           new FileStream(fileNameWithPath,
                                               FileMode.Create))
                                    {
                                        await file.CopyToAsync(stream);
                                        attachment.AttachmentPath =
                                            fileNameWithPath;
                                        attachment.DocumentName = file.FileName;
                                        _logger.LogwriteInfo(
                                            "Upload file success : " +
                                            fileNameWithPath, loginUserId);
                                        Attachmentlist.Add(attachment);
                                    }
                                }
                                else
                                {
                                    _logger.LogwriteInfo(
                                        "Because of restricted extension " +
                                        file.FileName + " can not be uploaded",
                                        loginUserId);
                                }
                            }

                            await _dbContext.Attachments.AddRangeAsync(
                                Attachmentlist);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogwriteInfo(
                            "Upload file exception : " + ex.StackTrace,
                            loginUserId);
                    }

                    await _dbContext.SaveChangesAsync();
                    #endregion

                    #region Remove Duplicate approver
                    //Check Approver Duplicate or Not
                    NoteIdForFetchApproverModel param = new()
                    { @NoteId = note.NoteId.ToString() };
                    FetchApproverModel Result =
                        await _iDapperFactory
                            .ExecuteSpDapperAsync<FetchApproverForCheckApprover,
                                FetchApproverModel>(
                                SpName: OraStoredProcedureNames
                                    .FetchApproverListByNoteId, Params: param);
                    foreach (var item in Result.fetchApproverForCheckApprover)
                    {
                        if (item.NoOfApprover > 1)
                        {
                            _logger.LogwriteInfo(
                                "Duplicate approver found against the note id-" +
                                note.NoteId.ToString(), loginUserId);
                            var inparam = new
                            {
                                @UserId = item.UserId,
                                @NoteId = item.NoteId
                            };
                            await _iDelete.DeleteDuplicateApprover(inparam);
                            _logger.LogwriteInfo(
                                "After delete Duplicate Approver method call",
                                loginUserId);
                        }
                    }
                    //End of Check Approver Duplicate or Not
                    #endregion

                    Response.NoteId = note.NoteId.ToString();
                }
                else
                {
                    _logger.LogwriteInfo("Note Not Save", loginUserId);
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during SavedNoteEF------ " + e.Message +
                    Environment.NewLine + e.StackTrace,
                    loginUserId);
            }

            return Response;
        }

        public bool CheckFileValidation(IFormFile file)
        {
            #region File Extension Check

            if (!_checkExtension.CheckFileExtension(
                    Path.GetExtension(file.FileName)))
            {
                return false;
            }

            #endregion

            #region MIME Type check

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(file.FileName,
                    out string? contentType) || contentType == null)
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

            const string eicarPattern =
                @"X5[0O]!P%@AP\[4\\PZX54\(P\^\)7CC\)7}\$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!\$H\+H\*";
            using (var streamReader = new StreamReader(file.OpenReadStream()))
            {
                string content = streamReader.ReadToEnd();
                if (Regex.IsMatch(content, eicarPattern,
                        RegexOptions.IgnoreCase,
                        TimeSpan.FromMilliseconds(300)))
                {
                    return false;
                }
            }

            #endregion

            return true;
        }

        public async Task<bool> CheckFileExistsInDB(IFormFile file,
            string noteid)
        {
            FetchAttachmentByNoteIdModel InParams = new()
            {
                @NoteId = noteid,
                @FileName = file.FileName
            };
            FetchAttachmentModel Result =
                await _iDapperFactory
                    .ExecuteSpDapperAsync<AttachmentModel,
                        FetchAttachmentModel>(
                        SpName: OraStoredProcedureNames.FetchAttachmentByNoteId,
                        Params: InParams);
            if (Result.attachmentModel.DocumentName.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<string> SaveTemplate(TemplateModel Request)
        {
            string str = "";
            try
            {
                TemplateMaster templateMaster = new();
                templateMaster.DateOfCreation = Request.DateOfCreation;
                templateMaster.UserId = Convert.ToInt32(Request.UserId);
                templateMaster.CategoryId = Convert.ToInt16(Request.CategoryId);
                templateMaster.TemplateBody = Request.TemplateBody;
                templateMaster.TemplateName = Request.TemplateName;
                templateMaster.IsActive = true;
                _logger.LogwriteInfo("before save the template", loginUserId);
                await _dbContext.TemplateMasters.AddAsync(templateMaster);
                int result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogwriteInfo(
                        "Template successfull save into database", loginUserId);
                    str = "success";
                }
                else
                {
                    _logger.LogwriteInfo("Template not save into database",
                        loginUserId);
                    str = "failed";
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during SaveTemplateEF-------" + e.Message +
                    Environment.NewLine + e.StackTrace,
                    loginUserId);
            }

            return str;
        }

        public async ValueTask<bool> SaveQueryInitiate(PendingNoteModel model)
        {
            bool str = false;
            try
            {
                NoteTracker noteTracker = new();
                noteTracker.NoteId = Convert.ToInt32(model.noteModel.NoteId);
                noteTracker.ApproverId =
                    Convert.ToInt32(model.querymodel.ApproverId);
                noteTracker.NoteStatus = "NoteQuery";
                noteTracker.CommentTime = System.DateTime.Now;
                noteTracker.Comment = model.querymodel.Comment;

                _logger.LogwriteInfo("before save the template", loginUserId);
                await _dbContext.NoteTrackers.AddAsync(noteTracker);
                int result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogwriteInfo(
                        "Note Query successfull save into database",
                        loginUserId);
                    str = true;
                }
                else
                {
                    _logger.LogwriteInfo("Note Query is not save into database",
                        loginUserId);
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during SaveQueryInitiateEF-------" +
                    e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
            }

            return str;
        }

        public async ValueTask<bool> SaveSendBack(PendingNoteModel Request)
        {
            bool str = false;
            try
            {
                NoteTracker noteTracker = new();
                noteTracker.NoteId = Convert.ToInt32(Request.noteModel.NoteId);
                noteTracker.ApproverId =
                    Convert.ToInt32(Request.querymodel.ApproverId);
                noteTracker.NoteStatus = "SendBack";
                noteTracker.CommentTime = System.DateTime.Now;
                noteTracker.Comment = Request.querymodel.Comment;
                _logger.LogwriteInfo(
                    "before save the Note tracker table for send back",
                    loginUserId);
                await _dbContext.NoteTrackers.AddAsync(noteTracker);
                int result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    DataAccessContents.Note DbResult =
                        _dbContext.Notes.FirstOrDefault(b =>
                            b.NoteId ==
                            Convert.ToInt32(Request.noteModel.NoteId)) ??
                        new();
                    DbResult.NoteId = Convert.ToInt32(Request.noteModel.NoteId);
                    DbResult.IsActive = false;
                    DbResult.NoteStatus = "SendBack";
                    _logger.LogwriteInfo(
                        "before update the Note table for send back",
                        loginUserId);
                    int result1 = await _dbContext.SaveChangesAsync();

                    if (result1 > 0)
                    {
                        _logger.LogwriteInfo(
                            "Send Back successfull save into database",
                            loginUserId);
                        str = true;
                    }
                    else
                    {
                        _logger.LogwriteInfo(
                            "Send Back is not save into note table but save into note tracker table",
                            loginUserId);
                    }
                }
                else
                {
                    _logger.LogwriteInfo(
                        "Send Back data not save in note tracker table",
                        loginUserId);
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during SaveSendBackEF-------" + e.Message +
                    Environment.NewLine + e.StackTrace,
                    loginUserId);
            }

            return str;
        }

        public async ValueTask<bool> SaveForYourInformatinData(
            PendingNoteModel model)
        {
            bool str = false;
            try
            {
                DataAccessContents.Fyi fyi = new();
                fyi.NoteId = Convert.ToInt32(model.fyiModel.NoteId);
                fyi.TaggedTime = DateTime.Now;
                fyi.WhoTagged = Convert.ToInt32(model.fyiModel.WhoTagged);
                fyi.ToWhome = Convert.ToInt32(model.fyiModel.ToWhome);
                _logger.LogwriteInfo("before save the FYI", loginUserId);
                await _dbContext.Fyis.AddAsync(fyi);
                int result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogwriteInfo("FYI successfull save into database",
                        loginUserId);
                    str = true;
                }
                else
                {
                    _logger.LogwriteInfo("FYI is not save into database",
                        loginUserId);
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during SaveForYourInformatinData-------" +
                    e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
            }

            return str;
        }

        public async ValueTask<string> SaveDelegateAndUpdateApprover(
            DelegateAsignModel model)
        {
            string str = "";
            try
            {
                AsignedDelegate asignedDelegate = new AsignedDelegate();
                asignedDelegate.DeligatedUserId =
                    Convert.ToInt32(model.delegateAsign.DeligatedUserId);
                asignedDelegate.AssignTime = DateTime.Now;
                asignedDelegate.ApproverId =
                    Convert.ToInt32(model.delegateAsign.ApproverID);
                _logger.LogwriteInfo("before save the Delegate asign",
                    loginUserId);
                await _dbContext.AsignedDelegates.AddAsync(asignedDelegate);
                int result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogwriteInfo(
                        "Previous user save into delegate table", loginUserId);
                    int upresult = 0;
                    var result1 = _dbContext.Approvers.FirstOrDefault(b =>
                        b.ApproverId ==
                        Convert.ToInt64(model.delegateApprover.ApproverId));
                    if (result1 != null)
                    {
                        result1.NoteId =
                            Convert.ToInt64(model.delegateApprover.NoteId);
                        result1.UserId =
                            Convert.ToInt32(model.delegateApprover.UserId);
                        result1.ApproverId =
                            Convert.ToInt64(model.delegateApprover.ApproverId);
                        upresult = await _dbContext.SaveChangesAsync();
                        if (upresult > 0)
                        {
                            _logger.LogwriteInfo(
                                "New user update in approver table",
                                loginUserId);
                            str = "success";
                        }
                        else
                        {
                            _logger.LogwriteInfo(
                                "New user not update in approver table",
                                loginUserId);
                            str = "Failed";
                        }
                    }
                    else
                    {
                        str = "Failed";
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during SaveDelegateAndUpdateApprover-------" +
                    e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
                str = "Failed";
            }

            return str;
        }

        public async ValueTask<string> SaveNotificationData(
            NotificationModel model)
        {
            string str = "";
            try
            {
                Notification notification = new();
                notification.NotificationTime = DateTime.Now;
                notification.NoteId = Convert.ToInt64(model.NoteId);
                notification.IsRead = false;
                notification.Heading = model.Heading;
                notification.Message = model.Message;
                notification.ReceiverUserId =
                    Convert.ToInt32(model.ReceiverUserId);

                _logger.LogwriteInfo("before save the Notification",
                    loginUserId);
                await _dbContext.Notifications.AddAsync(notification);
                int result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogwriteInfo(
                        "Notification successfull save into database",
                        loginUserId);
                    str = "success";
                }
                else
                {
                    _logger.LogwriteInfo(
                        "Notification is not save into database", loginUserId);
                    str = "failed";
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during SaveNotificationData-------" +
                    e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
                str = "failed";
            }

            return str;
        }

        public async ValueTask<bool> SaveQueryReply(WithdrawNoteModel model)
        {
            bool str = false;
            try
            {
                NoteTracker note = new();
                note.NoteId = Convert.ToInt64(model.noteModel.NoteId);
                note.ApproverId = Convert.ToInt32(model.querymodel.ApproverId);
                note.NoteStatus = "QueryReply";
                note.Comment = model.querymodel.Comment;
                note.CommentTime = DateTime.Now;

                _logger.LogwriteInfo("before save the NoteTracker",
                    loginUserId);
                await _dbContext.NoteTrackers.AddAsync(note);
                int result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogwriteInfo(
                        "Notification successfull save into database",
                        loginUserId);
                    str = true;
                }
                else
                {
                    _logger.LogwriteInfo(
                        "Notification is not save into database", loginUserId);
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during SaveQueryReply-------" + e.Message +
                    Environment.NewLine + e.StackTrace,
                    loginUserId);
            }

            return str;
        }

        public async ValueTask<NoteModel> InsertNoteTitleData(NoteModel Request)
        {
            NoteModel Response = new();
            try
            {
                DataAccessContents.Note note = new()
                {
                    NoteTitle = Request.NoteTitle,
                    NoteState = _draftNoteText,
                    NoteStatus = _pendingNoteText,
                    DateOfCreation = DateTime.Now,
                    UserId = Convert.ToInt32(Request.UserId)
                };
                note.IsActive = true;
                await _dbContext.Notes.AddAsync(note);
                var result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogwriteInfo("Note title save Successfully Done",
                        loginUserId);
                    Response.NoteId = note.NoteId.ToString();
                }
                else
                {
                    _logger.LogwriteInfo(
                        "Note title Not Save Successfully Done", loginUserId);
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during insert note title------ " +
                    e.Message + Environment.NewLine + e.StackTrace,
                    loginUserId);
            }

            return Response;
        }

        public async ValueTask<NoteModel> InsertNotecategoryData(
            NoteModel Request)
        {
            NoteModel Response = new();
            try
            {
                DataAccessContents.Note note = new()
                {
                    CategoryId = Convert.ToInt16(Request.CategoryId),
                    NoteState = _draftNoteText,
                    NoteStatus = _pendingNoteText,
                    UserId = Convert.ToInt32(Request.UserId)
                };
                note.IsActive = true;
                await _dbContext.Notes.AddAsync(note);
                var result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogwriteInfo("Note category save Successfully Done",
                        loginUserId);
                    Response.NoteId = note.NoteId.ToString();
                }
                else
                {
                    _logger.LogwriteInfo(
                        "Note category Not Save Successfully Done",
                        loginUserId);
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during insert note category------ " +
                    e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
            }

            return Response;
        }

        public async ValueTask<NoteModel> InsertExpenseIncuredAtData(
            NoteModel Request)
        {
            NoteModel Response = new();
            try
            {
                DataAccessContents.Note note = new()
                {
                    ExpenseIncurredAtId =
                        Convert.ToInt32(Request.ExpenseIncurredAtId),
                    NoteState = _draftNoteText,
                    NoteStatus = _pendingNoteText,
                    UserId = Convert.ToInt32(Request.UserId)
                };
                note.IsActive = true;
                await _dbContext.Notes.AddAsync(note);
                var result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogwriteInfo(
                        "Note expense incurred at save Successfully Done",
                        loginUserId);
                    Response.NoteId = note.NoteId.ToString();
                }
                else
                {
                    _logger.LogwriteInfo(
                        "Note expense incurred at Not Save Successfully Done",
                        loginUserId);
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during insert expense incured at------ " +
                    e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
            }

            return Response;
        }

        public async ValueTask<NoteModel> InsertNatureOfExpData(
            NoteModel Request)
        {
            NoteModel Response = new();
            try
            {
                DataAccessContents.Note note = new()
                {
                    NatureOfExpensesId =
                        Convert.ToInt32(Request.NatureOfExpensesId),
                    NoteState = _draftNoteText,
                    NoteStatus = _pendingNoteText,
                    UserId = Convert.ToInt32(Request.UserId)
                };
                note.IsActive = true;
                await _dbContext.Notes.AddAsync(note);
                var result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogwriteInfo(
                        "Note nature of expense save Successfully Done",
                        loginUserId);
                    Response.NoteId = note.NoteId.ToString();
                }
                else
                {
                    _logger.LogwriteInfo(
                        "Note nature of expense Not Save Successfully Done",
                        loginUserId);
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during insert nature of expense------ " +
                    e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
            }

            return Response;
        }

        public async ValueTask<NoteModel> InsertNoteOpexData(NoteModel Request)
        {
            NoteModel Response = new();
            try
            {
                DataAccessContents.Note note = new()
                {
                    OperationalExpenditure =
                        Convert.ToDecimal(Request.OperationalExpenditure),
                    NoteState = _draftNoteText,
                    NoteStatus = _pendingNoteText,
                    UserId = Convert.ToInt32(Request.UserId)
                };
                note.IsActive = true;
                await _dbContext.Notes.AddAsync(note);
                var result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogwriteInfo(
                        "Note Operational expenditure save Successfully Done",
                        loginUserId);
                    Response.NoteId = note.NoteId.ToString();
                }
                else
                {
                    _logger.LogwriteInfo(
                        "Note operational expenditure Not Save Successfully Done",
                        loginUserId);
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during insert note operational expenses------ " +
                    e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
            }

            return Response;
        }

        public async ValueTask<NoteModel> InsertNoteCapexData(NoteModel Request)
        {
            NoteModel Response = new();
            try
            {
                DataAccessContents.Note note = new()
                {
                    CapitalExpenditure =
                        Convert.ToDecimal(Request.CapitalExpenditure),
                    NoteState = _draftNoteText,
                    NoteStatus = _pendingNoteText,
                    UserId = Convert.ToInt32(Request.UserId)
                };
                note.IsActive = true;
                await _dbContext.Notes.AddAsync(note);
                var result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogwriteInfo(
                        "Note capital expenditure save Successfully Done",
                        loginUserId);
                    Response.NoteId = note.NoteId.ToString();
                }
                else
                {
                    _logger.LogwriteInfo(
                        "Note capital expenditure Not Save Successfully Done",
                        loginUserId);
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during insert note capital expenditure------ " +
                    e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
            }

            return Response;
        }

        public async ValueTask<NoteModel> InsertNoteBodyData(NoteModel Request)
        {
            NoteModel Response = new();
            try
            {
                DataAccessContents.Note note = new()
                {
                    NoteBody = Request.NoteBody,
                    NoteState = _draftNoteText,
                    NoteStatus = _pendingNoteText,
                    UserId = Convert.ToInt32(Request.UserId)
                };
                note.IsActive = true;
                await _dbContext.Notes.AddAsync(note);
                var result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogwriteInfo("Note body save Successfully Done",
                        loginUserId);
                    Response.NoteId = note.NoteId.ToString();
                }
                else
                {
                    _logger.LogwriteInfo("Note body Not Save Successfully Done",
                        loginUserId);
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during insert note body ------ " +
                    e.Message + Environment.NewLine + e.StackTrace,
                    loginUserId);
            }

            return Response;
        }

        public async ValueTask<NoteModel> InsertApproverByNoteIdData(
            NoteModel Request)
        {
            NoteModel Response = new();
            string[] values = Request.ApproverIdList is not null
                ? Request.ApproverIdList.Split(',')
                : [];
            try
            {
                if (values.Length > 0)
                {
                    List<Approver> _approverlist = new List<Approver>();

                    for (int i = 0; i < values.Length; i++)
                    {
                        Approver approver = new Approver();
                        approver.NoteId = Convert.ToInt64(Request.NoteId);
                        approver.IsApproved = false;
                        approver.UserId = Convert.ToInt32(values[i]);
                        if (i == 0)
                        {
                            approver.IsCurrentApprover = true;
                        }
                        else
                        {
                            approver.IsCurrentApprover = false;
                        }

                        _approverlist.Add(approver);
                    }

                    await _dbContext.Approvers.AddRangeAsync(_approverlist);
                    var result = await _dbContext.SaveChangesAsync();
                    if (result > 0)
                    {
                        _logger.LogwriteInfo(
                            "Note approver by note id save Successfully Done",
                            loginUserId);
                    }
                    else
                    {
                        _logger.LogwriteInfo(
                            "Note approver by note id Not Save Successfully Done",
                            loginUserId);
                    }
                }
                else
                {
                    _logger.LogwriteInfo("Approver not selected", loginUserId);
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during insert approver by note id------ " +
                    e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
            }

            return Response;
        }

        public async ValueTask<NoteModel>
            InsertSelectedApproverWthoutNoteIdData(NoteModel Request)
        {
            NoteModel Response = new();
            string[] values = Request.ApproverIdList.Split(',');
            try
            {
                if (Request.ApproverIdList != null)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = values[i].Trim();
                    }

                    Request.CurrentApproverId = values[0];
                }

                List<Approver> _approverlist = new List<Approver>();

                for (int i = 0; i < values.Length; i++)
                {
                    Approver approver = new Approver();
                    approver.NoteId = Convert.ToInt64(Request.NoteId);
                    approver.IsApproved = false;
                    approver.UserId = Convert.ToInt32(values[i]);
                    if (i == 0)
                    {
                        approver.IsCurrentApprover = true;
                    }
                    else
                    {
                        approver.IsCurrentApprover = false;
                    }

                    _approverlist.Add(approver);
                }

                await _dbContext.Approvers.AddRangeAsync(_approverlist);
                var result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogwriteInfo(
                        "Note approver without note id save Successfully Done",
                        loginUserId);
                }
                else
                {
                    _logger.LogwriteInfo(
                        "Note approver without note id Not Save Successfully Done",
                        loginUserId);
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during approver without note id------ " +
                    e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
            }

            return Response;
        }

        public async ValueTask<bool> SaveUserTracking(UserTrackingModel model)
        {
            bool str = false;
            try
            {
                //UserTracking userTracking = new();
                //userTracking.UserId = model.UserId;
                //userTracking.IsActive = model.IsActive;
                //userTracking.LastLoginTime = DateTime.Now;
                //userTracking.SessionId = model.SessionId;

                _logger.LogwriteInfo("before save the UserTracking Table for user-" +model.UserId,"User_" + model.UserId);
                //await _dbContext.UserTrackings.AddAsync(userTracking);
                //int result = await _dbContext.SaveChangesAsync();

                ProcSaveUserTrackingInput InParams = new()
                {
                    @UserId= model.UserId, 
                    @IsActive= model.IsActive, 
                    @LastLoginTime= DateTime.Now, 
                    @SessionId = model.SessionId
                };                
                int result =await _iDapperFactory.ExecuteSpDapperAsync(SpName: OraStoredProcedureNames.Proc_SaveUserTracking, Params: InParams);
                if (result > 0)
                {
                    _logger.LogwriteInfo("Data save into UserTracking Table for user-" +model.UserId,"User_" + model.UserId);
                    str = true;
                }
                else
                {
                    _logger.LogwriteInfo(
                        "Data not save into UserTracking Table", loginUserId);
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during SaveUserTracking-------" +
                    e.Message + Environment.NewLine + e.StackTrace,
                    loginUserId);
            }

            return str;
        }

        public async ValueTask<bool> SaveApproveComment(CommentReqModel model)
        {
            bool str = false;
            try
            {
                NoteTracker note = new();
                note.NoteId = Convert.ToInt64(model.NoteId);
                note.ApproverId = Convert.ToInt32(model.ApproverId);
                note.NoteStatus = "ApproveComment";
                note.Comment = model.Comment;
                note.CommentTime = DateTime.Now;

                _logger.LogwriteInfo(
                    "Save Approve Comment before save the NoteTracker",
                    loginUserId);
                await _dbContext.NoteTrackers.AddAsync(note);
                int result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogwriteInfo(
                        "Save Approve Comment successfull save into database",
                        loginUserId);
                    str = true;
                }
                else
                {
                    _logger.LogwriteInfo(
                        "Save Approve Comment is not save into database",
                        loginUserId);
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during SaveApproveComment-------" +
                    e.Message + Environment.NewLine + e.StackTrace,
                    loginUserId);
            }

            return str;
        }

        public async ValueTask<bool> SaveAttachment(WithNotesModel Request)
        {
            bool result = false;
            int saveresult = 0;
            //try
            //{
            //    string path = @"wwwroot\UploadedFiles";
            //    //create folder if not exist
            //    if (!Directory.Exists(path))
            //        Directory.CreateDirectory(path);
            //    if (Request.AttachFiles?.Count > 0)
            //    {
            //        List<DataAccessContents.Attachment> Attachmentlist = new();
            //        foreach (var file in Request.AttachFiles)
            //        {
            //            if (_checkExtension.CheckFileExtension(Path.GetExtension(file.FileName)))
            //            {
            //                DataAccessContents.Attachment attachment = new();
            //                attachment.NoteId =Convert.ToInt64(Request.NoteId);

            //                string fileNameWithPath = Path.Combine(path, Request.NoteId + "_Attachment_" + Request.AttachFiles.IndexOf(file) + Path.GetExtension(file.FileName));

            //                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            //                {
            //                    await file.CopyToAsync(stream);
            //                    attachment.AttachmentPath = fileNameWithPath;
            //                    attachment.DocumentName = file.FileName;
            //                    _logger.LogwriteInfo("Upload file success : " + fileNameWithPath, loginUserId);
            //                    Attachmentlist.Add(attachment);
            //                }
            //            }
            //            else
            //            {
            //                _logger.LogwriteInfo("Because of restricted extension " + file.FileName + " can not be uploaded", loginUserId);
            //            }
            //        }
            //        await _dbContext.Attachments.AddRangeAsync(Attachmentlist);                    
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogwriteInfo("Upload file exception : " + ex.StackTrace, loginUserId);
            //}

            try
            {
                string path = appConfig.Value.FileUploadPath;
                //create folder if not exist
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (Request.AttachFiles?.Count > 0)
                {
                    List<DataAccessContents.Attachment> Attachmentlist = new();
                    foreach (var file in Request.AttachFiles)
                    {
                        bool isFileValid = CheckFileValidation(file);
                        bool isFileExistsInDB =
                            await CheckFileExistsInDB(file,
                                Request.NoteId.ToString());

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

                        if (isFileValid && !isFileExistsInDB &&
                            !isDuplicateFileExists)
                        {
                            DataAccessContents.Attachment attachment = new();
                            attachment.NoteId = Convert.ToInt64(Request.NoteId);

                            #region Random number generation

                            int sixDigitNumber =
                                RandomNumberGenerator.GetInt32(100000, 1000000);

                            #endregion

                            string fileNameWithPath = Path.Combine(path,
                                Request.NoteId + "_Attachment_" +
                                sixDigitNumber +
                                Path.GetExtension(file.FileName));

                            using (var stream = new FileStream(fileNameWithPath,
                                       FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                                attachment.AttachmentPath = fileNameWithPath;
                                attachment.DocumentName = file.FileName;
                                _logger.LogwriteInfo(
                                    "Upload file success : " + fileNameWithPath,
                                    loginUserId);
                                Attachmentlist.Add(attachment);
                            }
                        }
                        else
                        {
                            _logger.LogwriteInfo(
                                "Because of restricted extension " +
                                file.FileName + " can not be uploaded",
                                loginUserId);
                        }
                    }

                    await _dbContext.Attachments.AddRangeAsync(Attachmentlist);
                }
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("Upload file exception : " + ex.StackTrace,
                    loginUserId);
            }

            saveresult = await _dbContext.SaveChangesAsync();
            if (saveresult > 0)
            {
                result = true;
            }
            return result;
        }

        public async ValueTask<bool> SaveAsignDelegateComment(
            AsignDelegateNoteInputModel Request)
        {
            bool str = false;
            try
            {
                NoteTracker note = new();
                note.NoteId = Convert.ToInt64(Request.NoteId);
                note.ApproverId = Convert.ToInt32(Request.ApproverId);
                note.NoteStatus = Request.NoteStatus;
                note.Comment = Request.Comment;
                note.CommentTime = DateTime.Now;

                _logger.LogwriteInfo("before save the NoteTracker",
                    loginUserId);
                await _dbContext.NoteTrackers.AddAsync(note);
                int result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogwriteInfo(
                        "Notification successfull save into database",
                        loginUserId);
                    str = true;
                }
                else
                {
                    _logger.LogwriteInfo(
                        "Notification is not save into database", loginUserId);
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during SaveQueryReply-------" + e.Message +
                    Environment.NewLine + e.StackTrace,
                    loginUserId);
            }

            return str;
        }

        public async ValueTask<bool> SaveDelegateByCreator(
            DelegateByCreatorModel model)
        {
            try
            {
                AsignedDelegate asignedDelegate = new AsignedDelegate();
                asignedDelegate.DeligatedUserId =
                    Convert.ToInt32(model.oldApprover.UserId);
                asignedDelegate.AssignTime = DateTime.Now;
                asignedDelegate.ApproverId = Convert.ToInt64(model.oldApprover.ApproverId);
                asignedDelegate.DelegateBy = Convert.ToInt64(model.creatorDetails.UserId);
                _logger.LogwriteInfo("before save the Delegate asign by creator", loginUserId);
                await _dbContext.AsignedDelegates.AddAsync(asignedDelegate);
                int result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogwriteInfo(
                        "Previous user save into delegate table for delegate by creator",
                        loginUserId);
                    int upresult = 0;
                    var result1 = _dbContext.Approvers.FirstOrDefault(b =>
                        b.ApproverId ==
                        Convert.ToInt64(model.oldApprover.ApproverId));
                    if (result1 != null)
                    {
                        result1.NoteId =
                            Convert.ToInt64(model.noteDetails.NoteId);
                        result1.UserId =
                            Convert.ToInt32(model.newApprover.UserId);
                        result1.ApproverId =
                            Convert.ToInt64(model.oldApprover.ApproverId);
                        upresult = await _dbContext.SaveChangesAsync();
                        if (upresult > 0)
                        {
                            _logger.LogwriteInfo(
                                "New user update in approver table for delegate by creator",
                                loginUserId);
                            return true;
                        }
                        else
                        {
                            _logger.LogwriteInfo(
                                "New user not update in approver table delegate by creator",
                                loginUserId);
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during SaveDelegateByCreator-------" +
                    e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
                return false;
            }
        }

        public async ValueTask<bool> TransferToNoteApproved(string noteid)
        {
            bool str = false;
            try
            {
                int result = await _iDapperFactory.ExecuteSpDapperAsync(SpName: OraStoredProcedureNames.InsertNoteApprovedAfterNoteApproved, Params: new { NoteId = noteid });
                if (result > 0)
                {
                    _logger.LogwriteInfo("Data save into TransferToNoteApproved Table for user-" + loginUserId, loginUserId);
                    str = true;
                }
                else
                {
                    _logger.LogwriteInfo("Data not save into TransferToNoteApproved Table", loginUserId);
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during TransferToNoteApproved-------" + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
            }
            return str;
        }

        public async ValueTask<ApproverReviewerResponseModelDto> SaveReviewerApprover(
        AppproverReviewerRequestModelDto request)
        {
            ApproverReviewerResponseModelDto response = new();
            try
            {
                //get the data against the current approver
                var existingData = await _dbContext.Approvers
                    .Where(m => m.UserId == request.AddedBy && m.NoteId == request.NoteId)
                    .FirstOrDefaultAsync();

                if (existingData is not null)
                {
                    //insert the new data
                    Approver approver = new()
                    {
                        NoteId = request.NoteId,
                        UserId = request.ApproverReviewerId,
                        IsCurrentApprover = (request.PrefixSuffixValue == 1), //if prefix then set IsCurrentApprover= true
                        SuffixPrefix = request.PrefixSuffixValue,
                        AddedBy = request.AddedBy,
                        VisibilityMode = request.VisibilityMode,
                        AssignTime = (request.PrefixSuffixValue == 1) ? DateTime.Now : null  //if approver then set AssignTime= current time
                    };
                    await _dbContext.Approvers.AddAsync(approver);

                    //check the request data is Reviewer or Approver
                    if (request.PrefixSuffixValue == 1)
                    {
                        existingData.IsCurrentApprover = false;
                        existingData.MyAssignTime = existingData.AssignTime;
                        existingData.AssignTime = null;
                        existingData.ChildAssignTime = DateTime.Now;
                        _dbContext.Approvers.Update(existingData);
                    }


                    //get the number of rows affected in database
                    int rowAffected = await _dbContext.SaveChangesAsync();

                    var suffixPrefixValue = await _dbContext.Approvers.AsNoTracking().Where(m => m.NoteId == request.NoteId && m.AddedBy == request.AddedBy).Select(m => m.SuffixPrefix).ToListAsync();

                    //prepare the response
                    response.IsSuccess = true;
                    response.IsCurrentApprover = existingData.IsCurrentApprover.GetValueOrDefault();
                    response.SuffixPrefixList = suffixPrefixValue;

                }
                else
                {
                    _logger.LogwriteInfo(
                        $"No records found during SaveReviewerApprover-------{Environment.NewLine}",
                        loginUserId);

                    //prepare the response
                    response.IsSuccess = false;
                    response.IsCurrentApprover = false;


                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    $"exception occur during SaveReviewerApprover-------{e.Message}{Environment.NewLine}{e.StackTrace}",
                    loginUserId);

                //prepare the response
                response.IsSuccess = false;
                response.IsCurrentApprover = false;
            }
            return response;
        }

        public async ValueTask<AmendmentNoteModel> SaveAmendNote(AmendmentNoteModel request)
        {
            AmendmentNoteModel Response = new();

            try
            {
                long noteId = Convert.ToInt64(request.NoteId);

                //fetch existing note data
                var note = await _dbContext.Notes.FirstOrDefaultAsync(x =>
                        x.NoteId.ToString() ==
                        request.NoteId) ?? new();


                if (note is not null)
                {
                    note.CategoryId = Convert.ToInt16(request.CategoryId);
                    note.UserId = Convert.ToInt32(request.UserId);
                    note.CreatorDepartment = request.CreatorDepartment;

                    #region assign data based on category functional/non-functional
                    if (request.CategoryId == "1")
                    {
                        note.ExpenseIncurredAtId =
                            Convert.ToInt32(request.ExpenseIncurredAtId);
                        note.NatureOfExpensesId =
                            Convert.ToInt32(request.NatureOfExpensesId);
                        note.CapitalExpenditure =
                            Convert.ToDecimal(request.CapitalExpenditure);
                        note.OperationalExpenditure =
                            Convert.ToDecimal(request.OperationalExpenditure);
                        note.TotalAmount = Convert.ToDecimal(request.TotalAmount);
                    }
                    else
                    {
                        note.ExpenseIncurredAtId = null;
                        note.NatureOfExpensesId = null;
                        note.CapitalExpenditure = null;
                        note.OperationalExpenditure = null;
                        note.TotalAmount = null;
                    }
                    #endregion

                    //assign data
                    note.NoteTitle = request.NoteTitle;
                    note.NoteBody = request.NoteBody;
                    note.DateOfCreation = System.DateTime.Now;
                    note.NoteStatus = _pendingNoteText;
                    note.IsActive = true;
                    note.NoteState = request.NoteState;
                    note.DateOfCreation = System.DateTime.Now;

                    #region assign TemplateId if it is not null
                    if (request.TemplateId != null && request.TemplateId != "")
                    {
                        note.TemplateId = Convert.ToUInt32(request.TemplateId);
                    }
                    #endregion

                    #region Fetch Note Version

                    VersionModel versionResult = await _iDapperFactory.ExecuteSpDapperAsync<AmendVersion, VersionModel>(
                        SpName: OraStoredProcedureNames.FetchNoteVersion,
                        Params: new { NoteId = note.NoteId, IsAmend = request.IsAmend });

                    note.MajorRevision = versionResult.version.MajorRevision;
                    note.MinorRevision = versionResult.version.MinorRevision == 0 ? 1 : versionResult.version.MinorRevision;
                    #endregion

                    #region Save Note Data
                    int result = 0;
                    if (note.NoteId == 0)
                    {
                        await _dbContext.Notes.AddAsync(note);
                        result = await _dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        result = await _dbContext.SaveChangesAsync();
                    }
                    #endregion

                    if (result > 0)
                    {

                        #region Approver Add
                        ProcFetchApproverByNoteIdInput InParams = new()
                        {
                            @NoteId = note.NoteId.ToString(),
                            @Approval = request.ApproverIdList,
                            @RecomendedApproval =
                                request.RecomendedApproverIdList ?? ""
                        };

                        string[] values = request.ApproverIdList is not null
                            ? request.ApproverIdList.Split(',')
                            : [];
                        ApproverForDraftModel DbResult =
                            await _iDapperFactory
                                .ExecuteSpDapperAsync<ApproverForDraft,
                                    ApproverForDraftModel>(
                                    SpName: OraStoredProcedureNames
                                        .ProcFetchApproverByNoteId,
                                    Params: InParams);
                        if (DbResult != null && values.Length ==
                            DbResult.approverForDraft.ToArray().Length)
                        {
                            _logger.LogwriteInfo("Note Save Successfully Done",
                                loginUserId);
                        }
                        else
                        {
                            _logger.LogwriteInfo("Note Approver Update failed", loginUserId);
                        }
                        #endregion

                        #region Attachment Save

                        #region get only the new attachments from this existing list
                        //var newAttachmentList = request.AttachmentList?.Count > 0 ? request.AttachmentList?.Where(m => m.AttachmentId == string.Empty).Select(m => new DNAS.Persistence.DataAccessContents.Attachment
                        var newAttachmentList = request.AttachmentList?.Count > 0 ? request.AttachmentList.Select(m => new DNAS.Persistence.DataAccessContents.Attachment
                        {
                            //AttachmentId = Convert.ToInt64(m.AttachmentId),
                            NoteId = noteId,
                            AttachmentPath = m.AttachmentPath,
                            DocumentName = m.DocumentName

                        }).ToList() : new List<DNAS.Persistence.DataAccessContents.Attachment>();
                        #endregion

                        #region add datalist to the attachment table

                        if (newAttachmentList!.Any())
                        {
                            await _dbContext.Attachments.AddRangeAsync(newAttachmentList!);
                        }

                        #endregion

                        await _dbContext.SaveChangesAsync();

                        #endregion

                        #region Remove Duplicate approver
                        //Check Approver Duplicate or Not
                        NoteIdForFetchApproverModel param = new()
                        { @NoteId = note.NoteId.ToString() };
                        FetchApproverModel Result =
                            await _iDapperFactory
                                .ExecuteSpDapperAsync<FetchApproverForCheckApprover,
                                    FetchApproverModel>(
                                    SpName: OraStoredProcedureNames
                                        .FetchApproverListByNoteId, Params: param);
                        foreach (var item in Result.fetchApproverForCheckApprover)
                        {
                            if (item.NoOfApprover > 1)
                            {
                                _logger.LogwriteInfo(
                                    "Duplicate approver found against the note id-" +
                                    note.NoteId.ToString(), loginUserId);
                                var inparam = new
                                {
                                    @UserId = item.UserId,
                                    @NoteId = item.NoteId
                                };
                                await _iDelete.DeleteDuplicateApprover(inparam);
                                _logger.LogwriteInfo(
                                    "After delete Duplicate Approver method call",
                                    loginUserId);
                            }
                        }
                        //End of Check Approver Duplicate or Not
                        #endregion

                        Response.NoteId = note.NoteId.ToString();
                    }
                    else
                    {
                        _logger.LogwriteInfo("Note Not Save", loginUserId);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during SavedAmendNoteEF------ " + e.Message +
                    Environment.NewLine + e.StackTrace,
                    loginUserId);
            }
            return Response;
        }
    }
}