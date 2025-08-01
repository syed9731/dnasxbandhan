using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class NoteTrackerApprovedVersion
{
    public long NoteTackerApprovedVersionId { get; set; }

    public long NoteApprovedVersionId { get; set; }

    public long NoteTackerApprovedId { get; set; }

    public long NoteTackerId { get; set; }

    public long NoteId { get; set; }

    public string NoteStatus { get; set; } = null!;

    public long ApproverId { get; set; }

    public string? Comment { get; set; }

    public DateTime? CommentTime { get; set; }
}
