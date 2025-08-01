using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class RoleCurrencyMapping
{
    public long MappingId { get; set; }

    public long? UserId { get; set; }

    public long? RoleId { get; set; }

    public string? BranchName { get; set; }
}
