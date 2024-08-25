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

		public ICollection<(string SourcePath, string TargetPath)> PronunciationRecordsToAdd { get; } = [];

		public ICollection<string> PronunciationRecordsToDelete { get; } = [];

		public bool IsEmpty => AddedTexts == 0 && UpdatedTexts == 0 && DeletedTexts == 0 &&
		                       AddedTranslations == 0 && DeletedTranslations == 0 &&
		                       AddedInflectWordExercises == 0 && UpdatedInflectWordExercises == 0 && DeletedInflectWordExercises == 0 &&
		                       !PronunciationRecordsToAdd.Any() && !PronunciationRecordsToDelete.Any();
	}
}
