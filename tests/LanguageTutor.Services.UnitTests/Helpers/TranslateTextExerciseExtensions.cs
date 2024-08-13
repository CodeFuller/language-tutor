using System.Collections.Generic;
using System.Linq;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;

namespace LanguageTutor.Services.UnitTests.Helpers
{
	internal static class TranslateTextExerciseExtensions
	{
		public static TranslateTextExercise Get(this IEnumerable<TranslateTextExercise> exercises, string textId)
		{
			return exercises.Single(x => x.TextInStudiedLanguage.Id == new ItemId(textId));
		}
	}
}
