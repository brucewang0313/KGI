# 權證發行風險監控與避險試算系統

本方案包含：

- `src/WarrantRisk.Domain`：Model、DTO 與以 `decimal` 實作的核心計算服務。
- `src/WarrantRisk.Api`：ASP.NET Core Web API，使用 SQL Server 與 EF Core。
- `src/WarrantRisk.WinForms`：.NET 8 WinForms 桌面前端，支援即時計算、儲存與歷史紀錄。
- `database/01_schema_and_seed.sql`：建表與約 800 筆測試權證資料。

## 環境需求

- Windows 10/11
- .NET 8 SDK
- SQL Server 2019+（LocalDB 亦可）

## 建置與執行

1. 建立資料庫並產生測試資料：

   ```powershell
   sqlcmd -S "(localdb)\MSSQLLocalDB" -E -i .\database\01_schema_and_seed.sql
   ```

   若 LocalDB 尚未啟動，可先執行 `sqllocaldb start MSSQLLocalDB`。若該指令回報 instance registry configuration 錯誤，請以一般使用者權限重新建立或修復 SQL Server LocalDB，再重試上述指令。

2. 本方案預設使用 Windows LocalDB `MSSQLLocalDB`。如 SQL Server 實例不同，再修改 `src/WarrantRisk.Api/appsettings.json` 的 `DefaultConnection`。

3. 啟動 API：

   ```powershell
   dotnet run --project .\src\WarrantRisk.Api
   ```

   預設網址為 `http://localhost:5080`。

4. 啟動 WinForms（另開 PowerShell）：

   ```powershell
   dotnet run --project .\src\WarrantRisk.WinForms
   ```

## API

- `GET /api/warrants?keyword=2330`
- `POST /api/warrants/{warrantId}/trials`
- `GET /api/warrants/{warrantId}/trials/recent`

`MarketPrice <= 0` 會回傳 HTTP 400，且不會寫入 `Warrant_Trial_Log`。所有金融計算及 DTO 金融欄位均採 `decimal`。

## 計算規則

- CALL：`max(0, (MarketPrice - StrikePrice) * ConversionRatio)`
- PUT：`max(0, (StrikePrice - MarketPrice) * ConversionRatio)`
- `HedgeQty = PositionQty * ConversionRatio * Delta`
- ITM/ATM/OTM Delta：`0.8 / 0.5 / 0.2`

正式環境請再加入登入授權、HTTPS、輸入稽核、資料庫 migration 與風控權限控管。
