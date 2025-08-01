using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class PreDefinedApprover
{
    public long PreDefinedApproverId { get; set; }

    public short CategoryId { get; set; }

    public int UserId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual UserMaster User { get; set; } = null!;
}
