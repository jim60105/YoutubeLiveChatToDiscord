using Discord.Webhook;
using LiveChatToDiscord;

Environment.SetEnvironmentVariable("VIDEOID", Environment.GetCommandLineArgs()[1]);
Environment.SetEnvironmentVariable("WEBHOOK", Environment.GetCommandLineArgs()[2]);

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
