/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

-- 1. Add database CompanyDb if not exists

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'CompanyDb')
BEGIN
    CREATE DATABASE CompanyDb;
END
GO

-- Use the CompanyDb database
USE CompanyDb;
GO

-- 2. Add table Company if not exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Company')
BEGIN
    CREATE TABLE [dbo].[Company]
    (
        [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
        [Name] NVARCHAR(50) NOT NULL, 
        [Exchange] NVARCHAR(50) NULL, 
        [Ticker] NVARCHAR(50) NULL, 
        [ISIN] NVARCHAR(50) NOT NULL unique, 
        [Website] NVARCHAR(MAX) NULL, 
        [CreatedOn] DATETIME NOT NULL, 
        [UpdatedOn] DATETIME NULL
    );
    CREATE INDEX IDX_Company_Id ON [dbo].[Company]([Id]);
    CREATE INDEX IDX_Company_ISIN ON [dbo].[Company]([ISIN]);
END
GO

-- 3. Add 3 dummy entries to Company table
INSERT INTO [dbo].[Company] ([Name], [Exchange], [Ticker], [ISIN], [Website], [CreatedOn], [UpdatedOn])
VALUES 
('Company A', 'NYSE', 'CMPA', 'US1234567890', 'http://companya.com', GETDATE(), NULL),
('Company B', 'NASDAQ', 'CMPB', 'US0987654321', 'http://companyb.com', GETDATE(), NULL),
('Company C', 'NYSE', 'CMPC', 'US1122334455', 'http://companyc.com', GETDATE(), NULL);
GO

