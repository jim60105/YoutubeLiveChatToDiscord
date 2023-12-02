# YoutubeLiveChatToDiscord

## Stream Youtube chat to Discord Webhook

|                                                Youtube Live Chat                                                |     |                                                 Discord Webhook                                                 |
| :-------------------------------------------------------------------------------------------------------------: | :-: | :-------------------------------------------------------------------------------------------------------------: |
| ![image](https://user-images.githubusercontent.com/16995691/151545455-af26cbe6-0942-464a-b15e-76ca67dfa142.png) | ➡️  | ![image](https://user-images.githubusercontent.com/16995691/151438025-d0c4a2de-6845-4d64-93db-89afb2f98e45.png) |
| ![image](https://user-images.githubusercontent.com/16995691/151545035-0dfc65e3-41a4-4342-b0c4-178b53a077d6.png) | ➡️  | ![image](https://user-images.githubusercontent.com/16995691/151545242-651cdbd1-ae8c-4a47-acda-7b9a3b4f59ba.png) |
| ![image](https://user-images.githubusercontent.com/16995691/151663570-999a5c8c-a336-407e-906a-56399530417b.png) | ➡️  | ![image](https://user-images.githubusercontent.com/16995691/151663574-dc5abbc2-cb5d-4e40-a4ce-bfc39f2a7029.png) |

<p align="center">
  <span>English</span> |
  <a href="https://github.com/jim60105/YoutubeLiveChatToDiscord/blob/master/README.zh.md">
    中文
  </a>
</p>

- Not appropriate for scenarios with a high message speed. This tool is designed to monitor FreeChat.
  This tool sends a discord webhook every two seconds at most, which may cause the forwarding speed to not keep up with the comment speed.
  > Discord's limitation is that Webhooks can be called 30 times per minute in the same channel [ref](https://twitter.com/lolpython/status/967621046277820416).
  > If multiple instances of this tool are activated and pushed to the same channel at the same time, it is easy to trigger Discord cooldown. Please pay attention to your usage environment.
- The underlying data retrieval uses yt-dlp instead of YouTube API, so there are no API quota issues.
- When idle, this tool reads the json file written by yt-dlp every 10 seconds.
- Upon startup, it waits for 1 minute for yt-dlp to download old comments before starting monitoring from there.
  > To skip this 1-minute waiting period, please pass the environment variable `SKIP_STARTUP_WAITING`.
- If a file named `cookies.txt` exists in the same directory as the program, it will be automatically imported into yt-dlp to used for membership live streaming.

## Membership (login required) videos

If a file named `cookies.txt` exists in the program's execution directory, it will be used automatically.

For Docker, please mount `cookies.txt` to `/app/cookies.txt`.

## Docker

> Please refer to `docker-compose.yml`.

Two parameters need to be passed in:

- Video ID
- Discord Webhook URL

```sh
docker run -rm ghcr.io/jim60105/youtubelivechattodiscord [Video_Id] [Discord_Webhook_Url]
```

Also available at [quay.io](https://quay.io/jim60105/youtubelivechattodiscord)

## Kubernetes Helm Chart

```sh
git clone https://github.com/jim60105/YoutubeLiveChatToDiscord.git
cd YoutubeLiveChatToDiscord/helm-chart
vim values.yaml
helm install [Release_Name] .
```

### Timezone

Default timezone is `Asia/Taipei`. Please change it with `TZ` environment variable.
