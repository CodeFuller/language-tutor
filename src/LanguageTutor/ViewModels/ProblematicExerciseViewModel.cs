using System.Collections.Generic;
using System.Linq;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;
using LanguageTutor.ViewModels.Extensions;

namespace LanguageTutor.ViewModels
{
	public class ProblematicExerciseViewModel
	{
		public LanguageText TextInStudiedLanguage { get; }

		public string TranslationsInKnownLanguage { get; }

		public IReadOnlyCollection<ProblematicExerciseResultViewModel> ExerciseResults { get; }

		public ProblematicExerciseViewModel(BasicExercise exercise)
		{
			// TODO: Add support for all exercises.
			var translateTextExercise = (TranslateTextExercise)exercise;

			TextInStudiedLanguage = translateTextExercise.TextInStudiedLanguage;
			TranslationsInKnownLanguage = translateTextExercise.GetTranslationsInKnownLanguage();

			// TODO: Remove cast to TranslateTextExerciseResult.
			ExerciseResults = translateTextExercise.SortedResults
				.Cast<TranslateTextExerciseResult>()
				.OrderBy(x => x.DateTime)
				.Select(x => new ProblematicExerciseResultViewModel(x)).ToList();
		}
	}
}
