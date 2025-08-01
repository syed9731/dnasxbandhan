using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class AsignedDelegate
{
    public long DelegateId { get; set; }

    public long ApproverId { get; set; }

    public int? DeligatedUserId { get; set; }

    public DateTime? AssignTime { get; set; }

    public long? DelegateBy { get; set; }

    public virtual Approver Approver { get; set; } = null!;

    public virtual UserMaster? DeligatedUser { get; set; }
}
