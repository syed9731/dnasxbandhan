namespace DNAS.Domian.DTO.DashBoard
{
    public class ApprovalData
    {
        public IEnumerable<Approval> ApprovalList { get; set; } = [];
        public IEnumerable<Draft> DraftList { get; set; } = [];
        public Count Totalcount { get; set; } = new();
    }

    public class Approval
    {
        public string NoteId { get; set; } =string.Empty;
        public int ApproverId { get; set; } = 0;
        public string NoteTitle { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string DateOfCreation { get; set; } = string.Empty;
    }

    public class Draft
    {
        public string NoteId { get; set; } = string.Empty;
        public string NoteTitle { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string DateOfCreation { get; set; } = string.Empty;
    }

    public class Count
    {
        public int TempMasterRowCount { get; set; } = 0;
        public int NoteRowCount { get; set; } = 0;
        public int ApproveRowCount { get; set; } = 0;
        public int NotificationRowCount { get; set; } = 0;
    }
}
