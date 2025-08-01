using ClosedXML.Excel;
using DNAS.Application.Common.Interface;
using DNAS.Application.Features.Login;
using DNAS.Application.Features.Template;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.Template;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DNAS.Application.Common.Filter;
using System.Reflection;
using System.Security.Claims;

namespace DNAS.WEB.Controllers
{
    [Authorize]
    [TypeFilter(typeof(UserCurrentAuth))]
    public class TemplateController(ISender iSender, ICustomLogger logger, IEncryption enc, IHttpContextAccessor haccess) : Controller
    {
        private readonly ISender _iSender = iSender;
        private readonly ICustomLogger _logger = logger;
        public readonly IEncryption _encryption = enc;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public IActionResult Index()
        {
            return View();
        }

        
        public async ValueTask<IActionResult> Edit()
        {
            try
            {
                var userId = User.FindFirstValue("UserId");
                if (userId != null)
                {
                    if (HttpContext.Request.Query["t"].ToString() != null)
                    {
                        TemplateModel Request = new();
                        Request.TemplateId = _encryption.AesDecrypt(HttpContext.Request.Query["t"].ToString());
                        Request.UserId = userId;
                        TemplateModel Response = await _iSender.Send(new FetchTemplateForViewCommand(Request));
                        return View(Response);
                    }
                    else
                    {
                        _logger.LogwriteInfo("template id is null or empty------ ", loginUserId);
                        return Redirect("/Template/Library");
                    }
                }
                else
                {
                    _logger.LogwriteInfo("Session expired from view template------ ", "Login");
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("Exception occur during Template Library------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                return Redirect("/");
            }
        }

        [HttpGet]
        public async ValueTask<IActionResult> Library(TemplateModelData Request)
        {
            try
            {
                Request.FilterTemplates.UserId = Convert.ToInt32(User.FindFirstValue("UserId") ?? "");
                if (Request.FilterTemplates.UserId == 0)
                {
                    _logger.LogwriteInfo("Session timeout", "Login");
                    return Redirect("/");
                }
                TemplateLibraryCommand Command = new()
                {
                    InputModel = new FilterTemplateModel
                    {
                        UserId = Request.FilterTemplates.UserId,
                        StartDate = Request.FilterTemplates.StartDate,
                        EndDate = Request.FilterTemplates.EndDate,
                        Category = Request.FilterTemplates.Category
                    }
                };
                CommonResponse<TemplateModelData> Response = await _iSender.Send(Command);
                return View(Response.Data);
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during Template Library------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                return View(new TemplateModel());
            }
        }

        [HttpPost]
        public async ValueTask<IActionResult> DownloadLibrary([FromBody] TemplateModelData Request)
        {
            byte[] content = [];
            await Task.Run(() =>
            {
                using XLWorkbook workbook = new();
                IXLWorksheet worksheet = workbook.Worksheets.Add("Report");

                int ColumnIndex = 0;
                PropertyInfo[] properties = typeof(TemplateModel).GetProperties();
                string[] RequiredColumns = ["TemplateName", "CategoryName", "DateOfCreation"];
                for (int i = 0; i < properties.Length; i++)
                {
                    if (RequiredColumns.Contains(properties[i].Name))
                    {
                        if (properties[i].Name == "TemplateName") { worksheet.Cell(1, ColumnIndex + 1).Value = "Template Name"; }
                        if (properties[i].Name == "CategoryName") { worksheet.Cell(1, ColumnIndex + 1).Value = "Category"; }
                        if (properties[i].Name == "DateOfCreation") { worksheet.Cell(1, ColumnIndex + 1).Value = "Create Date"; }
                        IXLCell cell = worksheet.Cell(1, ColumnIndex + 1);
                        cell.Style.Font.Bold = true;
                        worksheet.Cell(1, ColumnIndex + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ColumnIndex++;
                    }
                }

                int row = 2;
                foreach (TemplateModel item in Request.TemplateList)
                {
                    ColumnIndex = 0;
                    for (int i = 0; i < properties.Length; i++)
                    {
                        if (RequiredColumns.Contains(properties[i].Name))
                        {
                            worksheet.Cell(row, ColumnIndex + 1).Value = item.GetType().GetProperty(properties[i].Name)?.GetValue(item)?.ToString();
                            ColumnIndex++;
                        }
                    }
                    row++;
                }
                using MemoryStream stream = new();
                workbook.SaveAs(stream);
                content = stream.ToArray();
            });
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "dataReport.xlsx");
        }

        public async ValueTask<IActionResult> Views()
        {
            try
            {
                var userId = User.FindFirstValue("UserId");
                if (userId != null)
                {
                    if (HttpContext.Request.Query["t"].ToString() != null)
                    {
                        TemplateModel Request = new();
                        Request.TemplateId = _encryption.AesDecrypt(HttpContext.Request.Query["t"].ToString());
                        Request.UserId = userId;
                        TemplateModel Response = await _iSender.Send(new FetchTemplateForViewCommand(Request));
                        return View(Response);
                    }
                    else
                    {
                        _logger.LogwriteInfo("template id is null or empty------ ", loginUserId);
                        return Redirect("/Template/Library");
                    }
                }
                else
                {
                    _logger.LogwriteInfo("Session expired from view template------ ", "Login");
                    return Redirect("/");
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("Exception occur during Template Library------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                return Redirect("/");
            }

        }
        [HttpPost]
        public async Task<ActionResult> EditTemplate(DNAS.Domian.DTO.Template.TemplateModel templateModel, string userid, string tempname, string tempid, string notebody)
        {
            try
            {
                TemplateModel model = new();
                model.TemplateName = templateModel.TemplateName;
                model.TemplateBody = templateModel.TemplateBody;
                model.DateOfCreation = DateTime.Now;
                model.UserId = templateModel.UserId;
                model.TemplateId =_encryption.AesDecrypt(templateModel.TemplateId);
                if (await _iSender.Send(new UpdateTemplateCommand(model)))
                {
                    _logger.LogwriteInfo("Template update  command complete status: true------ ", loginUserId);
                    TempData["msg"] = "The template has been update successfully.";
                }
                else
                {
                    _logger.LogwriteInfo("Template update  command complete status: false------ ", loginUserId); 
                    TempData["msg"] = "The template update failed.";
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("Exception occur during edit template------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
            }
            return Redirect("/Template/Library");
        }
        public async Task<JsonResult> DeleteTemplate(string idd)
        {
            string str = "";
            try
            {
                TemplateModel model = new();
                model.TemplateId = _encryption.AesDecrypt(idd);
                model.IsActive = false;
                str = await _iSender.Send(new DeleteTemplateCommand(model));
                _logger.LogwriteInfo("Template delete command complete status:" + str + "------ ", loginUserId);
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("Exception occur during Delete Template------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
            }
            return Json(str);
        }
    }
}
