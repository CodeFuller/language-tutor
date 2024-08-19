using LanguageTutor.Models;

namespace LanguageTutor.Infrastructure.Sqlite.Entities
{
	internal sealed class TranslateTextExerciseResultEntity : BasicExerciseResultEntity
	{
		public int TextId { get; set; }

		public TextEntity Text { get; set; }

		public ExerciseResultType ResultType { get; set; }

		public string TypedText { get; set; }
	}
}
