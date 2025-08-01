using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class Notification
{
    public long NotificationId { get; set; }

    public int ReceiverUserId { get; set; }

    public long NoteId { get; set; }

    public string? Heading { get; set; }

    public string? Message { get; set; }

    public DateTime NotificationTime { get; set; }

    public bool? IsRead { get; set; }

    public string? Action { get; set; }

    public virtual Note Note { get; set; } = null!;

    public virtual UserMaster ReceiverUser { get; set; } = null!;
}
