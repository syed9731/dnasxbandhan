using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class Comment
{
    public long CommentId { get; set; }

    public long NoteTrackerId { get; set; }

    public int UserId { get; set; }

    public string? Comment1 { get; set; }

    public DateTime? CommentDate { get; set; }

    public virtual NoteTracker NoteTracker { get; set; } = null!;

    public virtual UserMaster User { get; set; } = null!;
}
