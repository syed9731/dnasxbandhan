using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class NatureOfExpense
{
    public int NatureOfExpensesId { get; set; }

    public int ExpensesIncurredAtId { get; set; }

    public long NatureOfExpensesMasterId { get; set; }

    public decimal? MinAmount { get; set; }

    public decimal? MaxAmount { get; set; }

    public string? Uom { get; set; }

    public int? RoleId { get; set; }

    public bool IsActive { get; set; }
}
