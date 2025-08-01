using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class Category
{
    public short CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<ExpenseIncurredAt> ExpenseIncurredAts { get; set; } = new List<ExpenseIncurredAt>();

    public virtual ICollection<TemplateMaster> TemplateMasters { get; set; } = new List<TemplateMaster>();
}
