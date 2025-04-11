
namespace TaskAspNet.Web.Helpers;

public static class TimeExtensions
{
    public static string ToRelativeTime(this DateTime dateTime)
    {
        var ts = DateTime.UtcNow - dateTime.ToUniversalTime();

        if (ts.TotalSeconds < 60)
            return "just now";
        if (ts.TotalMinutes < 60)
            return $"{(int)ts.TotalMinutes} minute{(ts.TotalMinutes >= 2 ? "s" : "")} ago";
        if (ts.TotalHours < 24)
            return $"{(int)ts.TotalHours} hour{(ts.TotalHours >= 2 ? "s" : "")} ago";
        if (ts.TotalDays < 7)
            return $"{(int)ts.TotalDays} day{(ts.TotalDays >= 2 ? "s" : "")} ago";
        if (ts.TotalDays < 30)
            return $"{(int)(ts.TotalDays / 7)} week{(ts.TotalDays >= 14 ? "s" : "")} ago";
        if (ts.TotalDays < 365)
            return $"{(int)(ts.TotalDays / 30)} month{(ts.TotalDays >= 60 ? "s" : "")} ago";

        return $"{(int)(ts.TotalDays / 365)} year{(ts.TotalDays >= 730 ? "s" : "")} ago";
    }
}
