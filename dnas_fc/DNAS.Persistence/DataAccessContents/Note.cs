using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class Note
{
    public long NoteId { get; set; }

    public int UserId { get; set; }

    public long? TemplateId { get; set; }

    public short? CategoryId { get; set; }

    public int? ExpenseIncurredAtId { get; set; }

    public int? NatureOfExpensesId { get; set; }

    public string? CreatorDepartment { get; set; }

    public string? NoteState { get; set; }

    public string? NoteTitle { get; set; }

    public decimal? CapitalExpenditure { get; set; }

    public decimal? OperationalExpenditure { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? NoteBody { get; set; }

    public DateTime? DateOfCreation { get; set; }

    public DateTime? WithdrawDate { get; set; }

    public string? NoteStatus { get; set; }

    public bool? IsActive { get; set; }

    public string? NoteUid { get; set; }

    public int? MajorRevision { get; set; }

    public int? MinorRevision { get; set; }

    public virtual ICollection<Approver> Approvers { get; set; } = new List<Approver>();

    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    public virtual ICollection<Fyi> Fyis { get; set; } = new List<Fyi>();

    public virtual ICollection<NoteVersion> NoteVersions { get; set; } = new List<NoteVersion>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
