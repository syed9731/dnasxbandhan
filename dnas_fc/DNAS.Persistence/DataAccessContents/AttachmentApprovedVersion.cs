using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class AttachmentApprovedVersion
{
    public long AttachmentApprovedVersionId { get; set; }

    public long NoteApprovedVersionId { get; set; }

    public long AttachmentApprovedId { get; set; }

    public long AttachmentId { get; set; }

    public long NoteId { get; set; }

    public string AttachmentPath { get; set; } = null!;

    public string DocumentName { get; set; } = null!;
}
