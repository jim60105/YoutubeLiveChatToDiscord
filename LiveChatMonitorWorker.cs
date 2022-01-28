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

            using StreamReader sr = new(liveChatFileInfo.OpenRead());

#if !DEBUG
            if (null == Environment.GetEnvironmentVariable("SKIP_STARTUP_WAITING"))
            {
                logger.LogInformation("Wait 20 sec to skip old chats");
                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            }
#endif
            position = sr.BaseStream.Length;

            logger.LogDebug("Start at position: {position}", position);
            logger.LogInformation("Start Monitoring!");

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
            EmbedBuilder eb = new();
            eb.WithTitle(Environment.GetEnvironmentVariable("TITLE") ?? "")
              .WithUrl($"https://youtu.be/{id}")
              .WithThumbnailUrl(Environment.GetEnvironmentVariable("VIDEO_THUMB"));
            string author = "";

            LiveChatTextMessageRenderer? liveChatTextMessage = chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatTextMessageRenderer;
            LiveChatPaidMessageRenderer? liveChatPaidMessage = chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatPaidMessageRenderer;
            LiveChatPaidStickerRenderer? liveChatPaidSticker = chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatPaidStickerRenderer;

            // Normal Message
            if (null != liveChatTextMessage)
            {
                List<Run> runs = liveChatTextMessage.message?.runs ?? new List<Run>();
                author = liveChatTextMessage.authorName?.simpleText ?? "";
                string authorPhoto = liveChatTextMessage.authorPhoto?.thumbnails?.LastOrDefault()?.url ?? "";

                eb.WithDescription(string.Join("", runs.Select(p => p.text ?? (p.emoji?.searchTerms?.FirstOrDefault()))))
                  .WithAuthor(new EmbedAuthorBuilder().WithName(author)
                                                      .WithUrl($"https://www.youtube.com/channel/{liveChatTextMessage.authorExternalChannelId}")
                                                      .WithIconUrl(authorPhoto));
                // From Stream Owner
                if (liveChatTextMessage.authorExternalChannelId == Environment.GetEnvironmentVariable("CHANNEL_ID"))
                {
                    eb.WithColor(Color.Gold);
                }

                // Timestamp
                long timeStamp = long.TryParse(liveChatTextMessage.timestampUsec, out long l) ? l / 1000 : 0;
                EmbedFooterBuilder ft = new();
                ft.WithText(DateTimeOffset.FromUnixTimeMilliseconds(timeStamp)
                                          .LocalDateTime
                                          .ToString("yyyy/MM/dd HH:mm:ss"))
                  .WithIconUrl(liveChatTextMessage.authorBadges?.FirstOrDefault()?.liveChatAuthorBadgeRenderer?.customThumbnail?.thumbnails?.LastOrDefault()?.url ?? "");
                eb.WithFooter(ft);
            }
            else if (null != liveChatPaidMessage)
            // Super Chat
            {
                List<Run> runs = liveChatPaidMessage.message?.runs ?? new List<Run>();

                author = liveChatPaidMessage.authorName?.simpleText ?? "";
                string authorPhoto = liveChatPaidMessage.authorPhoto?.thumbnails?.LastOrDefault()?.url ?? "";

                eb.WithDescription(string.Join("", runs.Select(p => p.text ?? (p.emoji?.searchTerms?.FirstOrDefault()))))
                  .WithAuthor(new EmbedAuthorBuilder().WithName(author)
                                                      .WithUrl($"https://www.youtube.com/channel/{liveChatPaidMessage.authorExternalChannelId}")
                                                      .WithIconUrl(authorPhoto));
                // From Stream Owner
                if (liveChatPaidMessage.authorExternalChannelId == Environment.GetEnvironmentVariable("CHANNEL_ID"))
                {
                    eb.WithColor(Color.Gold);
                }

                // Timestamp
                long timeStamp = long.TryParse(liveChatPaidMessage.timestampUsec, out long l) ? l / 1000 : 0;
                EmbedFooterBuilder ft = new();
                ft.WithText(DateTimeOffset.FromUnixTimeMilliseconds(timeStamp)
                                          .LocalDateTime
                                          .ToString("yyyy/MM/dd HH:mm:ss"))
                  .WithIconUrl("https://upload.cc/i1/2022/01/28/uL9JV0.png");
                eb.WithFooter(ft);

                // Super Chat Amount
                eb.WithFields(new EmbedFieldBuilder[] { new EmbedFieldBuilder().WithName("Amount").WithValue(liveChatPaidMessage.purchaseAmountText?.simpleText) });

                // Super Chat Background Color
                Color bgColor = (Color)System.Drawing.ColorTranslator.FromHtml(string.Format("#{0:X}", liveChatPaidMessage.bodyBackgroundColor));
                eb.WithColor(bgColor);
            }
            else if (null != liveChatPaidSticker)
            // Super Chat Sticker
            {
                author = liveChatPaidSticker.authorName?.simpleText ?? "";
                string authorPhoto = liveChatPaidSticker.authorPhoto?.thumbnails?.LastOrDefault()?.url ?? "";

                eb.WithDescription("")
                  .WithAuthor(new EmbedAuthorBuilder().WithName(author)
                                                      .WithUrl($"https://www.youtube.com/channel/{liveChatPaidSticker.authorExternalChannelId}")
                                                      .WithIconUrl(authorPhoto));
                // From Stream Owner
                if (liveChatPaidSticker.authorExternalChannelId == Environment.GetEnvironmentVariable("CHANNEL_ID"))
                {
                    eb.WithColor(Color.Gold);
                }

                // Timestamp
                long timeStamp = long.TryParse(liveChatPaidSticker.timestampUsec, out long l) ? l / 1000 : 0;
                EmbedFooterBuilder ft = new();
                ft.WithText(DateTimeOffset.FromUnixTimeMilliseconds(timeStamp)
                                          .LocalDateTime
                                          .ToString("yyyy/MM/dd HH:mm:ss"))
                  .WithIconUrl("https://upload.cc/i1/2022/01/28/uL9JV0.png");
                eb.WithFooter(ft);

                // Super Chat Amount
                eb.WithFields(new EmbedFieldBuilder[] { new EmbedFieldBuilder().WithName("Amount").WithValue(liveChatPaidSticker.purchaseAmountText?.simpleText) });

                // Super Chat Background Color
                Color bgColor = (Color)System.Drawing.ColorTranslator.FromHtml(string.Format("#{0:X}", liveChatPaidSticker.backgroundColor));
                eb.WithColor(bgColor);

                // Super Chat Sticker Picture
                string? stickerThumbUrl = liveChatPaidSticker.sticker?.thumbnails?.LastOrDefault()?.url;
                eb.WithThumbnailUrl("https:" + stickerThumbUrl);
            }
            else
            {
                logger.LogWarning("Message type not supported, skip sending to discord.");
                return;
            }

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