using System;

namespace LanguageTutor.Services.Data
{
	public class UserStatisticsData
	{
		public DateOnly Date { get; init; }

		public int TotalNumberOfExercises { get; init; }

		public int TotalNumberOfLearnedExercises { get; init; }

		public int NumberOfProblematicExercises { get; init; }

		public int RestNumberOfExercisesToPerformToday { get; init; }

		public int RestNumberOfExercisesToPerformTodayIfNoLimit { get; init; }

		public int NumberOfExercisesPerformedToday { get; init; }

		public int NumberOfExercisesLearnedToday { get; init; }
	}
}
