using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class NoteTrackerVersion
{
    public long NoteTackerVersionId { get; set; }

    public long NoteVersionId { get; set; }

    public long NoteTackerId { get; set; }

    public long NoteId { get; set; }

    public string NoteStatus { get; set; } = null!;

    public long ApproverId { get; set; }

    public string? Comment { get; set; }

    public DateTime? CommentTime { get; set; }

    public virtual NoteVersion NoteVersion { get; set; } = null!;
}
