CREATE TABLE [dbo].[Bank] (
    [BankId]         INT            IDENTITY (1, 1) NOT NULL,
    [BankName]       NVARCHAR (50)  NULL,
    [BankIdentifier] NVARCHAR (50)  NULL,
    [BankApiUrl]     NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([BankId] ASC)
);

