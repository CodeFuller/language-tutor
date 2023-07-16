namespace VocabularyCoach.Services.Data
{
	public class UserStatisticsData
	{
		public int TotalNumberOfTexts { get; init; }

		public int TotalNumberOfLearnedTexts { get; init; }

		public int RestNumberOfTextsToPracticeToday { get; init; }

		public int NumberOfTextsPracticedToday { get; init; }
	}
}
