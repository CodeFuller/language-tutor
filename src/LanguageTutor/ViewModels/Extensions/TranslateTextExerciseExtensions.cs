using System;
using System.Linq;
using LanguageTutor.Internal;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;

namespace LanguageTutor.ViewModels.Extensions
{
	internal static class TranslateTextExerciseExtensions
	{
		public static string GetTranslationsInKnownLanguage(this TranslateTextExercise translateTextExercise)
		{
			var sortedTranslations = translateTextExercise.SynonymsInKnownLanguage
				.Order(new LanguageTextComparer())
				.ToList();

			var translationTexts = sortedTranslations.Select(x => x.Text).ToHashSet(LanguageTextComparison.IgnoreCaseEqualityComparer);

			string GetTextWithNote(LanguageText translation, int index)
			{
				if (String.IsNullOrEmpty(translation.Note))
				{
					return translation.Text;
				}

				// If some other translation matches the note, we omit the note.
				// Example: prepare, arrange (prepare) => arrange, prepare.
				if (translationTexts.Contains(translation.Note))
				{
					return translation.Text;
				}

				// If note is duplicated for several translations, we put it once for the latest one.
				// Example: set up (prepare), arrange (prepare) => arrange, set up (prepare).
				if (sortedTranslations.Skip(index + 1).Any(x => String.Equals(translation.Note, x.Note, LanguageTextComparison.IgnoreCase)))
				{
					return translation.Text;
				}

				return translation.GetTextWithNote();
			}

			return String.Join(", ", sortedTranslations.Select(GetTextWithNote));
		}

		public static string GetHintForOtherSynonyms(this TranslateTextExercise translateTextExercise)
		{
			if (!translateTextExercise.OtherSynonymsInStudiedLanguage.Any())
			{
				return String.Empty;
			}

			var sortedSynonyms = translateTextExercise.OtherSynonymsInStudiedLanguage.Order(new LanguageTextComparer());
			return $"synonyms: {String.Join(", ", sortedSynonyms.Select(x => MaskSynonym(translateTextExercise.TextInStudiedLanguage.Text, x.Text)))}";
		}

		private static string MaskSynonym(string targetText, string synonym)
		{
			var targetTextWords = targetText.Split(' ');
			var synonymWords = synonym.Split(' ');

			if (targetTextWords.Length != synonymWords.Length || targetTextWords.Length == 1)
			{
				return synonym;
			}

			int? nonMatchingWordIndex = null;
			for (var i = 0; i < targetTextWords.Length; i++)
			{
				if (targetTextWords[i] == synonymWords[i])
				{
					continue;
				}

				if (nonMatchingWordIndex != null)
				{
					// There are several non-matching words, skipping masking.
					return synonym;
				}

				nonMatchingWordIndex = i;
			}

			return nonMatchingWordIndex != null ? $"*** {synonymWords[nonMatchingWordIndex.Value]} ***" : synonym;
		}
	}
}
