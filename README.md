# SlotGame SQL語法產生器

用於SlotGame 開新遊戲,快速產生塞DB資料的產生器

## slot.csv
| Ename | Cname | GameId | Reel | PayLine | RTP | Fun | Device |
| ------| ----- | ------ | ---- | ------- | ----| --- | ------ |
| Xiang Qi Ways 2 | 象棋之道 2 | 10042 | 3 * 4 * 5 * 4 * 3 | 720 Ways | 97.04% |1 | 0 |
| Cookie Hunter | 打打姜饼人 | 10043 |5 * 3 | 20 Lines | 97.08% | 1 | 0

- Ename: 英文遊戲名稱
- Cname: 簡中遊戲名稱
- GameId: 第三方定義的遊戲Id
- Reel: 轉輪(遊戲玩法)
- PayLine: 賠率線
- RTP: 玩家回報率
- Fun: 試玩遊戲(1:是、2:否)
- Device: 裝置(0:all、1:web、2:mobile、3:app)

## 流程

1. slot.csv 設定好要新增的遊戲
2. 執行 `SlotGameGenerator.exe`
3. 輸入TpId
4. 輸入遊戲開關 `遊戲開關(0:關閉、1:開啟、3:Release階段)`
5. 產生結果slot.sql

## 下載

### v0.1.1
[SlotGameGenerator-v0.1.1.zip](https://ailab-tool.s3.ap-southeast-1.amazonaws.com/SlotGameGenerator/SlotGameGenerator-v0.1.1.zip)

### v0.1.0
[SlotGameGenerator-v0.1.0.zip](https://ailab-tool.s3.ap-southeast-1.amazonaws.com/SlotGameGenerator/SlotGameGenerator-v0.1.0.zip)
