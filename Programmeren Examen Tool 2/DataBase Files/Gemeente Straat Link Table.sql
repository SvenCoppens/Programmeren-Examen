CREATE TABLE [dbo].[gemeenteStraatLink]
(
	[StraatId] INT NOT NULL,
	[GemeenteId] INT NOT NULL,
	PRIMARY KEY CLUSTERED ([StraatId] ASC, [GemeenteId] ASC),
    CONSTRAINT [FK_Straat_Gemeente] FOREIGN KEY ([StraatId]) REFERENCES [dbo].[straat] ([Id]),
    CONSTRAINT [FK_Gemeente_Straat] FOREIGN KEY ([GemeenteId]) REFERENCES [dbo].[gemeente] ([Id])

)
