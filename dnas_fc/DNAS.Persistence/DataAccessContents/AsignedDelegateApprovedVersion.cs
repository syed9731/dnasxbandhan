using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class AsignedDelegateApprovedVersion
{
    public long DelegateApprovedVersionId { get; set; }

    public long NoteApprovedVersionId { get; set; }

    public long DelegateApprovedId { get; set; }

    public long DelegateId { get; set; }

    public long ApproverId { get; set; }

    public int? DeligatedUserId { get; set; }

    public DateTime? AssignTime { get; set; }

    public long? DelegateBy { get; set; }
}
