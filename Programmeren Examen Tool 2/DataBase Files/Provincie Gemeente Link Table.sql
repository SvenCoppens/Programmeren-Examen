CREATE TABLE [dbo].[provincieGemeenteLink]
(
	[ProvincieId] INT NOT NULL,
	[GemeenteId] INT NOT NULL,
	CONSTRAINT [FK_Provincie_Gemeente] FOREIGN KEY ([ProvincieId]) REFERENCES [dbo].[provincie] ([Id]),
    CONSTRAINT [FK_Gemeente_Provincie] FOREIGN KEY ([GemeenteId]) REFERENCES [dbo].[gemeente] ([Id]),
	PRIMARY KEY(ProvincieId,GemeenteId)
)
