using Discord;
using System.Text.RegularExpressions;

namespace YoutubeLiveChatToDiscord;

public static class Helper
{
    /// <summary>
    /// Shared logger
    /// </summary>
    internal static class ApplicationLogging
    {
        internal static ILoggerFactory LoggerFactory { get; set; } = new LoggerFactory();
        internal static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
        internal static ILogger CreateLogger(string categoryName) => LoggerFactory.CreateLogger(categoryName);
    }

    private static readonly ILogger _logger = ApplicationLogging.CreateLogger("Helper");

    /// <summary>
    /// 尋找yt-dlp程式路徑
    /// </summary>
    /// <returns>Full path of yt-dlp</returns>
    public static string WhereIsYt_dlp()
    {
        // https://stackoverflow.com/a/63021455
        string file = "yt-dlp";
        string[] paths = Environment.GetEnvironmentVariable("PATH")?.Split(';') ?? [];
        string[] extensions = Environment.GetEnvironmentVariable("PATHEXT")?.Split(';') ?? [];
        string YtdlPath = (from p in new[] { Environment.CurrentDirectory }.Concat(paths)
                           from e in extensions
                           let path = Path.Combine(p.Trim(), file + e.ToLower())
                           where File.Exists(path)
                           select path)?.FirstOrDefault() ?? "/usr/bin/yt-dlp";
        _logger.LogDebug("Found yt-dlp at {path}", YtdlPath);
        return YtdlPath;
    }

    /// <summary>
    /// 把.NET Core logger對應到Discord內建的logger上面
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    internal static Task DiscordWebhookClient_Log(LogMessage arg)
        => Task.Run(() =>
        {
            switch (arg.Severity)
            {
                case LogSeverity.Critical:
                    _logger.LogCritical("{message}", arg);
                    break;
                case LogSeverity.Error:
                    _logger.LogError("{message}", arg);
                    break;
                case LogSeverity.Warning:
                    _logger.LogWarning("{message}", arg);
                    break;
                case LogSeverity.Info:
                    _logger.LogInformation("{message}", arg);
                    break;
                case LogSeverity.Verbose:
                    _logger.LogTrace("{message}", arg);
                    break;
                case LogSeverity.Debug:
                default:
                    _logger.LogDebug("{message}", arg);
                    break;
            }
        });

    /// <summary>
    /// 處理Youtube的圖片url，取得原始尺寸圖片
    /// </summary>
    /// <param name="url"></param>
    /// <returns>original big picture url</returns>
    public static string GetOriginalImage(string? url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return "";
        }

        string pattern1 = @"^(https?:\/\/lh\d+\.googleusercontent\.com\/.+\/)([^\/]+)(\/[^\/]+(\.(jpg|jpeg|gif|png|bmp|webp))?)(?:\?.+)?$";
        if (Regex.IsMatch(url, pattern1))
        {
            GroupCollection matches = Regex.Matches(url, pattern1)[0].Groups;

            return $"{matches[1]}s0{matches[3]}";
        }

        string pattern2 = @"^(https?:\/\/lh\d+\.googleusercontent\.com\/.+=)(.+)(?:\?.+)?$";
        if (Regex.IsMatch(url, pattern2))
        {
            return $"{Regex.Matches(url, pattern2)[0].Groups[1]}s0";
        }

        string pattern3 = @"^(https?:\/\/\w+\.ggpht\.com\/.+\/)([^\/]+)(\/[^\/]+(\.(jpg|jpeg|gif|png|bmp|webp))?)(?:\?.+)?$";
        if (Regex.IsMatch(url, pattern3))
        {
            return $"{Regex.Matches(url, pattern3)[0].Groups[1]}s0";
        }

        string pattern4 = @"^(https?:\/\/\w+\.ggpht\.com\/.+)=(?:[s|w|h])(\d+)(.+)?$";
        if (Regex.IsMatch(url, pattern4))
        {
            return $"{Regex.Matches(url, pattern4)[0].Groups[1]}=s0";
        }

        return url;
    }
}
