CREATE TABLE [Users] (
  [Id] INTEGER NOT NULL,
  [Name] ntext NOT NULL,

  CONSTRAINT [sqlite_master_PK_Users] PRIMARY KEY ([Id]),
  CONSTRAINT [sqlite_master_UC_Users] UNIQUE ([Name])
);

CREATE TABLE [Languages] (
  [Id] INTEGER NOT NULL,
  [Name] ntext NOT NULL,

  CONSTRAINT [sqlite_master_PK_Languages] PRIMARY KEY ([Id]),
  CONSTRAINT [sqlite_master_UC_Languages] UNIQUE ([Name])
);

CREATE TABLE [Texts] (
  [Id] INTEGER NOT NULL,
  [LanguageId] int NOT NULL,
  [UserId] int NULL,
  [Text] ntext NOT NULL,
  [Note] ntext NULL,

  CONSTRAINT [sqlite_master_PK_Texts] PRIMARY KEY ([Id]),
  FOREIGN KEY ([LanguageId]) REFERENCES [Languages] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE INDEX [IX_Texts_LanguageId] ON [Texts] ([LanguageId]);

CREATE TABLE [Translations] (
  [TextId1] int NOT NULL,
  [TextId2] int NOT NULL,

  CONSTRAINT [sqlite_master_PK_Translations] PRIMARY KEY ([TextId1], [TextId2]),
  FOREIGN KEY ([TextId1]) REFERENCES [Texts] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY ([TextId2]) REFERENCES [Texts] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);

-- We do not create index for TextId1, because PK index will be used for the search via (TextId1) - https://stackoverflow.com/questions/19384837
CREATE INDEX [IX_Translations_TextId2] ON [Translations] ([TextId2]);

CREATE TABLE [PronunciationRecords] (
  [Id] INTEGER NOT NULL,
  [TextId] int NOT NULL,
  [Format] int NOT NULL,
  [Source] ntext NULL,
  [Path] ntext NOT NULL,
  [DataLength] int NOT NULL,
  [DataChecksum] int NOT NULL,

  CONSTRAINT [sqlite_master_PK_PronunciationRecords] PRIMARY KEY ([Id]),
  FOREIGN KEY ([TextId]) REFERENCES [Texts] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT [sqlite_master_UC_PronunciationRecords] UNIQUE ([TextId])
);

CREATE INDEX [IX_PronunciationRecords_TextId] ON [PronunciationRecords] ([TextId]);

CREATE TABLE [CheckResults] (
  [Id] INTEGER NOT NULL,
  [UserId] int NOT NULL,
  [TextId] int NOT NULL,
  [DateTime] datetime NOT NULL,
  [ResultType] int NOT NULL,

  CONSTRAINT [sqlite_master_PK_CheckResults] PRIMARY KEY ([Id]),
  FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY ([TextId]) REFERENCES [Texts] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE [UserStatistics] (
  [UserId] int NOT NULL,
  [StudiedLanguageId] int NOT NULL,
  [KnownLanguageId] int NOT NULL,
  [Date] datetime NOT NULL,
  [TotalTextsNumber] int NOT NULL,
  [TotalLearnedTextsNumber] int NOT NULL,
  [RestNumberOfTextsToPractice] int NOT NULL,
  [NumberOfPracticedTexts] int NOT NULL,

  CONSTRAINT [sqlite_master_PK_UserStatistics] PRIMARY KEY ([UserId], [StudiedLanguageId], [KnownLanguageId], [Date]),
  FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY ([StudiedLanguageId]) REFERENCES [Languages] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY ([KnownLanguageId]) REFERENCES [Languages] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE INDEX [IX_CheckResults_UserId] ON [CheckResults] ([UserId]);
CREATE INDEX [IX_CheckResults_TextId] ON [CheckResults] ([TextId]);

INSERT INTO [Users]([Id], [Name]) VALUES(1, 'Default User');

INSERT INTO [Languages]([Id], [Name]) VALUES(1, 'Russian');
INSERT INTO [Languages]([Id], [Name]) VALUES(2, 'Polish');
