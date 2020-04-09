CREATE TABLE [dbo].[segment]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[BeginKnoopId] INT NOT NULL,
	[EindKnoopId] INT NOT NULL,
	CONSTRAINT [FK_BeginKnp_To_Segment] FOREIGN KEY ([BeginKnoopId]) REFERENCES [dbo].[knoop] ([Id]),
	CONSTRAINT [FK_EindKnp_To_Segment] FOREIGN KEY ([EindKnoopId]) REFERENCES [dbo].[knoop] ([Id])
)
