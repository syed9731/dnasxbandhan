using ClosedXML.Excel;
using DNAS.Application.Common.Filter;
using DNAS.Application.Features.Notification;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.Draft;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Security.Claims;

namespace DNAS.WEB.Controllers
{
    [Authorize]
    [TypeFilter(typeof(UserCurrentAuth))]
    public class NotificationController(ISender iSender) : Controller
    {
        private readonly ISender _iSender = iSender;

        public async ValueTask<IActionResult> Index(NotificationData Request)
        {
            try
            {
                NotificationsCommand Command = new()
                {
                    InputModel = new FilterNotification
                    {
                        Id = Convert.ToInt32(User.FindFirstValue("UserId") ?? string.Empty),
                        StartDate = Request.FilterNotifications.StartDate,
                        EndDate = Request.FilterNotifications.EndDate,
                        Category = Request.FilterNotifications.Category,
                        Status = Request.FilterNotifications.Status
                    }
                };
                CommonResponse<NotificationData> Response = await _iSender.Send(Command);
                return View(Response.Data);
            }
            catch
            {
                return View(new NotificationData());
            }
        }

        [HttpPost]
        public async Task<IActionResult> DownloadNotification([FromBody] NotificationData Request)
        {
            using XLWorkbook workbook = new();
            IXLWorksheet worksheet = workbook.Worksheets.Add("Report");

            PropertyInfo[] properties = typeof(Notification).GetProperties();
            //string[] RequiredColumns = { "Heading", "NoteTitle", "NotificationTime", "NoteStatus", "CategoryName" };

            //int columnIndex = 0;
            //foreach (PropertyInfo property in properties)
            //{
            //    if (RequiredColumns.Contains(property.Name))
            //    {
            //        if (property.Name == "Heading") { worksheet.Cell(1, columnIndex + 1).Value = "Type"; }
            //        if (property.Name == "NoteTitle") { worksheet.Cell(1, columnIndex + 1).Value = "Note Title"; }
            //        if (property.Name == "CategoryName") { worksheet.Cell(1, columnIndex + 1).Value = "Category"; }
            //        if (property.Name == "NotificationTime") { worksheet.Cell(1, columnIndex + 1).Value = "Time"; }
            //        if (property.Name == "NoteStatus") { worksheet.Cell(1, columnIndex + 1).Value = "Status"; }

            //        IXLCell cell = worksheet.Cell(1, columnIndex + 1);
            //        cell.Style.Font.Bold = true;
            //        cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            //        columnIndex++;
                    
            //    }
            //}

            string[] RequiredColumns = { "Heading", "NoteTitle", "NotificationTime", "NoteStatus", "CategoryName" };

            int columnIndex = 0;
            foreach (PropertyInfo property in properties)
            {
                if (RequiredColumns.Contains(property.Name))
                {
                    string columnName = property.Name switch
                    {
                        "Heading" => "Type",
                        "NoteTitle" => "Note Title",
                        "CategoryName" => "Category",
                        "NotificationTime" => "Time",
                        "NoteStatus" => "Status",
                        _ => string.Empty
                    };

                    worksheet.Cell(1, columnIndex + 1).Value = columnName;

                    IXLCell cell = worksheet.Cell(1, columnIndex + 1);
                    cell.Style.Font.Bold = true;
                    cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    columnIndex++;
                }
            }

            int row = 2;
            foreach (Notification item in Request.NotificationList)
            {
                columnIndex = 0;
                foreach (PropertyInfo property in properties)
                {
                    if (RequiredColumns.Contains(property.Name))
                    {
                        object value = property.GetValue(item);
                        string cellValue = value?.ToString() ?? string.Empty;

                        if (cellValue == "Approved") { cellValue = "Completed"; }
                        if (cellValue == "Pending") { cellValue = "In-Progress"; }
                        if (cellValue == "SendBack") { cellValue = "Send Back"; }
                        if (cellValue == "Withdraw") { cellValue = "Withdraw"; }

                        worksheet.Cell(row, columnIndex + 1).Value = cellValue;
                        columnIndex++;
                    }
                }
                row++;
            }

            using MemoryStream stream = new();
            workbook.SaveAs(stream);
            byte[] content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "dataReport.xlsx");
        }

        public async ValueTask<JsonResult> UpdateNotificationRead(int NotifId)
        {
            UpdateIsReadCommand Command = new() { NotificationId = NotifId };
            CommonResponse<Notification> Response = await _iSender.Send(Command);
            return Json(Response);
        }
    }
}