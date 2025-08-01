using DNAS.Domian.DTO.FYI;

namespace DNAS.Domian.DTO.WithdrawList
{
    public class WithdrawData
    {
        public IEnumerable<WithdrawTable> Table { get; set; } = [];
    }
    public class WithdrawTable
    {
        public int Noteid { get; set; } = 0;
        public string NoteTitle { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string WithdrawDate { get; set; } = string.Empty;
    }
}
