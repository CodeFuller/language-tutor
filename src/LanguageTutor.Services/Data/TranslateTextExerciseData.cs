using System.Collections.Generic;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;

namespace LanguageTutor.Services.Data
{
	public class TranslateTextExerciseData
	{
		public LanguageText TextInStudiedLanguage { get; init; }

		public IReadOnlyCollection<LanguageText> TextsInKnownLanguage { get; init; }

		public IReadOnlyCollection<TranslateTextExerciseResult> ExerciseResults { get; init; }
	}
}
