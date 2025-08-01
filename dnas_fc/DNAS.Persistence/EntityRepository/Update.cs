using Azure;
using Azure.Core;

using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Application.IRepository;
using DNAS.Domain.DAO.DbHelperModels.FetchExpenseIncurredAt;
using DNAS.Domain.DTO.Approver;
using DNAS.Domain.DTO.Login;
using DNAS.Domain.DTO.Note;
using DNAS.Domain.DTO.SendBack;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.Category;
using DNAS.Domian.DTO.CommonResponse;
using DNAS.Domian.DTO.Login;
using DNAS.Domian.DTO.Note;
using DNAS.Domian.DTO.Template;
using DNAS.Persistence.DataAccessContents;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

namespace DNAS.Persistence.Repository
{
	internal class Update(DataContext dataContext, ICustomLogger logger, IHttpContextAccessor haccess, IDapperFactory iDapperFactory, IDelete idelete) : IUpdate
	{
		private readonly DataContext _dbContext = dataContext;
		public readonly ICustomLogger _logger = logger;
		private readonly IDapperFactory _iDapperFactory = iDapperFactory;
		private readonly IDelete _iDelete = idelete;
		private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
		private readonly string _logfilename = "User_";
		public async Task<CommonResponse<CommonResp>> UpdatePassword(UserMasterModel Request)
		{
			CommonResponse<CommonResp> Response = new();
			try
			{
				var result = _dbContext.UserMasters.FirstOrDefault(b => b.UserId == Convert.ToInt32(Request.UserId));
				if (result != null)
				{
					if (result.Password.Equals(Request.Password))
					{
						_logger.LogwriteInfo("Predefined password and change password is same " + Environment.NewLine, _logfilename + Request.UserId);
						Response.ResponseStatus.ResponseCode = 502;
						Response.ResponseStatus.ResponseMessage = "Data not Update due to password and predefine is same";
					}
					else
					{
						result.Password = Request.Password;
						result.LastPassRecoveryTime = DateTime.Now;
						int upresult = await _dbContext.SaveChangesAsync();
						if (upresult > 0)
						{
							_logger.LogwriteInfo("Predefined password update successfully" + Environment.NewLine, _logfilename + Request.UserId);
							Response.ResponseStatus.ResponseCode = 200;
							Response.ResponseStatus.ResponseMessage = "Data Saved";
						}
						else
						{
							_logger.LogwriteInfo("Predefined password not updated" + Environment.NewLine, _logfilename + Request.UserId);
							Response.ResponseStatus.ResponseCode = 500;
							Response.ResponseStatus.ResponseMessage = "Data Not Saved";
						}
					}
				}

			}
			catch (Exception e)
			{
				_logger.LogwriteInfo("exception occur during UpdatePassword execution" + Environment.NewLine + "exception is-----" + e.StackTrace, _logfilename + Request.UserId);
			}
			return Response;
		}
		public async Task<bool> UpdateNoteTitleData(NoteModel Request)
		{
			try
			{
				int upresult = 0;
				DataAccessContents.Note result = _dbContext.Notes.FirstOrDefault(b => b.NoteId == Convert.ToInt32(Request.NoteId)) ?? new();
				if (result.NoteId != 0)
				{
					result.NoteTitle = Request.NoteTitle;
					result.DateOfCreation = DateTime.Now;
					upresult = await _dbContext.SaveChangesAsync();
				}
				if (upresult > 0)
				{
					_logger.LogwriteInfo("update note title successfully" + Environment.NewLine, loginUserId);
					return true;
				}
				else
				{
					_logger.LogwriteInfo("update note title not updated" + Environment.NewLine, loginUserId);
					return false;
				}
			}
			catch (Exception e)
			{
				_logger.LogwriteInfo("exception occur during UpdateNoteTitleData" + Environment.NewLine + "exception is------" + e.StackTrace, loginUserId);
				return false;
			}
		}
		public async Task<bool> UpdateNoteBodyData(NoteModel Request)
		{
			try
			{
				int upresult = 0;
				var result = _dbContext.Notes.FirstOrDefault(b => b.NoteId == Convert.ToInt32(Request.NoteId));
				if (result != null)
				{
					result.NoteBody = Request.NoteBody;
					result.DateOfCreation = DateTime.Now;
					upresult = await _dbContext.SaveChangesAsync();
				}
				if (upresult > 0)
				{
					_logger.LogwriteInfo("update note title successfully" + Environment.NewLine, loginUserId);
					return true;
				}
				else
				{
					_logger.LogwriteInfo("update note title not updated" + Environment.NewLine, loginUserId);
					return false;
				}
			}
			catch (Exception e)
			{
				_logger.LogwriteInfo("exception occur during UpdateNoteBodyData" + Environment.NewLine + "exception is-------" + e.StackTrace, loginUserId);
				return false;
			}
		}
		public async Task<bool> UpdateNoteCategoryData(NoteModel Request)
		{
			try
			{
				int upresult = 0;
				var result = _dbContext.Notes.FirstOrDefault(b => b.NoteId == Convert.ToInt32(Request.NoteId));
				if (result != null)
				{
					result.CategoryId = Convert.ToInt16(Request.CategoryId);
					if (result.CategoryId == 2)
					{
						result.ExpenseIncurredAtId = null;
						result.OperationalExpenditure = null;
						result.CapitalExpenditure = null;
						result.NatureOfExpensesId = null;
						result.TotalAmount = null;
					}
					upresult = await _dbContext.SaveChangesAsync();
				}
				if (upresult > 0)
				{
					_logger.LogwriteInfo("update note Category successfully" + Environment.NewLine, loginUserId);
					return true;
				}
				else
				{
					_logger.LogwriteInfo("update note Category not updated" + Environment.NewLine, loginUserId);
					return false;
				}
			}
			catch (Exception e)
			{
				_logger.LogwriteInfo("exception occur during UpdateNoteCategoryData" + Environment.NewLine + "exception -----" + e.StackTrace, loginUserId);
				return false;
			}
		}
		public async Task<bool> UpdateNoteExpenseIncurredAtData(NoteModel Request)
		{
			try
			{
				int upresult = 0;
				var result = _dbContext.Notes.FirstOrDefault(b => b.NoteId == Convert.ToInt32(Request.NoteId));
				if (result != null)
				{
					result.ExpenseIncurredAtId = Convert.ToInt32(Request.ExpenseIncurredAtId);
					upresult = await _dbContext.SaveChangesAsync();
				}
				if (upresult > 0)
				{
					_logger.LogwriteInfo("update note ExpenseIncurredAt successfully" + Environment.NewLine, loginUserId);
					return true;
				}
				else
				{
					_logger.LogwriteInfo("update note ExpenseIncurredAt not updated" + Environment.NewLine, loginUserId);
					return false;
				}
			}
			catch (Exception e)
			{
				_logger.LogwriteInfo("exception occur during UpdateNoteExpenseIncurredAtData" + Environment.NewLine + "exception-----" + e.StackTrace, loginUserId);
				return false;
			}
		}
		public async Task<bool> UpdateNoteNatureOfExpensesData(NoteModel Request)
		{
			try
			{
				int upresult = 0;
				var result = _dbContext.Notes.FirstOrDefault(b => b.NoteId == Convert.ToInt32(Request.NoteId));
				if (result != null)
				{
					result.NatureOfExpensesId = Convert.ToInt32(Request.NatureOfExpensesId);
					upresult = await _dbContext.SaveChangesAsync();
				}
				if (upresult > 0)
				{
					_logger.LogwriteInfo("update note UpdateNoteNatureOfExpensesEF successfully" + Environment.NewLine, loginUserId);
					return true;
				}
				else
				{
					_logger.LogwriteInfo("update note UpdateNoteNatureOfExpensesEF not updated" + Environment.NewLine, loginUserId);
					return false;
				}
			}
			catch (Exception e)
			{
				_logger.LogwriteInfo("exception occur during UpdateNoteNatureOfExpensesData" + Environment.NewLine + "exception ------" + e.StackTrace, loginUserId);
				return false;
			}
		}
		public async Task<bool> UpdateNoteCapexData(NoteModel Request)
		{
			try
			{
				int upresult = 0;
				var result = _dbContext.Notes.FirstOrDefault(b => b.NoteId == Convert.ToInt32(Request.NoteId));
				if (result != null)
				{
					result.CapitalExpenditure = Convert.ToDecimal(Request.CapitalExpenditure);
					result.TotalAmount = Convert.ToDecimal(Request.TotalAmount);
					upresult = await _dbContext.SaveChangesAsync();
				}
				if (upresult > 0)
				{
					_logger.LogwriteInfo("update note UpdateNoteCapexEF successfully" + Environment.NewLine, loginUserId);
					return true;
				}
				else
				{
					_logger.LogwriteInfo("update note UpdateNoteCapexEF not updated" + Environment.NewLine, loginUserId);
					return false;
				}
			}
			catch (Exception e)
			{
				_logger.LogwriteInfo("exception occur during UpdateNoteCapexData" + Environment.NewLine + "exception -------" + e.StackTrace, loginUserId);
				return false;
			}
		}
		public async Task<bool> UpdateNoteOpexData(NoteModel Request)
		{
			try
			{
				int upresult = 0;
				var result = _dbContext.Notes.FirstOrDefault(b => b.NoteId == Convert.ToInt32(Request.NoteId));
				if (result != null)
				{
					result.OperationalExpenditure = Convert.ToDecimal(Request.OperationalExpenditure);
					result.TotalAmount = Convert.ToDecimal(Request.TotalAmount);
					upresult = await _dbContext.SaveChangesAsync();
				}
				if (upresult > 0)
				{
					_logger.LogwriteInfo("update note UpdateNoteOpexEF successfully" + Environment.NewLine, loginUserId);
					return true;
				}
				else
				{
					_logger.LogwriteInfo("update note UpdateNoteOpexEF not updated" + Environment.NewLine, loginUserId);
					return false;
				}
			}
			catch (Exception e)
			{
				_logger.LogwriteInfo("exception occur during UpdateNoteOpexData" + Environment.NewLine + "exception-------" + e.StackTrace, loginUserId);
				return false;
			}
		}
		public async Task<bool> UpdateTemplateData(TemplateModel Request)
		{
			bool str = false;
			try
			{
				int upresult = 0;
				var result = _dbContext.TemplateMasters.FirstOrDefault(b => b.TemplateId == Convert.ToInt32(Request.TemplateId));
				if (result != null)
				{
					result.TemplateName = Request.TemplateName;
					result.TemplateBody = Request.TemplateBody;
					result.DateOfCreation = Request.DateOfCreation;
					result.TemplateId = Convert.ToInt32(Request.TemplateId);
					upresult = await _dbContext.SaveChangesAsync();
				}
				if (upresult > 0)
				{
					_logger.LogwriteInfo("update Template UpdateTemplateDataEF successfully" + Environment.NewLine, loginUserId);
					str = true;
				}
				else
				{
					_logger.LogwriteInfo("update Template UpdateTemplateDataEF not updated" + Environment.NewLine, loginUserId);
				}
			}
			catch (Exception e)
			{
				_logger.LogwriteInfo("exception occur during UpdateTemplateData" + Environment.NewLine + "exception---------" + e.StackTrace, loginUserId);
			}
			return str;
		}
		public async Task<string> DeleteTemplateData(TemplateModel Request)
		{
			string str = "";
			try
			{
				int upresult = 0;
				var result = _dbContext.TemplateMasters.FirstOrDefault(b => b.TemplateId == Convert.ToInt32(Request.TemplateId));
				if (result != null)
				{
					result.IsActive = Request.IsActive;
					result.TemplateId = Convert.ToInt32(Request.TemplateId);
					upresult = await _dbContext.SaveChangesAsync();
				}
				if (upresult > 0)
				{
					_logger.LogwriteInfo("Delete Template DeleteTemplateDataEF successfully" + Environment.NewLine, loginUserId);
					str = "success";
				}
				else
				{
					_logger.LogwriteInfo("Delete Template DeleteTemplateDataEF not updated" + Environment.NewLine, loginUserId);
					str = "fail";
				}
			}
			catch (Exception e)
			{
				_logger.LogwriteInfo("exception occur during DeleteTemplateData" + Environment.NewLine + "exception is----------" + e.StackTrace, loginUserId);
			}
			return str;
		}
		public async ValueTask<bool> UpdateNoteStatusData(PendingNoteModel Request)
		{
			bool str = false;
			try
			{
				int upresult = 0;
				DNAS.Persistence.DataAccessContents.Note result = await _dbContext.Notes.FirstOrDefaultAsync<DataAccessContents.Note>(b => b.NoteId == Convert.ToInt32(Request.noteModel.NoteId)) ?? new();
				if (result.NoteId != 0)
				{
					NoteTracker noteTracker = new();
					noteTracker.NoteId = Convert.ToInt32(Request.noteModel.NoteId);
					noteTracker.ApproverId = Convert.ToInt32(Request.noteModel.UserId);
					noteTracker.NoteStatus = "Withdraw";
					noteTracker.CommentTime = System.DateTime.Now;
					noteTracker.Comment = Request.querymodel.Comment;
					_logger.LogwriteInfo("before save the Note tracker table for Withdraw", loginUserId);
					await _dbContext.NoteTrackers.AddAsync(noteTracker);
					int trackerresult = await _dbContext.SaveChangesAsync();
					if (trackerresult > 0)
					{
						result.IsActive = Convert.ToBoolean(Request.noteModel.IsActive);
						result.NoteId = Convert.ToInt32(Request.noteModel.NoteId);
						result.NoteStatus = Request.noteModel.NoteStatus;
						result.WithdrawDate = Request.noteModel.WithdrawDate;
						upresult = await _dbContext.SaveChangesAsync();


                        await _iDelete.DeleteAttachmentByNoteId(Request.noteModel.NoteId);
                    }
                    else
                    {
                        _logger.LogwriteInfo("Withdraw data not save in note tracker table", loginUserId);
                    }
                }
                if (upresult > 0)
                {
					_logger.LogwriteInfo("Delete Template UpdateNoteStatusDataEF successfully" + Environment.NewLine, loginUserId);
					str = true;
				}
				else
				{
					_logger.LogwriteInfo("Delete Template UpdateNoteStatusDataEF not updated" + Environment.NewLine, loginUserId);
				}
			}
			catch (Exception e)
			{
				_logger.LogwriteInfo("exception occur during UpdateNoteStatusData" + Environment.NewLine + "exception is:-----" + e.StackTrace, loginUserId);
			}
			return str;
		}
		public async ValueTask<bool> UpdateApproverData(Domian.DTO.Approver.ApproverModel Request)
		{
			bool str = false;
			try
			{
				int upresult = 0;
				Approver result = await _dbContext.Approvers.FirstOrDefaultAsync<DataAccessContents.Approver>(b => b.ApproverId == Convert.ToInt32(Request.ApproverId)) ?? new();
				if (result.NoteId != 0)
				{

					result.IsApproved = Convert.ToBoolean(Request.IsApproved);
					result.IsCurrentApprover = Convert.ToBoolean(Request.IsCurrentApprover);
					result.NoteId = Convert.ToInt64(Request.NoteId);
					result.UserId = Convert.ToInt32(Request.UserId);
					if (Request.ApprovedTime == "ForApprovedUser")
					{
						result.ApprovedTime = DateTime.Now;
					}
					if (Request.ApprovedTime == "UpdateForNextApprover")
					{
						result.AssignTime = DateTime.Now;
					}
					result.ApproverId = Convert.ToInt64(Request.ApproverId);
					upresult = await _dbContext.SaveChangesAsync();
					_logger.LogwriteInfo("Update Approver successfully" + Environment.NewLine, loginUserId);
				}
				if (upresult > 0)
				{
					_logger.LogwriteInfo("Update Approver successfully" + Environment.NewLine, loginUserId);
					str = true;
				}
				else
				{
					_logger.LogwriteInfo("Approver not updated" + Environment.NewLine, loginUserId);
				}
			}
			catch (Exception e)
			{
				_logger.LogwriteInfo("exception occur during UpdateApproverData" + Environment.NewLine + "exception is:____" + e.StackTrace, loginUserId);
			}
			return str;
		}
		public async ValueTask<bool> UpdateNoteData(NoteModel Request)
		{
			try
			{
				int upresult = 0;
				var result = _dbContext.Notes.FirstOrDefault(b => b.NoteId == Convert.ToInt32(Request.NoteId));
				if (result != null)
				{
					result.NoteStatus = Request.NoteStatus;
					//result.MajorRevision = result.MajorRevision.HasValue ? result.MajorRevision.Value + 1 : 1;
					//result.MinorRevision = 0;
					upresult = await _dbContext.SaveChangesAsync();
				}
				if (upresult > 0)
				{
					_logger.LogwriteInfo("update note status successfully" + Environment.NewLine, loginUserId);
					return true;
				}
				else
				{
					_logger.LogwriteInfo("update note status not updated" + Environment.NewLine, loginUserId);
					return false;
				}
			}
			catch (Exception e)
			{
				_logger.LogwriteInfo("exception occur during UpdateNoteData" + Environment.NewLine + "exception_-----" + e.StackTrace, loginUserId);
				return false;
			}
		}
		public async ValueTask<bool> UpdateUserTracking(UserTrackingModel Request)
		{
			try
			{
				var result = _dbContext.UserTrackings.FirstOrDefault(b => b.UserId == Convert.ToInt32(Request.UserId));
				if (result != null)
				{
					result.LastLoginTime = DateTime.Now;
					result.SessionId = Request.SessionId;
					int upresult = await _dbContext.SaveChangesAsync();
					if (upresult > 0)
					{
						_logger.LogwriteInfo("UserTracking update successfully" + Environment.NewLine, _logfilename + Request.UserId);
					}
					else
					{
						_logger.LogwriteInfo("UserTracking not updated" + Environment.NewLine, loginUserId);
					}
					return true;
				}
				else
				{
					return false;
				}

			}
			catch (Exception e)
			{
				_logger.LogwriteInfo("exception occur during UpdateUserTracking execution" + Environment.NewLine + "exception is-----" + e.StackTrace, loginUserId);
				return false;
			}

		}
		public async ValueTask<bool> UpdateLatestLoginTime(int UserId)
		{
			try
			{
				var result = _dbContext.UserMasters.FirstOrDefault(b => b.UserId == Convert.ToInt32(UserId));
				if (result != null)
				{
					result.LastLoginTime = DateTime.Now;
					int upresult = await _dbContext.SaveChangesAsync();
					if (upresult > 0)
					{
						_logger.LogwriteInfo("LatestLoginTime update successfully" + Environment.NewLine, _logfilename + UserId);
					}
					else
					{
						_logger.LogwriteInfo("LatestLoginTime not updated" + Environment.NewLine, loginUserId);
					}
					return true;
				}
				else
				{
					_logger.LogwriteInfo("Data not available in the usermaster table for the userid- " + UserId + Environment.NewLine, loginUserId);
					return false;
				}

			}
			catch (Exception e)
			{
				_logger.LogwriteInfo("exception occur during UpdateLatestLoginTime execution" + Environment.NewLine + "exception is-----" + e.StackTrace, loginUserId);
				return false;
			}
		}
		public async ValueTask<ApproverDtlModel> UpdateSendBackNote(SendBackNoteDto Request)
		{
			ApproverDtlModel updateResulr = new();
            try
			{
				long noteId = Convert.ToInt64(Request.NoteModel!.NoteId);

				//get the existing note details by noteId
				var result = await _dbContext.Notes.Where(m => m.NoteId == noteId).FirstOrDefaultAsync();

				if (result is not null)
				{
					#region update the Note table
					result.NoteTitle = Request.NoteModel.NoteTitle;
					result.NoteBody = Request.NoteModel.NoteBody;
					result.IsActive = true;
					result.NoteStatus = "Pending";
					result.MinorRevision = (result.MinorRevision + 1);

					_dbContext.Update(result);
					#endregion

					#region get only the new attachments from this existing list

					var newAttachmentList = Request.AttachmentList?.Count > 0 ? Request.AttachmentList?.Where(m => m.AttachmentId == 0).Select(m => new DNAS.Persistence.DataAccessContents.Attachment
					{
						AttachmentId = m.AttachmentId,
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

                    #region Update Current Approver Asign Time

                    updateResulr = await _iDapperFactory.ExecuteSpDapperAsync<ApproverDtl, CreatorDtl, NoteDtl, ApproverDtlModel >
						(SpName: OraStoredProcedureNames.UpdateCurrentApproverAsigntime, Params: new { NoteId = noteId});
					if (!string.IsNullOrWhiteSpace(updateResulr.approverDtl.Email))
					{
                        _logger.LogwriteInfo("Current Approver asign time successfully update with note re-publish time" + Environment.NewLine , loginUserId);
                    }
					else
					{
                        _logger.LogwriteInfo("Current Approver asign time faild to update with note re-publish time" + Environment.NewLine, loginUserId);
                    }
                    #endregion
				}
			}
			catch (Exception ex)
			{
				_logger.LogwriteInfo("exception occur during UpdateSendBackNote execution" + Environment.NewLine + "exception is-----" + ex.StackTrace, loginUserId);
				
			}
			return updateResulr;
		}
		public async ValueTask<bool> UpdateForNextApprover(SkippByCreatorModel Request)
		{
			try
			{
				int upresult = 0;
				var result = _dbContext.Approvers.FirstOrDefault(b => b.ApproverId == Convert.ToInt32(Request.nextApprover.ApproverId));
				if (result != null)
				{
					result.IsCurrentApprover = true;
					result.AssignTime = DateTime.Now;
					upresult = await _dbContext.SaveChangesAsync();
				}
				if (upresult > 0)
				{
					_logger.LogwriteInfo("update next approver successfully" + Environment.NewLine, loginUserId);
					return true;
				}
				else
				{
					_logger.LogwriteInfo("update next approver not done" + Environment.NewLine, loginUserId);
					return false;
				}
			}
			catch (Exception e)
			{
				_logger.LogwriteInfo("exception occur during UpdateForNextApprover" + Environment.NewLine + "exception is-------" + e.StackTrace, loginUserId);
				return false;
			}
		}
		public async ValueTask<bool> UpdateRollBackNextApprover(SkippByCreatorModel Request)
		{
			try
			{
				int upresult = 0;
				var result = _dbContext.Approvers.FirstOrDefault(b => b.ApproverId == Convert.ToInt32(Request.nextApprover.ApproverId));
				if (result != null)
				{
					result.IsCurrentApprover = false;
					result.AssignTime = DateTime.Now;
					upresult = await _dbContext.SaveChangesAsync();
				}
				if (upresult > 0)
				{
					_logger.LogwriteInfo("RollBack update for next approver successfully" + Environment.NewLine, loginUserId);
					return true;
				}
				else
				{
					_logger.LogwriteInfo("RollBack update for update next approver not done" + Environment.NewLine, loginUserId);
					return false;
				}
			}
			catch (Exception e)
			{
				_logger.LogwriteInfo("exception occur during UpdateRoalBackNextApprover" + Environment.NewLine + "exception is-------" + e.StackTrace, loginUserId);
				return false;
			}
		}
		public async ValueTask<bool> UpdateCurrentApproverAsSkippedApprover(SkippByCreatorModel Request)
		{
			try
			{
				int upresult = 0;
				var result = _dbContext.Approvers.FirstOrDefault(b => b.ApproverId == Convert.ToInt32(Request.currentApprover.ApproverId));
				if (result != null)
				{
					result.IsCurrentApprover = false;
					result.SkippTime = DateTime.Now;
					result.SkippBy = Convert.ToInt32(Request.UserId);
					upresult = await _dbContext.SaveChangesAsync();
				}
				if (upresult > 0)
				{
					_logger.LogwriteInfo("update current approver as skipp approver successfully done" + Environment.NewLine, loginUserId);
					return true;
				}
				else
				{
					_logger.LogwriteInfo("update current approver as skipp approver not done" + Environment.NewLine, loginUserId);
					return false;
				}
			}
			catch (Exception e)
			{
				_logger.LogwriteInfo("exception occur during UpdateCurrentApproverAsSkippedApprover" + Environment.NewLine + "exception is-------" + e.StackTrace, loginUserId);
				return false;
			}
		}
		public async ValueTask<bool> RevartBackNoteStatus(string noteId, string approverId)
		{
			bool str = false;
			try
			{
				int result = await _iDapperFactory.ExecuteSpDapperAsync(SpName: OraStoredProcedureNames.RevartNoteStatus, Params: new { NoteId = noteId, ApproverId = approverId });
				if (result > 0)
				{
					_logger.LogwriteInfo("Data Update into Note and Approver Table for user-" + loginUserId, loginUserId);
					str = true;
				}
				else
				{
					_logger.LogwriteInfo("Data not Update into Note and Approver Table", loginUserId);
				}
			}
			catch (Exception e)
			{
				_logger.LogwriteInfo("exception occur during RevartBackNoteStatus-------" + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
			}
			return str;
		}
		public async ValueTask<bool> ResetPreviousNoteDetails(string noteId)
		{
			bool str = false;
			try
			{
				int result = await _iDapperFactory.ExecuteSpDapperAsync(SpName: OraStoredProcedureNames.ResetBeforeAmendment, Params: new { NoteId = noteId });
				if (result > 0)
				{
					_logger.LogwriteInfo("Data Update success before note amendment for note-" + noteId, loginUserId);
					str = true;
				}
				else
				{
					_logger.LogwriteInfo("Data Update failed before note amendment", loginUserId);
				}
			}
			catch (Exception e)
			{
				_logger.LogwriteInfo("exception occur during ResetPreviousNoteDetails-------" + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
			}
			return str;
		}
	}
}
