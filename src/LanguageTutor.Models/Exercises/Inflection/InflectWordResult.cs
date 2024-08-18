namespace LanguageTutor.Models.Exercises.Inflection
{
	public class InflectWordResult
	{
		public string FormHint { get; init; }

		public ExerciseResultType ResultType { get; init; }

		// This property is filled only if ResultType == ExerciseResultType.Failed.
		public string TypedWord { get; init; }
	}
}
