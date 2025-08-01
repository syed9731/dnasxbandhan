using System.Globalization;

namespace DNAS.Domian.DTO.SearchNotes
{
    public class SearchNoteData
    {
        public FilterSearchNote FilterSearchNotes { get; set; } = new();
        public IEnumerable<SearchNote> SearchNotes { get; set; } = [];
    }
    public class SearchNote
    {
        public string NoteId { get; set; } = string.Empty;
        public string NoteTitle { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string DateOfCreation { get; set; } = string.Empty;
        public string NoteStatus { get; set; } = string.Empty;
        public string QueryType { get; set; } = string.Empty;
    }
    public class FilterSearchNote
    {
        public int UserId { get; set; } = 0;
        public string StartDate { get; set; } = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        public string EndDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        public string Category { get; set; } = string.Empty;
        public string Title {  get; set; } = string.Empty;
    }
}
