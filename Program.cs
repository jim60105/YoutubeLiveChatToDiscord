using Discord.Webhook;
using YoutubeLiveChatToDiscord;
using YoutubeLiveChatToDiscord.Services;

Environment.SetEnvironmentVariable("VIDEO_ID", Environment.GetCommandLineArgs()[1]);
Environment.SetEnvironmentVariable("WEBHOOK", Environment.GetCommandLineArgs()[2]);

IEnumerable<string> oldFiles = Directory.GetFiles(Directory.GetCurrentDirectory())
                                        .Where(p => p.Contains($"{Environment.GetEnvironmentVariable("VIDEO_ID")}.live_chat.json"));
foreach (var file in oldFiles)
{
    File.Delete(file);
}

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<LiveChatMonitorWorker>()
                .AddSingleton<LiveChatDownloadService>()
                .AddSingleton<DiscordWebhookClient>((service) =>
                    new DiscordWebhookClient(Environment.GetEnvironmentVariable("WEBHOOK")));
    })
    .Build();

await host.RunAsync();
