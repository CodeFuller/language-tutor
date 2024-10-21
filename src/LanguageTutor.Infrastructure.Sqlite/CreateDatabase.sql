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

CREATE TABLE [UserSettings] (
  [UserId] int NOT NULL,
  [LastStudiedLanguageId] int NULL,
  [LastKnownLanguageId] int NULL,

  CONSTRAINT [sqlite_master_PK_UserSettings] PRIMARY KEY ([UserId]),
  FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY ([LastStudiedLanguageId]) REFERENCES [Languages] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE,
  FOREIGN KEY ([LastKnownLanguageId]) REFERENCES [Languages] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE
);

CREATE TABLE [Texts] (
  [Id] INTEGER NOT NULL,
  [LanguageId] int NOT NULL,
  [UserId] int NULL,
  [Text] ntext NOT NULL,
  [Note] ntext NULL,
  [CreationTimestamp] datetime NOT NULL,

  CONSTRAINT [sqlite_master_PK_Texts] PRIMARY KEY ([Id]),
  FOREIGN KEY ([LanguageId]) REFERENCES [Languages] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE INDEX [IX_Texts_LanguageId] ON [Texts] ([LanguageId]);

CREATE TABLE [Translations] (
  [Text1Id] int NOT NULL,
  [Text2Id] int NOT NULL,

  CONSTRAINT [sqlite_master_PK_Translations] PRIMARY KEY ([Text1Id], [Text2Id]),
  FOREIGN KEY ([Text1Id]) REFERENCES [Texts] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY ([Text2Id]) REFERENCES [Texts] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);

-- We do not create index for Text1Id, because PK index will be used for the search via (Text1Id) - https://stackoverflow.com/questions/19384837
CREATE INDEX [IX_Translations_Text2Id] ON [Translations] ([Text2Id]);

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

CREATE TABLE [TranslateTextExerciseResults] (
  [Id] INTEGER NOT NULL,
  [UserId] int NOT NULL,
  [TextId] int NOT NULL,
  [DateTime] datetime NOT NULL,
  [ResultType] int NOT NULL,
  [TypedText] ntext NULL,

  CONSTRAINT [sqlite_master_PK_TranslateTextExerciseResults] PRIMARY KEY ([Id]),
  FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY ([TextId]) REFERENCES [Texts] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE INDEX [IX_TranslateTextExerciseResults_UserId] ON [TranslateTextExerciseResults] ([UserId]);
CREATE INDEX [IX_TranslateTextExerciseResults_TextId] ON [TranslateTextExerciseResults] ([TextId]);

CREATE TABLE [InflectWordExercises] (
  [Id] INTEGER NOT NULL,
  [LanguageId] int NOT NULL,
  [TemplateId] int NULL,
  [Description] ntext NULL,
  [BaseForm] ntext NULL,
  [WordForms] ntext NOT NULL,
  [CreationTimestamp] datetime NOT NULL,

  CONSTRAINT [sqlite_master_PK_InflectWordExercises] PRIMARY KEY ([Id]),
  FOREIGN KEY ([LanguageId]) REFERENCES [Languages] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE INDEX [IX_InflectWordExercises_LanguageId] ON [InflectWordExercises] ([LanguageId]);

CREATE TABLE [InflectWordExerciseResults] (
  [Id] INTEGER NOT NULL,
  [UserId] int NOT NULL,
  [ExerciseId] int NOT NULL,
  [DateTime] datetime NOT NULL,
  [FormResults] ntext NOT NULL,

  CONSTRAINT [sqlite_master_PK_InflectWordExerciseResults] PRIMARY KEY ([Id]),
  FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY ([ExerciseId]) REFERENCES [InflectWordExercises] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE INDEX [IX_InflectWordExerciseResults_UserId] ON [InflectWordExerciseResults] ([UserId]);

CREATE TABLE [UserStatistics] (
  [UserId] int NOT NULL,
  [StudiedLanguageId] int NOT NULL,
  [KnownLanguageId] int NOT NULL,
  [Date] datetime NOT NULL,
  [TotalNumberOfExercises] int NOT NULL,
  [TotalNumberOfLearnedExercises] int NOT NULL,
  [NumberOfProblematicExercises] int NOT NULL,
  [RestNumberOfExercisesToPerformToday] int NOT NULL,
  [RestNumberOfExercisesToPerformTodayIfNoLimit] int NOT NULL,
  [NumberOfExercisesPerformedToday] int NOT NULL,
  [NumberOfExercisesLearnedToday] int NOT NULL,

  CONSTRAINT [sqlite_master_PK_UserStatistics] PRIMARY KEY ([UserId], [StudiedLanguageId], [KnownLanguageId], [Date]),
  FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY ([StudiedLanguageId]) REFERENCES [Languages] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY ([KnownLanguageId]) REFERENCES [Languages] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
);

INSERT INTO [Users]([Id], [Name]) VALUES(1, 'Default User');

INSERT INTO [Languages]([Id], [Name]) VALUES(1, 'Russian');
INSERT INTO [Languages]([Id], [Name]) VALUES(2, 'Polish');
