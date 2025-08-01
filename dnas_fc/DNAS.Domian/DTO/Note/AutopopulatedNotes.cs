using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNAS.Domain.DTO.Note
{
    public class AutopopulatedNotes
    {
        public IEnumerable<NoteData> NoteData { get; set; } = [];
        public FilterNote FilterNote { get; set; } = new();


    }
    public class NoteData
    {
        public string NoteId { get; set; } = string.Empty;
        public string NoteTitle { get; set; } = string.Empty;
    }

    public class FilterNote
    {
        public int UserId { get; set; } = 0;
        public string SearchText { get; set; } = string.Empty;
        public string DashboardType { get; set; } = string.Empty;
    }
}
