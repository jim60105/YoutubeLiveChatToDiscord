using Discord.Webhook;
using YoutubeLiveChatToDiscord;

Environment.SetEnvironmentVariable("VIDEOID", Environment.GetCommandLineArgs()[1]);
Environment.SetEnvironmentVariable("WEBHOOK", Environment.GetCommandLineArgs()[2]);

IEnumerable<string> oldFiles = Directory.GetFiles(Directory.GetCurrentDirectory())
                                        .Where(p => p.Contains($"{Environment.GetEnvironmentVariable("VIDEOID")}.live_chat.json"));
foreach (var file in oldFiles)
{
    File.Delete(file);
}

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<LiveChatMonitorWorker>()
                .AddHostedService<LiveChatDownloadWorker>()
                .AddSingleton<DiscordWebhookClient>((service) =>
                    new DiscordWebhookClient(Environment.GetEnvironmentVariable("WEBHOOK")));
    })
    .Build();

await host.RunAsync();
