using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using YoutubeLiveChatToDiscord.Services;
using Chat = YoutubeLiveChatToDiscord.Models.Chat.chat;
using Info = YoutubeLiveChatToDiscord.Models.Info.info;

namespace YoutubeLiveChatToDiscord;

public class LiveChatMonitorWorker : BackgroundService
{
    private readonly ILogger<LiveChatMonitorWorker> _logger;
    private readonly string _id;
    private readonly FileInfo _liveChatFileInfo;
    private long _position = 0;
    private readonly LiveChatDownloadService _liveChatDownloadService;
    private readonly DiscordService _discordService;

    public LiveChatMonitorWorker(
        ILogger<LiveChatMonitorWorker> logger,
        LiveChatDownloadService liveChatDownloadService,
        DiscordService discordService
        )
    {
        (_logger, _liveChatDownloadService, _discordService) = (logger, liveChatDownloadService, discordService);

        _id = Environment.GetEnvironmentVariable("VIDEO_ID") ?? "";
        if (string.IsNullOrEmpty(_id)) throw new ArgumentException(nameof(_id));

        _liveChatFileInfo = new($"{_id}.live_chat.json");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_liveChatDownloadService.downloadProcess.IsCompleted)
                {
                    _ = _liveChatDownloadService.ExecuteAsync(stoppingToken)
                                                .ContinueWith((_) => _logger.LogInformation("yt-dlp is stopped."), stoppingToken);
                }

                _logger.LogInformation("Wait 10 seconds.");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                _liveChatFileInfo.Refresh();

                try
                {
                    if (!_liveChatFileInfo.Exists)
                    {
                        throw new FileNotFoundException(null, _liveChatFileInfo.FullName);
                    }

                    await Monitoring(stoppingToken);
                }
                catch (FileNotFoundException e)
                {
                    _logger.LogWarning("Json file not found. {FileName}", e.FileName);
                }
            }
        }
        catch (TaskCanceledException) { }
        finally
        {
            _logger.LogError("Wait 10 seconds before closing the program. This is to prevent a restart loop from hanging the machine.");
#pragma warning disable CA2016 // 將 'CancellationToken' 參數轉送給方法
            await Task.Delay(TimeSpan.FromSeconds(10));
#pragma warning restore CA2016 // 將 'CancellationToken' 參數轉送給方法
        }
    }

    /// <summary>
    /// Monitoring
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <exception cref="FileNotFoundException"></exception>
    /// <returns></returns>
    private async Task Monitoring(CancellationToken stoppingToken)
    {
        await GetVideoInfo(stoppingToken);

#if !DEBUG
        if (null == Environment.GetEnvironmentVariable("SKIP_STARTUP_WAITING"))
        {
            _logger.LogInformation("Wait 1 miunute to skip old chats");
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            _liveChatFileInfo.Refresh();
        }
#endif

        _position = _liveChatFileInfo.Length;
        _logger.LogInformation("Start at position: {position}", _position);
        _logger.LogInformation("Start Monitoring!");

        while (!stoppingToken.IsCancellationRequested)
        {
            _liveChatFileInfo.Refresh();
            if (_liveChatFileInfo.Length > _position)
            {
                await ProcessChats(stoppingToken);
            }
            else if (_liveChatDownloadService.downloadProcess.IsCompleted)
            {
                _logger.LogInformation("Download process is stopped. Restart monitoring.");
                return;
            }
            else
            {
                _position = _liveChatFileInfo.Length;
                _logger.LogTrace("No new chat. Wait 10 seconds.");
                // 每10秒檢查一次json檔
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }

    /// <summary>
    /// GetVideoInfo
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = $"{nameof(SourceGenerationContext)} is used.")]
    private async Task GetVideoInfo(CancellationToken stoppingToken)
    {
        FileInfo videoInfo = new($"{_id}.info.json");
        if (!videoInfo.Exists)
        {
            // Chat json file 在 VideoInfo json file之後被產生，理論上這段不會進來
            throw new FileNotFoundException(null, videoInfo.FullName);
        }

        Info? info = JsonSerializer.Deserialize(json: await new StreamReader(videoInfo.OpenRead()).ReadToEndAsync(stoppingToken),
                                                jsonTypeInfo: SourceGenerationContext.Default.info);
        string? Title = info?.title;
        string? ChannelId = info?.channel_id;
        string? thumb = info?.thumbnail;

        Environment.SetEnvironmentVariable("TITLE", Title);
        Environment.SetEnvironmentVariable("CHANNEL_ID", ChannelId);
        Environment.SetEnvironmentVariable("VIDEO_THUMB", thumb);
    }

    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = $"{nameof(SourceGenerationContext)} is used.")]
    private async Task ProcessChats(CancellationToken stoppingToken)
    {
        // Notice: yt-dlp在Linux會使用lock鎖定此檔案，在Windows不鎖定。
        // 實作: https://github.com/yt-dlp/yt-dlp/commit/897376719871279eef89426b1452abb89051f0dc
        // Issue: https://github.com/yt-dlp/yt-dlp/issues/3124
        // 不像Windows是獨占鎖，Linux上是諮詢鎖，程式可以自行決定是否遵守鎖定。
        // FileStream「會」遵守鎖定，所以此處會在開啟檔案時報錯。
        // 詳細說明請參考這個issue，其中的討論過程非常清楚: https://github.com/dotnet/runtime/issues/34126
        // 這是.NET Core在Linux、Windows上關於鎖定設計的描述: https://github.com/dotnet/runtime/pull/55256
        // 如果要繞過這個問題，從.NET 6開始，可以加上環境變數「DOTNET_SYSTEM_IO_DISABLEFILELOCKING」讓FileStream「不」遵守鎖定。
        // (本專案已在Dockerfile加上此環境變數)
        using FileStream fs = new(_liveChatFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using StreamReader sr = new(fs);

        sr.BaseStream.Seek(_position, SeekOrigin.Begin);
        while (_position < sr.BaseStream.Length)
        {
            string? str = "";
            try
            {
                str = await sr.ReadLineAsync(stoppingToken);
                _position = sr.BaseStream.Position;
                if (string.IsNullOrEmpty(str)) continue;

                Chat? chat = JsonSerializer.Deserialize(json: str,
                                                        jsonTypeInfo: SourceGenerationContext.Default.chat);
                if (null == chat) continue;

                await _discordService.BuildRequestAndSendToDiscord(chat, stoppingToken);
            }
            catch (JsonException e)
            {
                _logger.LogError("{error}", e.Message);
                _logger.LogError("{originalString}", str);
            }
            catch (ArgumentException e)
            {
                _logger.LogError("{error}", e.Message);
                _logger.LogError("{originalString}", str);
            }
            catch (IOException e)
            {
                _logger.LogError("{error}", e.Message);
                break;
            }
        }
    }
}