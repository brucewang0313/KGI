# 權證發行風險監控與避險試算

這是一個用來練習權證試算流程的小型系統。畫面使用 WinForms，後端使用 ASP.NET Core Web API，資料放在 SQL Server / LocalDB。

## 專案內容

```text
src/WarrantRisk.Domain       權證模型與試算公式
src/WarrantRisk.Api          API、資料庫存取
src/WarrantRisk.WinForms     Windows 桌面畫面
database/                    建表和測試資料
```

我把計算放在 Domain 專案，這樣 API 和 WinForms 可以共用同一份公式，不用各寫一份。

## 架構說明

系統大致分成三層：

```text
WinForms 使用者介面
        │ HTTP
        ▼
ASP.NET Core Web API
        │ EF Core
        ▼
SQL Server / LocalDB
```

### WarrantRisk.WinForms

這是使用者操作的桌面程式，負責：

- 顯示權證清單和搜尋結果
- 顯示選取權證的基本資料
- 接收使用者輸入的標的股價
- 輸入時即時計算理論價值和建議避險張數
- 呼叫 API 儲存試算結果
- 顯示最近 10 筆歷史紀錄

WinForms 不直接連資料庫，所有查詢和儲存都透過 API 完成。

### WarrantRisk.Api

這是後端 API，負責：

- 接收 WinForms 的查詢和試算請求
- 從 `Warrant_Master` 查詢權證資料
- 呼叫 Domain 的計算方法
- 檢查標的股價是否大於 0
- 將試算結果寫入 `Warrant_Trial_Log`
- 回傳 JSON 給 WinForms

真正寫入資料庫前，API 會重新計算一次，不直接相信前端送來的計算結果。

### WarrantRisk.Domain

這是共用的核心邏輯，放置：

- `Warrant` 和 `TrialLog` 資料模型
- `TrialRequest` 和 `TrialResult` DTO
- `WarrantCalculator` 試算方法

核心計算不依賴 WinForms 或 SQL Server，因此 API 和 WinForms 都能共用同一套規則。

### 一次試算的流程

```text
1. 使用者在 WinForms 選取權證
2. 輸入標的股價
3. WinForms 使用 Domain 顯示即時預覽
4. 使用者按下「儲存試算結果」
5. WinForms 呼叫 POST /api/warrants/{id}/trials
6. API 查詢權證並重新執行計算
7. API 將結果寫入 Warrant_Trial_Log
8. WinForms 重新載入最近 10 筆紀錄
```

## 開發環境

- Windows 10/11
- .NET 8 SDK
- SQL Server LocalDB（執行個體名稱：`MSSQLLocalDB`）

## 第一次執行

請在方案根目錄，也就是有 `WarrantRiskMonitoring.slnx` 的資料夾執行：

```powershell
sqllocaldb start MSSQLLocalDB
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -i .\database\01_schema_and_seed.sql
dotnet run --project .\src\WarrantRisk.Api
```

API 啟動後，再開另一個 PowerShell：

```powershell
dotnet run --project .\src\WarrantRisk.WinForms
```

API 預設網址是 `http://localhost:5080`，連線設定在 `src/WarrantRisk.Api/appsettings.json`。

## API

```text
GET  /api/warrants?keyword=W000
GET  /api/warrants/{id}
POST /api/warrants/{id}/trials
GET  /api/warrants/{id}/trials/recent
```

試算請求範例：

```json
{
  "marketPrice": 250
}
```

標的股價必須大於 0。API 會重新計算結果，確認無誤後才寫入試算紀錄。

## 計算方式

```text
CALL 理論價 = max(0, (標的股價 - 履約價格) × 行使比例)
PUT  理論價 = max(0, (履約價格 - 標的股價) × 行使比例)
避險張數    = 庫存張數 × 行使比例 × Delta
```

本練習依題目指定的簡化規則使用 Delta：

```text
價內 0.8、價平 0.5、價外 0.2
```

目前的 SQL 腳本會刪除後重新建立兩張表，適合開發環境初始化，不要直接拿去正式環境執行。正式環境還需要登入權限、HTTPS、Migration、稽核紀錄和正式的資料庫部署流程。
