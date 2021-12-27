CREATE TABLE [dbo].[Merchant] (
    [MerchantId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]       VARCHAR (100) NULL,
    [Address]    VARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([MerchantId] ASC)
);

