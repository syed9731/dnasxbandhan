using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class AttachmentApproved
{
    public long AttachmentApprovedId { get; set; }

    public long AttachmentId { get; set; }

    public long NoteId { get; set; }

    public string AttachmentPath { get; set; } = null!;

    public string DocumentName { get; set; } = null!;
}
