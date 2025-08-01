using System.Globalization;

namespace DNAS.Domian.DTO.FYI
{
    public class FyiData
    {
        public FilterFyi FilterFYIs { get; set; } = new();
        public IEnumerable<FyiTable> Table { get; set; } = [];
    }

    public class FyiTable
    {
        public string noteid { get; set; } = string.Empty;
        public int FyiId { get; set; } = 0;
        public string NoteTitle { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string DateOfCreation { get; set; } = string.Empty;
        public string NoteStatus { get; set; } = string.Empty;
    }

    public class FilterFyi
    {
        public int UserId { get; set; } = 0;
        public string StartDate { get; set; } = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        public string EndDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        public string Status { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
