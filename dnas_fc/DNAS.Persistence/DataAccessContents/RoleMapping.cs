using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class RoleMapping
{
    public long MappingId { get; set; }

    public long? UserId { get; set; }

    public long? RoleId { get; set; }
}
