using System.ComponentModel;

namespace DNAS.Domain.DTO.DashBoard
{
    public class CreatorDashboard
    {
        public IEnumerable<CreatorData> CreatorData { get; set; } = [];
        public FilterCreatorData FilterCreatorData { get; set; } = new();
        public bool IsNoteApprovedDataExist { get; set; }
    }

    public class CreatorData
    {
        public string NoteId { get; set; } = string.Empty;

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

    }

    public class FilterCreatorData
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
