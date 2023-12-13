# Youtube Live Chat To Discord

[![CodeFactor](https://www.codefactor.io/repository/github/jim60105/youtubelivechattodiscord/badge/master?style=for-the-badge)](https://www.codefactor.io/repository/github/jim60105/youtubelivechattodiscord/overview/master)

> [!CAUTION]  
> 請留意我所使用的 **AGPLv3** 授權條款。  
> 你 _**必須**_ 將 **原始碼** 公開給 **任何能存取到服務的人** (服務，也就是指此程式所發布的 Discord 訊息)。  
> 請分享此 GitHub 儲存庫的網址，或是公開修改過的原始碼。

## 將 Youtube 聊天室串流至 Discord Webhook

|                                                Youtube Live Chat                                                |     |                                                 Discord Webhook                                                 |
| :-------------------------------------------------------------------------------------------------------------: | :-: | :-------------------------------------------------------------------------------------------------------------: |
| ![image](https://user-images.githubusercontent.com/16995691/151545455-af26cbe6-0942-464a-b15e-76ca67dfa142.png) | ➡️  | ![image](https://user-images.githubusercontent.com/16995691/151438025-d0c4a2de-6845-4d64-93db-89afb2f98e45.png) |
| ![image](https://user-images.githubusercontent.com/16995691/151545035-0dfc65e3-41a4-4342-b0c4-178b53a077d6.png) | ➡️  | ![image](https://user-images.githubusercontent.com/16995691/151545242-651cdbd1-ae8c-4a47-acda-7b9a3b4f59ba.png) |
| ![image](https://user-images.githubusercontent.com/16995691/151663570-999a5c8c-a336-407e-906a-56399530417b.png) | ➡️  | ![image](https://user-images.githubusercontent.com/16995691/151663574-dc5abbc2-cb5d-4e40-a4ce-bfc39f2a7029.png) |

<p align="center">
  <a href="https://github.com/jim60105/YoutubeLiveChatToDiscord/blob/master/README.md">
    English
  </a> |
  <span>中文</span>
</p>

- 不適合用在有大量留言的狀況，此工具是設計來監控 FreeChat  
  此工具最高每兩秒打一次 discord webhook ，可能造成轉送速度跟不上留言速度
  > Discord 方面的限制為，同一頻道中每分鐘可呼叫 Webhook 30 次 [ref](https://twitter.com/lolpython/status/967621046277820416)  
  > 若同時啟動複數此工具並推送至同一個頻道，很容易觸發 Discord 冷卻，請留意你的使用環境
- 底層使用 yt-dlp 來抓資料，並不是 youtube api ，沒有 API 額度問題
- 此工具在閒置時，讀取 yt-dlp 寫的 json 檔案間隔為 10 秒
- 啟動時會等待 1 分鐘，使 yt-dlp 下載舊留言，再由此開始監控
  > 如果要跳過此 1 分鐘，請傳入環境變數 `SKIP_STARTUP_WAITING`
- 在程式同一層若存在名為 `cookies.txt` 的檔案，就會讀入 yt-dlp，使能在會員限定直播使用

## 會員限定 (需登入) 的影片

在程式的執行目錄若存在名為 `cookies.txt` 的檔案，它會自動使用

Docker 請將 `cookies.txt` mount 至 `/app/cookies.txt`

## Docker

> 請參考 `docker-compose.yml`

需傳入兩個參數

- 影片 ID
- Discord Webhook 網址

```sh
docker run --rm ghcr.io/jim60105/youtubelivechattodiscord [Video_Id] [Discord_Webhook_Url]
```

也可在[quay.io](https://quay.io/jim60105/youtubelivechattodiscord)取得。

## Kubernetes Helm Chart

```sh
git clone https://github.com/jim60105/YoutubeLiveChatToDiscord.git
cd YoutubeLiveChatToDiscord/helm-chart
vim values.yaml
helm install [Release_Name] .
```

### Timezone

預設時區為 `Asia/Taipei`。請使用 `TZ` 環境變數進行更改。

## LICENSE

[![AGPL-3.0](https://github.com/jim60105/YoutubeLiveChatToDiscord/assets/16995691/8c588957-5e07-4f6d-a116-b3366c064342)](LICENSE)

[GNU AFFERO GENERAL PUBLIC LICENSE Version 3](LICENSE)

This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.

> [!CAUTION]  
> 請留意我們使用的 **AGPLv3** 授權條款。  
> 你 _**必須**_ 將 **原始碼** 公開給 **任何能存取到服務的人** (也就是此程式所發布的 Discord 訊息)。  
> 請分享此 GitHub 儲存庫的網址，或是公開修改過的原始碼。
