CREATE TABLE [dbo].[knoop] (
    [Id]     INT  NOT NULL,
    [PuntId] INT NOT NULL,
    CONSTRAINT [PK_knoop] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Punt_To_Knoop] FOREIGN KEY ([PuntId]) REFERENCES [dbo].[punt] ([Id])
);

