using System.Globalization;

namespace DNAS.Domian.DTO.Note
{
    public class PendingData
    {
        public IEnumerable<PendingTable> Table { get; set; } = [];
        public FilterPendingNote FilterPendings { get; set; } = new();
    }

    public class PendingTable
    {
        public string NoteId { get; set; } = string.Empty;
        public string NoteTitle { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string DateOfCreation { get; set; } = string.Empty;
        public string Aging { get; set; } = string.Empty;
    }

    public class FilterPendingNote
    {
        public int UserId { get; set; } = 0;
        public string StartDate { get; set; } = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        public string EndDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        public string Category { get; set; } = string.Empty;
    }
}
