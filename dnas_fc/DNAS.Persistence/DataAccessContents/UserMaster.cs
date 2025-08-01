using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class UserMaster
{
    public int UserId { get; set; }

    public int ManagerId { get; set; }

    public string UserEmpId { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string Grade { get; set; } = null!;

    public string Department { get; set; } = null!;

    public DateTime? LastLoginTime { get; set; }

    public DateTime? LastPassRecoveryTime { get; set; }

    public bool IsActive { get; set; }

    public string? Region { get; set; }

    public string? Cluster { get; set; }

    public string BranchName { get; set; } = null!;

    public string? ReportingTo { get; set; }

    public string? FunctionalReportingTo { get; set; }

    public string? Designation { get; set; }

    public string? Zone { get; set; }

    public string? Seniority { get; set; }

    public virtual ICollection<Approver> Approvers { get; set; } = new List<Approver>();

    public virtual ICollection<AsignedDelegate> AsignedDelegates { get; set; } = new List<AsignedDelegate>();

    public virtual ICollection<Fyi> FyiToWhomeNavigations { get; set; } = new List<Fyi>();

    public virtual ICollection<Fyi> FyiWhoTaggedNavigations { get; set; } = new List<Fyi>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<TemplateMaster> TemplateMasters { get; set; } = new List<TemplateMaster>();
}
