using System;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;

namespace LanguageTutor.Services.UnitTests.Helpers
{
	internal static class DateTimeExtensions
	{
		public static TranslateTextExerciseResult ToSuccessfulExerciseResult(this DateTime dateTime)
		{
			return new TranslateTextExerciseResult(dateTime, ExerciseResultType.Successful, typedText: null);
		}

		public static TranslateTextExerciseResult ToFailedExerciseResult(this DateTime dateTime)
		{
			return new TranslateTextExerciseResult(dateTime, ExerciseResultType.Failed, typedText: null);
		}
	}
}
