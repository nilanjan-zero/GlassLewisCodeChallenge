CREATE TABLE [dbo].[Company]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Name] NVARCHAR(50) NOT NULL, 
    [Exchange] NVARCHAR(50) NULL, 
    [Ticker] NVARCHAR(50) NULL, 
    [ISIN] NVARCHAR(50) NOT NULL unique, 
    [Website] NVARCHAR(MAX) NULL, 
    [CreatedOn] DATETIME NOT NULL, 
    [UpdatedOn] DATETIME NULL,
);
GO;
CREATE INDEX IDX_Company_Id ON [dbo].[Company]([Id]);
GO;
CREATE INDEX IDX_Company_ISIN ON [dbo].[Company]([ISIN]);