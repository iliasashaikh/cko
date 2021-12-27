CREATE TABLE [dbo].[Customer] (
    [CustomerId]        INT              IDENTITY (1, 1) NOT NULL,
    [CustomerName]      NVARCHAR (100)   NULL,
    [CustomerAddress]   NVARCHAR (255)   NULL,
    [CustomerReference] UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([CustomerId] ASC)
);

