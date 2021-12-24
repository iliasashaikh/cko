CREATE TABLE [dbo].[Customer]
(
	[CustomerId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CustomerName] NVARCHAR(100) NULL, 
    [CustomerAddress] NVARCHAR(255) NULL, 
    [CustomerReference] UNIQUEIDENTIFIER NULL
)
