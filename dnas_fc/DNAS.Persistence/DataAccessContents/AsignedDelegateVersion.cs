using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class AsignedDelegateVersion
{
    public long DelegateIdVersionId { get; set; }

    public long? NoteVersionId { get; set; }

    public long DelegateId { get; set; }

    public long ApproverId { get; set; }

    public long? DeligatedUserId { get; set; }

    public DateTime? AssignTime { get; set; }

    public long? DelegateBy { get; set; }
}
