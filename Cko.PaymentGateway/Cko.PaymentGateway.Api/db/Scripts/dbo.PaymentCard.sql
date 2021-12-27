CREATE TABLE [dbo].[PaymentCard]
(
	[PaymentCardId] INT NOT NULL PRIMARY KEY, 
    [BankIdentifierCode] NVARCHAR(50) NULL, 
    [CardExpiry] DATETIME2 NULL, 
    [CardNumber] NVARCHAR(50) NULL, 
    [CustomerReference] UNIQUEIDENTIFIER NULL, 
    [CustomerName] NVARCHAR(100) NULL, 
    [CustomerAddress] NVARCHAR(255) NULL, 
    [Cvv] NVARCHAR(10) NULL,

)
