using System;

namespace youtubeDowloader.Models;

public class StreamInfo
{
    public required string Quality { get; set; }
    public required string Container { get; set; }
    public double FileSize { get; set; }
    public required string Url { get; set; }
}