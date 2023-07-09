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

CREATE TABLE [Translations] (
  [TextId1] int NOT NULL,
  [TextId2] int NOT NULL,
  CONSTRAINT [sqlite_master_PK_Translations] PRIMARY KEY ([TextId1], [TextId2]),
  FOREIGN KEY ([TextId1]) REFERENCES [Texts] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY ([TextId2]) REFERENCES [Texts] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE [PronunciationRecords] (
  [Id] INTEGER NOT NULL,
  [TextId] int NOT NULL,
  [Format] int NOT NULL,
  [Source] ntext NULL,
  [Path] ntext NOT NULL,
  CONSTRAINT [sqlite_master_PK_PronunciationRecords] PRIMARY KEY ([Id]),
  FOREIGN KEY ([TextId]) REFERENCES [Texts] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT [sqlite_master_UC_PronunciationRecords] UNIQUE ([TextId])
);

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

INSERT INTO [Users]([Id], [Name]) VALUES(1, 'Default User');

INSERT INTO [Languages]([Id], [Name]) VALUES(1, 'Russian');
INSERT INTO [Languages]([Id], [Name]) VALUES(2, 'Polish');
