CREATE TABLE [dbo].[Payment] (
    [PaymentId]        INT             NOT NULL,
    [PaymentReference] NVARCHAR (10)   NULL,
    [MerchantId]       INT             NULL,
    [BankId]           INT             NULL,
    [PaymentTime]      DATETIME2 (7)   NULL,
    [Status]           NVARCHAR (10)   NULL,
    [Amount]           DECIMAL (18, 3) NULL,
    [Ccy]              NVARCHAR (3)    NULL,
    [CreatedTime]      DATETIME2 (7)   DEFAULT (getdate()) NOT NULL,
    [CustomerReference] UNIQUEIDENTIFIER NULL, 
    PRIMARY KEY CLUSTERED ([PaymentId] ASC)
);

