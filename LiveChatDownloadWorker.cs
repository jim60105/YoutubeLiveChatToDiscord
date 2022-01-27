using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace YoutubeLiveChatToDiscord;

public class LiveChatDownloadWorker : BackgroundService
{
    private readonly ILogger<LiveChatDownloadWorker> logger;
    private readonly string id;
    public string YtdlPath;

    public LiveChatDownloadWorker(ILogger<LiveChatDownloadWorker> _logger)
    {
        logger = _logger;

        id = Environment.GetEnvironmentVariable("VIDEOID") ?? "";
        if (string.IsNullOrEmpty(id)) throw new ArgumentException(nameof(id));

        YtdlPath = WhereIsYt_dlp();
        logger.LogDebug("Found yt-dlp at {path}", YtdlPath);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        OptionSet optionSet = new()
        {
            IgnoreConfig = true,
            WriteSub = true,
            SubLang = "live_chat",
            SkipDownload = true,
            NoPart = true,
            Output = "%(id)s"
        };
        optionSet.AddCustomOption("--ignore-no-formats-error", "");

        YoutubeDLProcess ytdlProc = new(YtdlPath);
        ytdlProc.OutputReceived += (o, e) => logger.LogTrace("{message}", e.Data);
        ytdlProc.ErrorReceived += (o, e) => logger.LogError("{error}", e.Data);

        logger.LogInformation("Start yt-dlp!");
        return ytdlProc.RunAsync(new string[] { id },
                                 optionSet,
                                 stoppingToken);
    }

    public static string WhereIsYt_dlp()
    {
        // https://stackoverflow.com/a/63021455
        string file = "yt-dlp";
        string[] paths = Environment.GetEnvironmentVariable("PATH")?.Split(';') ?? Array.Empty<string>();
        string[] extensions = Environment.GetEnvironmentVariable("PATHEXT")?.Split(';') ?? Array.Empty<string>();
        return (from p in new[] { Environment.CurrentDirectory }.Concat(paths)
                from e in extensions
                let path = Path.Combine(p.Trim(), file + e.ToLower())
                where File.Exists(path)
                select path)?.FirstOrDefault() ?? "/usr/bin/yt-dlp";
    }
}
