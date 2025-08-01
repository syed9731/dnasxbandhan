using System.ComponentModel;

namespace DNAS.Domain.DTO.Note
{
    public class ApproverHistoryNotes
    {
        public IEnumerable<ApproverHistoryData> ApproverHistoryData { get; set; } = [];
        public FilterApproverHistory FilterApproverHistory { get; set; } = new();
    }
    public class ApproverHistoryData
    {
        public string NoteId { get; set; } = string.Empty;
        public long ApproverId { get; set; } = default;

        [DisplayName("Note Number")]
        public string NoteNumber { get; set; } = string.Empty;

        [DisplayName("Note Title")]
        public string NoteTitle { get; set; } = string.Empty;

        [DisplayName("Category")]
        public string CategoryName { get; set; } = string.Empty;

        [DisplayName("Create Date")]
        public string DateOfCreation { get; set; } = string.Empty;

        [DisplayName("Status")]
        public string Status { get; set; } = string.Empty;

        [DisplayName("Aging")]
        public string Aging { get; set; } = string.Empty;
        public string addedby { get; set; } = string.Empty;
        public string Suffix_Prefix { get; set; } = string.Empty;
        public string isVersion { get; set; } = string.Empty;

    }

    public class FilterApproverHistory
    {
        public int UserId { get; set; } = 0;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string DateRange { get; set; } = string.Empty;

    }


}
