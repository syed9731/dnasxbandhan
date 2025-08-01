using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class MailConfiguration
{
    public short Id { get; set; }

    public string MailKey { get; set; } = null!;

    public string MailSubject { get; set; } = null!;

    public string MailBody { get; set; } = null!;

    public bool? IsActive { get; set; }
}
