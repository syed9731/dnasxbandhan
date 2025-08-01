using Azure.Core;
using ClosedXML.Excel;

using DNAS.Application.Common.Filter;
using DNAS.Application.Common.Interface;
using DNAS.Application.Features.Attachment;
using DNAS.Application.Features.DashBoard;
using DNAS.Application.Features.Login;
using DNAS.Application.Features.Note;
using DNAS.Application.Features.Note.Amendment;
using DNAS.Application.Features.Note.Approved;
using DNAS.Application.Features.Template;
using DNAS.Application.Features.Validation;
using DNAS.Domain.DTO.Amendment;
using DNAS.Domain.DTO.Approver;
using DNAS.Domain.DTO.CommonModel;
using DNAS.Domain.DTO.DashBoard;
using DNAS.Domain.DTO.Note;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.ApprovedNotes;
using DNAS.Domian.DTO.Category;
using DNAS.Domian.DTO.Draft;
using DNAS.Domian.DTO.FYI;
using DNAS.Domian.DTO.Login;
using DNAS.Domian.DTO.Note;
using DNAS.Domian.DTO.SearchNotes;
using DNAS.Domian.DTO.Template;
using DNAS.Domian.DTO.WithdrawList;
using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

using static DNAS.Domian.DTO.Note.NoteasDraft;

namespace DNAS.WEB.Controllers
{
    [Authorize]
    [TypeFilter(typeof(UserCurrentAuth))]
    public class NoteController(
        ISender iSender,
        ICustomLogger iCustomLogger,
        IEncryption iEncryption,
        IHttpContextAccessor haccess,
        IEncryption encryption) : Controller
    {
        private readonly ISender _iSender = iSender;
        private readonly ICustomLogger _iCustomLogger = iCustomLogger;
        private readonly IEncryption _iEncryption = iEncryption;
        private readonly IEncryption _encryption = encryption;

        private readonly string loginUserId =
            $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";

        private readonly string _createNotePathUrl = "/note/create";
        private readonly string _commonlogpath = "Login";
        private readonly string _dashboardUrl = "/Dashboard";
        private readonly string _approvalRequestUrl = "/Note/ApprovalRequest";
        private readonly string _withdrawUrl = "/Note/Withdraw";

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        #region Create Note

        [HttpGet]
        public async ValueTask<IActionResult> Create()
        {
            NoteModel notempdel = new NoteModel();
            try
            {
                TempData["encdata"] = DateTime.Now.ToString("yyyyMMddHHmmssff");
                TempData.Keep("encdata");
                NoteModel Request = new();
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId == "")
                {
                    _iCustomLogger.LogwriteInfo("Session timeout", _commonlogpath);
                    return Redirect("/");
                }

                _iCustomLogger.LogwriteInfo("user exists for user id------ " + userId, loginUserId);
                if (HttpContext.Request.Query["s"].ToString() != "")
                {
                    Request.NoteId =
                        _iEncryption.AesDecrypt(HttpContext.Request.Query["s"].ToString());
                    DraftNoteModel Response1 =
                        await _iSender.Send(new FetchSaveNoteDataCommand(Request));
                    _iCustomLogger.LogwriteInfo(
                        "FetchSaveNoteDataCommand return value------ note title-- " +
                        Response1.noteModel.NoteTitle, loginUserId);
                    notempdel.NoteStatus = Response1.noteModel.NoteStatus;
                    notempdel.ApproverIdList = "";
                    notempdel.NoteId = _iEncryption.AesEncrypt(Response1.noteModel.NoteId);
                    notempdel.NoteTitle = Response1.noteModel.NoteTitle;
                    notempdel.CategoryId = Response1.noteModel.CategoryId;
                    notempdel.NoteBody = Response1.noteModel.NoteBody;
                    notempdel.NoteState = Response1.noteModel.NoteState.ToString();
                    notempdel.NatureOfExpensesId = Response1.noteModel.NatureOfExpensesId;
                    notempdel.ExpenseIncurredAtId = Response1.noteModel.ExpenseIncurredAtId;
                    notempdel.CapitalExpenditure = Response1.noteModel.CapitalExpenditure ?? "";
                    notempdel.OperationalExpenditure =
                        Response1.noteModel.OperationalExpenditure ?? "";
                    notempdel.TotalAmount = Response1.noteModel.TotalAmount;

                    if (Response1.userMaster != null)
                    {
                        int i = 1;
                        IEnumerable<UserMasterModel> users = Response1.userMaster;
                        StringBuilder sb = new();
                        foreach (var dr in users.Select(dr => dr.UserId))
                        {
                            if (i == 1)
                            {
                                notempdel.ApproverIdList = dr.ToString();
                            }
                            else
                            {
                                sb.Append("," + dr);
                            }

                            i++;
                        }

                        notempdel.ApproverIdList = sb.ToString();
                    }

                    ViewBag.ApproverList = Response1.userMaster;
                }
                else if (HttpContext.Request.Query["t"].ToString() != "")
                {
                    TemplateModel Request1 = new();
                    Request1.TemplateId =
                        _encryption.AesDecrypt(HttpContext.Request.Query["t"].ToString());
                    Request1.UserId = userId;
                    _iCustomLogger.LogwriteInfo(
                        "template fetch for the user------ " + userId + "and template id is---" +
                        Request1.TemplateId, loginUserId);
                    TemplateModel Response1 =
                        await _iSender.Send(new FetchTemplateForViewCommand(Request1));

                    _iCustomLogger.LogwriteInfo(
                        "FetchTemplateForViewCommand return value" +
                        JsonSerializer.Serialize(Response1), loginUserId);
                    notempdel.TemplateId = Response1.TemplateId;
                    notempdel.NoteBody = Response1.TemplateBody;
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("No template selected for the user------ " + userId,
                        loginUserId);
                    notempdel.NoteStatus = "";
                    notempdel.ApproverIdList = "";
                    notempdel.NoteId = "";
                    notempdel.TotalAmount = "0";
                }

                _iCustomLogger.LogwriteInfo("before calling CreateNoteCommand ----- ", loginUserId);
                IEnumerable<CategoryRespModel> Response =
                    await _iSender.Send(new CreateNoteCommand(Request));
                _iCustomLogger.LogwriteInfo(
                    "CreateNoteCommand return value - " + JsonSerializer.Serialize(Response),
                    loginUserId);
                List<SelectListItem> li = [];
                foreach (var item in Response)
                {
                    li.Add(new SelectListItem
                    {
                        Text = item.CategoryName.ToString(),
                        Value = item.CategoryId.ToString()
                    });
                }

                ViewBag.CategoryList = new SelectList(li, "Value", "Text");
                notempdel.UserId = userId;
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during new create page load------ " + e.Message +
                    Environment.NewLine + e.StackTrace, loginUserId);
            }

            _iCustomLogger.LogwriteInfo(
                "Before sending the data to create note view. The data --- " +
                JsonSerializer.Serialize(notempdel), loginUserId);
            return View(notempdel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async ValueTask<IActionResult> Create(NoteModel model)
        {
            NoteModel Response = new();
            try
            {
                
                if (!ModelState.IsValid)
                {
                    var validationContext = new ValidationContext(model)
                    {
                        MemberName = nameof(model.NoteBody)
                    };

                    var validationResults = new List<ValidationResult>();
                    if (!Validator.TryValidateProperty(model.NoteBody, validationContext,
                            validationResults))
                    {
                        _iCustomLogger.LogwriteInfo("form post is not valid for invalid note body",
                            loginUserId);
                        TempData["errormsg"] = "Please fill the proper detail in note body";
                    }
                    else if (Validator.TryValidateProperty(model.NoteTitle, validationContext, validationResults))
                    {
                        _iCustomLogger.LogwriteInfo("Invalid Character in Note Title",
                            loginUserId);
                        TempData["errormsg"] = "Special characters are not allowed.";
                    }
                    else
                    {
                        _iCustomLogger.LogwriteInfo("form post is not valid for create note page",
                            loginUserId);
                        TempData["errormsg"] = "Please fill the required details";
                    }

                    return Redirect(_createNotePathUrl);
                }

                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    if (!await _iSender.Send(new FileExtensionValidationCommand(model)))
                    {
                        _iCustomLogger.LogwriteInfo("file type not supported", loginUserId);
                        TempData["errormsg"] = "Opps! File type not supported by this system.";
                        return Redirect(_createNotePathUrl);
                    }

                    model.UserId = userId;
                    if (TempData["encdata"] == null)
                    {
                        return Redirect("/");
                    }

                    model.ApproverIdList = _encryption.DecryptStringAES(model.ApproverIdList,
                        TempData["encdata"]?.ToString() ?? "");
                    if (model.RecomendedApproverIdList != null)
                    {
                        model.RecomendedApproverIdList = _encryption.DecryptStringAES(
                            model.RecomendedApproverIdList, TempData["encdata"]?.ToString() ?? "");
                    }

                    if (model.ApproverIdList == "keyError")
                    {
                        _iCustomLogger.LogwriteInfo("Exception On Approver Descryption:",
                            loginUserId);
                        return Redirect(_createNotePathUrl);
                    }

                    if (model.RecomendedApproverIdList != "" &&
                        model.RecomendedApproverIdList == "keyError")
                    {
                        _iCustomLogger.LogwriteInfo("Exception On Recomended Approver Descryption:",
                            loginUserId);
                        return Redirect(_createNotePathUrl);
                    }

                    if (model.CategoryId == "1")
                    {
                        if (model.CapitalExpenditure != "" && model.OperationalExpenditure != "" &&
                            model.ExpenseIncurredAtId != "" && model.NatureOfExpensesId != "")
                        {
                            _iCustomLogger.LogwriteInfo(
                                "before SaveNoteCommand execution for financial-------",
                                loginUserId);
                            Response = await _iSender.Send(new SaveNoteCommand(model));
                            _iCustomLogger.LogwriteInfo(
                                "after SaveNoteCommand execute. and the result for financial-" +
                                JsonSerializer.Serialize(Response), loginUserId);
                        }
                        else
                        {
                            TempData["errormsg"] = "Fields are mandetory";
                            return Redirect(_createNotePathUrl);
                        }
                    }
                    else if (model.CategoryId == "2")
                    {
                        _iCustomLogger.LogwriteInfo(
                            "before SaveNoteCommand execution for non financial-------",
                            loginUserId);
                        Response = await _iSender.Send(new SaveNoteCommand(model));
                        _iCustomLogger.LogwriteInfo(
                            "after SaveNoteCommand execute. and the result for non financial-" +
                            JsonSerializer.Serialize(Response), loginUserId);
                    }
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Session timeout", _commonlogpath);
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during Create Note page ------ " + e.Message +
                    Environment.NewLine + e.StackTrace, loginUserId);
                return Redirect(_createNotePathUrl);
            }

            if (model.NoteState == "Draft" && Response.NoteId != "")
            {
                _iCustomLogger.LogwriteInfo(
                    "Note Save as draft from create note page and NoteState=Draft -------",
                    loginUserId);
                TempData["msg"] = "Note Successfully Save as Draft";
                return Redirect("/note/create?s=" + _iEncryption.AesEncrypt(Response.NoteId));
            }
            else if (model.NoteStatus == "Publish" && Response.NoteId != "")
            {
                _iCustomLogger.LogwriteInfo(
                    "Note Save as Publish from create note page and NoteStatus=Publish -------",
                    loginUserId);
                TempData["msg"] = "Note Successfully Created and Published";
                return Redirect("_createNotePathUrl");
            }
            else if (model.NoteState == "Publish" && Response.NoteId != "")
            {
                _iCustomLogger.LogwriteInfo("Note created successdully -------", loginUserId);
                TempData["msg"] = "Note Successfully Created";
                return Redirect("/Note/CreatorDashboard");
            }
            else
            {
                _iCustomLogger.LogwriteInfo("Note not note created and NoteStatus= '' -------",
                    loginUserId);
                TempData["msg"] = "Something went wrong. Please try after some time";
                return Redirect(_createNotePathUrl);
            }
        }

        #endregion

        #region SendBack Note Edit and Re-publish

        //[HttpGet("SendBackNote")]
        public async ValueTask<IActionResult> EditSendBackNote()
        {
            try
            {
                CommonResponse<SendBackNoteDto> response = new();
                //get the UserId
                string userId = User.FindFirstValue("UserId") ?? "";

                #region if userId null/empty => redirect to login page

                if (string.IsNullOrWhiteSpace(userId))
                {
                    _iCustomLogger.LogwriteInfo("Session timeout", _commonlogpath);
                    return Redirect("/");
                }

                #endregion

                //log the UserId
                _iCustomLogger.LogwriteInfo($"user exists for user id------ userId", loginUserId);

                #region if userId not empty, but the noteId is empty => redirect to SendBack page

                if (string.IsNullOrWhiteSpace(HttpContext.Request?.Query["NoteId"].ToString()))
                {
                    _iCustomLogger.LogwriteInfo(
                        "NoteId value is empty" +
                        JsonSerializer.Serialize(HttpContext.Request?.Query["NoteId"].ToString()),
                        loginUserId);
                    return RedirectToAction("SendBack", new SendBackData());
                }

                #endregion

                #region if userId not empty and noteId is not empty, then redirect to Edit Note page

                string noteId = string.Empty;
                if (!string.IsNullOrWhiteSpace(HttpContext.Request?.Query["NoteId"].ToString()))
                {
                    noteId = _iEncryption.AesDecrypt(HttpContext.Request.Query["NoteId"]
                        .ToString());
                    response = await _iSender.Send(new FetchSendBackNoteCommand(noteId));

                    _iCustomLogger.LogwriteInfo(
                        "FetchSendBackNoteCommand return value" +
                        JsonSerializer.Serialize(response), loginUserId);
                }

                if (response.ResponseStatus.ResponseCode == StatusCodes.Status200OK)
                {
                    return View(response.Data);
                }
                else if (response.ResponseStatus.ResponseCode == StatusCodes.Status404NotFound)
                {
                    TempData["errormsg"] =
                        $"Something went wrong when fetching the data against NoteId:{noteId}";
                    return View(response.Data);
                }
                else
                {
                    return Redirect("/");
                }

                #endregion
            }
            catch (Exception ex)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during SendBack Note page ------ " + ex.Message +
                    Environment.NewLine + ex.StackTrace, loginUserId);
                return View(SendBackNoteDto.Empty);
            }
        }

        [HttpPost]
        public async ValueTask<IActionResult> EditSendBackNote(SendBackNoteDto model)
        {
            model.NoteModel!.NoteId = _iEncryption.AesDecrypt(model.NoteModel.NoteId);
            var response = await _iSender.Send(new UpdateSendBackNoteCommand(model));

            if (response.ResponseStatus.ResponseCode == StatusCodes.Status200OK)
            {
                TempData["msg"] = "Note Successfully Edited";
                return Redirect("/Note/CreatorDashboard");
            }
            else
            {
                TempData["errormsg"] = "Edit Note Failed";
            }

            return View(model);
        }

        #endregion


        [HttpGet]
        public async ValueTask<IActionResult> Draft(DraftNoteData Request)
        {
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    _iCustomLogger.LogwriteInfo(
                        "Before executing DraftNoteCommand in draft note get method function----- and request is---" +
                        JsonSerializer.Serialize(Request), loginUserId);
                    DraftNoteCommand Command = new()
                    {
                        FilterDraftNotes = new FilterDraftNote()
                        {
                            UserId = Convert.ToInt32(User.FindFirstValue("UserId")),
                            StartDate = Request.FilterDrafts.StartDate,
                            EndDate = Request.FilterDrafts.EndDate,
                            Category = Request.FilterDrafts.Category
                        },
                    };
                    CommonResponse<DraftNoteData> Response = await _iSender.Send(Command);
                    Response.Data.DraftNotes = Response.Data.DraftNotes.Select(item =>
                    {
                        item.NoteId = _iEncryption.AesEncrypt(item.NoteId.ToString());
                        return item;
                    }).ToList(); //useless
                    return View(Response.Data);
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Session timeout", _commonlogpath);
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during Draft Note page ------ " + e.Message +
                    Environment.NewLine + e.StackTrace, loginUserId);
                return View(new DraftNoteData());
            }
        }

        [HttpPost]
        public async ValueTask<IActionResult> DownloadDraft([FromBody] DraftNoteData Request)
        {
            _iCustomLogger.LogwriteInfo(
                "Download draft method call start for excel download ---" +
                JsonSerializer.Serialize(Request), loginUserId);
            byte[] content = [];
            try
            {
                await Task.Run(() =>
                {
                    using XLWorkbook workbook = new();
                    IXLWorksheet worksheet = workbook.Worksheets.Add("Report");

                    int ColumnIndex = 0;
                    PropertyInfo[] properties = typeof(DraftNote).GetProperties();
                    string[] RequiredColumns = ["NoteTitle", "CategoryName", "DateOfCreation"];
                    for (int i = 0; i < properties.Length; i++)
                    {
                        if (RequiredColumns.Contains(properties[i].Name))
                        {
                            if (properties[i].Name == "NoteTitle")
                            {
                                worksheet.Cell(1, ColumnIndex + 1).Value = "Note Title";
                            }

                            if (properties[i].Name == "CategoryName")
                            {
                                worksheet.Cell(1, ColumnIndex + 1).Value = "Category";
                            }

                            if (properties[i].Name == "DateOfCreation")
                            {
                                worksheet.Cell(1, ColumnIndex + 1).Value = "Create Date";
                            }

                            IXLCell cell = worksheet.Cell(1, ColumnIndex + 1);
                            cell.Style.Font.Bold = true;
                            worksheet.Cell(1, ColumnIndex + 1).Style.Alignment
                                .SetHorizontal(XLAlignmentHorizontalValues.Center);
                            ColumnIndex++;
                        }
                    }

                    int row = 2;
                    foreach (DraftNote item in Request.DraftNotes)
                    {
                        ColumnIndex = 0;
                        for (int i = 0; i < properties.Length; i++)
                        {
                            if (RequiredColumns.Contains(properties[i].Name))
                            {
                                item.GetType().GetProperty(properties[i].Name)?.GetValue(item);
                                worksheet.Cell(row, ColumnIndex + 1).Value = item.GetType()
                                    .GetProperty(properties[i].Name)?.GetValue(item)?.ToString();
                                ColumnIndex++;
                            }
                        }

                        row++;
                    }

                    using MemoryStream stream = new();
                    workbook.SaveAs(stream);
                    content = stream.ToArray();
                });
            }
            catch (Exception ex)
            {
                _iCustomLogger.LogwriteInfo("exception occur during DownloadDraft excel execution" +
                                            Environment.NewLine + "exception message-" +
                                            ex.Message + Environment.NewLine + ex.StackTrace,
                    loginUserId);
            }

            return File(content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "dataReport.xlsx");
        }

        [HttpPost]
        public async ValueTask<IActionResult> DeleteDraft(string noteid)
        {
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    _iCustomLogger.LogwriteInfo(
                        "Delete draft method initiated------ for noteid-" + noteid, loginUserId);
                    bool Data = await _iSender.Send(
                        new DraftNoteDeleteCommand(
                            Convert.ToInt32(_iEncryption.AesDecrypt(noteid))));
                    if (Data)
                    {
                        _iCustomLogger.LogwriteInfo(
                            "delete draft successfully------ for noteid-" + noteid, loginUserId);
                        TempData["msg"] = "Draft Note delete sucessfully";
                    }
                    else
                    {
                        _iCustomLogger.LogwriteInfo("draft not deleted ------ for noteid-" + noteid,
                            loginUserId);
                        TempData["errormsg"] = "Draft Note not deleted";
                    }

                    return RedirectToAction("Draft");
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Session timeout", _commonlogpath);
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during Delete Draft ------ " + e.Message +
                    Environment.NewLine + e.StackTrace, loginUserId);
                return RedirectToAction("Draft");
            }
        }

        [HttpGet]
        public async ValueTask<IActionResult> Search(SearchNoteData Request)
        {
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    _iCustomLogger.LogwriteInfo(
                        "Search page get method initiated------ and search request---" +
                        JsonSerializer.Serialize(Request), loginUserId);
                    SearchNoteCommand Command = new()
                    {
                        InputModel = new FilterSearchNote()
                        {
                            UserId = Convert.ToInt32(User.FindFirstValue("UserId") ?? string.Empty),
                            StartDate = Request.FilterSearchNotes.StartDate,
                            EndDate = Request.FilterSearchNotes.EndDate,
                            Category = Request.FilterSearchNotes.Category,
                            Title = Request.FilterSearchNotes.Title
                        }
                    };
                    CommonResponse<SearchNoteData> Response = await _iSender.Send(Command);
                    _iCustomLogger.LogwriteInfo("data load for search field and search result",
                        loginUserId);
                    return View(Response.Data);
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Session timeout", _commonlogpath);
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur in search --" + e.Message + Environment.NewLine + e.StackTrace,
                    loginUserId);
                return View(new PendingData());
            }
        }

        [HttpGet]
        public async ValueTask<IActionResult> Approved(ApprovedNoteData Request)
        {
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    _iCustomLogger.LogwriteInfo(
                        "Approved page get method initiated------ and request---" +
                        JsonSerializer.Serialize(Request), loginUserId);

                    ApprovedNoteCommand Command = new()
                    {
                        InputModel = new FilterApprovedNote()
                        {
                            UserId = Convert.ToInt32(User.FindFirstValue("UserId") ?? string.Empty),
                            StartDate = Request.FilterApprovedNotes.StartDate,
                            EndDate = Request.FilterApprovedNotes.EndDate,
                            Category = Request.FilterApprovedNotes.Category
                        },
                    };
                    CommonResponse<ApprovedNoteData> Response = await _iSender.Send(Command);
                    _iCustomLogger.LogwriteInfo(
                        "approved note data called and search result -" +
                        JsonSerializer.Serialize(Response), loginUserId);
                    return View(Response.Data);
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Session timeout", _commonlogpath);
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur in approved page --" + e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
                return View(new PendingData());
            }
        }

        [HttpPost]
        public async ValueTask<IActionResult> DownloadApproved([FromBody] ApprovedNoteData Request)
        {
            _iCustomLogger.LogwriteInfo(
                "Download Approved note method call start for excel download ---" +
                JsonSerializer.Serialize(Request), loginUserId);
            byte[] content = [];
            try
            {
                await Task.Run(() =>
                {
                    using XLWorkbook workbook = new();
                    IXLWorksheet worksheet = workbook.Worksheets.Add("Report");

                    int ColumnIndex = 0;
                    PropertyInfo[] properties = typeof(ApprovedNote).GetProperties();
                    string[] RequiredColumns = ["NoteTitle", "CategoryName", "DateOfCreation"];
                    for (int i = 0; i < properties.Length; i++)
                    {
                        if (RequiredColumns.Contains(properties[i].Name))
                        {
                            if (properties[i].Name == "NoteTitle")
                            {
                                worksheet.Cell(1, ColumnIndex + 1).Value = "Note Title";
                            }

                            if (properties[i].Name == "CategoryName")
                            {
                                worksheet.Cell(1, ColumnIndex + 1).Value = "Category";
                            }

                            if (properties[i].Name == "DateOfCreation")
                            {
                                worksheet.Cell(1, ColumnIndex + 1).Value = "Create Date";
                            }

                            IXLCell cell = worksheet.Cell(1, ColumnIndex + 1);
                            cell.Style.Font.Bold = true;
                            worksheet.Cell(1, ColumnIndex + 1).Style.Alignment
                                .SetHorizontal(XLAlignmentHorizontalValues.Center);
                            ColumnIndex++;
                        }
                    }

                    int row = 2;
                    foreach (ApprovedNote item in Request.Table)
                    {
                        ColumnIndex = 0;
                        for (int i = 0; i < properties.Length; i++)
                        {
                            if (RequiredColumns.Contains(properties[i].Name))
                            {
                                item.GetType().GetProperty(properties[i].Name)?.GetValue(item);
                                worksheet.Cell(row, ColumnIndex + 1).Value = item.GetType()
                                    .GetProperty(properties[i].Name)?.GetValue(item)?.ToString();
                                ColumnIndex++;
                            }
                        }

                        row++;
                    }

                    using MemoryStream stream = new();
                    workbook.SaveAs(stream);
                    content = stream.ToArray();
                });
            }
            catch (Exception ex)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during DownloadApproved excel execution" +
                    Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine +
                    ex.StackTrace, loginUserId);
            }

            return File(content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "dataReport.xlsx");
        }

        [HttpGet]
        public async ValueTask<IActionResult> Pending(PendingData Request)
        {
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    _iCustomLogger.LogwriteInfo(
                        "Pending page get method initiated------ and request---" +
                        JsonSerializer.Serialize(Request), loginUserId);

                    PendingCommand Command = new()
                    {
                        InputModel = new FilterPendingNote()
                        {
                            UserId = Convert.ToInt32(userId),
                            StartDate = Request.FilterPendings.StartDate,
                            EndDate = Request.FilterPendings.EndDate,
                            Category = Request.FilterPendings.Category
                        },
                    };
                    CommonResponse<PendingData> Response = await _iSender.Send(Command);
                    return View(Response.Data);
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Session timeout", _commonlogpath);
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur in pending page --" + e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
                return View(new PendingData());
            }
        }

        [HttpPost]
        public async ValueTask<IActionResult> DownloadPending([FromBody] PendingData Request)
        {
            _iCustomLogger.LogwriteInfo(
                "Download Pending note method call start for excel download ---" +
                JsonSerializer.Serialize(Request), loginUserId);
            byte[] content = [];
            try
            {
                await Task.Run(() =>
                {
                    using XLWorkbook workbook = new();
                    IXLWorksheet worksheet = workbook.Worksheets.Add("Report");

                    int ColumnIndex = 0;
                    PropertyInfo[] properties = typeof(PendingTable).GetProperties();
                    string[] RequiredColumns =
                        ["NoteTitle", "CategoryName", "Aging", "DateOfCreation"];
                    for (int i = 0; i < properties.Length; i++)
                    {
                        if (RequiredColumns.Contains(properties[i].Name))
                        {
                            if (properties[i].Name == "NoteTitle")
                            {
                                worksheet.Cell(1, ColumnIndex + 1).Value = "Note Title";
                            }

                            if (properties[i].Name == "CategoryName")
                            {
                                worksheet.Cell(1, ColumnIndex + 1).Value = "Category";
                            }

                            if (properties[i].Name == "DateOfCreation")
                            {
                                worksheet.Cell(1, ColumnIndex + 1).Value = "Create Date";
                            }

                            if (properties[i].Name == "Aging")
                            {
                                worksheet.Cell(1, ColumnIndex + 1).Value = "Aging";
                            }

                            IXLCell cell = worksheet.Cell(1, ColumnIndex + 1);
                            cell.Style.Font.Bold = true;
                            worksheet.Cell(1, ColumnIndex + 1).Style.Alignment
                                .SetHorizontal(XLAlignmentHorizontalValues.Center);
                            ColumnIndex++;
                        }
                    }


                    int row = 2;
                    foreach (PendingTable item in Request.Table)
                    {
                        ColumnIndex = 0;
                        for (int i = 0; i < properties.Length; i++)
                        {
                            if (RequiredColumns.Contains(properties[i].Name))
                            {
                                item.GetType().GetProperty(properties[i].Name)?.GetValue(item);
                                worksheet.Cell(row, ColumnIndex + 1).Value = item.GetType()
                                    .GetProperty(properties[i].Name)?.GetValue(item)?.ToString();
                                ColumnIndex++;
                            }
                        }

                        row++;
                    }

                    using MemoryStream stream = new();
                    workbook.SaveAs(stream);
                    content = stream.ToArray();
                });
            }
            catch (Exception ex)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during DownloadPending excel execution" +
                    Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine +
                    ex.StackTrace, loginUserId);
            }

            return File(content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "dataReport.xlsx");
        }

        [HttpGet]
        public async ValueTask<IActionResult> FyiList(FyiData Request)
        {
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    _iCustomLogger.LogwriteInfo(
                        "FYI List page get method initiated------ and request---" +
                        JsonSerializer.Serialize(Request), loginUserId);

                    FyiCommand Command = new()
                    {
                        InputModel = new FilterFyi()
                        {
                            UserId = Convert.ToInt32(userId),
                            StartDate = Request.FilterFYIs.StartDate,
                            EndDate = Request.FilterFYIs.EndDate,
                            Category = Request.FilterFYIs.Category
                        }
                    };
                    CommonResponse<FyiData> Response = await _iSender.Send(Command);
                    _iCustomLogger.LogwriteInfo(
                        "Fyilist note data called and search result -" +
                        JsonSerializer.Serialize(Response), loginUserId);
                    return View(Response.Data);
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Session timeout", _commonlogpath);
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur in fyilist page --" + e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
                return View(new FyiData());
            }
        }

        [HttpPost]
        public async ValueTask<IActionResult> DownloadFyiList([FromBody] FyiData Request)
        {
            _iCustomLogger.LogwriteInfo(
                "Download FYI List note method call start for excel download ---" +
                JsonSerializer.Serialize(Request), loginUserId);
            byte[] content = [];
            try
            {
                await Task.Run(() =>
                {
                    using XLWorkbook workbook = new();
                    IXLWorksheet worksheet = workbook.Worksheets.Add("Report");

                    int ColumnIndex = 0;
                    PropertyInfo[] properties = typeof(FyiTable).GetProperties();

                    string[] RequiredColumns =
                        ["NoteTitle", "CategoryName", "DateOfCreation", "NoteStatus"];
                    for (int i = 0; i < properties.Length; i++)
                    {
                        if (RequiredColumns.Contains(properties[i].Name))
                        {
                            if (properties[i].Name == "NoteTitle")
                            {
                                worksheet.Cell(1, ColumnIndex + 1).Value = "Note Title";
                            }

                            if (properties[i].Name == "CategoryName")
                            {
                                worksheet.Cell(1, ColumnIndex + 1).Value = "Category";
                            }

                            if (properties[i].Name == "DateOfCreation")
                            {
                                worksheet.Cell(1, ColumnIndex + 1).Value = "Create Date";
                            }

                            if (properties[i].Name == "NoteStatus")
                            {
                                worksheet.Cell(1, ColumnIndex + 1).Value = "Status";
                            }

                            IXLCell cell = worksheet.Cell(1, ColumnIndex + 1);
                            cell.Style.Font.Bold = true;
                            worksheet.Cell(1, ColumnIndex + 1).Style.Alignment
                                .SetHorizontal(XLAlignmentHorizontalValues.Center);
                            ColumnIndex++;
                        }
                    }

                    int row = 2;
                    foreach (FyiTable item in Request.Table)
                    {
                        ColumnIndex = 0;
                        for (int i = 0; i < properties.Length; i++)
                        {
                            if (RequiredColumns.Contains(properties[i].Name))
                            {
                                item.GetType().GetProperty(properties[i].Name)?.GetValue(item);
                                string CellValue =
                                    item.GetType().GetProperty(properties[i].Name)?.GetValue(item)
                                        ?.ToString() ?? string.Empty;
                                if (CellValue == "Approved")
                                {
                                    CellValue = "Completed";
                                }

                                if (CellValue == "Pending")
                                {
                                    CellValue = "In-Progress";
                                }

                                if (CellValue == "SendBack")
                                {
                                    CellValue = "Send Back";
                                }

                                worksheet.Cell(row, ColumnIndex + 1).Value = CellValue;
                                ColumnIndex++;
                            }
                        }

                        row++;
                    }

                    using MemoryStream stream = new();
                    workbook.SaveAs(stream);
                    content = stream.ToArray();
                });
            }
            catch (Exception ex)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during DownloadPending excel execution" +
                    Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine +
                    ex.StackTrace, loginUserId);
            }

            return File(content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "dataReport.xlsx");
        }

        [HttpGet]
        public async ValueTask<IActionResult> SendBack(SendBackData Request)
        {
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId == "")
                {
                    _iCustomLogger.LogwriteInfo("Session timeout", _commonlogpath);
                    return Redirect("/");
                }

                _iCustomLogger.LogwriteInfo(
                    "Send Back List page get method initiated------ and request---" +
                    JsonSerializer.Serialize(Request), loginUserId);

                SendBackCommand Command = new()
                {
                    InputModel = new FilterSendBack()
                    {
                        UserId = Convert.ToInt32(userId),
                        StartDate = Request.FilterSendBacks.StartDate,
                        EndDate = Request.FilterSendBacks.EndDate,
                        Category = Request.FilterSendBacks.Category
                    }
                };
                var response = await _iSender.Send(Command);
                _iCustomLogger.LogwriteInfo(
                    "send back note data called and search result -" +
                    JsonSerializer.Serialize(response), loginUserId);
                return View(response.Data);
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur in SendBack page --" + e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
                return View(new SendBackData());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async ValueTask<IActionResult> SendBack(RequestApproverNoteModel note)
        {
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    note.querymodel.ApproverId = userId;
                }
                else
                {
                    return Redirect("/");
                }

                if (string.IsNullOrEmpty(note.querymodel.Comment))
                {
                    TempData["commentmsg"] = "Comment field can not blank";
                    return Redirect(_approvalRequestUrl + "?p=" + note.noteModel.NoteId);
                }

                _iCustomLogger.LogwriteInfo(
                    "Note SendBack method initiated --- and request-" +
                    JsonSerializer.Serialize(note), loginUserId);
                bool str = await _iSender.Send(new SaveSendBackCommand(note));
                _iCustomLogger.LogwriteInfo("Note SendBack response ----" + str, loginUserId);
                if (str)
                {
                    TempData["msg"] = "Successfully sent back the note.";
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during Send back ------ " + e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
                return Redirect("/");
            }

            return Redirect(_dashboardUrl);
        }

        [HttpPost]
        public async ValueTask<IActionResult> DownloadSendBack([FromBody] SendBackData Request)
        {
            _iCustomLogger.LogwriteInfo(
                "Download Send back List note method call start for excel download ---" +
                JsonSerializer.Serialize(Request), loginUserId);
            byte[] content = [];
            try
            {
                await Task.Run(() =>
                {
                    using XLWorkbook workbook = new();
                    IXLWorksheet worksheet = workbook.Worksheets.Add("Report");

                    int ColumnIndex = 0;
                    PropertyInfo[] properties = typeof(SendBackTable).GetProperties();
                    string[] RequiredColumns =
                        ["NoteTitle", "CategoryName", "DateOfCreation", "CommentTime"];
                    for (int i = 0; i < properties.Length; i++)
                    {
                        if (RequiredColumns.Contains(properties[i].Name))
                        {
                            if (properties[i].Name == "NoteTitle")
                            {
                                worksheet.Cell(1, ColumnIndex + 1).Value = "Note Title";
                            }

                            if (properties[i].Name == "CategoryName")
                            {
                                worksheet.Cell(1, ColumnIndex + 1).Value = "Category";
                            }

                            if (properties[i].Name == "DateOfCreation")
                            {
                                worksheet.Cell(1, ColumnIndex + 1).Value = "Create Date";
                            }

                            if (properties[i].Name == "CommentTime")
                            {
                                worksheet.Cell(1, ColumnIndex + 1).Value = "Send Back Date";
                            }

                            IXLCell cell = worksheet.Cell(1, ColumnIndex + 1);
                            cell.Style.Font.Bold = true;
                            worksheet.Cell(1, ColumnIndex + 1).Style.Alignment
                                .SetHorizontal(XLAlignmentHorizontalValues.Center);
                            ColumnIndex++;
                        }
                    }

                    int row = 2;
                    foreach (SendBackTable item in Request.Table)
                    {
                        ColumnIndex = 0;
                        for (int i = 0; i < properties.Length; i++)
                        {
                            if (RequiredColumns.Contains(properties[i].Name))
                            {
                                worksheet.Cell(row, ColumnIndex + 1).Value =
                                    item.GetType().GetProperty(properties[i].Name)?.GetValue(item)
                                        ?.ToString() ?? string.Empty;
                                ColumnIndex++;
                            }
                        }

                        row++;
                    }

                    using MemoryStream stream = new();
                    workbook.SaveAs(stream);
                    content = stream.ToArray();
                });
            }
            catch (Exception ex)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during DownloadSendBack excel execution" +
                    Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine +
                    ex.StackTrace, loginUserId);
            }

            return File(content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "dataReport.xlsx");
        }

        [HttpGet]
        public async ValueTask<IActionResult> Withdraw()
        {
            try
            {
                string noteid = string.Empty;
                if (HttpContext.Request.Query["p"].ToString() != "")
                {
                    noteid = _iEncryption.AesDecrypt(HttpContext.Request.Query["p"].ToString());
                }
                else if (HttpContext.Request.Query["m"].ToString() != "")
                {
                    noteid = _iEncryption.AesDecryptForEmail(HttpContext.Request.Query["m"].ToString());
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("note id not found in Pending note page ------ ",
                        loginUserId);
                    return Redirect(_dashboardUrl);
                }

                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    _iCustomLogger.LogwriteInfo("Withdraw page get method initiated------",
                        loginUserId);
                    NoteModel Request = new() { NoteId = noteid, UserId = userId };
                    WithdrawNoteModel Response = await _iSender.Send(new FetchWithdrawNoteCommand(Request));
                    Response.approverModel = Response.approverModel.Select(item =>
                    {
                        item.UserId = _iEncryption.AesEncrypt(item.UserId.ToString());
                        return item;
                    }).ToList();
                    Response.recomendedapproverModel = Response.recomendedapproverModel.Select(
                        item =>
                        {
                            item.UserId = _iEncryption.AesEncrypt(item.UserId.ToString());
                            return item;
                        }).ToList();
                    return View(Response);
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Session timeout", _commonlogpath);
                    HttpContext.Session.SetString("RedirectFromWithdraw", "/Withdraw?p=" + noteid);
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during withdraw get method ------ " + e.Message +
                    Environment.NewLine + e.StackTrace, loginUserId);
                return Redirect("/");
            }
        }

        [HttpPost]
        public async ValueTask<IActionResult> Widthdraw(PendingNoteModel Request)
        {
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    _iCustomLogger.LogwriteInfo(
                        "Widthdraw post method initiated------ and request---" +
                        JsonSerializer.Serialize(Request), loginUserId);
                    Request.noteModel.UserId = userId;
                    bool str = await _iSender.Send(new WithdrawNoteCommand(Request));
                    if (str)
                    {
                        TempData["WithdrawMsgSuccess"] = "Withdraw note successfully done.";
                    }
                    else
                    {
                        TempData["WithdrawMsgFail"] = "Failed! Note not withdraw.";
                    }
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Session timeout", _commonlogpath);
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during withdraw post method ------ " + e.Message +
                    Environment.NewLine + e.StackTrace, loginUserId);
            }

            return Redirect(_dashboardUrl);
        }

        [HttpGet]
        public async ValueTask<IActionResult> MyApprovedNote()
        {
            try
            {
                string noteid = string.Empty;
                if (HttpContext.Request.Query["p"].ToString() != "")
                {
                    noteid = _iEncryption.AesDecrypt(HttpContext.Request.Query["p"].ToString());
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("note id not found in MyApprovedNote note page ------ ", loginUserId);
                    return Redirect(_dashboardUrl);
                }
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    _iCustomLogger.LogwriteInfo("MyApprovedNote page get method initiated------", loginUserId);
                    NoteModel Request = new() { NoteId = noteid, UserId = userId };
                    MyApprovedNoteModel Response = await _iSender.Send(new FetchMyApprovedNoteCommand(Request));
                    Response.approverModel = Response.approverModel.Select(item => { item.UserId = _iEncryption.AesEncrypt(item.UserId.ToString()); return item; }).ToList();
                    Response.recomendedapproverModel = Response.recomendedapproverModel.Select(item => { item.UserId = _iEncryption.AesEncrypt(item.UserId.ToString()); return item; }).ToList();
                    return View(Response);
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Session timeout in MyApprovedNote", _commonlogpath);
                    HttpContext.Session.SetString("RedirectFromWithdraw", "/Withdraw?p=" + noteid);
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo("exception occur during withdraw get method ------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                return Redirect("/");
            }
        }
        public async ValueTask<JsonResult> FetchExpensesIncurredAt(string idd)
        {
            var list = new List<KeyValuePair<string, int>>();
            try
            {
                ExpenseIncurredAtModel Request = new() { CategoryId = idd };
                IEnumerable<ExpenseIncurredAtModel> Response =
                    await _iSender.Send(new ExpenseIncurredAtFetchCommand(Request));
                foreach (var item in Response)
                {
                    list.Add(new KeyValuePair<string, int>(item.ExpenseIncurredAtName.ToString(),
                        Convert.ToInt32(item.ExpenseIncurredAtId)));
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during fetchExpensesIncurredAt------ " + e.Message +
                    Environment.NewLine + e.StackTrace, loginUserId);
            }

            return Json(list);
        }

        public async ValueTask<JsonResult> FetchNatureOfExpense(string idd)
        {
            var list = new List<KeyValuePair<string, int>>();
            try
            {
                NatureOfExpensesModel Request = new()
                {
                    ExpensesIncurredAtId = idd
                };
                IEnumerable<NatureOfExpensesModel> Response =
                    await _iSender.Send(new NatureOfExpenseFetchCommand(Request));
                foreach (var item in Response)
                {
                    list.Add(new KeyValuePair<string, int>(item.NatureOfExpensesName.ToString(),
                        Convert.ToInt32(item.NatureOfExpensesId)));
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during fetchNatureOfExpense------ " + e.Message +
                    Environment.NewLine + e.StackTrace, loginUserId);
            }

            return Json(list);
        }

        public async ValueTask<JsonResult> FetchApproverList(string idd, string catname,
            string netofexp, string totAmt)
        {
            var list = new List<KeyValuePair<string, int>>();
            IEnumerable<UserMasterModel> Response = [];
            try
            {
                UserMasterModel Request = new();
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    Request.UserId = Convert.ToInt32(userId);
                }

                _iCustomLogger.LogwriteInfo(
                    "FetchApproverList method initiated------ and expense incurred id-" + idd +
                    " ----category-" + catname + " ---nature of exp-" + netofexp +
                    " ---total amount-" + totAmt, loginUserId);
                if (catname != null && catname == "Financial")
                {
                    if (netofexp != null && totAmt != null && idd != null)
                    {
                        Request.ExpenseIncurredAtId = idd;
                        Request.NetureOfExpenseId = netofexp;
                        Request.TotalAmount = Convert.ToDecimal(totAmt);
                        Response = await _iSender.Send(new ApproverFetchFinancialCommand(Request));
                    }
                    else
                    {
                        return Json(Response);
                    }
                }
                else
                {
                    Request.FirstName = idd;
                    Response = await _iSender.Send(new ApproverFetchNonFinanceCommand(Request));
                }

                foreach (var item in Response)
                {
                    string strvalue = item.FirstName +
                                      (item.MiddleName != "" ? " " + item.MiddleName : "") + " " +
                                      item.LastName + " " + " | " + item.Designation +
                                      (!string.IsNullOrEmpty(item.Grade)
                                          ? " | " + item.Grade
                                          : "") + (!string.IsNullOrEmpty(item.Department)
                                          ? " | " + item.Department
                                          : "");
                    list.Add(new KeyValuePair<string, int>(strvalue, Convert.ToInt32(item.UserId)));
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during fetchApproverlist------ " + e.Message +
                    Environment.NewLine + e.StackTrace, loginUserId);
            }

            return Json(list);
        }

        public async ValueTask<JsonResult> UpdateNoteTitle(string noteid, string NoteTitle)
        {
            try
            {
                NoteModel model = new NoteModel();
                model.NoteId = noteid;
                model.NoteTitle = NoteTitle;
                await _iSender.Send(new UpdateNoteTitleCommand(model));
            }
            catch (Exception ex)
            {
                _iCustomLogger.LogwriteInfo("exception occur during InsertNoteTitle execution" +
                                            Environment.NewLine + "exception message-" +
                                            ex.Message + Environment.NewLine + ex.StackTrace,
                    loginUserId);
            }

            return Json("");
        }

        public async ValueTask<JsonResult> InsertNoteTitle(string NoteTitle)
        {
            try
            {
                NoteModel model = new NoteModel();
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    model.UserId = userId;
                    model.NoteTitle = NoteTitle;
                    model = await _iSender.Send(new InsertNoteTitleCommand(model));
                    return Json(model);
                }
                else
                {
                    return Json("");
                }
            }
            catch (Exception ex)
            {
                _iCustomLogger.LogwriteInfo("exception occur during InsertNoteTitle execution" +
                                            Environment.NewLine + "exception message-" +
                                            ex.Message + Environment.NewLine + ex.StackTrace,
                    loginUserId);
                return Json("");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async ValueTask<IActionResult> UpdateNoteBody([FromBody] ContentUpdate myJSON)
        {
            try

            {
                NoteModel model = new NoteModel();
                model.NoteId = myJSON.noteid;
                model.NoteBody = myJSON.NoteBody;
                await _iSender.Send(new UpdateNoteBodyCommand(model));
            }
            catch (Exception ex)
            {
                _iCustomLogger.LogwriteInfo("exception occur during UpdateNoteBody execution" +
                                            Environment.NewLine + "exception message-" +
                                            ex.Message + Environment.NewLine + ex.StackTrace,
                    loginUserId);
            }

            return Json("");
        }

        public async ValueTask<JsonResult> UpdateNoteCategory(string noteid, string CategoryId)
        {
            try
            {
                NoteModel model = new NoteModel();
                model.NoteId = noteid;
                model.CategoryId = CategoryId;
                bool str = await _iSender.Send(new UpdateNoteCategoryCommand(model));
                if (str)
                {
                    return Json("success");
                }
                else
                {
                    return Json("failed");
                }
            }
            catch (Exception ex)
            {
                _iCustomLogger.LogwriteInfo("exception occur during UpdateNoteCategory execution" +
                                            Environment.NewLine + "exception message-" +
                                            ex.Message + Environment.NewLine + ex.StackTrace,
                    loginUserId);
                return Json("failed");
            }
        }

        public async ValueTask<JsonResult> InsertCategory(string cat)
        {
            try
            {
                NoteModel model = new NoteModel();
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    model.UserId = userId;
                    model.CategoryId = cat;
                    model = await _iSender.Send(new InsertNoteCategoryCommand(model));
                    return Json(model);
                }
                else
                {
                    return Json("");
                }
            }
            catch (Exception ex)
            {
                _iCustomLogger.LogwriteInfo("exception occur during InsertCategory execution" +
                                            Environment.NewLine + "exception message-" +
                                            ex.Message + Environment.NewLine + ex.StackTrace,
                    loginUserId);
                return Json("");
            }
        }

        public async ValueTask<JsonResult> UpdateNoteExpenseIncurredAt(string noteid,
            string ExpAtId)
        {
            try
            {
                NoteModel model = new NoteModel();
                model.NoteId = noteid;
                model.ExpenseIncurredAtId = ExpAtId;
                await _iSender.Send(new UpdateNoteExpenseIncurredAtCommand(model));
            }
            catch (Exception ex)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during UpdateNoteExpenseIncurredAt execution" +
                    Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine +
                    ex.StackTrace, loginUserId);
            }

            return Json("");
        }

        public async ValueTask<JsonResult> InsertNoteExpensesIncurredAt(string exp)
        {
            try
            {
                NoteModel model = new NoteModel();
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    model.UserId = userId;
                    model.ExpenseIncurredAtId = exp;
                    model = await _iSender.Send(new InsertNoteExpensesIncurredAtCommand(model));
                    return Json(model);
                }
                else
                {
                    return Json("");
                }
            }
            catch (Exception ex)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during InsertNoteExpensesIncurredAt execution" +
                    Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine +
                    ex.StackTrace, loginUserId);
                return Json("");
            }
        }

        public async ValueTask<JsonResult> UpdateNoteNetureOfExpense(string noteid, string expId)
        {
            try
            {
                NoteModel model = new NoteModel();
                model.NoteId = noteid;
                model.NatureOfExpensesId = expId;
                await _iSender.Send(new UpdateNoteNetureOfExpenseCommand(model));
            }
            catch (Exception ex)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during InsertNoteExpensesIncurredAt execution" +
                    Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine +
                    ex.StackTrace, loginUserId);
            }

            return Json("");
        }

        public async ValueTask<JsonResult> InsertNatureOfExp(string exp)
        {
            NoteModel model = new NoteModel();
            string userId = User.FindFirstValue("UserId") ?? "";
            if (userId != "")
            {
                model.UserId = userId;
                model.NatureOfExpensesId = exp;
                model = await _iSender.Send(new InsertNatureOfExpCommand(model));
                return Json(model);
            }
            else
            {
                return Json("");
            }
        }

        public async ValueTask<JsonResult> UpdateNoteCapex(string noteid, string capex,
            string totalamt)
        {
            NoteModel model = new NoteModel();
            model.NoteId = noteid;
            model.CapitalExpenditure = capex;
            model.TotalAmount = totalamt;
            await _iSender.Send(new UpdateNoteCapexCommand(model));
            return Json("");
        }

        public async ValueTask<JsonResult> UpdateNoteOpex(string noteid, string opex,
            string totalamt)
        {
            NoteModel model = new NoteModel();
            model.NoteId = noteid;
            model.OperationalExpenditure = opex;
            model.TotalAmount = totalamt;
            await _iSender.Send(new UpdateNoteOpexCommand(model));
            return Json("");
        }

        public async ValueTask<JsonResult> InsertNoteOpex(string opex)
        {
            NoteModel model = new NoteModel();
            string userId = User.FindFirstValue("UserId") ?? "";
            if (userId != "")
            {
                model.UserId = userId;
                model.OperationalExpenditure = opex;
                model = await _iSender.Send(new InsertNoteOpexCommand(model));
                return Json(model);
            }
            else
            {
                return Json("");
            }
        }

        public async ValueTask<JsonResult> InsertNoteCapex(string opex)
        {
            NoteModel model = new NoteModel();
            string userId = User.FindFirstValue("UserId") ?? "";
            if (userId != "")
            {
                model.UserId = userId;
                model.CapitalExpenditure = opex;
                model = await _iSender.Send(new InsertNoteOpexCommand(model));
                return Json(model);
            }
            else
            {
                return Json("");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async ValueTask<IActionResult> InsertNoteBody([FromBody] ContentNew contentNew)
        {
            NoteModel model = new NoteModel();
            string userId = User.FindFirstValue("UserId") ?? "";
            if (userId != "")
            {
                model.UserId = userId;
                model.NoteBody = contentNew.NoteBody;
                model = await _iSender.Send(new InsertNoteBodyCommand(model));
                return Json(model);
            }
            else
            {
                return Json("");
            }
        }

        public async ValueTask<JsonResult> RemoveApprover(string noteid, string approveruserid,
            string approverid)
        {
            ApproverModel model = new ApproverModel();
            model.NoteId = noteid;
            model.UserId = approveruserid;
            model.ApproverId = approverid;
            await _iSender.Send(new RemoveApproverCommand(model));
            return Json("");
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async ValueTask<IActionResult> SaveTemplate([FromBody] Savetemplate savetemplate)
        {
            string str = "";
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    _iCustomLogger.LogwriteInfo(
                        "SaveTemplate method initiated------ and request---" +
                        JsonSerializer.Serialize(savetemplate), loginUserId);

                    TemplateModel model = new TemplateModel();
                    model.TemplateName = savetemplate.tempname;
                    model.TemplateBody = savetemplate.notebody;
                    model.DateOfCreation = DateTime.Now;
                    model.UserId = userId;
                    model.CategoryId = savetemplate.catid;
                    str = await _iSender.Send(new SaveTemplateCommand(model));
                    _iCustomLogger.LogwriteInfo("SaveTemplateCommand command return ------" + str,
                        loginUserId);
                    if (str == "success")
                    {
                        TempData["msg"] = "Successfully Template Saved.";
                    }
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Session timeout", _commonlogpath);
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during SaveTemplate ------ " + e.Message +
                    Environment.NewLine + e.StackTrace, _commonlogpath);
            }

            return Json(str);
        }

        public async ValueTask<JsonResult> InsertApproverByNoteId(string noteid,
            string approverlist)
        {
            NoteModel model = new NoteModel();
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    _iCustomLogger.LogwriteInfo(
                        "InsertApproverByNoteId method initiated------ and noteid---" + noteid +
                        "  --- approver list-" +
                        _encryption.DecryptStringAES(
                            approverlist.Replace("%2b", "+").Replace("%3D", "=")
                                .Replace("%2F", "/"), TempData["encdata"]?.ToString() ?? ""),
                        loginUserId);
                    model.UserId = userId;
                    model.NoteId = noteid;

                    model.ApproverIdList = _encryption.DecryptStringAES(
                        approverlist.Replace("%2b", "+").Replace("%3D", "=").Replace("%2F", "/"),
                        TempData["encdata"]?.ToString() ?? "");
                    TempData.Keep("encdata");
                    model = await _iSender.Send(new InsertApproverByNoteIdCommand(model));
                    return Json(model);
                }
                else
                {
                    return Json("");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during InsertApproverByNoteId ------ " + e.Message +
                    Environment.NewLine + e.StackTrace, _commonlogpath);
                return Json("");
            }
        }

        public async ValueTask<JsonResult> InsertSelectedApproverWthoutNoteId(string approverlist)
        {
            NoteModel model = new NoteModel();
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    _iCustomLogger.LogwriteInfo(
                        "InsertSelectedApproverWthoutNoteId method initiated------ approver list-" +
                        approverlist, loginUserId);
                    model.UserId = userId;
                    model.ApproverIdList = approverlist;
                    model = await _iSender.Send(
                        new InsertSelectedApproverWthoutNoteIdCommand(model));
                    return Json(model);
                }
                else
                {
                    return Json("");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during InsertApproverByNoteId ------ " + e.Message +
                    Environment.NewLine + e.StackTrace, _commonlogpath);
                return Json("");
            }
        }


        public IActionResult Final()
        {
            return View();
        }

        public IActionResult Preview()
        {
            return View();
        }

        public async ValueTask<IActionResult> Views()
        {
            try
            {
                string noteid = string.Empty;
                if (HttpContext.Request.Query["p"].ToString() != "")
                {
                    noteid = _iEncryption.AesDecrypt(HttpContext.Request.Query["p"].ToString());
                }
                else if (HttpContext.Request.Query["m"].ToString() != "")
                {
                    noteid = _iEncryption.AesDecryptForEmail(HttpContext.Request.Query["m"].ToString());
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("note id not found in Pending note page ------ ",
                        loginUserId);
                    return Redirect(_dashboardUrl);
                }

                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    _iCustomLogger.LogwriteInfo("Note View method initiated ---", loginUserId);

                    NoteModel Request = new();
                    Request.NoteId = noteid;
                    Request.UserId = userId;
                    _iCustomLogger.LogwriteInfo(
                        "Note View method request noteid---" + Request.NoteId, loginUserId);
                    ViewsNoteModel Response =
                        await _iSender.Send(new FetchViewsNoteCommand(Request));
                    return View(Response);
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Session timeout", _commonlogpath);
                    HttpContext.Session.SetString("RedirectFromView", "/Views?p=" + noteid);
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during view note ------ " + e.Message + Environment.NewLine +
                    e.StackTrace, _commonlogpath);
                return Redirect(_dashboardUrl);
            }
        }

        public async ValueTask<IActionResult> ViewFyi()
        {
            try
            {
                string noteid = string.Empty;
                if (HttpContext.Request.Query["p"].ToString() != "")
                {
                    noteid = _iEncryption.AesDecrypt(HttpContext.Request.Query["p"].ToString());
                }
                else if (HttpContext.Request.Query["m"].ToString() != "")
                {
                    noteid = _iEncryption.AesDecryptForEmail(HttpContext.Request.Query["m"].ToString());
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("note id not found in Fyi view page ------ ",
                        loginUserId);
                    return Redirect(_dashboardUrl);
                }

                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    _iCustomLogger.LogwriteInfo("Fyi View method initiated ---", loginUserId);

                    NoteModel Request = new();
                    Request.NoteId = noteid;
                    Request.UserId = userId;
                    _iCustomLogger.LogwriteInfo(
                        "Fyi View method request noteid---" + Request.NoteId, loginUserId);
                    ViewFyiNoteModel Response =
                        await _iSender.Send(new FetchFyiNoteDetailsCommand(Request));
                    return View(Response);
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Session timeout", _commonlogpath);
                    HttpContext.Session.SetString("RedirectFromView", "/Views?p=" + noteid);
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during view note ------ " + e.Message + Environment.NewLine +
                    e.StackTrace, _commonlogpath);
                return Redirect(_dashboardUrl);
            }
        }

        public async ValueTask<IActionResult> ApprovalRequest()
        {
            try
            {
                string noteid = string.Empty;
                if (HttpContext.Request.Query["p"].ToString() != "")
                {
                    noteid = _iEncryption.AesDecrypt(HttpContext.Request.Query["p"].ToString());
                }
                else if (HttpContext.Request.Query["m"].ToString() != "")
                {
                    noteid = _iEncryption.AesDecryptForEmail(HttpContext.Request.Query["m"].ToString());
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("note id not found in Approval Request page ------ ", _commonlogpath);
                    return Redirect(_dashboardUrl);
                }

                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    _iCustomLogger.LogwriteInfo("ApprovalRequest method initiated ---", loginUserId);

                    ViewBag.UserId = userId;

                    NoteModel Request = new();
                    Request.NoteId = noteid;
                    Request.UserId = userId;
                    _iCustomLogger.LogwriteInfo("ApprovalRequest method request noteid---" + Request.NoteId, loginUserId);
                    RequestApproverNoteModel Response = await _iSender.Send(new FetchApprovalRequestCommand(Request));
                    _iCustomLogger.LogwriteInfo("ApprovalRequest method response-------Notetitle is--" + Response.noteModel.NoteTitle, loginUserId);
                    return View(Response);
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Userid not found in Approval Request page ------ ", _commonlogpath);
                    HttpContext.Session.SetString("RedirectFromApprovalRequest", "/ApprovalRequest?p=" + _iEncryption.AesEncrypt(noteid));
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo("exception occur during Approval Request ------ " + e.Message + Environment.NewLine + e.StackTrace, _commonlogpath);
                return Redirect(_dashboardUrl);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async ValueTask<IActionResult> QueryInitiate(RequestApproverNoteModel note)
        {
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    note.querymodel.ApproverId = userId;
                }

                if (string.IsNullOrEmpty(note.querymodel.Comment))
                {
                    TempData["commentmsg"] = "Comment field can not blank";
                    return Redirect(_approvalRequestUrl + "?p=" + note.noteModel.NoteId);
                }

                _iCustomLogger.LogwriteInfo(
                    "Note Query method initiated --- and request-" + JsonSerializer.Serialize(note),
                    loginUserId);
                bool str = await _iSender.Send(new SaveQueryInitiateCommand(note));
                _iCustomLogger.LogwriteInfo("Note Query response ----" + str, loginUserId);
                if (str)
                {
                    TempData["msg"] = "Query Successfuly Initiated";
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during Query Initiate ------ " + e.Message +
                    Environment.NewLine + e.StackTrace, _commonlogpath);
                return Redirect("/");
            }

            return Redirect(_approvalRequestUrl + "?p=" +
                            _iEncryption.AesEncrypt(note.noteModel.NoteId));
        }


        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async ValueTask<IActionResult> ForYourInformation(RequestApproverNoteModel model)
        {
            try
            {
                model.noteModel.CreatorUserId = model.noteModel.UserId;

                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    model.noteModel.UserId = userId;
                }
                else
                {
                    return Redirect("/");
                }

                _iCustomLogger.LogwriteInfo(
                    "For Your Information method initiated --- and request-" +
                    JsonSerializer.Serialize(model), loginUserId);
                string str = await _iSender.Send(new SaveFyiCommand(model));
                _iCustomLogger.LogwriteInfo("Note For Your Information response ----" + str,
                    loginUserId);
                if (str == "success")
                {
                    TempData["FyiMessageSuccess"] = "FYI Successfully done.";
                }
                else if (str == "CreatorSame")
                {
                    TempData["FyiMessageFailed"] = "You Can Not FYI Note Creator";
                }
                else
                {
                    TempData["FyiMessageFailed"] = "Failed FYI User Not Found.";
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during FYI ------ " + e.Message + Environment.NewLine +
                    e.StackTrace, _commonlogpath);
                return Redirect("/");
            }

            return Redirect(_approvalRequestUrl + "?p=" +
                            _iEncryption.AesEncrypt(model.noteModel.NoteId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async ValueTask<IActionResult> NoteApprove(RequestApproverNoteModel model)
        {
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    model.noteModel.UserId = userId;
                }
                else
                {
                    return Redirect("/");
                }

                if (string.IsNullOrEmpty(model.querymodel.Comment))
                {
                    TempData["commentmsg"] = "Comment field can not blank";
                    return Redirect(_approvalRequestUrl + "?p=" + model.noteModel.NoteId);
                }

                _iCustomLogger.LogwriteInfo(
                    "Note Approve method initiated --- and request-" +
                    JsonSerializer.Serialize(model), loginUserId);
                bool str = await _iSender.Send(new NoteApproveCommand(model));
                _iCustomLogger.LogwriteInfo("Note Approve response ----" + str, loginUserId);
                if (str)
                {
                    TempData["msg"] = "Note Successfully Approved.";
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during Send back ------ " + e.Message + Environment.NewLine +
                    e.StackTrace, _commonlogpath);
                return Redirect("/");
            }

            return Redirect(_dashboardUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async ValueTask<IActionResult> AsignDelegate(RequestApproverNoteModel model)
        {
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    model.noteModel.UserId = userId;
                }
                else
                {
                    return Redirect("/");
                }

                if (!EmailValidation(model.noteModel.SearchKey))
                {
                    TempData["WrongApprovalEmail"] = "Please Enter Valid Email.";
                    return Redirect(_approvalRequestUrl + "?p=" + model.noteModel.NoteId);
                }

                _iCustomLogger.LogwriteInfo(
                    "Asign Delegate method initiated --- and request-" +
                    JsonSerializer.Serialize(model), loginUserId);
                int str = await _iSender.Send(new AsignDelegateCommand(model));
                _iCustomLogger.LogwriteInfo("Asign Delegate response ----" + str, loginUserId);
                switch (str)
                {
                    case 0:
                        TempData["msg"] = "Note Delegated Successfully";
                        return Redirect("/Note/Pending");
                    case 1:
                        TempData["WrongApprovalEmail"] = "Assign Delegate Failed UserId Not Found.";
                        return Redirect(_approvalRequestUrl + "?p=" +
                                        _iEncryption.AesEncrypt(model.noteModel.NoteId));
                    case 2:
                        TempData["WrongApprovalEmail"] = "You Can Not Deligate Note Creator.";
                        return Redirect(_approvalRequestUrl + "?p=" +
                                        _iEncryption.AesEncrypt(model.noteModel.NoteId));
                    case 3:
                        TempData["WrongApprovalEmail"] =
                            "You Can Not Deligate Existing Approver. Or You Can Not Deligate Yourself.";
                        return Redirect(_approvalRequestUrl + "?p=" +
                                        _iEncryption.AesEncrypt(model.noteModel.NoteId));
                    default:
                        TempData["WrongApprovalEmail"] = "Unknow Error Happend.";
                        return Redirect(_approvalRequestUrl + "?p=" +
                                        _iEncryption.AesEncrypt(model.noteModel.NoteId));
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during AsignDelegate ------ " + e.Message +
                    Environment.NewLine + e.StackTrace, _commonlogpath);
                return Redirect("/");
            }
        }

        public bool EmailValidation(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }

        public async ValueTask<IActionResult> ViewDraft()
        {
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    if (HttpContext.Request.Query["p"].ToString() != "")
                    {
                        NoteModel Request = new();
                        Request.NoteId =
                            _iEncryption.AesDecrypt(HttpContext.Request.Query["p"].ToString());
                        Request.UserId = userId;
                        _iCustomLogger.LogwriteInfo("View Draft method initiated -------",
                            loginUserId);
                        PendingNoteModel Response =
                            await _iSender.Send(new FetchPendingNoteCommand(Request));
                        _iCustomLogger.LogwriteInfo(
                            "View Draft response ----" + JsonSerializer.Serialize(Response),
                            loginUserId);
                        return View(Response);
                    }
                    else
                    {
                        _iCustomLogger.LogwriteInfo(
                            "note id not found in Pending note page ------ ", _commonlogpath);
                        return Redirect("/");
                    }
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Userid not found in Pending note page ------ ",
                        _commonlogpath);
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during view draft note ------ " + e.Message +
                    Environment.NewLine + e.StackTrace, _commonlogpath);
                return Redirect("/");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async ValueTask<IActionResult> SaveQueryReply(WithdrawNoteModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string userId = User.FindFirstValue("UserId") ?? "";
                    if (userId != "")
                    {
                        model.noteModel.UserId = userId;
                        _iCustomLogger.LogwriteInfo(
                            "SaveQueryReply method initiated -------and request-- " +
                            JsonSerializer.Serialize(model), loginUserId);
                        bool str = await _iSender.Send(new QueryReplyCommand(model));
                        _iCustomLogger.LogwriteInfo(
                            "SaveQueryReply response ----and status- " + str, loginUserId);
                        if (str)
                        {
                            TempData["msg"] = "Reply Successfully Sent.";
                            return Redirect("/Note/Withdraw?p=" +
                                            _encryption.AesEncrypt(model.noteModel.NoteId));
                        }
                        else
                        {
                            return Redirect(_approvalRequestUrl + "?p=" +
                                            _encryption.AesEncrypt(model.noteModel.NoteId));
                        }
                    }
                    else
                    {
                        return Redirect("/");
                    }
                }
                catch (Exception e)
                {
                    _iCustomLogger.LogwriteInfo(
                        "exception occur during SaveQueryReply ------ " + e.Message +
                        Environment.NewLine + e.StackTrace, _commonlogpath);
                    return Redirect("/");
                }
            }

            return View();
        }

        public async ValueTask<IActionResult> WithdrawList(WithdrawListModel Request)
        {
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    FilterWithdrawListNote filterWithdrawListNote = new();
                    filterWithdrawListNote.UserId = Convert.ToInt32(userId);
                    filterWithdrawListNote.StartDate = Request.withdrawlist.StartDate;
                    filterWithdrawListNote.EndDate = Request.withdrawlist.EndDate;
                    filterWithdrawListNote.Category = Request.withdrawlist.Category;

                    WithdrawListModel Response =
                        await _iSender.Send(new FetchWidthdrawListCommand(filterWithdrawListNote));

                    return View(Response);
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Userid not found in Withdraw List page ------ ",
                        _commonlogpath);
                    return Redirect(_dashboardUrl);
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during show Withdraw List ------ " + e.Message +
                    Environment.NewLine + e.StackTrace, _commonlogpath);
                return Redirect(_dashboardUrl);
            }
        }

        [HttpPost]
        public async ValueTask<IActionResult> DownloadWithdrawList(
            [FromBody] WithdrawListModel Request)
        {
            _iCustomLogger.LogwriteInfo(
                "Download Send back List note method call start for excel download ---" +
                JsonSerializer.Serialize(Request), loginUserId);
            byte[] content = [];
            try
            {
                await Task.Run(() =>
                {
                    using XLWorkbook workbook = new();
                    IXLWorksheet worksheet = workbook.Worksheets.Add("Report");

                    int ColumnIndex = 0;
                    PropertyInfo[] properties = typeof(WithdrawListOutModel).GetProperties();

                    string[] RequiredColumns = ["NoteTitle", "CategoryName", "WithdrawDate"];
                    for (int i = 0; i < properties.Length; i++)
                    {
                        if (RequiredColumns.Contains(properties[i].Name))
                        {
                            if (properties[i].Name == "NoteTitle")
                            {
                                worksheet.Cell(1, ColumnIndex + 1).Value = "Note Title";
                            }

                            if (properties[i].Name == "CategoryName")
                            {
                                worksheet.Cell(1, ColumnIndex + 1).Value = "Category";
                            }

                            if (properties[i].Name == "WithdrawDate")
                            {
                                worksheet.Cell(1, ColumnIndex + 1).Value = "Withdraw Date";
                            }

                            IXLCell cell = worksheet.Cell(1, ColumnIndex + 1);
                            cell.Style.Font.Bold = true;
                            worksheet.Cell(1, ColumnIndex + 1).Style.Alignment
                                .SetHorizontal(XLAlignmentHorizontalValues.Center);
                            ColumnIndex++;
                        }
                    }

                    int row = 2;
                    foreach (WithdrawListOutModel item in Request.withdrawListOutModels)
                    {
                        ColumnIndex = 0;
                        for (int i = 0; i < properties.Length; i++)
                        {
                            if (RequiredColumns.Contains(properties[i].Name))
                            {
                                item.GetType().GetProperty(properties[i].Name)?.GetValue(item);
                                string CellValue =
                                    item.GetType().GetProperty(properties[i].Name)?.GetValue(item)
                                        ?.ToString() ?? string.Empty;
                                worksheet.Cell(row, ColumnIndex + 1).Value = CellValue;
                                ColumnIndex++;
                            }
                        }

                        row++;
                    }

                    using MemoryStream stream = new();
                    workbook.SaveAs(stream);
                    content = stream.ToArray();
                });
            }
            catch (Exception ex)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during DownloadWithdrawList excel execution" +
                    Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine +
                    ex.StackTrace, loginUserId);
            }

            return File(content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "dataReport.xlsx");
        }

        public async ValueTask<IActionResult> WithdrawNoteDetails()
        {
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    if (HttpContext.Request.Query["p"].ToString() != "")
                    {
                        NoteModel Request = new();
                        Request.NoteId =
                            _iEncryption.AesDecrypt(HttpContext.Request.Query["p"].ToString());
                        Request.UserId = userId;
                        WithdrawNoteDetailsModel Response =
                            await _iSender.Send(new FetchWithdrawNoteDetailsCommand(Request));
                        _iCustomLogger.LogwriteInfo(
                            "WithdrawNoteDetails response ---- " + Response.noteModel.NoteTitle,
                            loginUserId);
                        return View(Response);
                    }
                    else
                    {
                        _iCustomLogger.LogwriteInfo(
                            "note id not found in Withdraw Note Details Page ------ ",
                            _commonlogpath);
                        return Redirect(_dashboardUrl);
                    }
                }
                else
                {
                    _iCustomLogger.LogwriteInfo(
                        "Userid not found in Withdraw Note Details Page ------ ", _commonlogpath);
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during show Withdraw Note Details Page ------ " + e.Message +
                    Environment.NewLine + e.StackTrace, _commonlogpath);
                return Redirect(_dashboardUrl);
            }
        }

        public async ValueTask<IActionResult> SaveDraftFromWithdrawList(
            WithdrawNoteDetailsModel Request)
        {
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    Request.noteModel.UserId = userId;
                    _iCustomLogger.LogwriteInfo(
                        "SaveDraftFromWithdrawList method initiated -------and request-- " +
                        JsonSerializer.Serialize(Request), loginUserId);
                    string str = await _iSender.Send(new SaveAsDraftFromWithdrawCommand(Request));
                    _iCustomLogger.LogwriteInfo(
                        "SaveDraftFromWithdrawList response ---- status- " + str, loginUserId);
                    if (str == "success")
                    {
                        TempData["msg"] = "Withdraw note save as draft successfully";
                        return Redirect("/Note/WithdrawList");
                    }
                    else
                    {
                        TempData["errormsg"] = "Something went wrong.";
                        return Redirect("/Note/WithdrawList");
                    }
                }
                else
                {
                    _iCustomLogger.LogwriteInfo(
                        "Userid not found in Save Draft From Withdraw List Page ------ ",
                        _commonlogpath);
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during show Save Draft From Withdraw List Page ------ " +
                    e.Message + Environment.NewLine + e.StackTrace, _commonlogpath);
                return Redirect(_dashboardUrl);
            }
        }

        public async ValueTask<JsonResult> FetchRecomendedApproverlist(string idd)
        {
            var list = new List<KeyValuePair<string, int>>();
            IEnumerable<UserMasterModel> Response = [];
            try
            {
                UserMasterModel Request = new();
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    Request.UserId = Convert.ToInt32(userId);
                }

                _iCustomLogger.LogwriteInfo("FetchRecomendedApproverlist method initiated------ ",
                    loginUserId);

                Request.FirstName = idd;
                Response = await _iSender.Send(new RecomendedApproverListFetchCommand(Request));

                foreach (var item in Response)
                {
                    string strvalue = item.FirstName +
                                      (item.MiddleName != "" ? " " + item.MiddleName : "") + " " +
                                      item.LastName + " " + " | " + item.Designation +
                                      (!string.IsNullOrEmpty(item.Grade)
                                          ? " | " + item.Grade
                                          : "") + (!string.IsNullOrEmpty(item.Department)
                                          ? " | " + item.Department
                                          : "");
                    list.Add(new KeyValuePair<string, int>(strvalue, Convert.ToInt32(item.UserId)));
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during FetchRecomendedApproverlist------ " + e.Message +
                    Environment.NewLine + e.StackTrace, loginUserId);
            }

            return Json(list);
        }

        public async ValueTask<JsonResult> InsertRecomendedApproverByNoteId(string noteid,
            string approverlist)
        {
            NoteModel model = new NoteModel();
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    _iCustomLogger.LogwriteInfo(
                        "InsertRecomendedApproverByNoteId method initiated------ and noteid---" +
                        noteid + "  --- approver list-" +
                        _encryption.DecryptStringAES(
                            approverlist.Replace("%2b", "+").Replace("%3D", "=")
                                .Replace("%2F", "/"), TempData["encdata"]?.ToString() ?? ""),
                        loginUserId);
                    model.UserId = userId;
                    model.NoteId = noteid;
                    model.ApproverIdList = _encryption.DecryptStringAES(
                        approverlist.Replace("%2b", "+").Replace("%3D", "=").Replace("%2F", "/"),
                        TempData["encdata"]?.ToString() ?? "");
                    TempData.Keep("encdata");
                    model = await _iSender.Send(new InsertRecomendedApproverByNoteIdCommand(model));
                    return Json(model);
                }
                else
                {
                    return Json("");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during InsertRecomendedApproverByNoteId ------ " + e.Message +
                    Environment.NewLine + e.StackTrace, _commonlogpath);
                return Json("");
            }
        }

        #region Approver Dashboard Section

        [HttpGet]
        public async ValueTask<IActionResult> ApproverDashboard(ApproverHistoryNotes requestModel)
        {
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    _iCustomLogger.LogwriteInfo(
                        "Approver History page get method initiated -------- and request --------" +
                        JsonSerializer.Serialize(requestModel), loginUserId);

                    ApproverHistoryCommand Command = new()
                    {
                        FilterData = new FilterApproverHistory()
                        {
                            Category = requestModel.FilterApproverHistory.Category,
                            StartDate = requestModel.FilterApproverHistory.StartDate,
                            EndDate = requestModel.FilterApproverHistory.EndDate,
                            UserId = Convert.ToInt32(userId),
                            DateRange = requestModel.FilterApproverHistory.DateRange,
                            Title = requestModel.FilterApproverHistory.Title,
                            Status = requestModel.FilterApproverHistory.Status,
                        }
                    };
                    CommonResponse<ApproverHistoryNotes> response = await _iSender.Send(Command);
                    return View(response.Data);
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Session timeout", _commonlogpath);
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    $"Exception occur during ApproverHistory page ---------------- {e.Message}{Environment.NewLine} {e.StackTrace} ",
                    loginUserId);
                return View(new ApproverHistoryNotes());
            }
        }

        [HttpPost]
        public async ValueTask<IActionResult> DownloadApproverHistory(
            [FromBody] ApproverHistoryNotes Request)
        {
            _iCustomLogger.LogwriteInfo(
                "Download Pending note method call start for excel download ---" +
                JsonSerializer.Serialize(Request), loginUserId);
            byte[] content = [];
            try
            {
                await Task.Run(() =>
                {
                    using XLWorkbook workbook = new();
                    IXLWorksheet worksheet = workbook.Worksheets.Add("Approver Dashboard");

                    PropertyInfo[] properties = typeof(ApproverHistoryData).GetProperties();

                    // initialize the columnIndex and row
                    int columnIndex = 0;
                    int row = 1;

                    //write column names as header
                    foreach (PropertyInfo property in properties)
                    {
                        var displayName = property.GetCustomAttribute<DisplayNameAttribute>();
                        if (displayName != null)
                        {
                            worksheet.Cell(row, columnIndex + 1).Value = displayName.DisplayName;
                            IXLCell cell = worksheet.Cell(1, columnIndex + 1);
                            cell.Style.Font.Bold = true;
                            worksheet.Cell(1, columnIndex + 1).Style.Alignment
                                .SetHorizontal(XLAlignmentHorizontalValues.Center);
                            columnIndex++;
                        }
                    }

                    row++;

                    //write the column values
                    foreach (ApproverHistoryData item in Request.ApproverHistoryData)
                    {
                        columnIndex = 0;

                        foreach (var property in properties)
                        {
                            var displayNameAttribute =
                                property.GetCustomAttribute<DisplayNameAttribute>();
                            if (displayNameAttribute != null)
                            {
                                var value = property.GetValue(item)?.ToString();
                                worksheet.Cell(row, columnIndex + 1).Value = value;
                                columnIndex++;
                            }
                        }

                        row++;
                    }

                    using MemoryStream stream = new();
                    workbook.SaveAs(stream);
                    content = stream.ToArray();
                });
            }
            catch (Exception ex)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during DownloadPending excel execution" +
                    Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine +
                    ex.StackTrace, loginUserId);
            }

            return File(content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "dataReport.xlsx");
        }

        [HttpGet]
        public async ValueTask<IActionResult> FetchAutopopulateNoteTitleList(string query,
            string dashboardType)
        {
            CommonResponse<AutopopulatedNotes> response = new();

            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";

                if (string.IsNullOrWhiteSpace(userId))
                {
                    _iCustomLogger.LogwriteInfo("Session timeout", _commonlogpath);
                    return Redirect("/");
                }

                _iCustomLogger.LogwriteInfo(
                    "FetchAutopopulateNoteTitleList get method initiated -------- and request --------" +
                    "Search Text : " + query, loginUserId);

                AutopopulatedNoteCommand Command = new()
                {
                    FilterData = new FilterNote()
                    {
                        UserId = Convert.ToInt32(userId),
                        SearchText = query,
                        DashboardType = dashboardType
                    }
                };
                response = await _iSender.Send(Command);
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during FetchAutopopulateNoteTitleList------ " + e.Message +
                    Environment.NewLine + e.StackTrace, loginUserId);
            }

            return Json(response.Data.NoteData.ToList());
        }

        #endregion

        #region Creator Dashboard Section

        [HttpGet]
        public async ValueTask<IActionResult> CreatorDashboard(CreatorDashboard requestModel)
        {
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    _iCustomLogger.LogwriteInfo(
                        "Creator Dashboard page get method initiated -------- and request --------" +
                        JsonSerializer.Serialize(requestModel), loginUserId);

                    CreatorDashboardCommand Command = new()
                    {
                        FilterData = new FilterCreatorData()
                        {
                            Category = requestModel.FilterCreatorData.Category,
                            StartDate = requestModel.FilterCreatorData.StartDate,
                            EndDate = requestModel.FilterCreatorData.EndDate,
                            UserId = Convert.ToInt32(userId),
                            DateRange = requestModel.FilterCreatorData.DateRange,
                            Title = requestModel.FilterCreatorData.Title,
                            Status = requestModel.FilterCreatorData.Status,
                        }
                    };
                    CommonResponse<CreatorDashboard> response = await _iSender.Send(Command);
                    return View(response.Data);
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Session timeout", _commonlogpath);
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    $"Exception occur during CreatorDashboard page ---------------- {e.Message}{Environment.NewLine} {e.StackTrace} ",
                    loginUserId);
                return View(new CreatorDashboard());
            }
        }

        [HttpPost]
        public async ValueTask<IActionResult> DownloadCreatorDashboard(
            [FromBody] CreatorDashboard Request)
        {
            _iCustomLogger.LogwriteInfo(
                "Download Pending note method call start for excel download ---" +
                JsonSerializer.Serialize(Request), loginUserId);
            byte[] content = [];
            try
            {
                await Task.Run(() =>
                {
                    using XLWorkbook workbook = new();
                    IXLWorksheet worksheet = workbook.Worksheets.Add("Creator Dashboard");

                    PropertyInfo[] properties = typeof(CreatorData).GetProperties();

                    int columnIndex = 0;
                    int row = 1;

                    //write column names as header
                    foreach (PropertyInfo property in properties)
                    {
                        var displayName = property.GetCustomAttribute<DisplayNameAttribute>();

                        if (displayName != null)
                        {
                            worksheet.Cell(row, columnIndex + 1).Value = displayName.DisplayName;
                            IXLCell cell = worksheet.Cell(1, columnIndex + 1);
                            cell.Style.Font.Bold = true;
                            worksheet.Cell(1, columnIndex + 1).Style.Alignment
                                .SetHorizontal(XLAlignmentHorizontalValues.Center);

                            columnIndex++;
                        }
                    }

                    row++;

                    //write the column values
                    foreach (CreatorData item in Request.CreatorData)
                    {
                        columnIndex = 0;

                        foreach (var property in properties)
                        {
                            var displayNameAttribute =
                                property.GetCustomAttribute<DisplayNameAttribute>();
                            if (displayNameAttribute != null)
                            {
                                var value = property.GetValue(item)?.ToString();
                                worksheet.Cell(row, columnIndex + 1).Value = value;
                                columnIndex++;
                            }
                        }

                        row++;
                    }

                    using MemoryStream stream = new();
                    workbook.SaveAs(stream);
                    content = stream.ToArray();
                });
            }
            catch (Exception ex)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during DownloadPending excel execution" +
                    Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine +
                    ex.StackTrace, loginUserId);
            }

            return File(content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "dataReport.xlsx");
        }

        #endregion

        public async ValueTask<IActionResult> DelegateNote()
        {
            try
            {
                string noteid = string.Empty;
                if (HttpContext.Request.Query["p"].ToString() != "")
                {
                    noteid = _iEncryption.AesDecrypt(HttpContext.Request.Query["p"].ToString());
                }
                else
                {
                    _iCustomLogger.LogwriteInfo(
                        "note id not found in Approval Request page ------ ", _commonlogpath);
                    return Redirect(_dashboardUrl);
                }

                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    _iCustomLogger.LogwriteInfo("DelegateNote method initiated ---", loginUserId);

                    NoteModel Request = new();
                    Request.NoteId = noteid;
                    Request.UserId = userId;
                    _iCustomLogger.LogwriteInfo(
                        "DelegateNote method request noteid---" + Request.NoteId, loginUserId);
                    DelegateNoteModel Response =
                        await _iSender.Send(new FetchDelegateNoteCommand(Request));
                    _iCustomLogger.LogwriteInfo(
                        "ApprovalRequest method response-------Notetitle is--" +
                        Response.noteModel.NoteTitle, loginUserId);
                    return View(Response);
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Userid not found in Approval Request page ------ ",
                        _commonlogpath);
                    HttpContext.Session.SetString("RedirectFromApprovalRequest",
                        "/DelegateNote?p=" + _iEncryption.AesEncrypt(noteid));
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during Approval Request ------ " + e.Message +
                    Environment.NewLine + e.StackTrace, _commonlogpath);
                return Redirect(_dashboardUrl);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async ValueTask<IActionResult> UploadAttachment(WithdrawNoteModel model)
        {
            if (model.noteModel.AttachFiles != null)
            {
                string result = await _iSender.Send(new SaveAttachmentCommand(model));
                TempData["Attachsuccessmsg"] = result;
            }

            return Redirect("/note/Withdraw?p=" + _encryption.AesEncrypt(model.noteModel.NoteId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async ValueTask<IActionResult> DelegateByCreator(WithdrawNoteModel model)
        {
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId == "")
                {
                    return Redirect("/");
                }

                if (string.IsNullOrEmpty(model.querymodel.Comment) ||
                    string.IsNullOrEmpty(model.noteModel.SearchKey))
                {
                    TempData["WrongApprovalEmail"] = "Field can not be blank.";
                    return Redirect(_withdrawUrl + "?p=" + model.noteModel.NoteId);
                }

                model.creatorDelegate.creatorid = userId;
                model.creatorDelegate.oldapproverid =
                    _encryption.AesDecrypt(model.creatorDelegate.oldapproverid);
                int returnvalue = await _iSender.Send(new DelegateByCreatorCommand(model));
                switch (returnvalue)
                {
                    case 0:
                        TempData["msg"] = "Note Delegated Successfully";
                        return Redirect("/Note/Pending");
                    case 1:
                        TempData["WrongApprovalEmail"] = "Assign Delegate Failed UserId Not Found.";
                        return Redirect(_withdrawUrl + "?p=" +
                                        _iEncryption.AesEncrypt(model.noteModel.NoteId));
                    case 2:
                        TempData["WrongApprovalEmail"] = "You Can Not Deligate Note Creator.";
                        return Redirect(_withdrawUrl + "?p=" +
                                        _iEncryption.AesEncrypt(model.noteModel.NoteId));
                    case 3:
                        TempData["WrongApprovalEmail"] =
                            "You Can Not Deligate Existing Approver. Or You Can Not Deligate Yourself.";
                        return Redirect(_withdrawUrl + "?p=" +
                                        _iEncryption.AesEncrypt(model.noteModel.NoteId));
                    case 4:
                        TempData["WrongApprovalEmail"] = "Assign Delegate UserId Not Found.";
                        return Redirect(_withdrawUrl + "?p=" +
                                        _iEncryption.AesEncrypt(model.noteModel.NoteId));
                    default:
                        TempData["WrongApprovalEmail"] = "Unknow Error Happend.";
                        return Redirect(_withdrawUrl + "?p=" +
                                        _iEncryption.AesEncrypt(model.noteModel.NoteId));
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during DelegateByCreator ------ " + e.Message +
                    Environment.NewLine + e.StackTrace, loginUserId);
            }

            return Redirect("/");
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async ValueTask<IActionResult> FyiByCreator(RequestApproverNoteModel model)
        {
            try
            {
                model.noteModel.CreatorUserId = model.noteModel.UserId;
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    model.noteModel.UserId = userId;
                }
                else
                {
                    return Redirect("/");
                }

                _iCustomLogger.LogwriteInfo(
                    "For Your Information method initiated --- and request-" +
                    JsonSerializer.Serialize(model), loginUserId);
                string str = await _iSender.Send(new FyiByCreatorCommand(model));
                _iCustomLogger.LogwriteInfo("Note For Your Information response ----" + str,
                    loginUserId);
                if (str == "success")
                {
                    TempData["FyiMessageSuccess"] = "FYI Successfully done.";
                }
                else if (str == "CreatorSame")
                {
                    TempData["FyiMessageFailed"] = "You Can Not FYI Note Creator";
                }
                else if (str == "ApproverSame")
                {
                    TempData["FyiMessageFailed"] = "You Can Not FYI Note Approver";
                }
                else if (str == "SameFyiUser")
                {
                    TempData["FyiMessageFailed"] = "User Already FYI";
                }
                else
                {
                    TempData["FyiMessageFailed"] = "Failed FYI User Not Found.";
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during FYI ------ " + e.Message + Environment.NewLine +
                    e.StackTrace, _commonlogpath);
                return Redirect("/");
            }

            return Redirect(_withdrawUrl + "?p=" + _iEncryption.AesEncrypt(model.noteModel.NoteId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async ValueTask<IActionResult> SkippApprover(WithdrawNoteModel model)
        {
            try
            {
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId == "")
                {
                    return Redirect("/");
                }

                model.skippApprover.NoteId = _encryption.AesDecrypt(model.noteModel.NoteId);
                model.skippApprover.UserId = userId;
                int returnvalue = await _iSender.Send(new SkippByCreatorCommand(model));
                switch (returnvalue)
                {
                    case 0:
                        TempData["SkipSuccess"] = "Approver Skipped Successfully.";
                        return Redirect(_withdrawUrl + "?p=" + model.noteModel.NoteId);
                    case 1:
                        TempData["SkipFail"] = "You Can Not Skip Final Approver.";
                        return Redirect(_withdrawUrl + "?p=" + model.noteModel.NoteId);
                    case 2:
                        TempData["SkipFail"] =
                            "Something Went Wrong! Next Approver Not Set As Current Approver.";
                        return Redirect(_withdrawUrl + "?p=" + model.noteModel.NoteId);
                    case 3:
                        TempData["SkipFail"] =
                            "Something Went Wrong! Current Approver Not Skipped Properly.";
                        return Redirect(_withdrawUrl + "?p=" + model.noteModel.NoteId);
                    default:
                        TempData["SkipFail"] = "Unknow Error Happend.";
                        return Redirect(_withdrawUrl + "?p=" + model.noteModel.NoteId);
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo(
                    "exception occur during SkippApprover ------ " + e.Message +
                    Environment.NewLine + e.StackTrace, loginUserId);
            }

            return Redirect("/");
        }

        [HttpPost]
        public async ValueTask<IActionResult> RemoveAttachment(string NoteId, string AttachmentId)
        {
            string noteId = _iEncryption.AesDecrypt(NoteId);
            var result = await _iSender.Send(new RemoveAttachmentCommand(noteId, AttachmentId));
            return Json(result.Data);
        }

        [HttpGet]
        public async ValueTask<IActionResult> EditDraftNote()
        {
            NoteModel notempdel = new NoteModel();
            try
            {
                TempData["encdata"] = DateTime.Now.ToString("yyyyMMddHHmmssff");
                TempData.Keep("encdata");
                NoteModel Request = new();
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId == "")
                {
                    _iCustomLogger.LogwriteInfo("Session timeout", _commonlogpath);
                    return Redirect("/");
                }
                _iCustomLogger.LogwriteInfo("user exists for user id------ " + userId, loginUserId);
                if (HttpContext.Request.Query["s"].ToString() != "")
                {
                    Request.NoteId = _iEncryption.AesDecrypt(HttpContext.Request.Query["s"].ToString());
                    DraftNoteModel Response1 = await _iSender.Send(new FetchSaveNoteDataCommand(Request));
                    _iCustomLogger.LogwriteInfo("FetchSaveNoteDataCommand return value------ note title-- " + Response1.noteModel.NoteTitle, loginUserId);
                    notempdel.NoteStatus = Response1.noteModel.NoteStatus;
                    notempdel.ApproverIdList = "";
                    notempdel.NoteId = _iEncryption.AesEncrypt(Response1.noteModel.NoteId);
                    notempdel.NoteTitle = Response1.noteModel.NoteTitle;
                    notempdel.CategoryId = Response1.noteModel.CategoryId;
                    notempdel.NoteBody = Response1.noteModel.NoteBody;
                    notempdel.NoteState = Response1.noteModel.NoteState.ToString();
                    notempdel.NatureOfExpensesId = Response1.noteModel.NatureOfExpensesId;
                    notempdel.ExpenseIncurredAtId = Response1.noteModel.ExpenseIncurredAtId;
                    notempdel.CapitalExpenditure = Response1.noteModel.CapitalExpenditure ?? "";
                    notempdel.OperationalExpenditure = Response1.noteModel.OperationalExpenditure ?? "";
                    notempdel.TotalAmount = Response1.noteModel.TotalAmount;
                    notempdel.RecomendedApproverIdList = "";
                    if (Response1.userMaster != null)
                    {
                        int i = 1;
                        int j = 1;
                        IEnumerable<UserMasterModel> users = Response1.userMaster;
                        StringBuilder sb = new();
                        StringBuilder recomendSb = new();
                        foreach (var dr in users)
                        {
                            if (dr.ApproverType != "R")
                            {
                                if (i == 1)
                                {
                                    notempdel.ApproverIdList = dr.UserId.ToString();
                                }
                                else
                                {
                                    sb.Append("," + dr.UserId);
                                }
                                i++;
                            }
                            else
                            {
                                if (j == 1)
                                {
                                    notempdel.RecomendedApproverIdList = dr.UserId.ToString();
                                }
                                else
                                {
                                    recomendSb.Append("," + dr.UserId);
                                }
                                j++;
                            }
                        }
                        notempdel.ApproverIdList = string.Concat(notempdel.ApproverIdList, sb.ToString());
                        notempdel.RecomendedApproverIdList = string.Concat(notempdel.RecomendedApproverIdList, recomendSb.ToString());
                    }
                    ViewBag.ApproverList = Response1.userMaster!.Where(user => user.ApproverType != "R").ToList();
                    ViewBag.RecomendedApproverList = Response1.userMaster!.Where(user => user.ApproverType == "R").ToList();
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("No template selected for the user------ " + userId, loginUserId);
                    notempdel.NoteStatus = "";
                    notempdel.ApproverIdList = "";
                    notempdel.NoteId = "";
                    notempdel.TotalAmount = "0";
                }
                _iCustomLogger.LogwriteInfo("before calling CreateNoteCommand ----- ", loginUserId);
                IEnumerable<CategoryRespModel> Response = await _iSender.Send(new CreateNoteCommand(Request));
                _iCustomLogger.LogwriteInfo("CreateNoteCommand return value - " + JsonSerializer.Serialize(Response), loginUserId);
                List<SelectListItem> li = [];
                foreach (var item in Response)
                {
                    li.Add(new SelectListItem { Text = item.CategoryName.ToString(), Value = item.CategoryId.ToString() });
                }
                ViewBag.CategoryList = new SelectList(li, "Value", "Text");
                notempdel.UserId = userId;


            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo("exception occur during new create page load------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
            }
            _iCustomLogger.LogwriteInfo("Before sending the data to create note view. The data --- " + JsonSerializer.Serialize(notempdel), loginUserId);
            return View(notempdel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async ValueTask<IActionResult> EditDraftNote(NoteModel model)
        {
            NoteModel Response = new();
            try
            {
                

                if (!ModelState.IsValid)
                {
                    var validationContext = new ValidationContext(model)
                    {
                        MemberName = nameof(model.NoteBody)
                    };

                    var validationResults = new List<ValidationResult>();
                    if (!Validator.TryValidateProperty(model.NoteBody, validationContext, validationResults))
                    {
                        _iCustomLogger.LogwriteInfo("form post is not valid for invalid note body", loginUserId);
                        TempData["errormsg"] = "Please fill the proper detail in note body";
                    }
                    else
                    {
                        _iCustomLogger.LogwriteInfo("form post is not valid for create note page", loginUserId);
                        TempData["errormsg"] = "Please fill the required details";
                    }
                    return Redirect(_createNotePathUrl);
                }
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    if (!await _iSender.Send(new FileExtensionValidationCommand(model)))
                    {
                        _iCustomLogger.LogwriteInfo("file type not supported", loginUserId);
                        TempData["errormsg"] = "Opps! File type not supported by this system.";
                        return Redirect(_createNotePathUrl);
                    }
                    model.UserId = userId;
                    if (TempData["encdata"] == null)
                    {
                        return Redirect("/");
                    }

                    model.ApproverIdList = _encryption.DecryptStringAES(model.ApproverIdList, TempData["encdata"]?.ToString() ?? "");
                    if (model.RecomendedApproverIdList != null)
                    {
                        model.RecomendedApproverIdList = _encryption.DecryptStringAES(model.RecomendedApproverIdList, TempData["encdata"]?.ToString() ?? "");
                    }
                    if (model.ApproverIdList == "keyError")
                    {
                        _iCustomLogger.LogwriteInfo("Exception On Approver Descryption:", loginUserId);
                        return Redirect(_createNotePathUrl);
                    }
                    if (model.RecomendedApproverIdList != "" && model.RecomendedApproverIdList == "keyError")
                    {
                        _iCustomLogger.LogwriteInfo("Exception On Recomended Approver Descryption:", loginUserId);
                        return Redirect(_createNotePathUrl);
                    }
                    if (model.CategoryId == "1")
                    {
                        if (model.CapitalExpenditure != "" && model.OperationalExpenditure != "" && model.ExpenseIncurredAtId != "" && model.NatureOfExpensesId != "")
                        {
                            _iCustomLogger.LogwriteInfo("before SaveNoteCommand execution for financial-------", loginUserId);
                            Response = await _iSender.Send(new SaveNoteCommand(model));
                            _iCustomLogger.LogwriteInfo("after SaveNoteCommand execute. and the result for financial-" + JsonSerializer.Serialize(Response), loginUserId);
                        }
                        else
                        {
                            TempData["errormsg"] = "Fields are mandetory";
                            return Redirect(_createNotePathUrl);
                        }
                    }
                    else if (model.CategoryId == "2")
                    {
                        _iCustomLogger.LogwriteInfo("before SaveNoteCommand execution for non financial-------", loginUserId);
                        Response = await _iSender.Send(new SaveNoteCommand(model));
                        _iCustomLogger.LogwriteInfo("after SaveNoteCommand execute. and the result for non financial-" + JsonSerializer.Serialize(Response), loginUserId);
                    }
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Session timeout", _commonlogpath);
                    return Redirect("/");
                }

            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo("exception occur during Create Note page ------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                return Redirect(_createNotePathUrl);
            }
            if (model.NoteState == "Draft" && Response.NoteId != "")
            {
                _iCustomLogger.LogwriteInfo("Note Save as draft from create note page and NoteState=Draft -------", loginUserId);
                TempData["msg"] = "Note Successfully Save as Draft";
                return Redirect("/note/create?s=" + _iEncryption.AesEncrypt(Response.NoteId));
            }
            else if (model.NoteStatus == "Publish" && Response.NoteId != "")
            {
                _iCustomLogger.LogwriteInfo("Note Save as Publish from create note page and NoteStatus=Publish -------", loginUserId);
                TempData["msg"] = "Note Successfully Created and Published";
                return Redirect("_createNotePathUrl");
            }
            else if (model.NoteState == "Publish" && Response.NoteId != "")
            {
                _iCustomLogger.LogwriteInfo("Note created successdully -------", loginUserId);
                TempData["msg"] = "Note Successfully Created";
                return Redirect("/Note/CreatorDashboard");
            }
            else
            {
                _iCustomLogger.LogwriteInfo("Note not note created and NoteStatus= '' -------", loginUserId);
                TempData["msg"] = "Something went wrong. Please try after some time";
                return Redirect(_createNotePathUrl);
            }
        }

        [HttpGet]
        public async ValueTask<IActionResult> AmendNote()
        {
            AmendmentNoteModel notempdel = new AmendmentNoteModel();
            try
            {
                TempData["encdata"] = DateTime.Now.ToString("yyyyMMddHHmmssff");
                TempData.Keep("encdata");
                //NoteModel Request = new();
                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId == "")
                {
                    _iCustomLogger.LogwriteInfo("Session timeout", _commonlogpath);
                    return Redirect("/");
                }
                _iCustomLogger.LogwriteInfo("user exists for user id------ " + userId, loginUserId);
                if (HttpContext.Request.Query["q"].ToString() != "")
                {
                    string NoteId = _iEncryption.AesDecrypt(HttpContext.Request.Query["q"].ToString());
                    //DraftNoteModel Response1 = await _iSender.Send(new FetchSaveNoteDataCommand(Request));
                    NoteAmendmentModel Response = await _iSender.Send(new FetchAmendmentCommand(NoteId));
                    _iCustomLogger.LogwriteInfo("FetchSaveNoteDataCommand return value------ note title-- " + Response.noteModel.NoteTitle, loginUserId);
                    notempdel.NoteStatus = Response.noteModel.NoteStatus;
                    notempdel.ApproverIdList = "";
                    notempdel.NoteId = _iEncryption.AesEncrypt(Response.noteModel.NoteId);
                    notempdel.NoteTitle = Response.noteModel.NoteTitle;
                    notempdel.CategoryId = Response.noteModel.CategoryId;
                    notempdel.NoteBody = Response.noteModel.NoteBody;
                    notempdel.NoteState = Response.noteModel.NoteState.ToString();
                    notempdel.NatureOfExpensesId = Response.noteModel.NatureOfExpensesId;
                    notempdel.ExpenseIncurredAtId = Response.noteModel.ExpenseIncurredAtId;
                    notempdel.CapitalExpenditure = Response.noteModel.CapitalExpenditure ?? "";
                    notempdel.OperationalExpenditure = Response.noteModel.OperationalExpenditure ?? "";
                    notempdel.TotalAmount = Response.noteModel.TotalAmount;
                    notempdel.RecomendedApproverIdList = "";
                    notempdel.CategoryName=Response.noteModel.CategoryName;
                    notempdel.ExpenseIncurredAtName = Response.noteModel.ExpenseIncurredAtName;
                    // Update the NatureOfExpensesName property
                    notempdel.NatureOfExpensesName = $"{(!string.IsNullOrWhiteSpace(Response.noteModel.NatureOfExpenseCode) ? $"{Response.noteModel.NatureOfExpenseCode}) " : "")}{Response.noteModel.NatureOfExpensesName}";
                    
                    if (Response.approverDetails != null)
                    {
                        int i = 1;
                        int j = 1;
                        IEnumerable<UserMasterModel> users = Response.approverDetails;
                        StringBuilder sb = new();
                        StringBuilder recomendSb = new();
                        foreach (var dr in users)
                        {
                            if (dr.ApproverType != "R")
                            {
                                if (i == 1)
                                {
                                    notempdel.ApproverIdList = dr.UserId.ToString();
                                }
                                else
                                {
                                    sb.Append("," + dr.UserId);
                                }
                                i++;
                            }
                            else
                            {
                                if (j == 1)
                                {
                                    notempdel.RecomendedApproverIdList = dr.UserId.ToString();
                                }
                                else
                                {
                                    recomendSb.Append("," + dr.UserId);
                                }
                                j++;
                            }
                        }
                        notempdel.ApproverIdList = string.Concat(notempdel.ApproverIdList, sb.ToString());
                        notempdel.RecomendedApproverIdList = string.Concat(notempdel.RecomendedApproverIdList, recomendSb.ToString());
                    }
                    ViewBag.ApproverList = Response.approverDetails!.Where(user => user.ApproverType != "R").ToList();
                    ViewBag.RecomendedApproverList = Response.approverDetails!.Where(user => user.ApproverType == "R").ToList();
                    //notempdel.AttachmentList =(IList<CommonAttachmentModel>)Response.attachementDetails.ToList();

                    foreach (var dr in Response.attachementDetails)
                    {
                        CommonAttachmentModel obj = new CommonAttachmentModel();
                        obj.AttachmentId = dr.AttachmentId;
                        obj.DocumentName = dr.DocumentName;
                        obj.AttachmentPath = dr.AttachmentPath;
                        notempdel.AttachmentList.Add(obj);
                    }
                    notempdel.AttachmentListJson = notempdel.AttachmentList?.Count > 0 ? JsonSerializer.Serialize(notempdel.AttachmentList) : "";
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("No template selected for the user------ " + userId, loginUserId);
                    notempdel.NoteStatus = "";
                    notempdel.ApproverIdList = "";
                    notempdel.NoteId = "";
                    notempdel.TotalAmount = "0";
                }
                _iCustomLogger.LogwriteInfo("before calling CreateNoteCommand ----- ", loginUserId);
                NoteModel Request = new();
                Request.NoteId = _iEncryption.AesDecrypt(HttpContext.Request.Query["q"].ToString());
                IEnumerable<CategoryRespModel> ResponseCategory = await _iSender.Send(new CreateNoteCommand(Request));
                _iCustomLogger.LogwriteInfo("CreateNoteCommand return value - " + JsonSerializer.Serialize(ResponseCategory), loginUserId);
                List<SelectListItem> li = [];
                foreach (var item in ResponseCategory)
                {
                    li.Add(new SelectListItem { Text = item.CategoryName.ToString(), Value = item.CategoryId.ToString() });
                }
                ViewBag.CategoryList = new SelectList(li, "Value", "Text");
                notempdel.UserId = userId;


            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo("exception occur during new create page load------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
            }
            _iCustomLogger.LogwriteInfo("Before sending the data to create note view. The data --- " + JsonSerializer.Serialize(notempdel), loginUserId);
            return View(notempdel);
        }

        [HttpPost]
        public async ValueTask<IActionResult> AmendNote(AmendmentNoteModel model)
        {
            AmendmentNoteModel Response = new();
            try
            {                

                #region model state is not valid

                if (!ModelState.IsValid)
                {
                    var validationContext = new ValidationContext(model)
                    {
                        MemberName = nameof(model.NoteBody)
                    };

                    var validationResults = new List<ValidationResult>();
                    if (!Validator.TryValidateProperty(model.NoteBody, validationContext, validationResults))
                    {
                        _iCustomLogger.LogwriteInfo("form post is not valid for invalid note body", loginUserId);
                        TempData["errormsg"] = "Please fill the proper detail in note body";
                    }
                    else
                    {
                        _iCustomLogger.LogwriteInfo("form post is not valid for create note page", loginUserId);
                        TempData["errormsg"] = "Please fill the required details";
                    }
                    return Redirect("/Note/AmendNote" + "?q=" + model.NoteId);
                }

                #endregion

                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    #region check file extension

                    if (!await _iSender.Send(new FileExtensionValidationCommand(model)))
                    {
                        _iCustomLogger.LogwriteInfo("file type not supported", loginUserId);
                        TempData["errormsg"] = "Opps! File type not supported by this system.";
                        return Redirect("/Note/AmendNote" + "?q=" + model.NoteId);
                    }

                    #endregion

                    model.UserId = userId;

                    #region encdata is null

                    if (TempData["encdata"] == null)
                    {
                        return Redirect("/");
                    }

                    #endregion

                    #region decrypt approver and recomended approver

                    model.ApproverIdList = _encryption.DecryptStringAES(model.ApproverIdList, TempData["encdata"]?.ToString() ?? "");
                    if (model.RecomendedApproverIdList != null)
                    {
                        model.RecomendedApproverIdList = _encryption.DecryptStringAES(model.RecomendedApproverIdList, TempData["encdata"]?.ToString() ?? "");
                    }
                    if (model.ApproverIdList == "keyError")
                    {
                        _iCustomLogger.LogwriteInfo("Exception On Approver Descryption:", loginUserId);
                        return Redirect("/Note/AmendNote" + "?q=" + model.NoteId);
                    }
                    if (model.RecomendedApproverIdList != "" && model.RecomendedApproverIdList == "keyError")
                    {
                        _iCustomLogger.LogwriteInfo("Exception On Recomended Approver Descryption:", loginUserId);
                        return Redirect("/Note/AmendNote" + "?q=" + model.NoteId);
                    }

                    #endregion

                    #region Reset Previous Note

                    bool IsChange = await _iSender.Send(new ResetPreviousNoteDetailsCommand(model.NoteId));
                    if (IsChange)
                    {
                        _iCustomLogger.LogwriteInfo("before saving Amendment data update the related table data successfully", loginUserId);
                    }

                    #endregion

                    model.IsAmend = true;

                    if (model.CategoryId == "1")
                    {
                        if (model.CapitalExpenditure != "" && model.OperationalExpenditure != "" && model.ExpenseIncurredAtId != "" && model.NatureOfExpensesId != "")
                        {
                            _iCustomLogger.LogwriteInfo("before UpdateAmendmentNoteCommand execution for financial-------", loginUserId);

                            var response = await _iSender.Send(new UpdateAmendmentNoteCommand(model));
                            if (response.ResponseStatus.ResponseCode == StatusCodes.Status200OK)
                            {
                                Response = response.Data;
                                _iCustomLogger.LogwriteInfo("after UpdateAmendmentNoteCommand execute. and the result for financial-" + JsonSerializer.Serialize(Response), loginUserId);
                            }
                        }
                        else
                        {
                            TempData["errormsg"] = "Fields are mandetory";
                            return Redirect("/Note/AmendNote" + "?q=" + model.NoteId);
                        }
                    }
                    else if (model.CategoryId == "2")
                    {
                        _iCustomLogger.LogwriteInfo("before UpdateAmendmentNoteCommand execution for non financial-------", loginUserId);

                        var response = await _iSender.Send(new UpdateAmendmentNoteCommand(model));

                        if (response.ResponseStatus.ResponseCode == StatusCodes.Status200OK)
                        {
                            Response = response.Data;
                            _iCustomLogger.LogwriteInfo("after UpdateAmendmentNoteCommand execute. and the result for non-financial-" + JsonSerializer.Serialize(Response), loginUserId);

                        }
                    }
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Session timeout", _commonlogpath);
                    return Redirect("/");
                }


                if (model.NoteState == "Publish" && Response.NoteId != "")
                {
                    _iCustomLogger.LogwriteInfo("Note amend successfully -------", loginUserId);
                    TempData["msg"] = "Note Successfully amended";
                    return Redirect("/Note/CreatorDashboard");
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Note amendment not successfully created and NoteStatus= '' -------", loginUserId);
                    TempData["msg"] = "Something went wrong. Please try after some time";
                    return Redirect("/Note/AmendNote" + "?q=" + model.NoteId);
                }


            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo("exception occur during Create Note page ------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                return Redirect("/Note/AmendNote" + "?q=" + model.NoteId);
            }
        }

        #region ReviewerOrApprover Section

        [HttpPost]
        public async ValueTask<IActionResult> InsertReviewerApprover(string approverReviewerId, string noteId, string prefixSuffix, string addedBy, string visibilityMode)
        {
            int NoteId = Convert.ToInt32(_iEncryption.AesDecrypt(noteId));
            AppproverReviewerRequestModelDto model = new()
            {
                AddedBy = Convert.ToInt32(addedBy),
                ApproverReviewerId = Convert.ToInt32(_iEncryption.AesDecrypt(approverReviewerId)),
                NoteId = NoteId,
                PrefixSuffixValue = Convert.ToInt32(prefixSuffix),
                VisibilityMode = Convert.ToInt32(visibilityMode)
            };

            InsertReviewerApproverCommand command = new(model);

            var result = await _iSender.Send(command);
            return Json(new { result });
        }

        [HttpGet]
        public async ValueTask<IActionResult> FetchReviewerOrApproverList(string searchKey, string noteId)
        {
            var list = new List<KeyValuePair<string, string>>();
            var userData = haccess.HttpContext?.User.FindFirstValue("UserId");
            int userId = userData != "" ? Convert.ToInt32(userData) : 0;
            int NoteId = Convert.ToInt32(_iEncryption.AesDecrypt(noteId));


            FetchReviewerOrApproverListCommand command = new(searchKey, userId, NoteId);
            var result = await _iSender.Send(command);

            foreach (var item in result)
            {
                string strvalue = item.FirstName +
                                  (item.MiddleName != "" ? " " + item.MiddleName : "") + " " +
                                  item.LastName + " " + " | " + item.Designation +
                                  (!string.IsNullOrEmpty(item.Grade)
                                      ? " | " + item.Grade
                                      : "") + (!string.IsNullOrEmpty(item.Department)
                                      ? " | " + item.Department
                                      : "");
                list.Add(new KeyValuePair<string, string>(strvalue, _iEncryption.AesEncrypt(item.UserId.ToString())));
            }

            return Json(list);
        }

        #endregion

        [HttpGet]
        public async ValueTask<IActionResult> ApprovedNote()
        {
            try
            {
                string noteid = string.Empty;
                string notetype = string.Empty;
                if (HttpContext.Request.Query["p"].ToString() != "")
                {
                    noteid = _iEncryption.AesDecrypt(HttpContext.Request.Query["p"].ToString());
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("note id not found in ApprovedNote note page ------ ", loginUserId);
                    return Redirect(_dashboardUrl);
                }

                string userId = User.FindFirstValue("UserId") ?? "";
                if (userId != "")
                {
                    _iCustomLogger.LogwriteInfo("ApprovedNote page get method initiated------", loginUserId);
                    NoteModel Request = new() { NoteId = noteid, notetype = notetype };
                    MyApprovedNoteModel Response = await _iSender.Send(new FetchApprovedNoteCommand(Request));
                    Response.approverModel = Response.approverModel.Select(item => { item.UserId = _iEncryption.AesEncrypt(item.UserId.ToString()); return item; }).ToList();
                    Response.recomendedapproverModel = Response.recomendedapproverModel.Select(item => { item.UserId = _iEncryption.AesEncrypt(item.UserId.ToString()); return item; }).ToList();
                    return View(Response);
                }
                else
                {
                    _iCustomLogger.LogwriteInfo("Session timeout in MyApprovedNote", _commonlogpath);
                    HttpContext.Session.SetString("RedirectFromWithdraw", "/Withdraw?p=" + noteid);
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo("exception occur during withdraw get method ------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                return Redirect("/");
            }
        }
    }
}