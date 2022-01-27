using Discord;
using Discord.Webhook;
using Newtonsoft.Json;
using YoutubeLiveChatToDiscord.Models;

namespace YoutubeLiveChatToDiscord
{
    public class LiveChatMonitorWorker : BackgroundService
    {
        private readonly ILogger<LiveChatMonitorWorker> logger;
        private readonly string id;
        private readonly DiscordWebhookClient client;
        private readonly FileInfo liveChatFileInfo;
        private long position = 0;

        public LiveChatMonitorWorker(ILogger<LiveChatMonitorWorker> _logger,
                                     DiscordWebhookClient _client)
        {
            (logger, client) = (_logger, _client);
            client.Log += Client_Log;

            id = Environment.GetEnvironmentVariable("VIDEO_ID") ?? "";
            if (string.IsNullOrEmpty(id)) throw new ArgumentException(nameof(id));

            liveChatFileInfo = new($"{id}.live_chat.json");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!liveChatFileInfo.Exists && !stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("{jsonFile} not found.", liveChatFileInfo.FullName);
                logger.LogInformation($"Wait for {nameof(LiveChatDownloadWorker)} to start.");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                liveChatFileInfo.Refresh();
            }

            await GetVideoInfo(stoppingToken);

            logger.LogInformation("Start Monitoring!");

            using StreamReader sr = new(liveChatFileInfo.OpenRead());
            position = sr.BaseStream.Length;
            logger.LogDebug("Start at position: {position}", position);

            while (!stoppingToken.IsCancellationRequested)
            {
                if (sr.BaseStream.Length <= position)
                {
                    position = sr.BaseStream.Length;
                    logger.LogTrace("No new chat. Wait 10 sec");
                    // 每10秒檢查一次json檔
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                    continue;
                }

                await ProcessChats(sr, stoppingToken);
            }
        }

        private async Task GetVideoInfo(CancellationToken stoppingToken)
        {
            FileInfo videoInfo = new($"{id}.info.json");
            while (!videoInfo.Exists && !stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("{jsonFile} not found.", videoInfo.FullName);
                logger.LogInformation($"Wait for {nameof(LiveChatDownloadWorker)} to start.");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                videoInfo.Refresh();
            }
            Info? info = JsonConvert.DeserializeObject<Info>(await new StreamReader(videoInfo.OpenRead()).ReadToEndAsync());
            string? Title = info?.title;
            string? ChannelId = info?.uploader_id;
            string? thumb = info?.thumbnail;

            Environment.SetEnvironmentVariable("TITLE", Title);
            Environment.SetEnvironmentVariable("CHANNEL_ID", ChannelId);
            Environment.SetEnvironmentVariable("VIDEO_THUMB", thumb);
        }

        private async Task ProcessChats(StreamReader sr, CancellationToken stoppingToken)
        {
            sr.BaseStream.Seek(position, SeekOrigin.Begin);
            while (position < sr.BaseStream.Length)
            {
                try
                {
                    string? str = await sr.ReadLineAsync();
                    if (string.IsNullOrEmpty(str)) continue;

                    Chat? chat = JsonConvert.DeserializeObject<Chat>(str);
                    position = sr.BaseStream.Position;
                    if (null == chat) continue;

                    await BuildRequestAndSendToDiscord(chat, stoppingToken);
                }
                catch (JsonSerializationException e)
                {
                    logger.LogError("{error}", e);
                    position = sr.BaseStream.Position;
                }
            }
        }

        private async Task BuildRequestAndSendToDiscord(Chat chat, CancellationToken stoppingToken)
        {
            LiveChatTextMessageRenderer? lctmr = chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatTextMessageRenderer;
            List<Run>? runs = lctmr?.message?.runs;

            if (null != runs)
            {
                string author = lctmr?.authorName?.simpleText ?? "";
                string authorPhoto = lctmr?.authorPhoto?.thumbnails?.LastOrDefault()?.url ?? "";
                long timeStamp = long.TryParse(lctmr?.timestampUsec, out long l) ? l / 1000 : 0;

                EmbedBuilder eb = new();
                eb.WithDescription(string.Join("", runs.Select(p => p.text ?? (p.emoji?.searchTerms?.FirstOrDefault()))))
                  .WithTitle(Environment.GetEnvironmentVariable("TITLE") ?? "")
                  .WithUrl($"https://youtu.be/{id}")
                  .WithThumbnailUrl(Environment.GetEnvironmentVariable("VIDEO_THUMB"))
                  .WithAuthor(new EmbedAuthorBuilder().WithName(author)
                                                      .WithUrl($"https://www.youtube.com/channel/{lctmr?.authorExternalChannelId}")
                                                      .WithIconUrl(authorPhoto));
                if (lctmr?.authorExternalChannelId == Environment.GetEnvironmentVariable("CHANNEL_ID"))
                {
                    eb.WithColor(Color.Gold);
                }

                EmbedFooterBuilder ft = new();
                ft.WithText(DateTimeOffset.FromUnixTimeMilliseconds(timeStamp).LocalDateTime.ToString())
                  .WithIconUrl(lctmr?.authorBadges?.FirstOrDefault()?.liveChatAuthorBadgeRenderer?.customThumbnail?.thumbnails?.LastOrDefault()?.url ?? "");
                eb.WithFooter(ft);

                try
                {
                    logger.LogDebug("Sending Request to Discord: {author}: {message}", author, eb.Description);
                    ulong messageId = await client.SendMessageAsync(embeds: new Embed[] { eb.Build() });
                    logger.LogDebug("Message sent to discord, message id: {messageId}", messageId);

                    // The rate for Discord webhooks are 30 requests/minute per channel.
                    // Be careful when you run multiple instances in the same channel!
                    logger.LogTrace("Wait 2 seconds for discord webhook rate limit");
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                }
                catch (TimeoutException) { }
            }
        }

        private Task Client_Log(LogMessage arg)
        {
            return Task.Run(() =>
            {
                switch (arg.Severity)
                {
                    case LogSeverity.Critical:
                        logger.LogCritical("{message}", arg);
                        break;
                    case LogSeverity.Error:
                        logger.LogError("{message}", arg);
                        break;
                    case LogSeverity.Warning:
                        logger.LogWarning("{message}", arg);
                        break;
                    case LogSeverity.Info:
                        logger.LogInformation("{message}", arg);
                        break;
                    case LogSeverity.Verbose:
                        logger.LogTrace("{message}", arg);
                        break;
                    case LogSeverity.Debug:
                        logger.LogDebug("{message}", arg);
                        break;
                    default:
                        break;
                }
            });
        }

    }
}