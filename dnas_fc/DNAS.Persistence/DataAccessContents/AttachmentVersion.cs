using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class AttachmentVersion
{
    public long AttachmentVersionId { get; set; }

    public long NoteVersionId { get; set; }

    public long AttachmentId { get; set; }

    public long NoteId { get; set; }

    public string AttachmentPath { get; set; } = null!;

    public string DocumentName { get; set; } = null!;
}
