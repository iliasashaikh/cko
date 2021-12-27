CREATE TABLE [dbo].[PaymentCard] (
    [PaymentCardId]      INT              IDENTITY (1, 1) NOT NULL,
    [BankIdentifierCode] NVARCHAR (50)    NULL,
    [CardExpiry]         DATETIME2 (7)    NULL,
    [CardNumber]         NVARCHAR (50)    NULL,
    [CustomerReference]  UNIQUEIDENTIFIER NULL,
    [CustomerName]       NVARCHAR (100)   NULL,
    [CustomerAddress]    NVARCHAR (255)   NULL,
    [Cvv]                NVARCHAR (10)    NULL,
    PRIMARY KEY CLUSTERED ([PaymentCardId] ASC)
);

