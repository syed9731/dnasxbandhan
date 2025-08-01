using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class TemplateMaster
{
    public long TemplateId { get; set; }

    public string TemplateName { get; set; } = null!;

    public int UserId { get; set; }

    public short CategoryId { get; set; }

    public string TemplateBody { get; set; } = null!;

    public DateTime DateOfCreation { get; set; }

    public bool IsActive { get; set; }

    public bool IsPublish { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual UserMaster User { get; set; } = null!;
}
