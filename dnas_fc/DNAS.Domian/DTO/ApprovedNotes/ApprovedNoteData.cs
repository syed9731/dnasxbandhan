using System.Globalization;

namespace DNAS.Domian.DTO.ApprovedNotes
{
    public class ApprovedNoteData
    {
        public FilterApprovedNote FilterApprovedNotes { get; set; } = new();
        public IEnumerable<ApprovedNote> Table { get; set; } = [];
    }

    public class ApprovedNote
    {
        public string NoteId { get; set; } = string.Empty;
        public string NoteTitle { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string DateOfCreation { get; set; } = string.Empty;
        public string Aging { get; set; } = string.Empty;
    }
    
    public class FilterApprovedNote
    {
        public int UserId { get; set; } = 0;
        public string StartDate { get; set; } = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        public string EndDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        public string Category { get; set; } = string.Empty;
    }
}
