using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class NoteVersion
{
    public long NoteVersionId { get; set; }

    public long NoteId { get; set; }

    public long UserId { get; set; }

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

    public virtual Note Note { get; set; } = null!;

    public virtual ICollection<NoteTrackerVersion> NoteTrackerVersions { get; set; } = new List<NoteTrackerVersion>();
}
