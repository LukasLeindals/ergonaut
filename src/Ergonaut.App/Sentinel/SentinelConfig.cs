using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace Ergonaut.App.Sentinel;

public sealed class SentinelConfig
{

    /// <summary>
    /// Name of the project that should receive Sentinel-created work items.
    /// </summary>
    [Required]
    public string ProjectName { get; set; }

    /// <summary>
    /// Minimum log severity that Sentinel should process.
    /// </summary>
    public LogLevel MinimumLevel { get; set; } = LogLevel.Warning;

    public SentinelConfig()
    {
        ProjectName = string.Empty;
    }

    public SentinelConfig(string projectName)
    {
        ProjectName = projectName;
    }




}
