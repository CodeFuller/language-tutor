using System;
using System.Linq;
using VocabularyCoach.Internal;
using VocabularyCoach.Models;

namespace VocabularyCoach.ViewModels.Extensions
{
	internal static class StudiedTextExtensions
	{
		public static string GetTranslationsInKnownLanguage(this StudiedText studiedText)
		{
			var sortedTranslations = studiedText.SynonymsInKnownLanguage
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
	}
}
