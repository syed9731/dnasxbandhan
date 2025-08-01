using DNAS.Domain.DTO.CommonModel;
using DNAS.Domian.DTO.Note;

namespace DNAS.Domain.DTO.Amendment
{
    public class AmendmentNoteModel:NoteModel
    {
        public IList<CommonAttachmentModel>? AttachmentList { get; set; } = [];

        // storing AttachmentList as JSON
        public string? AttachmentListJson { get; set; } = string.Empty;
        public string ExpenseIncurredAtName { get; set; } = string.Empty;
        public string NatureOfExpenseCode { get; set; } = string.Empty;
        public string NatureOfExpensesName { get; set; } = string.Empty;
    }    
}
