using DNAS.Application.Common.Interface;
using DNAS.Application.IService;
using DNAS.Domian.DTO.MailSend;

using Microsoft.AspNetCore.Http;

using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace DNAS.Shared.Service
{
    internal class MailService(ICustomLogger logger, IHttpContextAccessor haccess) : IMailService
    {
        public readonly ICustomLogger _logger = logger;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<bool> EmailSend(MailSender Request)
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

                    _logger.LogwriteInfo("Exception Details Log: " + DateTime.Now.ToString() + Environment.NewLine +
                           "Message :" + errmsg + Environment.NewLine +
                           "StackTrace :" + errstack + Environment.NewLine, string.IsNullOrEmpty(haccess.HttpContext?.User.FindFirstValue("UserId")) ? "Login" : loginUserId);

                    msg = false;
                }

            }
            catch (Exception e)
            {
                string errmsg = e.InnerException == null ? e.Message : e.InnerException.Message;
                string errstack = e.InnerException?.StackTrace ?? "";
                _logger.LogwriteInfo("Exception Details Log: " + DateTime.Now.ToString() + Environment.NewLine +
                                "Message :" + errmsg + Environment.NewLine +
                                "StackTrace :" + errstack + Environment.NewLine, string.IsNullOrEmpty(haccess.HttpContext?.User.FindFirstValue("UserId")) ? "Login" : loginUserId);
                msg = false;
            }
            return msg;
        }
    }
}
