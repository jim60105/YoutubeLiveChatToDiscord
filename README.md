# Youtube Live Chat To Discord

[![CodeFactor](https://www.codefactor.io/repository/github/jim60105/youtubelivechattodiscord/badge/master)](https://www.codefactor.io/repository/github/jim60105/youtubelivechattodiscord/overview/master) [![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fjim60105%2FYoutubeLiveChatToDiscord.svg?type=small)](https://app.fossa.com/projects/git%2Bgithub.com%2Fjim60105%2FYoutubeLiveChatToDiscord?ref=badge_small)

> [!CAUTION]  
> Please take note of the **AGPLv3** license that we are using.  
> You _**MUST**_ share **the source code** with **anyone who can access the services** (service, which means the Discord messages published by this program).  
> Share the URL of this GitHub repository, or publish the modified source code if any changes were made.

## Stream Youtube chat to Discord Webhook

|                                                  Youtube Live Chat                                                  |     |                                                   Discord Webhook                                                   |
| :-----------------------------------------------------------------------------------------------------------------: | :-: | :-----------------------------------------------------------------------------------------------------------------: |
| ![image](https://github.com/jim60105/YoutubeLiveChatToDiscord/assets/16995691/a979ae6a-8b99-4887-92bb-e08773f9c064) | ➡️  | ![image](https://github.com/jim60105/YoutubeLiveChatToDiscord/assets/16995691/2e58c0b6-6a34-4664-afd9-c16ea378987a) |
|   ![image](https://user-images.githubusercontent.com/16995691/151545455-af26cbe6-0942-464a-b15e-76ca67dfa142.png)   | ➡️  |   ![image](https://user-images.githubusercontent.com/16995691/151438025-d0c4a2de-6845-4d64-93db-89afb2f98e45.png)   |
| ![image](https://github.com/jim60105/YoutubeLiveChatToDiscord/assets/16995691/4d8d6417-4dda-4c42-a179-da7557d6a608) | ➡️  | ![image](https://github.com/jim60105/YoutubeLiveChatToDiscord/assets/16995691/77b4aced-0f82-48be-a591-fa351e1e5246) |
|   ![image](https://user-images.githubusercontent.com/16995691/151663570-999a5c8c-a336-407e-906a-56399530417b.png)   | ➡️  |   ![image](https://user-images.githubusercontent.com/16995691/151663574-dc5abbc2-cb5d-4e40-a4ce-bfc39f2a7029.png)   |
| ![image](https://github.com/jim60105/YoutubeLiveChatToDiscord/assets/16995691/c62951ef-1268-462f-8955-a5f507b9be43) | ➡️  | ![image](https://github.com/jim60105/YoutubeLiveChatToDiscord/assets/16995691/7307d06e-5cc9-4fd4-a489-2dd21b29abc6) |
| ![image](https://github.com/jim60105/YoutubeLiveChatToDiscord/assets/16995691/d6d3338f-846e-4ee8-8a74-85d0b4d0479b) | ➡️  | ![image](https://github.com/jim60105/YoutubeLiveChatToDiscord/assets/16995691/6b72474d-99c6-4006-a165-d25ca4cb7474) |

<p align="center">
  <span>English</span> |
  <a href="https://github.com/jim60105/YoutubeLiveChatToDiscord/blob/master/README.zh.md">
    中文
  </a>
</p>

- The underlying implementation uses yt-dlp instead of the YouTube API, so there is no API quota limit.
- When this tool is idle, it reads the JSON file generated by yt-dlp every 10 seconds.
- Upon startup, it waits for 1 minute to skip old chats before starting monitoring.
  > If you want to skip this waiting and start immediately, please pass the environment variable `SKIP_STARTUP_WAITING`.
- It can monitor membership-only live streams by automatically detect and import the `cookies.txt` file in the execution directory into yt-dlp.
- It is not suitable for for scenarios with a high message speed.
  It sends a maximum of one Discord webhook every two seconds, which may cause delays if the new chat speed exceeds the forwarding speed.
  > Discord has a limitation that allows calling webhooks up to 30 times per minute in the same channel [ref](https://twitter.com/lolpython/status/967621046277820416).  
  > If multiple instances of this tool are simultaneously running and pushed to the same channel, it's easy to trigger Discord cooldown. Please be aware of your usage environment.

## Membership-only (login required) videos

If a file named `cookies.txt` exists in the program's execution directory, it will be used automatically.

For Docker, please mount `cookies.txt` to `/app/cookies.txt`.

## Docker

> Please refer to `docker-compose.yml`.

Two parameters need to be passed in:

- Video ID
- Discord Webhook URL

```sh
docker run --rm ghcr.io/jim60105/youtubelivechattodiscord [Video_Id] [Discord_Webhook_Url]
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

## LICENSE

[![AGPL-3.0](https://github.com/jim60105/YoutubeLiveChatToDiscord/assets/16995691/8c588957-5e07-4f6d-a116-b3366c064342)](LICENSE)

[GNU AFFERO GENERAL PUBLIC LICENSE Version 3](LICENSE)

This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.

> [!CAUTION]  
> Please take note of the **AGPLv3** license that we are using.  
> You _**MUST**_ share **the source code** with **anyone who can access the services** (the Discord messages published by this program).  
> Share the URL of this GitHub repository, or publish the modified source code if any changes were made.
