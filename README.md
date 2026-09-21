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

如果目前已經切換到某個專案資料夾，就直接執行 `dotnet run`，不要再重複加 `src\...` 路徑。

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
