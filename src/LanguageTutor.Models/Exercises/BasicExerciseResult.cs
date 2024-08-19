using System;

namespace LanguageTutor.Models.Exercises
{
	public abstract class BasicExerciseResult
	{
		public DateTimeOffset DateTime { get; init; }

		public abstract ExerciseResultType ResultType { get; }

		public bool IsSuccessful => ResultType == ExerciseResultType.Successful;

		public bool IsFailed => !IsSuccessful;
	}
}
