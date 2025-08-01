using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class NatureExpensesMaster04122024
{
    public long NatureExpensesId { get; set; }

    public string? NatureOfExpenseCode { get; set; }

    public string? NatureOfExpensesName { get; set; }

    public int? ExpensesIncurredAtId { get; set; }

    public bool? IsActive { get; set; }
}
