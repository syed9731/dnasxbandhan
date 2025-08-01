using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class Attachment
{
    public long AttachmentId { get; set; }

    public long NoteId { get; set; }

    public string AttachmentPath { get; set; } = null!;

    public string DocumentName { get; set; } = null!;

    public virtual Note Note { get; set; } = null!;
}
