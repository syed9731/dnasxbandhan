using DNAS.Application.Common.Interface;
using DNAS.Application.IEntityRepository;
using DNAS.Domain.DTO.Attachment;
using DNAS.Domain.DTO.DelegateByCreator;
using DNAS.Domain.DTO.Note;
using DNAS.Domian.DTO.Note;
using DNAS.Persistence.DataAccessContents;
using DNAS.Persistence.Repository;

using Microsoft.EntityFrameworkCore;

using System.Diagnostics;

namespace DNAS.Persistence.EntityRepository
{
    internal sealed class Fetch(DataContext dataContext, ICustomLogger logger) : IFetch
    {
        private readonly DataContext _dbContext = dataContext;
        public readonly ICustomLogger _logger = logger;


        public async ValueTask<SendBackNoteDto> FetchSendBackNoteByNoteId(long noteId)
        {
            try
            {
                SendBackNoteDto sendBackNoteDto = new();

                var data = await (from nt in _dbContext.Notes.AsNoTracking()

                                      // Left join with Categories
                                  join cat in _dbContext.Categories.AsNoTracking()
                                  on nt.CategoryId equals cat.CategoryId into noteCategory
                                  from cat in noteCategory.DefaultIfEmpty()

                                      // Left join with ExpenseIncurredAts
                                  join eia in _dbContext.ExpenseIncurredAts.AsNoTracking()
                                  on nt.ExpenseIncurredAtId equals eia.ExpenseIncurredAtId into noteExpense
                                  from eia in noteExpense.DefaultIfEmpty()

                                      // Left join with NatureExpensesMasters
                                  join nem in _dbContext.NatureExpensesMasters.AsNoTracking()
                                  on nt.NatureOfExpensesId equals (long?)nem.NatureExpensesId into noteNature
                                  from nem in noteNature.DefaultIfEmpty()

                                  where nt.NoteId == noteId
                                  select new
                                  {
                                      Note = nt,
                                      Category = cat,
                                      ExpenseIncurredAt = eia,
                                      NatureExpensesMaster = nem
                                  }).FirstOrDefaultAsync();





                sendBackNoteDto.NoteModel = new NoteModel
                {

                    NoteId = data?.Note.NoteId.ToString() ?? "",
                    UserId = data?.Note.UserId.ToString() ?? "",
                    CategoryId = data?.Category.CategoryId.ToString() ?? "",
                    CategoryName = data?.Category != null ? data?.Category.CategoryName : "",
                    NoteTitle = data?.Note.NoteTitle ?? "",
                    NoteBody = data?.Note.NoteBody ?? "",
                    NoteState = data?.Note.NoteState ?? "",
                    NoteStatus = data?.Note.NoteStatus ?? "",
                    TemplateId = data?.Note.TemplateId.GetValueOrDefault().ToString() ?? "",
                    TotalAmount = data?.Note.TotalAmount.GetValueOrDefault().ToString(),
                    ExpenseIncurredAtId = data?.Note.ExpenseIncurredAtId.GetValueOrDefault().ToString(),
                    NatureOfExpensesId = data?.Note.NatureOfExpensesId.GetValueOrDefault().ToString(),
                    CreatorDepartment = data?.Note.CreatorDepartment ?? "",
                    CapitalExpenditure = data?.Note.CapitalExpenditure.GetValueOrDefault().ToString(),
                    OperationalExpenditure = data?.Note.OperationalExpenditure.GetValueOrDefault().ToString(),
                    DateOfCreation = data?.Note.DateOfCreation.GetValueOrDefault().ToString() ?? "",
                    WithdrawDate = data?.Note.WithdrawDate.GetValueOrDefault().ToString() ?? "",
                    IsActive = data?.Note.IsActive ?? false

                };

                sendBackNoteDto.OperationalExpenditure = data?.Note.OperationalExpenditure.GetValueOrDefault().ToString() ?? "";
                sendBackNoteDto.CapitalExpenditure = data?.Note.CapitalExpenditure.GetValueOrDefault().ToString() ?? "";
                sendBackNoteDto.ExpenseIncurredAtName = data?.ExpenseIncurredAt?.ExpenseIncurredAtName ?? "";
                sendBackNoteDto.NatureOfExpensesName = data?.NatureExpensesMaster?.NatureOfExpensesName ?? "";
                sendBackNoteDto.TotalAmount = data?.Note.TotalAmount.GetValueOrDefault().ToString() ?? "";

                sendBackNoteDto.ApproverList = await (from app in _dbContext.Approvers.AsNoTracking()
                                                      join usr in _dbContext.UserMasters.AsNoTracking()
                                                       on app.UserId equals usr.UserId
                                                      where app.NoteId == noteId
                                                      select new SendBackNoteApproverModel
                                                      {
                                                          NoteId = noteId.ToString(),
                                                          ApproverId = app.ApproverId.ToString(),
                                                          UserId = usr.UserId.ToString(),
                                                          FirstName = usr.FirstName,
                                                          LastName = usr.LastName,
                                                          MiddleName = usr.MiddleName ?? "",
                                                          UserName = usr.UserName,
                                                          ApproverType = app.ApproverType ?? "",
                                                          Email = usr.Email,
                                                          Department = usr.Department,
                                                          Grade = usr.Grade,
                                                          Role = usr.Role,
                                                      }).ToListAsync();

                sendBackNoteDto.AttachmentList = await (from att in _dbContext.Attachments.AsNoTracking()
                                                        where att.NoteId == noteId
                                                        select new AttachmentDto
                                                        {
                                                            NoteId = att.NoteId,
                                                            AttachmentId = att.AttachmentId,
                                                            DocumentName = att.DocumentName,
                                                            AttachmentPath = att.AttachmentPath
                                                        }).ToListAsync();


                return sendBackNoteDto!;
            }
            catch (Exception ex)
            {
                _logger.LogwriteError($"Exception occur during FetchSendBackNoteByNoteId----------------NoteId:{noteId}{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}", "Fetch.cs");
                throw;
            }
        }
    }
}
