using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Application.IRepository;
using DNAS.Domain.DTO.Approver;
using DNAS.Domain.DTO.Login;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.Approver;
using DNAS.Persistence.DataAccessContents;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

namespace DNAS.Persistence.Repository
{
    internal class Delete(DataContext dataContext, ICustomLogger logger, IDapperFactory iDapperFactory, IHttpContextAccessor haccess) : IDelete
    {
        private readonly DataContext _dbContext = dataContext;
        private readonly ICustomLogger _logger = logger;
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        private readonly string logfile = "Login";
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<string> DeleteApproverData(DNAS.Domian.DTO.Note.ApproverModel Request)
        {
            string str = "";
            try
            {
                Approver approver = new();
                approver.NoteId = Convert.ToInt32(Request.NoteId);
                approver.UserId = Convert.ToInt32(Request.UserId);
                approver.IsApproved = true;
                approver.ApproverId = Convert.ToInt32(Request.ApproverId);
                _dbContext.Approvers.Remove(approver);
                var result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    str = "success";
                }
                else
                {
                    str = "fail";
                }
            }
            catch (Exception ex)
            {
                str = "fail";
                _logger.LogwriteInfo("exception occur during DeleteApproverData execution" +
                Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine + ex.StackTrace, loginUserId ?? logfile);
            }
            return str;
        }
        public async ValueTask<string> DeleteApproverByNoteId(ApproverForDraftModel Request)
        {
            string str = "";
            try
            {
                foreach (var item in Request.approverForDraft)
                {
                    Approver approver = new();
                    approver.NoteId = Convert.ToInt32(item.NoteId);
                    approver.ApproverId = Convert.ToInt32(item.ApproverId);
                    approver.NoteId = Convert.ToInt32(item.NoteId);
                    _dbContext.Approvers.Remove(approver);
                }
                var result = await _dbContext.SaveChangesAsync();
                if (result > 0 || !Request.approverForDraft.Any())
                {
                    str = "success";
                }
                else
                {
                    str = "Failed";
                }
            }
            catch (Exception e)
            {
                str = "Failed";
                _logger.LogwriteInfo("Exception occur during the DeleteApproverByNoteId----" + e.Message + Environment.NewLine + e.StackTrace, loginUserId ?? logfile);
            }
            return str;
        }
        public async ValueTask<bool> DeleteUserTracking(object inparam)
        {
            try
            {
                UserTrackingData Response = await _iDapperFactory.ExecuteSpDapperAsync<UserTrackingModel, UserTrackingData>
                    (SpName: OraStoredProcedureNames.ProcDeleteUserTracking, inparam);
                if (Response.UserTracking != null)
                {
                    _logger.LogwriteInfo("UserTracking delete success------ " + Environment.NewLine, logfile);
                    return true;
                }
                else
                {
                    _logger.LogwriteInfo("UserTracking delete failed------ " + Environment.NewLine, logfile);
                    return false;
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during DeleteUserTracking------ " + e.Message + Environment.NewLine + e.StackTrace, logfile);
                return false;
            }

        }
        public async ValueTask<bool> DeleteDuplicateApprover(object inparam)
        {
            try
            {
                await _iDapperFactory.ExecuteSpDapperAsync<FetchApproverForCheckApprover, FetchApproverModel>(
                        SpName: OraStoredProcedureNames.DeleteDuplicateApprover, Params: inparam);

                _logger.LogwriteInfo("Delete Duplicate Approver------ " + Environment.NewLine, logfile);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during DeleteDuplicateApprover------ " + e.Message + Environment.NewLine + e.StackTrace, logfile);
                return false;
            }

        }

        public async ValueTask<int> DeleteAttachment(int NoteId, int AttachmentId)
        {
            int response = 0;
            try
            {
                var data = await _dbContext.Attachments.Where(m => m.AttachmentId == AttachmentId && m.NoteId == NoteId).FirstOrDefaultAsync();

                if (data is not null)
                {
                    _dbContext.Remove(data);
                    await _dbContext.SaveChangesAsync();
                    response = 1;
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo($"exception occur during DeleteAttachment {e.Message}{Environment.NewLine}{e.StackTrace}", logfile);
                response = -1;

            }
            return response;
        }
        public async ValueTask<bool> DeleteAttachmentByNoteId(string noteid)
        {
            try
            {
                int executeresult=await _iDapperFactory.ExecuteSpDapperAsync(SpName: OraStoredProcedureNames.DeleteAttachmentByNoteId, Params: new {NoteId= noteid });
                if (executeresult > 0)
                {
                    _logger.LogwriteInfo("Delete Attachment done" + Environment.NewLine, logfile);
                    return true;
                }
                else
                {
                    _logger.LogwriteInfo("Delete Attachment failed" + Environment.NewLine, logfile);
                    return false;
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during DeleteDuplicateApprover------ " + e.Message + Environment.NewLine + e.StackTrace, logfile);
                return false;
            }

        }
    }
}
