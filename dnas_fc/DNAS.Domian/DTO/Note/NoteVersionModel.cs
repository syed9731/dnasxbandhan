using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNAS.Domain.DTO.Note
{
    public class NoteVersionModel
    {
        public NoteApprovedDto NoteApproved { get; set; } = new();
        public IEnumerable<NoteApprovedVersionDto> NoteApprovedVersion { get; set; } = new List<NoteApprovedVersionDto>();
        public IEnumerable<NoteVersionDto> NoteVersion { get; set; } = new List<NoteVersionDto>();
    }
    public class NoteApprovedDto
    {
        public string Note_ApprovedId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string NoteTitle { get; set; } = string.Empty;
        public string NoteUID { get; set; } = string.Empty;
        public int MajorRevision { get; set; } 
        public int MinorRevision { get; set; } 
        public string DateOfCreation { get; set; } = string.Empty;
    }

    public class NoteApprovedVersionDto
    {
        public string NoteApproved_VersionId { get; set; } = string.Empty;
        public string Note_ApprovedId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string NoteTitle { get; set; } = string.Empty;
        public string NoteUID { get; set; } = string.Empty;
        public int MajorRevision { get; set; }
        public int MinorRevision { get; set; }
        public string DateOfCreation { get; set; } = string.Empty;
    }

    public class NoteVersionDto
    {
        public string Note_VersionId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string NoteTitle { get; set; } = string.Empty;
        public string NoteUID { get; set; } = string.Empty;
        public int MajorRevision { get; set; }
        public int MinorRevision { get; set; }
        public string DateOfCreation { get; set; } = string.Empty;
    }
}
