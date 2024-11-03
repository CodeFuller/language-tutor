using System.Collections.Generic;
using System.Linq;

namespace LanguageTutor.MergeDatabases
{
	internal sealed class MergeUpdate
	{
		public int AddedTexts { get; set; }

		public int UpdatedTexts { get; set; }

		public int DeletedTexts { get; set; }

		public int AddedTranslations { get; set; }

		public int DeletedTranslations { get; set; }

		public int AddedInflectWordExercises { get; set; }

		public int UpdatedInflectWordExercises { get; set; }

		public int DeletedInflectWordExercises { get; set; }

		public int AddedPronunciationRecords { get; set; }

		public int UpdatedPronunciationRecords { get; set; }

		public int DeletedPronunciationRecords { get; set; }

		public ICollection<(string SourcePath, string TargetPath)> PronunciationRecordsToAddToFileStorage { get; } = [];

		public ICollection<string> PronunciationRecordsToDeleteFromFileStorage { get; } = [];

		public bool IsEmpty => AddedTexts == 0 && UpdatedTexts == 0 && DeletedTexts == 0 &&
							   AddedTranslations == 0 && DeletedTranslations == 0 &&
							   AddedInflectWordExercises == 0 && UpdatedInflectWordExercises == 0 && DeletedInflectWordExercises == 0 &&
							   AddedPronunciationRecords == 0 && UpdatedPronunciationRecords == 0 && DeletedPronunciationRecords == 0 &&
							   !PronunciationRecordsToAddToFileStorage.Any() && !PronunciationRecordsToDeleteFromFileStorage.Any();
	}
}
