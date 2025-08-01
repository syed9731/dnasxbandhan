using System.Globalization;

namespace DNAS.Domian.DTO.Draft
{
    public class DraftNoteData
    {
        public FilterDraftNote FilterDrafts { get; set; } = new();
        public IEnumerable<DraftNote> DraftNotes { get; set; } = [];
    }

    public class DraftNote
    {
        public string NoteId { get; set; } = string.Empty;
        public string NoteTitle { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public DateTime DateOfCreation { get; set; } = DateTime.MinValue;
        public string NoteUID { get; set; } = string.Empty;
    }

    public class FilterDraftNote
    {
        public int UserId { get; set; } = 0;
        public string StartDate { get; set; } = new DateTime(2024, 1, 1,0,0,0,DateTimeKind.Utc).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        public string EndDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        public string Category { get; set; } = string.Empty;
    }
}
