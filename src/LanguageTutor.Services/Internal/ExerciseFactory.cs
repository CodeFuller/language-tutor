using System;
using System.Collections.Generic;
using System.Linq;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;
using LanguageTutor.Services.Data;
using LanguageTutor.Services.LanguageTraits;

namespace LanguageTutor.Services.Internal
{
	internal class ExerciseFactory : IExerciseFactory
	{
		private readonly ISupportedLanguageTraits supportedLanguageTraits;

		public ExerciseFactory(ISupportedLanguageTraits supportedLanguageTraits)
		{
			this.supportedLanguageTraits = supportedLanguageTraits ?? throw new ArgumentNullException(nameof(supportedLanguageTraits));
		}

		public IEnumerable<TranslateTextExercise> CreateTranslateTextExercises(IReadOnlyCollection<TranslateTextExerciseData> exercisesData)
		{
			var translationsFromStudiedLanguage = exercisesData.ToDictionary(x => x.TextInStudiedLanguage.Id, x => x.TextsInKnownLanguage);
			var translationsFromKnownLanguage = exercisesData.SelectMany(x => x.TextsInKnownLanguage.Select(y => new
				{
					TextInStudiedLanguage = x.TextInStudiedLanguage,
					TextInKnownLanguage = y,
				}))
				.ToLookup(x => x.TextInKnownLanguage.Id, x => x.TextInStudiedLanguage);

			foreach (var exerciseData in exercisesData)
			{
				var textInStudiedLanguage = exerciseData.TextInStudiedLanguage;

				var synonymsInKnownLanguage = translationsFromStudiedLanguage[textInStudiedLanguage.Id].ToList();
				var synonymIdsInKnownLanguage = synonymsInKnownLanguage.Select(x => x.Id).ToList();

				// For synonyms in studied language, we pick all texts, which translations are a superset of translations for current text.
				// To achieve this we do the following:
				//   1. Take current text in known language.
				//   2. Get all translations to studied language for this text.
				//   3. Select those texts, which translations are a superset of translations for current text.
				var otherSynonymsInStudiedLanguage = exerciseData.TextsInKnownLanguage
					.SelectMany(x => translationsFromKnownLanguage[x.Id])
					.DistinctBy(x => x.Id)
					.Where(x => x.Id != textInStudiedLanguage.Id)
					.Where(x => !synonymIdsInKnownLanguage.Except(translationsFromStudiedLanguage[x.Id].Select(y => y.Id)).Any())
					.ToList();

				yield return new TranslateTextExercise(exerciseData.ExerciseResults)
				{
					TextInStudiedLanguage = textInStudiedLanguage,
					OtherSynonymsInStudiedLanguage = otherSynonymsInStudiedLanguage.ToList(),
					SynonymsInKnownLanguage = synonymsInKnownLanguage.ToList(),
				};
			}
		}

		public IEnumerable<InflectWordExercise> CreateInflectWordExercises(ItemId studiedLanguageId, IReadOnlyCollection<InflectWordExerciseData> exercisesData)
		{
			var languageTraits = supportedLanguageTraits.GetLanguageTraits(studiedLanguageId);
			var templates = languageTraits.GetInflectWordExerciseTypes()
				.Select(x => x.DescriptionTemplate)
				.ToDictionary(x => x.Id);

			return exercisesData.Select(x => new InflectWordExercise(x.ExerciseId, x.CreationTimestamp, GetDescriptionForInflectWordExercise(x, templates), x.BaseForm, x.WordForms, x.ExerciseResults));
		}

		private static string GetDescriptionForInflectWordExercise(InflectWordExerciseData exerciseData, Dictionary<ItemId, InflectWordExerciseDescriptionTemplate> templates)
		{
			return !String.IsNullOrEmpty(exerciseData.Description)
				? exerciseData.Description
				: templates[exerciseData.DescriptionTemplateId].GetDescription(exerciseData.BaseForm);
		}
	}
}
