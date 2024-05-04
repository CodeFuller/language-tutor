using System;

namespace VocabularyCoach.Infrastructure.Sqlite.Entities
{
	internal sealed class UserStatisticsEntity
	{
		public int UserId { get; set; }

		public int StudiedLanguageId { get; set; }

		public int KnownLanguageId { get; set; }

		public DateOnly Date { get; set; }

		public int TotalNumberOfTexts { get; set; }

		public int TotalNumberOfLearnedTexts { get; set; }

		public int NumberOfProblematicTexts { get; set; }

		public int RestNumberOfTextsToPracticeToday { get; set; }

		public int RestNumberOfTextsToPracticeTodayIfNoLimit { get; set; }

		public int NumberOfTextsPracticedToday { get; set; }

		public int NumberOfTextsLearnedToday { get; set; }
	}
}
