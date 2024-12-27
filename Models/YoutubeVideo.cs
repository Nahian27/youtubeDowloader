using System;

namespace youtubeDowloader.Models;

public class YoutubeVideo
{
    public required string Title { get; set; }
    public required string Author { get; set; }
    public required string Duration { get; set; }
    public required string Thumbnail { get; set; }
    public required List<StreamInfo> VideoStreams { get; set; }
    public required List<StreamInfo> AudioStreams { get; set; }
    public required string Url { get; set; }
}
