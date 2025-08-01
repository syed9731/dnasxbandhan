using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class ExpenseIncurredAt
{
    public int ExpenseIncurredAtId { get; set; }

    public short? CategoryId { get; set; }

    public string? ExpenseIncurredAtName { get; set; }

    public bool? IsActive { get; set; }

    public virtual Category? Category { get; set; }
}
