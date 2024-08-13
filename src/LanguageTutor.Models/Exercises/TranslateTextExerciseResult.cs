namespace LanguageTutor.Models.Exercises
{
	public sealed class TranslateTextExerciseResult : BasicExerciseResult
	{
		// This property is filled only for ExerciseResultType.Failed.
		// Also, it could be missing for checks saved before introducing TypedText property.
		public string TypedText { get; init; }
	}
}
