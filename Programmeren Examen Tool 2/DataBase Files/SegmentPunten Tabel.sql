CREATE TABLE [dbo].[vertices]
(
	[SegmentId] INT NOT NULL,
	[PuntId] INT NOT NULL,
	CONSTRAINT [FK_Seg_To_vertices] FOREIGN KEY ([SegmentId]) REFERENCES [dbo].[segment] ([Id]),
	CONSTRAINT [FK_Punt_To_vertices] FOREIGN KEY ([PuntId]) REFERENCES [dbo].[punt] ([Id]),
	PRIMARY KEY(SegmentId,PuntId)
)
