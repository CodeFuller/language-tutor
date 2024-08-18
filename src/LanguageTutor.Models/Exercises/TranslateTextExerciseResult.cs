using System;

namespace LanguageTutor.Models.Exercises
{
	public sealed class TranslateTextExerciseResult : BasicExerciseResult
	{
		public override ExerciseResultType ResultType { get; }

		// This property is filled only for ExerciseResultType.Failed.
		// Also, it could be missing for checks saved before introducing TypedText property.
		public string TypedText { get; init; }

		public TranslateTextExerciseResult(DateTimeOffset dateTime, ExerciseResultType resultType, string typedText)
		{
			DateTime = dateTime;
			ResultType = resultType;
			TypedText = typedText;
		}
	}
}
