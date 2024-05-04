using System;

namespace VocabularyCoach.Services.Data
{
	public class UserStatisticsData
	{
		public DateOnly Date { get; init; }

		public int TotalNumberOfTexts { get; init; }

		public int TotalNumberOfLearnedTexts { get; init; }

		public int NumberOfProblematicTexts { get; init; }

		public int RestNumberOfTextsToPracticeToday { get; init; }

		public int RestNumberOfTextsToPracticeTodayIfNoLimit { get; init; }

		public int NumberOfTextsPracticedToday { get; init; }

		public int NumberOfTextsLearnedToday { get; init; }
	}
}
