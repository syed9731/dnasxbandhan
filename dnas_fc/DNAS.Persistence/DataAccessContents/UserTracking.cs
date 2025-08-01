using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class UserTracking
{
    public long Id { get; set; }

    public int? UserId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? LastLoginTime { get; set; }

    public string? SessionId { get; set; }
}
