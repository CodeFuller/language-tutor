using System;

namespace LanguageTutor.Models.Exercises
{
	public abstract class BasicExerciseResult
	{
		public ItemId Id { get; set; }

		public DateTimeOffset DateTime { get; init; }

		public ExerciseResultType ResultType { get; init; }

		public bool IsSuccessful => ResultType == ExerciseResultType.Successful;

		public bool IsFailed => !IsSuccessful;
	}
}
