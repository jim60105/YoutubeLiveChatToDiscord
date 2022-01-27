# YoutubeLiveChatToDiscord

將Youtube聊天室塞進Discord Webhook

- 不適合用在有大量留言的狀況，此工具是設計來監控FreeChat\
    此工具最高每兩秒打一次discord webhook，可能造成轉送速度跟不上留言速度
    > Discord Webhook的限制為，同一頻道中每分鐘30次 [ref](https://twitter.com/lolpython/status/967621046277820416)
- 底層使用yt-dlp來抓資料，並不是youtube-api，沒有額度問題
- 此工具在閒置時，讀取yt-dlp寫的json檔案間隔為每10秒

## Docker

需傳入兩個參數

- 影片ID
- Discord Webhook網址

```sh
docker run -rm ghcr.io/jim60105/youtubelivechattodiscord [Video_Id] [Discord_Webhook_Url]
```
