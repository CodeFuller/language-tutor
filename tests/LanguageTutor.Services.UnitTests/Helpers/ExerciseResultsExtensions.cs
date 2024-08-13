using System.Collections.Generic;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;

namespace LanguageTutor.Services.UnitTests.Helpers
{
	internal static class ExerciseResultsExtensions
	{
		public static TranslateTextExercise ToExercise(this IEnumerable<TranslateTextExerciseResult> exerciseResults)
		{
			return new TranslateTextExercise(exerciseResults)
			{
				TextInStudiedLanguage = new LanguageText
				{
					Id = new ItemId("test"),
				},
			};
		}
	}
}
