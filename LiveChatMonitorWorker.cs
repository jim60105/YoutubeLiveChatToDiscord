using Discord;
using Discord.Webhook;
using Newtonsoft.Json;
using YoutubeLiveChatToDiscord.Models;
using YoutubeLiveChatToDiscord.Services;

namespace YoutubeLiveChatToDiscord
{
    public class LiveChatMonitorWorker : BackgroundService
    {
        private readonly ILogger<LiveChatMonitorWorker> logger;
        private readonly string id;
        private readonly DiscordWebhookClient client;
        private readonly FileInfo liveChatFileInfo;
        private long position = 0;
        private readonly LiveChatDownloadService liveChatDownloadService;

        public LiveChatMonitorWorker(
            ILogger<LiveChatMonitorWorker> _logger,
            DiscordWebhookClient _client,
            LiveChatDownloadService _liveChatDownloadService
            )
        {
            (logger, client, liveChatDownloadService) = (_logger, _client, _liveChatDownloadService);
            client.Log += Helper.DiscordWebhookClient_Log;

            id = Environment.GetEnvironmentVariable("VIDEO_ID") ?? "";
            if (string.IsNullOrEmpty(id)) throw new ArgumentException(nameof(id));

            liveChatFileInfo = new($"{id}.live_chat.json");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (liveChatDownloadService.DownloadProcess.IsCompleted)
                    {
                        _ = liveChatDownloadService.ExecuteAsync(stoppingToken)
                                                   .ContinueWith((_) =>
                                                   {
                                                       logger.LogInformation("yt-dlp is stopped.");
                                                   }, stoppingToken);
                    }

                    logger.LogInformation("Wait 10 seconds.");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                    liveChatFileInfo.Refresh();

                    try
                    {
                        if (!liveChatFileInfo.Exists)
                        {
                            throw new FileNotFoundException(null, liveChatFileInfo.FullName);
                        }

                        await Monitoring(stoppingToken);
                    }
                    catch (FileNotFoundException e)
                    {
                        logger.LogWarning("Json file not found. {FileName}", e.FileName);
                    }
                }
            }
            catch (TaskCanceledException) { }
            finally
            {
                logger.LogError("Wait 10 seconds before closing the program. This is to prevent a restart loop from hanging the machine.");
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
                logger.LogInformation("Wait 1 miunute to skip old chats");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                liveChatFileInfo.Refresh();
            }
#endif

            position = liveChatFileInfo.Length;
            logger.LogInformation("Start at position: {position}", position);
            logger.LogInformation("Start Monitoring!");

            while (!stoppingToken.IsCancellationRequested)
            {
                liveChatFileInfo.Refresh();
                if (liveChatFileInfo.Length > position)
                {
                    await ProcessChats(stoppingToken);
                }
                else if (liveChatDownloadService.DownloadProcess.IsCompleted)
                {
                    logger.LogInformation("Download process is stopped. Restart monitoring.");
                    return;
                }
                else
                {
                    position = liveChatFileInfo.Length;
                    logger.LogTrace("No new chat. Wait 10 seconds.");
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
        private async Task GetVideoInfo(CancellationToken stoppingToken)
        {
            FileInfo videoInfo = new($"{id}.info.json");
            if (!videoInfo.Exists)
            {
                // Chat json file 在 VideoInfo json file之後被產生，理論上這段不會進來
                throw new FileNotFoundException(null, videoInfo.FullName);
            }

            Info? info = JsonConvert.DeserializeObject<Info>(await new StreamReader(videoInfo.OpenRead()).ReadToEndAsync());
            string? Title = info?.title;
            string? ChannelId = info?.uploader_id;
            string? thumb = info?.thumbnail;

            Environment.SetEnvironmentVariable("TITLE", Title);
            Environment.SetEnvironmentVariable("CHANNEL_ID", ChannelId);
            Environment.SetEnvironmentVariable("VIDEO_THUMB", thumb);
        }

        private async Task ProcessChats(CancellationToken stoppingToken)
        {
            // Reading a file used by another process
            // https://stackoverflow.com/a/9760751
            using FileStream fs = new(liveChatFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader sr = new(fs);

            sr.BaseStream.Seek(position, SeekOrigin.Begin);
            while (position < sr.BaseStream.Length)
            {
                string? str = "";
                try
                {
                    str = await sr.ReadLineAsync();
                    position = sr.BaseStream.Position;
                    if (string.IsNullOrEmpty(str)) continue;

                    Chat? chat = JsonConvert.DeserializeObject<Chat>(str);
                    if (null == chat) continue;

                    await BuildRequestAndSendToDiscord(chat, stoppingToken);
                }
                catch (JsonSerializationException e)
                {
                    logger.LogError("{error}", e.Message);
                    logger.LogError("{originalString}", str);
                }
                catch (ArgumentException e)
                {
                    logger.LogError("{error}", e.Message);
                    logger.LogError("{originalString}", str);
                }
                catch (IOException e)
                {
                    logger.LogError("{error}", e.Message);
                    break;
                }
            }
        }

        /// <summary>
        /// 建立Discord embed並送出至Webhook
        /// </summary>
        /// <param name="chat"></param>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">訊息格式未支援</exception>
        private async Task BuildRequestAndSendToDiscord(Chat chat, CancellationToken stoppingToken)
        {
            EmbedBuilder eb = new();
            eb.WithTitle(Environment.GetEnvironmentVariable("TITLE") ?? "")
              .WithUrl($"https://youtu.be/{id}")
              .WithThumbnailUrl(Helper.GetOriginalImage(Environment.GetEnvironmentVariable("VIDEO_THUMB")));
            string author = "";

            LiveChatTextMessageRenderer? liveChatTextMessage = chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatTextMessageRenderer;
            LiveChatPaidMessageRenderer? liveChatPaidMessage = chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatPaidMessageRenderer;
            LiveChatPaidStickerRenderer? liveChatPaidSticker = chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatPaidStickerRenderer;

            // ReplaceChat: Treat as a new message
            // This is rare and not easy to test.
            // If it behaves strangely, please open a new issue with more examples.
            LiveChatTextMessageRenderer? replaceChat = chat.replayChatItemAction?.actions?.FirstOrDefault()?.replaceChatItemAction?.replacementItem?.liveChatTextMessageRenderer;
            if (null != replaceChat)
            {
                liveChatTextMessage = replaceChat;
            }

            // Normal Message
            if (null != liveChatTextMessage)
            {
                List<Run> runs = liveChatTextMessage.message?.runs ?? new List<Run>();
                author = liveChatTextMessage.authorName?.simpleText ?? "";
                string authorPhoto = Helper.GetOriginalImage(liveChatTextMessage.authorPhoto?.thumbnails?.LastOrDefault()?.url);

                eb.WithDescription(string.Join("", runs.Select(p => p.text ?? (p.emoji?.searchTerms?.FirstOrDefault()))))
                  .WithAuthor(new EmbedAuthorBuilder().WithName(author)
                                                      .WithUrl($"https://www.youtube.com/channel/{liveChatTextMessage.authorExternalChannelId}")
                                                      .WithIconUrl(authorPhoto));

                // Timestamp
                long timeStamp = long.TryParse(liveChatTextMessage.timestampUsec, out long l) ? l / 1000 : 0;
                EmbedFooterBuilder ft = new();
                string authorBadgeUrl = Helper.GetOriginalImage(liveChatTextMessage.authorBadges?.FirstOrDefault()?.liveChatAuthorBadgeRenderer?.customThumbnail?.thumbnails?.LastOrDefault()?.url);
                ft.WithText(DateTimeOffset.FromUnixTimeMilliseconds(timeStamp)
                                          .LocalDateTime
                                          .ToString("yyyy/MM/dd HH:mm:ss"))
                  .WithIconUrl(authorBadgeUrl);

                // From Stream Owner
                if (liveChatTextMessage.authorExternalChannelId == Environment.GetEnvironmentVariable("CHANNEL_ID"))
                {
                    eb.WithColor(Color.Gold);
                    ft.WithIconUrl("https://raw.githubusercontent.com/jim60105/YoutubeLiveChatToDiscord/master/assets/crown.png");
                }

                eb.WithFooter(ft);
            }
            else if (null != liveChatPaidMessage)
            // Super Chat
            {
                List<Run> runs = liveChatPaidMessage.message?.runs ?? new List<Run>();

                author = liveChatPaidMessage.authorName?.simpleText ?? "";
                string authorPhoto = Helper.GetOriginalImage(liveChatPaidMessage.authorPhoto?.thumbnails?.LastOrDefault()?.url);

                eb.WithDescription(string.Join("", runs.Select(p => p.text ?? (p.emoji?.searchTerms?.FirstOrDefault()))))
                  .WithAuthor(new EmbedAuthorBuilder().WithName(author)
                                                      .WithUrl($"https://www.youtube.com/channel/{liveChatPaidMessage.authorExternalChannelId}")
                                                      .WithIconUrl(authorPhoto));

                // Super Chat Amount
                eb.WithFields(new EmbedFieldBuilder[] { new EmbedFieldBuilder().WithName("Amount").WithValue(liveChatPaidMessage.purchaseAmountText?.simpleText) });

                // Super Chat Background Color
                Color bgColor = (Color)System.Drawing.ColorTranslator.FromHtml(string.Format("#{0:X}", liveChatPaidMessage.bodyBackgroundColor));
                eb.WithColor(bgColor);

                // Timestamp
                long timeStamp = long.TryParse(liveChatPaidMessage.timestampUsec, out long l) ? l / 1000 : 0;
                EmbedFooterBuilder ft = new();
                ft.WithText(DateTimeOffset.FromUnixTimeMilliseconds(timeStamp)
                                          .LocalDateTime
                                          .ToString("yyyy/MM/dd HH:mm:ss"))
                  .WithIconUrl("https://raw.githubusercontent.com/jim60105/YoutubeLiveChatToDiscord/master/assets/wallet.png");

                // From Stream Owner
                if (liveChatPaidMessage.authorExternalChannelId == Environment.GetEnvironmentVariable("CHANNEL_ID"))
                {
                    eb.WithColor(Color.Gold);
                    ft.WithIconUrl("https://raw.githubusercontent.com/jim60105/YoutubeLiveChatToDiscord/master/assets/crown.png");
                }

                eb.WithFooter(ft);
            }
            else if (null != liveChatPaidSticker)
            // Super Chat Sticker
            {
                author = liveChatPaidSticker.authorName?.simpleText ?? "";
                string authorPhoto = Helper.GetOriginalImage(liveChatPaidSticker.authorPhoto?.thumbnails?.LastOrDefault()?.url);

                eb.WithDescription("")
                  .WithAuthor(new EmbedAuthorBuilder().WithName(author)
                                                      .WithUrl($"https://www.youtube.com/channel/{liveChatPaidSticker.authorExternalChannelId}")
                                                      .WithIconUrl(authorPhoto));

                // Super Chat Amount
                eb.WithFields(new EmbedFieldBuilder[] { new EmbedFieldBuilder().WithName("Amount").WithValue(liveChatPaidSticker.purchaseAmountText?.simpleText) });

                // Super Chat Background Color
                Color bgColor = (Color)System.Drawing.ColorTranslator.FromHtml(string.Format("#{0:X}", liveChatPaidSticker.backgroundColor));
                eb.WithColor(bgColor);

                // Super Chat Sticker Picture
                string stickerThumbUrl = Helper.GetOriginalImage("https:" + liveChatPaidSticker.sticker?.thumbnails?.LastOrDefault()?.url);
                eb.WithThumbnailUrl(stickerThumbUrl);

                // Timestamp
                long timeStamp = long.TryParse(liveChatPaidSticker.timestampUsec, out long l) ? l / 1000 : 0;
                EmbedFooterBuilder ft = new();
                ft.WithText(DateTimeOffset.FromUnixTimeMilliseconds(timeStamp)
                                          .LocalDateTime
                                          .ToString("yyyy/MM/dd HH:mm:ss"))
                  .WithIconUrl("https://raw.githubusercontent.com/jim60105/YoutubeLiveChatToDiscord/master/assets/wallet.png");

                // From Stream Owner
                if (liveChatPaidSticker.authorExternalChannelId == Environment.GetEnvironmentVariable("CHANNEL_ID"))
                {
                    eb.WithColor(Color.Gold);
                    ft.WithIconUrl("https://raw.githubusercontent.com/jim60105/YoutubeLiveChatToDiscord/master/assets/crown.png");
                }

                eb.WithFooter(ft);
            }
            // Discrad known garbage messages.
            else if (
                // Banner Pinned message.
                null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.addBannerToLiveChatCommand
                // Click to show less.
                || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.showLiveChatTooltipCommand
                // Welcome to live chat! Remember to guard your privacy and abide by our community guidelines.
                || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatViewerEngagementMessageRenderer
                // Membership messages.
                || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatMembershipItemRenderer
                // SC Ticker messages.
                || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.addLiveChatTickerItemAction
                // Delete messages.
                || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.markChatItemAsDeletedAction
                // Remove Chat Item. Not really sure what this is.
                || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.removeChatItemAction
                // Live chat mode change.
                || null != chat.replayChatItemAction?.actions?.FirstOrDefault()?.addChatItemAction?.item?.liveChatModeChangeMessageRenderer
            ) { return; }
            else
            {
                logger.LogWarning("Message type not supported, skip sending to discord.");
                throw new ArgumentException("Message type not supported", nameof(chat));
            }

            if (stoppingToken.IsCancellationRequested) return;

            logger.LogDebug("Sending Request to Discord: {author}: {message}", author, eb.Description);

            try
            {
                await SendMessage();
            }
            catch (TimeoutException) { }
            // System.Net.Http.HttpRequestException: Resource temporarily unavailable (discord.com:443)
            catch (HttpRequestException)
            {
                // Retry once after 5 sec
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                await SendMessage();
            }

            // The rate for Discord webhooks are 30 requests/minute per channel.
            // Be careful when you run multiple instances in the same channel!
            logger.LogTrace("Wait 2 seconds for discord webhook rate limit");
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

            Task SendMessage()
               => client.SendMessageAsync(embeds: new Embed[] { eb.Build() })
                   .ContinueWith(async p =>
                   {
                       ulong messageId = await p;
                       logger.LogDebug("Message sent to discord, message id: {messageId}", messageId);
                   }, stoppingToken);
        }
    }
}