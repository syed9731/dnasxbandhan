using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class Designation
{
    public int DesignationId { get; set; }

    public string DesignationName { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<UserMaster> UserMasters { get; set; } = new List<UserMaster>();
}
