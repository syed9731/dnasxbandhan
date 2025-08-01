using DNAS.Domain.DTO.CommonModel;
using DNAS.Domian.DTO.Login;

namespace DNAS.Domain.DTO.Amendment
{
    public class NoteAmendmentModel
    {
        public NoteDetails noteModel { get; set; } = new NoteDetails();
        public IEnumerable<ApproverDetails> approverDetails { get; set; } = [];
        public IEnumerable<AttachementDetails> attachementDetails { get; set; } = [];
    }
    public class NoteDetails : CommonNoteModel
    {
        public string ExpenseIncurredAtName { get; set; } = string.Empty;
        public string NatureOfExpenseCode { get; set; } = string.Empty;
        public string NatureOfExpensesName { get; set; } = string.Empty;
    }
    public class ApproverDetails : UserMasterModel
    {

    }
    public class AttachementDetails : CommonAttachmentModel
    {

    }
    
}
