using DNAS.Application.Business.Interface;
using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Application.IRepository;
using DNAS.Domain.Common;
using DNAS.Domain.DTO.SendBack;
using DNAS.Domian.Common;
using DNAS.Domian.DAO.DbHelperModels.MailConfiguration;
using DNAS.Domian.DTO.Confguration;
using DNAS.Domian.DTO.DelegateAsign;
using DNAS.Domian.DTO.MailConfiguration;
using DNAS.Domian.DTO.MailSend;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace DNAS.Application.Business.Implementation
{
    internal sealed class EmailService(IConfiguration iConfiguration, IDapperFactory iDapperFactory,
        ICustomLogger logger, IHttpContextAccessor haccess, IOptions<AppConfig> appConfig, IEncryption encryption) : IEmailService
    {
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        private readonly IConfiguration _configurationResp = iConfiguration;
        private readonly ICustomLogger _logger = logger;
        private readonly AppConfig _appConfig = appConfig.Value;
        private readonly string _loginUserId = $"user_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        private readonly IEncryption _encryption = encryption;
        public async Task<MailSender> GetMailConfiguration()
        {
            MailSender objMail = new();
            try
            {
                var inparam = new
                {
                    @ConfigurationFor = "SMTPCredential"
                };
                CommonResponse<ConfigurationRespModel> Response = await _configurationResp.FetchConfiguration(inparam);
                if (Response.Data != null)
                {
                    objMail.Sender = Response.Data.configurationResp.First(x => x.ConfigurationKey == "SMTPSender").ConfigurationValue;
                    objMail.SMTPHost = Response.Data.configurationResp.First(x => x.ConfigurationKey == "SMTPHost").ConfigurationValue;
                    objMail.SMTPPort = Convert.ToInt32(Response.Data.configurationResp.First(x => x.ConfigurationKey == "SMTPPort").ConfigurationValue);
                    objMail.SMTPLoginUserID = Response.Data.configurationResp.FirstOrDefault(x => x.ConfigurationKey == "SMTPLoginUserID")?.ConfigurationValue;
                    objMail.SMTPPassword = Response.Data.configurationResp.FirstOrDefault(x => x.ConfigurationKey == "SMTPPassword")?.ConfigurationValue;
                    objMail.SSL = Convert.ToBoolean(Response.Data.configurationResp.First(x => x.ConfigurationKey == "SMTPSSL").ConfigurationValue);
                }
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("Exeption GetMailConfiguration: " + ex.StackTrace!, string.IsNullOrEmpty(haccess.HttpContext?.User.FindFirstValue("UserId")) ? "Login" : _loginUserId);
            }
            return objMail;
        }

        public async Task<DeligateBodySubject> GetMailNoteCreatorMailForDelegate(DeligateMail deligateMail)
        {
            DeligateBodySubject response = new();
            try
            {

                MailConfigModelInparam inparam2 = new()
                {
                    @MailKey = "NoteCreatorMailForDelegate"
                };
                MailConfigModel MailData = await _iDapperFactory.ExecuteSpDapperAsync<MailConfig, MailConfigModel>(OraStoredProcedureNames.ProcFetchMailConfiguration, inparam2);

                string body = MailData.mailConfig.MailBody;
                body = body.Replace(nameof(MailBodyConcentData.Creators_Name), deligateMail.notecreator);
                body = body.Replace(nameof(MailBodyConcentData.Approvers_Name), deligateMail.delegateSender);
                body = body.Replace(nameof(MailBodyConcentData.Note_Name), deligateMail.NoteTitle);
                body = body.Replace(nameof(MailBodyConcentData.Delegates_Name), deligateMail.delegateReceiver);
                body = body.Replace(nameof(MailBodyConcentData.Link), string.Concat(_appConfig.BaseUrl, string.Concat(MailBodyLink.NoteWithdraw, _encryption.AesEncryptForEmail(deligateMail.noteId))));
                response.Body = body;
                response.Subject = MailData.mailConfig.MailSubject;
                _logger.LogwriteInfo("mail body replace done for note creator mail during deligate", _loginUserId);
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("Exeption GetMailNoteCreatorMailForDelegate: " + ex.StackTrace!, _loginUserId);
            }
            return response;
        }

        public async Task<DeligateBodySubject> GetMailNoteCreatorMailForWhome(DeligateMail deligateMail)
        {
            DeligateBodySubject response = new();
            try
            {
                MailConfigModelInparam inparam3 = new()
                {
                    @MailKey = "ToWhomeDelegate"
                };
                MailConfigModel MailData = await _iDapperFactory.ExecuteSpDapperAsync<MailConfig, MailConfigModel>(OraStoredProcedureNames.ProcFetchMailConfiguration, inparam3);

                string body = MailData.mailConfig.MailBody;
                body = body.Replace(nameof(MailBodyConcentData.Delegates_Name), deligateMail.delegateReceiver);
                body = body.Replace(nameof(MailBodyConcentData.Approvers_Name), deligateMail.delegateSender);
                body = body.Replace(nameof(MailBodyConcentData.Note_Name), deligateMail.NoteTitle);
                body = body.Replace(nameof(MailBodyConcentData.Creators_Name), deligateMail.notecreator);
                body = body.Replace(nameof(MailBodyConcentData.Link), string.Concat(_appConfig.BaseUrl, string.Concat(MailBodyLink.ApprovalRequest, deligateMail.noteId)));
                response.Body = body;
                response.Subject = MailData.mailConfig.MailSubject;
                _logger.LogwriteInfo("mail body replace done for note approver mail during deligate", _loginUserId);
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("Exeption GetMailNoteCreatorMailForDelegate: " + ex.StackTrace!, _loginUserId);
            }
            return response;
        }

        public async Task<DeligateBodySubject> GetMailNoteCreatorQueryInitiate(DeligateMail deligateMail)
        {
            DeligateBodySubject response = new();
            try
            {
                MailConfigModelInparam inparam3 = new()
                {
                    @MailKey = "QueryInitiate"
                };
                MailConfigModel MailData = await _iDapperFactory.ExecuteSpDapperAsync<MailConfig, MailConfigModel>(OraStoredProcedureNames.ProcFetchMailConfiguration, inparam3);

                string body = MailData.mailConfig.MailBody;
                body = body.Replace(nameof(MailBodyConcentData.Creators_Name), deligateMail.notecreator);
                body = body.Replace(nameof(MailBodyConcentData.Approvers_Name), deligateMail.noteApprover);
                body = body.Replace(nameof(MailBodyConcentData.Note_Name), deligateMail.NoteTitle);
                body = body.Replace(nameof(MailBodyConcentData.Query_provided_by_Approver), deligateMail.notecomment);
                body = body.Replace(nameof(MailBodyConcentData.Link), string.Concat(_appConfig.BaseUrl, string.Concat(MailBodyLink.NoteWithdraw, _encryption.AesEncryptForEmail(deligateMail.noteId))));
                response.Body = body;
                response.Subject = MailData.mailConfig.MailSubject;
                _logger.LogwriteInfo("mail body replace done to note creator for note query ", _loginUserId);
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("Exeption GetMailNoteCreatorQueryInitiate: " + ex.StackTrace!, _loginUserId);
            }
            return response;
        }
        public async Task<DeligateBodySubject> GetMailNextNoteApprover(DeligateMail deligateMail)
        {
            DeligateBodySubject response = new();
            try
            {
                MailConfigModelInparam inparam3 = new()
                {
                    @MailKey = "NextApprover"
                };
                MailConfigModel MailData = await _iDapperFactory.ExecuteSpDapperAsync<MailConfig, MailConfigModel>(OraStoredProcedureNames.ProcFetchMailConfiguration, inparam3);

                string body = MailData.mailConfig.MailBody;
                body = body.Replace(nameof(MailBodyConcentData.ApproverB_Name), deligateMail.nextApprover);
                body = body.Replace(nameof(MailBodyConcentData.ApproverA_name), deligateMail.noteApprover);
                body = body.Replace(nameof(MailBodyConcentData.Creators_Name), deligateMail.notecreator);
                body = body.Replace(nameof(MailBodyConcentData.Note_Name), deligateMail.NoteTitle);
                body = body.Replace(nameof(MailBodyConcentData.Link), string.Concat(_appConfig.BaseUrl, string.Concat(MailBodyLink.ApprovalRequest, deligateMail.noteId)));
                response.Body = body;
                response.Subject = MailData.mailConfig.MailSubject;
                _logger.LogwriteInfo("mail body replace done for note approver for next note asign", _loginUserId);
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("Exeption GetMailNextNoteApprover: " + ex.StackTrace!, _loginUserId);
            }
            return response;
        }
        public async Task<DeligateBodySubject> GetMailNextNoteApproverAfterSkip(DeligateMail deligateMail)
        {
            DeligateBodySubject response = new();
            try
            {
                MailConfigModelInparam inparam3 = new()
                {
                    @MailKey = "NextApproverForSkip"
                };
                MailConfigModel MailData = await _iDapperFactory.ExecuteSpDapperAsync<MailConfig, MailConfigModel>(OraStoredProcedureNames.ProcFetchMailConfiguration, inparam3);

                string body = MailData.mailConfig.MailBody;
                body = body.Replace(nameof(MailBodyConcentData.ApproverB_Name), deligateMail.nextApprover);
                body = body.Replace(nameof(MailBodyConcentData.Creators_Name), deligateMail.notecreator);
                body = body.Replace(nameof(MailBodyConcentData.Note_Name), deligateMail.NoteTitle);
                body = body.Replace(nameof(MailBodyConcentData.Link), string.Concat(_appConfig.BaseUrl, string.Concat(MailBodyLink.ApprovalRequest, deligateMail.noteId)));
                response.Body = body;
                response.Subject = MailData.mailConfig.MailSubject;
                _logger.LogwriteInfo("mail body replace done for note approver for next note asign in skip the note", _loginUserId);
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("Exeption GetMailNextNoteApprover: " + ex.StackTrace!, _loginUserId);
            }
            return response;
        }

        public async Task<DeligateBodySubject> GetMailApproverApprovedToCreator(DeligateMail deligateMail)
        {
            DeligateBodySubject response = new();
            try
            {
                MailConfigModelInparam inparam3 = new()
                {
                    @MailKey = "NoteApproved"
                };
                MailConfigModel MailData = await _iDapperFactory.ExecuteSpDapperAsync<MailConfig, MailConfigModel>(OraStoredProcedureNames.ProcFetchMailConfiguration, inparam3);

                string body = MailData.mailConfig.MailBody;
                body = body.Replace(nameof(MailBodyConcentData.Approvers_Name), deligateMail.noteApprover);
                body = body.Replace(nameof(MailBodyConcentData.Creators_Name), deligateMail.notecreator);
                body = body.Replace(nameof(MailBodyConcentData.Note_Name), deligateMail.NoteTitle);
                body = body.Replace(nameof(MailBodyConcentData.Link), string.Concat(_appConfig.BaseUrl, string.Concat(MailBodyLink.NoteWithdraw, _encryption.AesEncryptForEmail(deligateMail.noteId))));
                response.Body = body;
                response.Subject = MailData.mailConfig.MailSubject;
                _logger.LogwriteInfo("mail body replace done for previous approver", _loginUserId);
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("Exeption GetMailApproverApprovedToCreator: " + ex.StackTrace!, _loginUserId);
            }
            return response;
        }
        public async Task<DeligateBodySubject> GetMailApproverApprovedToCreatorFinal(DeligateMail deligateMail)
        {
            DeligateBodySubject response = new();
            try
            {
                MailConfigModelInparam inparam3 = new()
                {
                    @MailKey = "NoteApproved"
                };
                MailConfigModel MailData = await _iDapperFactory.ExecuteSpDapperAsync<MailConfig, MailConfigModel>(OraStoredProcedureNames.ProcFetchMailConfiguration, inparam3);

                string body = MailData.mailConfig.MailBody;
                body = body.Replace(nameof(MailBodyConcentData.Approvers_Name), deligateMail.noteApprover);
                body = body.Replace(nameof(MailBodyConcentData.Creators_Name), deligateMail.notecreator);
                body = body.Replace(nameof(MailBodyConcentData.Note_Name), deligateMail.NoteTitle);
                body = body.Replace(nameof(MailBodyConcentData.Link), string.Concat(_appConfig.BaseUrl, string.Concat(MailBodyLink.NoteView, _encryption.AesEncryptForEmail(deligateMail.noteId))));
                response.Body = body;
                response.Subject = MailData.mailConfig.MailSubject;
                _logger.LogwriteInfo("mail body replace done for final approver", _loginUserId);
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("Exeption GetMailApproverApprovedToCreator: " + ex.StackTrace!, _loginUserId);
            }
            return response;
        }
        public async Task<DeligateBodySubject> GetMailNoteQueryReply(DeligateMail deligateMail)
        {
            DeligateBodySubject response = new();
            try
            {
                MailConfigModelInparam inparam3 = new()
                {
                    @MailKey = "ReplyQuery"
                };
                MailConfigModel MailData = await _iDapperFactory.ExecuteSpDapperAsync<MailConfig, MailConfigModel>(OraStoredProcedureNames.ProcFetchMailConfiguration, inparam3);

                string body = MailData.mailConfig.MailBody;
                body = body.Replace(nameof(MailBodyConcentData.Creators_Name), deligateMail.notecreator);
                body = body.Replace(nameof(MailBodyConcentData.Approvers_Name), deligateMail.noteApprover);
                body = body.Replace(nameof(MailBodyConcentData.Note_Name), deligateMail.NoteTitle);
                body = body.Replace(nameof(MailBodyConcentData.Query_provided_by_Approver), deligateMail.notecomment);
                body = body.Replace(nameof(MailBodyConcentData.Link), string.Concat(_appConfig.BaseUrl, string.Concat(MailBodyLink.ApprovalRequest, deligateMail.noteId)));
                response.Body = body;
                response.Subject = MailData.mailConfig.MailSubject;
                _logger.LogwriteInfo("mail body replace done for query reply of note", _loginUserId);
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("Exeption GetMailNoteQueryReply: " + ex.StackTrace!, _loginUserId);
            }
            return response;
        }
        public async Task<DeligateBodySubject> GetMailSendBackToCreator(DeligateMail deligateMail)
        {
            DeligateBodySubject response = new();
            try
            {
                MailConfigModelInparam inparam3 = new()
                {
                    @MailKey = "SendBack"
                };
                MailConfigModel MailData = await _iDapperFactory.ExecuteSpDapperAsync<MailConfig, MailConfigModel>(OraStoredProcedureNames.ProcFetchMailConfiguration, inparam3);

                string body = MailData.mailConfig.MailBody;
                body = body.Replace(nameof(MailBodyConcentData.Creators_Name), deligateMail.notecreator);
                body = body.Replace(nameof(MailBodyConcentData.Approvers_Name), deligateMail.noteApprover);
                body = body.Replace(nameof(MailBodyConcentData.Note_Name), deligateMail.NoteTitle);
                body = body.Replace(nameof(MailBodyConcentData.Comment_provided_by_Approver), deligateMail.notecomment);
                body = body.Replace(nameof(MailBodyConcentData.Link), string.Concat(_appConfig.BaseUrl, string.Concat(MailBodyLink.NoteView, _encryption.AesEncryptForEmail(deligateMail.noteId))));
                response.Body = body;
                response.Subject = MailData.mailConfig.MailSubject;
                _logger.LogwriteInfo("mail body replace done for send back note", _loginUserId);
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("Exeption GetMailSendBackToCreator: " + ex.StackTrace!, _loginUserId);
            }
            return response;
        }
        public async Task<DeligateBodySubject> GetMailNoteFyiToUser(DeligateMail deligateMail)
        {
            DeligateBodySubject response = new();
            try
            {
                MailConfigModelInparam inparam3 = new()
                {
                    @MailKey = "FYI"
                };
                MailConfigModel MailData = await _iDapperFactory.ExecuteSpDapperAsync<MailConfig, MailConfigModel>(OraStoredProcedureNames.ProcFetchMailConfiguration, inparam3);

                string body = MailData.mailConfig.MailBody;
                body = body.Replace(nameof(MailBodyConcentData.Users_Name), deligateMail.FyiReceiver);
                body = body.Replace(nameof(MailBodyConcentData.Approvers_Name), deligateMail.FyiSender);
                body = body.Replace(nameof(MailBodyConcentData.Creators_Name), deligateMail.notecreator);
                body = body.Replace(nameof(MailBodyConcentData.Note_Name), deligateMail.NoteTitle);
                body = body.Replace(nameof(MailBodyConcentData.Link), string.Concat(_appConfig.BaseUrl, string.Concat(MailBodyLink.NoteViewFyi, _encryption.AesEncryptForEmail(deligateMail.noteId))));
                response.Body = body;
                response.Subject = MailData.mailConfig.MailSubject;
                _logger.LogwriteInfo("mail body replace done for FYI user", _loginUserId);
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("Exeption GetMailNoteFyiToUser: " + ex.StackTrace!, _loginUserId);
            }
            return response;
        }
        public async Task<DeligateBodySubject> GetMailToApproverForNoteAsign(DeligateMail deligateMail)
        {
            DeligateBodySubject response = new();
            try
            {
                MailConfigModelInparam inparam3 = new()
                {
                    @MailKey = "NoteAssign"
                };
                MailConfigModel MailData = await _iDapperFactory.ExecuteSpDapperAsync<MailConfig, MailConfigModel>(OraStoredProcedureNames.ProcFetchMailConfiguration, inparam3);

                string body = MailData.mailConfig.MailBody;
                body = body.Replace(nameof(MailBodyConcentData.Creators_Name), deligateMail.notecreator);
                body = body.Replace(nameof(MailBodyConcentData.Approvers_Name), deligateMail.noteApprover);
                body = body.Replace(nameof(MailBodyConcentData.Note_Name), deligateMail.NoteTitle);
                body = body.Replace(nameof(MailBodyConcentData.Link), string.Concat(_appConfig.BaseUrl, string.Concat(MailBodyLink.ApprovalRequest, deligateMail.noteId)));
                response.Body = body;
                response.Subject = MailData.mailConfig.MailSubject;
                _logger.LogwriteInfo("mail body replace done for note asign to approvr", _loginUserId);
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("Exeption GetMailSendBackToCreator: " + ex.StackTrace!, _loginUserId);
            }
            return response;
        }
		public async Task<DeligateBodySubject> GetMailToCreatorDelegate(DeligateMail deligateMail)
		{
			DeligateBodySubject response = new();
			try
			{
				MailConfigModelInparam inparam3 = new()
				{
					@MailKey = "CreatorDelegate"
				};
				MailConfigModel MailData = await _iDapperFactory.ExecuteSpDapperAsync<MailConfig, MailConfigModel>(OraStoredProcedureNames.ProcFetchMailConfiguration, inparam3);

				string body = MailData.mailConfig.MailBody;
				body = body.Replace(nameof(MailBodyConcentData.Creators_Name), deligateMail.notecreator);
				body = body.Replace(nameof(MailBodyConcentData.Users_Name), deligateMail.delegateReceiver);
				body = body.Replace(nameof(MailBodyConcentData.Note_Name), deligateMail.NoteTitle);
				body = body.Replace(nameof(MailBodyConcentData.Creator_Comment), deligateMail.Comment);
				response.Body = body;
				response.Subject = MailData.mailConfig.MailSubject;
				_logger.LogwriteInfo("mail body replace done for note asign to approvr", _loginUserId);
			}
			catch (Exception ex)
			{
				_logger.LogwriteInfo("Exeption GetMailToCreatorDelegate: " + ex.StackTrace!, _loginUserId);
			}
			return response;
		}
		public async Task<DeligateBodySubject> GetMailToSkip(DeligateMail deligateMail)
		{
			DeligateBodySubject response = new();
			try
			{
				MailConfigModelInparam inparam3 = new()
				{
					@MailKey = "SkipApprover"
				};
				MailConfigModel MailData = await _iDapperFactory.ExecuteSpDapperAsync<MailConfig, MailConfigModel>(OraStoredProcedureNames.ProcFetchMailConfiguration, inparam3);

				string body = MailData.mailConfig.MailBody;
				body = body.Replace(nameof(MailBodyConcentData.Creators_Name), deligateMail.notecreator);
				body = body.Replace(nameof(MailBodyConcentData.Users_Name), deligateMail.delegateReceiver);
				body = body.Replace(nameof(MailBodyConcentData.Note_Name), deligateMail.NoteTitle);
				response.Body = body;
				response.Subject = MailData.mailConfig.MailSubject;
				_logger.LogwriteInfo("mail body replace done for note asign to approvr", _loginUserId);
			}
			catch (Exception ex)
			{
				_logger.LogwriteInfo("Exeption GetMailToCreatorDelegate: " + ex.StackTrace!, _loginUserId);
			}
			return response;
		}
		public async Task<MailBodySubject> GetMailConfigurationData(MailModel deligateMail,string MailKey)
		{
			MailBodySubject response = new();
			try
			{
				MailConfigModelInparam inparam3 = new()
				{
					@MailKey = MailKey
				};
				MailConfigModel MailData = await _iDapperFactory.ExecuteSpDapperAsync<MailConfig, MailConfigModel>(OraStoredProcedureNames.ProcFetchMailConfiguration, inparam3);

				string body = MailData.mailConfig.MailBody;
				body = body.Replace(nameof(MailBodyConcentData.Creators_Name), deligateMail.notecreator);
				body = body.Replace(nameof(MailBodyConcentData.Users_Name), deligateMail.delegateReceiver);
				body = body.Replace(nameof(MailBodyConcentData.Note_Name), deligateMail.NoteTitle);
				body = body.Replace(nameof(MailBodyConcentData.Creator_Comment), deligateMail.Comment);
				body = body.Replace(nameof(MailBodyConcentData.All_Approvers), deligateMail.All_Approvers);
				response.Body = body;
				response.Subject = MailData.mailConfig.MailSubject;
				_logger.LogwriteInfo("mail body replace done for note asign to approvr", _loginUserId);
			}
			catch (Exception ex)
			{
				_logger.LogwriteInfo("Exeption GetMailToCreatorDelegate: " + ex.StackTrace!, _loginUserId);
			}
			return response;
		}

        public async Task<DeligateBodySubject> GetMailToApproverForReSubmitNote(ApproverDtlModel approverDtlModel)
        {
            DeligateBodySubject response = new();
            try
            {
                MailConfigModelInparam inparam3 = new()
                {
                    @MailKey = "ReSubmitNote"
                };
                MailConfigModel MailData = await _iDapperFactory.ExecuteSpDapperAsync<MailConfig, MailConfigModel>(OraStoredProcedureNames.ProcFetchMailConfiguration, inparam3);

                string body = MailData.mailConfig.MailBody;
                body = body.Replace(nameof(MailBodyConcentData.Creators_Name), approverDtlModel.creatorDtl.FirstName);
                body = body.Replace(nameof(MailBodyConcentData.Approvers_Name), approverDtlModel.approverDtl.FirstName);
                body = body.Replace(nameof(MailBodyConcentData.Note_Name), approverDtlModel.noteDtl.NoteTitle);
                body = body.Replace(nameof(MailBodyConcentData.Link), string.Concat(_appConfig.BaseUrl, string.Concat(MailBodyLink.ApprovalRequest, _encryption.AesEncrypt(approverDtlModel.noteDtl.NoteId))));
                response.Body = body;
                response.Subject = MailData.mailConfig.MailSubject;
                _logger.LogwriteInfo("mail body replace done for resubmit mail to approver", _loginUserId);
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("Exeption GetMailToApproverForReSubmitNote: " + ex.StackTrace!, _loginUserId);
            }
            return response;
        }
    }
}
