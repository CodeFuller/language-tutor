using System;

namespace LanguageTutor.Infrastructure.Sqlite.Entities
{
	internal sealed class UserStatisticsEntity
	{
		public int UserId { get; set; }

		public int StudiedLanguageId { get; set; }

		public int KnownLanguageId { get; set; }

		public DateOnly Date { get; set; }

		public int TotalNumberOfExercises { get; set; }

		public int TotalNumberOfLearnedExercises { get; set; }

		public int NumberOfProblematicExercises { get; set; }

		public int RestNumberOfExercisesToPerformToday { get; set; }

		public int RestNumberOfExercisesToPerformTodayIfNoLimit { get; set; }

		public int NumberOfExercisesPerformedToday { get; set; }

		public int NumberOfExercisesLearnedToday { get; set; }
	}
}
