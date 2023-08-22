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
			var sortedSynonyms = studiedText.SynonymsInKnownLanguage.Order(new LanguageTextComparer());

			return String.Join(", ", sortedSynonyms.Select(x => x.GetTextWithNote()));
		}
	}
}
