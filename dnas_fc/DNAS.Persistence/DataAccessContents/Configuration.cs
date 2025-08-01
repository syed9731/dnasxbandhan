using System;
using System.Collections.Generic;

namespace DNAS.Persistence.DataAccessContents;

public partial class Configuration
{
    public short ConfigurationId { get; set; }

    public string? ConfigurationFor { get; set; }

    public string? ConfigurationKey { get; set; }

    public string? ConfigurationValue { get; set; }
}
