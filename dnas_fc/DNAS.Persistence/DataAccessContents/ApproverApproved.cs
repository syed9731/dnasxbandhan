using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class ApproverApproved
{
    public long ApproverApprovedId { get; set; }

    public long ApproverId { get; set; }

    public long NoteId { get; set; }

    public long UserId { get; set; }

    public bool IsApproved { get; set; }

    public DateTime? ApprovedTime { get; set; }

    public bool? IsCurrentApprover { get; set; }

    public DateTime? AssignTime { get; set; }

    public string? ApproverType { get; set; }

    public long? SkippBy { get; set; }

    public DateTime? SkippTime { get; set; }

    public DateTime? MyAssignTime { get; set; }

    public DateTime? ChildAssignTime { get; set; }

    public int? SuffixPrefix { get; set; }

    public int? AddedBy { get; set; }

    public int? VisibilityMode { get; set; }
}
