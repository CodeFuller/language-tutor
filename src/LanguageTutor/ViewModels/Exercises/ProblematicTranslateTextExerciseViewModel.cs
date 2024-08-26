using System.Collections.Generic;
using System.Linq;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;
using LanguageTutor.ViewModels.Extensions;

namespace LanguageTutor.ViewModels.Exercises
{
	public class ProblematicTranslateTextExerciseViewModel : BasicProblematicExerciseViewModel
	{
		public LanguageText TextInStudiedLanguage { get; }

		public string TranslationsInKnownLanguage { get; }

		public IReadOnlyCollection<ProblematicTranslateTextExerciseResultViewModel> TranslateTextExerciseResults { get; }

		protected override IEnumerable<BasicProblematicExerciseResultViewModel> ExerciseResults => TranslateTextExerciseResults;

		public ProblematicTranslateTextExerciseViewModel(TranslateTextExercise exercise)
			: base(exercise.TextInStudiedLanguage.Text)
		{
			TextInStudiedLanguage = exercise.TextInStudiedLanguage;
			TranslationsInKnownLanguage = exercise.GetTranslationsInKnownLanguage();
			TranslateTextExerciseResults = exercise.TranslateTextExerciseResults
				.OrderBy(x => x.DateTime)
				.Select(x => new ProblematicTranslateTextExerciseResultViewModel(x))
				.ToList();
		}
	}
}
