IF DB_ID(N'WarrantRiskDb') IS NULL CREATE DATABASE WarrantRiskDb;
GO
USE WarrantRiskDb;
GO
IF OBJECT_ID(N'dbo.Warrant_Trial_Log', N'U') IS NOT NULL DROP TABLE dbo.Warrant_Trial_Log;
IF OBJECT_ID(N'dbo.Warrant_Master', N'U') IS NOT NULL DROP TABLE dbo.Warrant_Master;
CREATE TABLE dbo.Warrant_Master (
 Warrant_ID varchar(10) NOT NULL CONSTRAINT PK_Warrant_Master PRIMARY KEY,
 Strike_Price decimal(18,4) NOT NULL CONSTRAINT CK_Warrant_Strike CHECK (Strike_Price > 0),
 Conversion_Ratio decimal(18,4) NOT NULL CONSTRAINT CK_Warrant_Ratio CHECK (Conversion_Ratio >= 0),
 Warrant_Type varchar(4) NOT NULL CONSTRAINT CK_Warrant_Type CHECK (Warrant_Type IN ('CALL','PUT')),
 Position_Qty int NOT NULL CONSTRAINT CK_Warrant_Position CHECK (Position_Qty >= 0)
);
CREATE TABLE dbo.Warrant_Trial_Log (
 Log_ID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Warrant_Trial_Log PRIMARY KEY,
 Warrant_ID varchar(10) NOT NULL CONSTRAINT FK_Trial_Warrant REFERENCES dbo.Warrant_Master(Warrant_ID),
 Market_Price decimal(18,4) NOT NULL CONSTRAINT CK_Trial_Market CHECK (Market_Price > 0),
 Theory_Price decimal(18,4) NOT NULL,
 Hedge_Qty decimal(18,2) NOT NULL,
 Created_Time datetime NOT NULL CONSTRAINT DF_Trial_Created DEFAULT GETDATE()
);
;WITH N AS (SELECT TOP (800) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n FROM sys.all_objects a CROSS JOIN sys.all_objects b)
INSERT dbo.Warrant_Master (Warrant_ID, Strike_Price, Conversion_Ratio, Warrant_Type, Position_Qty)
SELECT CONCAT('W', RIGHT('000000000' + CONVERT(varchar(9), n), 9)), CONVERT(decimal(18,4), 50 + ((n * 37) % 450)), CONVERT(decimal(18,4), 0.1000 + ((n % 10) * 0.0500)), CASE WHEN n % 2 = 0 THEN 'CALL' ELSE 'PUT' END, 100 + ((n * 17) % 9901) FROM N;
GO
