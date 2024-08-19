namespace LanguageTutor.Infrastructure.Sqlite.Entities
{
	internal sealed class InflectWordExerciseResultEntity : BasicExerciseResultEntity
	{
		public int ExerciseId { get; set; }

		public string FormResults { get; set; }
	}
}
