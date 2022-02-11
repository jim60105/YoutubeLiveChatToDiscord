using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace YoutubeLiveChatToDiscord;

public class LiveChatDownloadWorker : BackgroundService
{
    private readonly ILogger<LiveChatDownloadWorker> logger;
    private readonly string id;

    public LiveChatDownloadWorker(ILogger<LiveChatDownloadWorker> _logger)
    {
        logger = _logger;

        id = Environment.GetEnvironmentVariable("VIDEO_ID") ?? "";
        if (string.IsNullOrEmpty(id)) throw new ArgumentException(nameof(id));

        // 更新ytdlp
        logger.LogInformation("Start updating yt-dlp.");
        new YoutubeDL()
        {
            YoutubeDLPath = WhereIsYt_dlp()
        }.RunUpdate().Wait();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        OptionSet live_chatOptionSet = new()
        {
            IgnoreConfig = true,
            WriteSub = true,
            SubLang = "live_chat",
            SkipDownload = true,
            NoPart = true,
            Output = "%(id)s"
        };
        live_chatOptionSet.AddCustomOption("--ignore-no-formats-error", true);

        OptionSet info_jsonOptionSet = new()
        {
            IgnoreConfig = true,
            WriteInfoJson = true,
            SkipDownload = true,
            NoPart = true,
            Output = "%(id)s"
        };
        info_jsonOptionSet.AddCustomOption("--ignore-no-formats-error", true);

        YoutubeDLProcess ytdlProc = new(WhereIsYt_dlp());
        ytdlProc.OutputReceived += (o, e) => logger.LogTrace("{message}", e.Data);
        ytdlProc.ErrorReceived += (o, e) => logger.LogError("{error}", e.Data);

        try
        {
            while (true)
            {
                string url = $"https://www.youtube.com/watch?v={id}";
                logger.LogInformation("Start yt-dlp with url: {url}", url);
                _ = await ytdlProc.RunAsync(new string[] { url },
                                            info_jsonOptionSet,
                                            stoppingToken)
                                  .ContinueWith((e) => ytdlProc.RunAsync(new string[] { url },
                                                                         live_chatOptionSet,
                                                                         stoppingToken))
                                  .Unwrap();

                logger.LogInformation("yt-dlp is stopped. Wait 20 seconds and start it again.");
                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            }
        }
        catch (TaskCanceledException)
        {
            logger.LogCritical("yt-dlp is stop with error! Wait 10 seconds before closing the program. This is to prevent a restart loop from hanging the machine.");
#pragma warning disable CA2016 // 將 'CancellationToken' 參數轉送給方法
            await Task.Delay(TimeSpan.FromSeconds(10));
#pragma warning restore CA2016 // 將 'CancellationToken' 參數轉送給方法
        }
    }

    /// <summary>
    /// 尋找yt-dlp程式路徑
    /// </summary>
    /// <returns>Full path of yt-dlp</returns>
    public string WhereIsYt_dlp()
    {
        // https://stackoverflow.com/a/63021455
        string file = "yt-dlp";
        string[] paths = Environment.GetEnvironmentVariable("PATH")?.Split(';') ?? Array.Empty<string>();
        string[] extensions = Environment.GetEnvironmentVariable("PATHEXT")?.Split(';') ?? Array.Empty<string>();
        string YtdlPath = (from p in new[] { Environment.CurrentDirectory }.Concat(paths)
                           from e in extensions
                           let path = Path.Combine(p.Trim(), file + e.ToLower())
                           where File.Exists(path)
                           select path)?.FirstOrDefault() ?? "/usr/bin/yt-dlp";
        logger.LogDebug("Found yt-dlp at {path}", YtdlPath);
        return YtdlPath;
    }
}
