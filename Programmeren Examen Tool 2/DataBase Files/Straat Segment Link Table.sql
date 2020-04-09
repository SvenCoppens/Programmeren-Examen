CREATE TABLE [dbo].[straatSegmenten]
(
	[StraatId] INT NOT NULL,
	[SegmentId] INT NOT NULL,
	PRIMARY KEY CLUSTERED ([StraatId] ASC, [SegmentId] ASC),
    CONSTRAINT [FK_Straat_Segment] FOREIGN KEY ([StraatId]) REFERENCES [dbo].[straat] ([Id]),
    CONSTRAINT [FK_Segment_Straat] FOREIGN KEY ([SegmentId]) REFERENCES [dbo].[segment] ([Id])
)
