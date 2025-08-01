using Dapper;

using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Application.IRepository;
using DNAS.Domain.DAO.DbHelperModels.FetchCategory;
using DNAS.Domain.DAO.DbHelperModels.FetchExpenseIncurredAt;
using DNAS.Domain.DAO.DbHelperModels.FetchNatureOfExpenses;
using DNAS.Domain.DAO.DbHelperModels.FetchNonFinancialApprover;
using DNAS.Domain.DAO.DbHelperModels.FinancialApprover;
using DNAS.Domain.DAO.DbHelperModels.RecomendedApprover;
using DNAS.Domain.DTO.Amendment;
using DNAS.Domain.DTO.Note;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.Category;
using DNAS.Domian.DTO.Login;
using DNAS.Domian.DTO.Note;
using DNAS.Persistence.DataAccessContents;

using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using System.Data;
using System.Security.Claims;

namespace DNAS.Persistence.Repository
{
    internal class Note(
        ICustomLogger logger,
        DataContext dataContext,
        IHttpContextAccessor haccess,
        IDapperFactory iDapperFactory) : INote
    {
        private readonly ICustomLogger _logger = logger;
        private readonly DataContext _dbContext = dataContext;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        private readonly string wwwroot = "wwwroot";

        public async ValueTask<IEnumerable<CategoryRespModel>> FetchCategory()
        {
            try
            {
                FetchCategoryOutput Response =
                    await _iDapperFactory.ExecuteSpDapperAsync<CategoryRespModel, FetchCategoryOutput>
                        (SpName: OraStoredProcedureNames.ProcFetchCategory);
                return Response.CategoryRespModelList;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during fetchCategory------ " + e.Message + Environment.NewLine + e.StackTrace,
                    loginUserId);
                return [];
            }
        }

        public async ValueTask<IEnumerable<ExpenseIncurredAtModel>> FetchExpenseIncurredAt(object inparam)
        {
            try
            {
                FetchExpenseIncurredAtOutput Response =
                    await _iDapperFactory.ExecuteSpDapperAsync<ExpenseIncurredAtModel, FetchExpenseIncurredAtOutput>
                        (SpName: OraStoredProcedureNames.ProcFetchExpenseIncurredAt, Params: inparam);
                return Response.ExpenseIncurredAtModelList;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during fetchExpenseIncurredAt------ " + e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
                return [];
            }
        }

        public async ValueTask<IEnumerable<NatureOfExpensesModel>> FetchNatureOfExpenses(object inparam)
        {
            try
            {
                FetchNatureOfExpensesOutput Response =
                    await _iDapperFactory.ExecuteSpDapperAsync<NatureOfExpensesModel, FetchNatureOfExpensesOutput>
                        (SpName: OraStoredProcedureNames.ProcFetchNatureOfExpenses, Params: inparam);
                return Response.NatureOfExpensesModelList;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during fetchNatureOfExpenses------ " + e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
                return [];
            }
        }

        public async ValueTask<IEnumerable<UserMasterModel>> FetchNonFinancialApprover(object inparam)
        {
            try
            {
                FetchNonFinancialApproverOutput Response =
                    await _iDapperFactory.ExecuteSpDapperAsync<UserMasterModel, FetchNonFinancialApproverOutput>
                        (SpName: OraStoredProcedureNames.ProcFetchNonFinancialApprover, Params: inparam);
                return Response.UserMasterModelList;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during fetchNonFinancialApprover------ " + e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
                return [];
            }
        }

        public async ValueTask<IEnumerable<UserMasterModel>> FetchFinancialApprover(object inparam)
        {
            try
            {
                ProcFetchFinancialApproverOutput Response =
                    await _iDapperFactory.ExecuteSpDapperAsync<UserMasterModel, ProcFetchFinancialApproverOutput>
                        (SpName: OraStoredProcedureNames.ProcFetchFinancialApprover, Params: inparam);
                return Response.UserMasterModelList;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during fetchNonFinancialApprover------ " + e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
                return [];
            }
        }

        public async ValueTask<DraftNoteModel> FetchSaveNoteData(object inparam)
        {
            DraftNoteModel Response = new();
            try
            {
                Response = await _iDapperFactory.ExecuteSpDapperAsync<NoteModel, UserMasterModel, DraftNoteModel>(
                    OraStoredProcedureNames.ProcFetchSaveNoteData, inparam);
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during FetchSaveNoteData------ " + e.Message + Environment.NewLine + e.StackTrace,
                    loginUserId);
            }

            return Response;
        }

        public async ValueTask<int> DeleteDraftNote(int Noteid)
        {
            int data = 0;
            try
            {
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        DataAccessContents.Note note =
                            await _dbContext.Notes.Where(x => x.NoteId == Noteid).FirstOrDefaultAsync() ?? new();
                        note.IsActive = false;
                        data = await _dbContext.SaveChangesAsync();
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        _logger.LogwriteInfo(
                            "exception occur during DeleteDraftNote Trsanction------ " + e.Message +
                            Environment.NewLine + e.StackTrace, loginUserId);
                        transaction.Rollback();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during DeleteDraftNote------ " + e.Message + Environment.NewLine + e.StackTrace,
                    loginUserId);
            }

            return data;
        }

        public async ValueTask<PendingNoteModel> FetchPendingNote(object inparam)
        {
            PendingNoteModel Response = new();
            try
            {
                Response = await _iDapperFactory
                    .ExecuteSpDapperAsync<NotesModel, ApproversModel, NoteComment, Domian.DTO.Note.Attachment,
                        PendingNoteModel>
                    (SpName: OraStoredProcedureNames.ProcFetchPendingNoteData,
                        Params: inparam);
            }
            catch (SqlException e)
            {
                _logger.LogwriteInfo(
                    "SqlException occur during FetchPendingNote------ " + e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during FetchPendingNote------ " + e.Message + Environment.NewLine + e.StackTrace,
                    loginUserId);
            }

            Response.attachmentsModel = Response.attachmentsModel.Select(w =>
            {
                w.AttachmentPath = w.AttachmentPath.Replace(wwwroot, "");
                return w;
            }).ToList();
            return Response;
        }

        public async ValueTask<WithdrawNoteDetailsModel> FetchWithdrawDetailsNote(object inparam)
        {
            WithdrawNoteDetailsModel Response = new();
            try
            {
                Response = await _iDapperFactory
                    .ExecuteSpDapperAsync<WithDtlNotesModel, WithDtlApproversModel, WithDtlNoteComment,
                        WithDtlAttachment, WithDtlRecomendedApproverModel, WithdrawNoteDetailsModel>
                    (SpName: OraStoredProcedureNames.ProcFetchWithdrawNoteDetails,
                        Params: inparam);
            }
            catch (SqlException e)
            {
                _logger.LogwriteInfo(
                    "SqlException occur during FetchPendingNote------ " + e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during FetchPendingNote------ " + e.Message + Environment.NewLine + e.StackTrace,
                    loginUserId);
            }

            Response.attachmentsModel = Response.attachmentsModel.Select(w =>
            {
                w.AttachmentPath = w.AttachmentPath.Replace(wwwroot, "");
                return w;
            }).ToList();
            return Response;
        }

        public async ValueTask<ViewsNoteModel> FetchViewsNote(object inparam)
        {
            ViewsNoteModel Response = new();
            try
            {
                Response = await _iDapperFactory
                    .ExecuteSpDapperAsync<ViewNotesModel, ViewApproversModel, ViewNoteComment, ViewAttachment,
                        ViewRecomendedApproverModel, ViewsNoteModel>
                    (SpName: OraStoredProcedureNames.ProcFetchViewsNoteData,
                        Params: inparam);
            }
            catch (SqlException e)
            {
                _logger.LogwriteInfo(
                    "SqlException occur during FetchViewsNote------ " + e.Message + Environment.NewLine + e.StackTrace,
                    loginUserId);
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during FetchViewsNote------ " + e.Message + Environment.NewLine + e.StackTrace,
                    loginUserId);
            }

            Response.attachmentsModel = Response.attachmentsModel.Select(w =>
            {
                w.AttachmentPath = w.AttachmentPath.Replace(wwwroot, "");
                return w;
            }).ToList();
            return Response;
        }

        public async ValueTask<WithdrawNoteModel> FetchWithdrawNote(object inparam)
        {
            WithdrawNoteModel Response = new();
            long? noteId = default;

            try
            {
                var noteIdProperty = inparam.GetType().GetProperty("NoteId");
                noteId = (long?)noteIdProperty?.GetValue(inparam);

                Response = await _iDapperFactory
                    .ExecuteSpDapperAsync<WithNotesModel, WithApproversModel, WithNoteComment, WithAttachment,
                        WithRecomendedApproverModel, WithdrawNoteModel>
                    (SpName: OraStoredProcedureNames.ProcFetchPendingNoteData,
                        Params: inparam);
            }
            catch (SqlException e)
            {
                _logger.LogwriteInfo(
                    "SqlException occur during FetchWithdrawNote------ " + e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during FetchWithdrawNote------ " + e.Message + Environment.NewLine + e.StackTrace,
                    loginUserId);
            }
            Response.IsNoteApprovedDataExist = await _dbContext.NoteApproveds.AnyAsync(x => x.NoteId == noteId.GetValueOrDefault());


            Response.attachmentsModel = Response.attachmentsModel.Select(w =>
            {
                w.AttachmentPath = w.AttachmentPath.Replace(wwwroot, "");
                return w;
            }).ToList();
            return Response;
        }
        public async ValueTask<MyApprovedNoteModel> FetchMyApprovedNote(string NoteId, string UserId)
        {
            MyApprovedNoteModel Response = new();

            try
            {
                DynamicParameters parameters = new();
                parameters.Add("@NoteId", value: Convert.ToInt64(NoteId), dbType: DbType.Int64, direction: ParameterDirection.Input);
                parameters.Add("@UserId", Convert.ToInt64(UserId), dbType: DbType.Int64, direction: ParameterDirection.Input);
                parameters.Add("@IsNoteApprovedDataExist", dbType: DbType.Boolean, direction: ParameterDirection.Output);


                Response = await _iDapperFactory.ExecuteSpDapperAsync<MyAppNotesModel, MyAppApproversModel, MyAppNoteComment, MyAppAttachment, MyAppRecomendedApproverModel, NoteModel, MyApprovedNoteModel>
                            (SpName: OraStoredProcedureNames.ProcMyApprovedNoteData,
                            Params: parameters);
                Response.IsNoteApprovedDataExist = parameters.Get<bool>("@IsNoteApprovedDataExist");

            }
            catch (SqlException e)
            {
                _logger.LogwriteInfo("SqlException occur during FetchMyApprovedNote------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during FetchMyApprovedNote------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
            }
            Response.attachmentsModel = Response.attachmentsModel.Select(w => { w.AttachmentPath = w.AttachmentPath.Replace(wwwroot, ""); return w; }).ToList();

            return Response;
        }
        public async ValueTask<ViewFyiNoteModel> FetchFyiNoteData(object inparam)
        {
            ViewFyiNoteModel Response = new();
            try
            {
                Response = await _iDapperFactory
                    .ExecuteSpDapperAsync<VwFyiNotesModel, VwFyiApproversModel, VwFyiNoteComment, VwFyiAttachment,
                        VwFyiRecomendedApproverModel, ViewFyiNoteModel>(OraStoredProcedureNames.ProcFetchFyiNoteData,
                        inparam);
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during FetchFyiNoteData------ " + e.Message + Environment.NewLine + e.StackTrace,
                    loginUserId);
            }

            Response.attachmentsModel = Response.attachmentsModel.Select(w =>
            {
                w.AttachmentPath = w.AttachmentPath.Replace(wwwroot, "");
                return w;
            }).ToList();
            return Response;
        }

        public async ValueTask<PendingNoteModel> FetchNoteStatus(object inparam)
        {
            PendingNoteModel Response = new();
            try
            {
                Response = await _iDapperFactory.ExecuteSpDapperAsync<NotesModel, ApproversModel, PendingNoteModel>(OraStoredProcedureNames.FetchNoteStatus, inparam);
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during FetchNoteStatus------ " + e.Message + Environment.NewLine + e.StackTrace,
                    loginUserId);
            }

            return Response;
        }

        public async ValueTask<RequestApproverNoteModel> FetchApprovalRequestNote(string NoteId, string UserId)
        {
            RequestApproverNoteModel Response = new();

            try
            {
                DynamicParameters parameters = new();
                parameters.Add("@NoteId", value: Convert.ToInt64(NoteId), dbType: DbType.Int64, direction: ParameterDirection.Input);
                parameters.Add("@UserId", Convert.ToInt64(UserId), dbType: DbType.Int64, direction: ParameterDirection.Input);
                parameters.Add("@IsNoteApprovedDataExist", dbType: DbType.Boolean, direction: ParameterDirection.Output);

                Response = await _iDapperFactory
                    .ExecuteSpDapperAsync<ReqNotesModel, ReqApproversModel, ReqNoteComment, ReqAttachment,
                        ReqRecomendedApproverModel, RequestApproverNoteModel>(
                        OraStoredProcedureNames.ProcFetchApprovalRequestNoteData, parameters);

                Response.IsNoteApprovedDataExist = parameters.Get<bool>("@IsNoteApprovedDataExist");
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during FetchApprovalRequestNote------ " + e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
            }

            Response.attachmentsModel = Response.attachmentsModel.Select(w =>
            {
                w.AttachmentPath = w.AttachmentPath.Replace(wwwroot, "");
                return w;
            }).ToList();
            return Response;
        }

        public async ValueTask<DelegateNoteModel> FetchDelegateNoteDetails(object inparam)
        {
            DelegateNoteModel Response = new();
            try
            {
                Response = await _iDapperFactory.ExecuteSpDapperAsync<DelNotesModel, DelApproversModel, DelNoteComment, DelAttachment, DelRecomendedApproverModel, DelegateNoteModel>(OraStoredProcedureNames.ProcFetchDelegateNoteData, inparam);
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during FetchDelegateNoteDetails------ " + e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
            }

            Response.attachmentsModel = Response.attachmentsModel.Select(w =>
            {
                w.AttachmentPath = w.AttachmentPath.Replace(wwwroot, "");
                return w;
            }).ToList();
            return Response;
        }

        public async ValueTask<IEnumerable<UserMasterModel>> FetchRecomendedApproverList(object inparam)
        {
            try
            {
                RecomendedApproverOutput Response =
                    await _iDapperFactory.ExecuteSpDapperAsync<UserMasterModel, RecomendedApproverOutput>
                        (SpName: OraStoredProcedureNames.ProcFetchRecomendedApprover, Params: inparam);
                return Response.UserMasterModelList;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    "exception occur during FetchRecomendedApproverList------ " + e.Message + Environment.NewLine +
                    e.StackTrace, loginUserId);
                return [];
            }
        }

        public async ValueTask<IEnumerable<UserMasterModel>> FetchReviewerApproverList(object inparam)
        {
            try
            {
                RecomendedApproverOutput response =
                    await _iDapperFactory.ExecuteSpDapperAsync<UserMasterModel, RecomendedApproverOutput>
                        (SpName: OraStoredProcedureNames.ProcFetchReviewerOrApprover, Params: inparam);
                return response.UserMasterModelList;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo(
                    $"exception occur during FetchReviewerApproverList------ {e.Message}{Environment.NewLine}{e.StackTrace}",
                    loginUserId);
                return [];
            }
        }
        public async ValueTask<NoteAmendmentModel> FetchAmendmentData(object inparam)
        {
            NoteAmendmentModel Response = new();
            try
            {

                Response = await _iDapperFactory.ExecuteSpDapperAsync<NoteDetails, ApproverDetails, AttachementDetails, NoteAmendmentModel>
                            (SpName: OraStoredProcedureNames.ProcFetchAmendmentData,
                            Params: inparam);

            }
            catch (SqlException e)
            {
                _logger.LogwriteInfo("SqlException occur during FetchAmendmentData------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during FetchAmendmentData------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
            }
            Response.attachementDetails = Response.attachementDetails.Select(w => { w.AttachmentPath = w.AttachmentPath.Replace(wwwroot, ""); return w; }).ToList();
            return Response;
        }
        public async ValueTask<MyApprovedNoteModel> FetchApprovedNote(object inparam)
        {
            MyApprovedNoteModel Response = new();
            try
            {

                Response = await _iDapperFactory.ExecuteSpDapperAsync<MyAppNotesModel, MyAppApproversModel, MyAppNoteComment, MyAppAttachment, MyAppRecomendedApproverModel, MyApprovedNoteModel>
                            (SpName: OraStoredProcedureNames.ProcApprovedNoteData,
                            Params: inparam);

            }
            catch (SqlException e)
            {
                _logger.LogwriteInfo("SqlException occur during FetchApprovedNote------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during FetchApprovedNote------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
            }
            Response.attachmentsModel = Response.attachmentsModel.Select(w => { w.AttachmentPath = w.AttachmentPath.Replace(wwwroot, ""); return w; }).ToList();
            return Response;
        }
    }
}