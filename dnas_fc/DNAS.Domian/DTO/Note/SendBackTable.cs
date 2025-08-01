using System.Globalization;

namespace DNAS.Domian.DTO.Note
{
    public class SendBackData
    {
        public FilterSendBack FilterSendBacks { get; set; } = new();
        public IEnumerable<SendBackTable> Table { get; set; } = [];
    }
    public class SendBackTable
    {
        public string NoteTitle { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public string DateOfCreation { get; set; } = string.Empty;

        public string CommentTime { get; set; } = string.Empty;

        public string NoteId {  get; set; } = string.Empty;
    }
    public class FilterSendBack
    {
        public int UserId { get; set; } = 0;
        public string StartDate { get; set; } = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        public string EndDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        public string Category { get; set; } = string.Empty;
    }
}
