using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class NewNatureOfExpensesbkp
{
    public int NatureOfExpensesId { get; set; }

    public int ExpensesIncurredAtId { get; set; }

    public string NatureOfExpensesName { get; set; } = null!;

    public decimal? MinAmount { get; set; }

    public decimal? MaxAmount { get; set; }

    public string? Uom { get; set; }

    public int? DesignationId { get; set; }

    public bool IsActive { get; set; }
}
