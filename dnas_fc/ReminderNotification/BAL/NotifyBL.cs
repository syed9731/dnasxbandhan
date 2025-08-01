using Azure.Core;

using Microsoft.Extensions.Configuration;

using ReminderNotification.Common;
using ReminderNotification.DAL;
using ReminderNotification.Model;

using System.Data;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;

namespace ReminderNotification.BAL
{
    public class NotifyBL
    {
        private readonly IConfiguration _iconfiguration;
        private readonly string _baseurl;
        private const string constNotification = "Notification";
        public NotifyBL()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                 .AddUserSecrets(Assembly.GetEntryAssembly()!);
            _iconfiguration = builder.Build();
            _baseurl = _iconfiguration.GetSection("AppConfig:BaseURL").Value ?? string.Empty;
        }
        public async Task SendNotificationMail()
        {
            try
            {
                string CompleteUrl = string.Concat(_baseurl, "/Note/ApprovalRequest?p=");
                FetchUsers fetchUsers = new(_iconfiguration);
                NotificationModel datalist = await fetchUsers.GetList();
                MailSender objMail = new();
                if (datalist.EmailConfigs.Any())
                {
                    objMail.Sender = datalist.EmailConfigs.First(x => x.ConfigurationKey == "SMTPSender").ConfigurationValue;
                    objMail.SMTPHost = datalist.EmailConfigs.First(x => x.ConfigurationKey == "SMTPHost").ConfigurationValue;
                    objMail.SMTPPort = Convert.ToInt32(datalist.EmailConfigs.First(x => x.ConfigurationKey == "SMTPPort").ConfigurationValue);
                    objMail.SMTPLoginUserID = datalist.EmailConfigs.FirstOrDefault(x => x.ConfigurationKey == "SMTPLoginUserID")?.ConfigurationValue;
                    objMail.SMTPPassword = datalist.EmailConfigs.FirstOrDefault(x => x.ConfigurationKey == "SMTPPassword")?.ConfigurationValue;
                    objMail.SSL = Convert.ToBoolean(datalist.EmailConfigs.First(x => x.ConfigurationKey == "SMTPSSL").ConfigurationValue);
                }

                foreach (NotificationUserDetail item in datalist.NotificationUserDetails)
                {
                    objMail.Subject = datalist.MailBodyContents.MailSubject;
                    objMail.Body = datalist.MailBodyContents.MailBody;
                    SaveNotificationModel notificationModel = new()
                    {
                        Message = $"A gentle reminder that the note titled: {item.NoteTitle} is pending in your approval queue. This note was created by {item.CreatorName}. Please review and take appropriate action at your earliest convenience.",
                        NoteId =Convert.ToInt64(item.NoteId),
                        Heading = "Note Approval Reminder",
                        ReceiverUserId = Convert.ToInt32(item.ApproverUserId),
                        Action = "PendingApproval",
                    };
                    SaveNotification saveNotification = new(_iconfiguration);
                    bool result = await saveNotification.NotificationSave(notificationModel);
                    Console.WriteLine(result ? "Notification for portal saved successfully" : "Notification for portal failed to save");
                    SchedulerLogger.LogwriteInfo(result ? "Notification for portal saved successfully" : "Notification for portal failed to save", constNotification);

                    SchedulerLogger.LogwriteInfo("Mail To--" + objMail.Receiver, constNotification);
                    objMail.Subject = objMail.Subject.Replace("Note_Name", item.NoteTitle);
                    //objMail.Body = objMail.Body.Replace("Aging", item.Aging);
                    objMail.Body = objMail.Body.Replace("Approvers_Name", item.ApproverName);
                    objMail.Body = objMail.Body.Replace("Creators_Name", item.CreatorName);
                    objMail.Body = objMail.Body.Replace("Note_Name", item.NoteTitle);
                    objMail.Body = objMail.Body.Replace("Link", string.Concat(CompleteUrl, AesEncrypt(item.NoteId)));
                    SchedulerLogger.LogwriteInfo("Mail Body --" + objMail.Body, constNotification);
                    objMail.Receiver = item.ApproverEmail;
                    objMail.CC = item.CreatorEmail;
                    SchedulerLogger.LogwriteInfo("Check before sending mail", constNotification);
                    bool isMailSend = await EmailSend(objMail);
                    if (isMailSend)
                    {
                        Console.WriteLine("Mail send sucessfully to--" + objMail.Receiver);
                        SchedulerLogger.LogwriteInfo("Mail send sucessfully to--" + objMail.Receiver, constNotification);
                    }
                    else
                    {
                        Console.WriteLine("Mail sending failed to--" + objMail.Receiver);
                        SchedulerLogger.LogwriteInfo("Mail sending failed to--" + objMail.Receiver, constNotification);
                    }
                }
            }
            catch (Exception e)
            {
                SchedulerLogger.LogwriteInfo("exception occur during SendNotificationMail method. message is- --" + e.Message + Environment.NewLine + e.StackTrace, constNotification);
            }

        }

        public static async Task<bool> EmailSend(MailSender Request)
        {
            bool msg = false;
            try
            {
                var message = new MailMessage();

                string[] emailIds = Request.Receiver.Split(',');
                foreach (var item in emailIds)
                {
                    message.To.Add(new MailAddress(item));
                }
                string[] ccemails = string.IsNullOrEmpty(Request.CC) ? [] : Request.CC.Split(",");
                foreach (var item in ccemails)
                {
                    message.CC.Add(new MailAddress(item));
                }
                message.From = new MailAddress(Request.Sender);  // replace with valid value
                message.Subject = (Request.Subject.Length == 0) ? "Test" : Request.Subject;
                message.Body = Request.Body;
                message.IsBodyHtml = true;

                if (Request.atchfile != null)
                {
                    foreach (var i in Request.atchfile)
                    {
                        Attachment attach = new(i);
                        message.Attachments.Add(attach);
                    }
                }
                var loginUserID = Request.SMTPLoginUserID ?? "";
                var password = Request.SMTPPassword ?? "";
                using var smtp = new SmtpClient(Request.SMTPHost, Request.SMTPPort) { EnableSsl = true /*Request.SSL*/ };
                if (loginUserID.Trim().Length > 0)
                {
                    var credential = new NetworkCredential
                    {
                        UserName = loginUserID,  // replace with valid value
                        Password = password  // replace with valid value
                    };

                    smtp.Credentials = credential;
                }
                else
                {
                    smtp.UseDefaultCredentials = false;
                }
                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    await smtp.SendMailAsync(message);
                    msg = true;
                }
                catch (Exception e)
                {
                    string errmsg = e.InnerException == null ? e.Message : e.InnerException.Message;
                    string errstack = e.InnerException?.StackTrace ?? "";

                    SchedulerLogger.LogwriteInfo("Exception Details Log: " + DateTime.Now.ToString() + Environment.NewLine +
                          "Message :" + errmsg + Environment.NewLine +
                           "StackTrace :" + errstack + Environment.NewLine, constNotification);

                    msg = false;
                }

            }
            catch (Exception e)
            {
                string errmsg = e.InnerException == null ? e.Message : e.InnerException.Message;
                string errstack = e.InnerException?.StackTrace ?? "";
                SchedulerLogger.LogwriteInfo("Exception Details Log: " + DateTime.Now.ToString() + Environment.NewLine +
                                "Message :" + errmsg + Environment.NewLine +
                                "StackTrace :" + errstack + Environment.NewLine, constNotification);
                msg = false;
            }
            return msg;
        }

        public static string AesEncrypt(string PlainText)
        {
            try
            {
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(PlainText);
                return Convert.ToBase64String(plainTextBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }


    }
}
