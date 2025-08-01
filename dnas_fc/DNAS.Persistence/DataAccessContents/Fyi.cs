using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class Fyi
{
    public long Fyiid { get; set; }

    public long NoteId { get; set; }

    public int WhoTagged { get; set; }

    public int ToWhome { get; set; }

    public DateTime? TaggedTime { get; set; }

    public virtual Note Note { get; set; } = null!;

    public virtual UserMaster ToWhomeNavigation { get; set; } = null!;

    public virtual UserMaster WhoTaggedNavigation { get; set; } = null!;
}
