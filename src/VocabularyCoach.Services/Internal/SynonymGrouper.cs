using System.Collections.Generic;
using System.Linq;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;

namespace VocabularyCoach.Services.Internal
{
	internal class SynonymGrouper : ISynonymGrouper
	{
		public IEnumerable<StudiedText> GroupStudiedTranslationsBySynonyms(IReadOnlyCollection<StudiedTranslationData> studiedTranslations)
		{
			var translationsFromStudiedLanguage = studiedTranslations.ToLookup(x => x.TextInStudiedLanguage.Id, x => x.TextInKnownLanguage);
			var translationsFromKnownLanguage = studiedTranslations.ToLookup(x => x.TextInKnownLanguage.Id, x => x.TextInStudiedLanguage);

			foreach (var studiedTranslation in studiedTranslations.DistinctBy(x => x.TextInStudiedLanguage.Id))
			{
				var textInStudiedLanguage = studiedTranslation.TextInStudiedLanguage;

				var synonymsInKnownLanguage = translationsFromStudiedLanguage[textInStudiedLanguage.Id].ToList();

				// For synonyms in studied language, we pick all texts in studied language with exactly the same set of translations to known language.
				// To achieve this we do the following:
				//   1. Take current text in known language.
				//   2. Get all translations to studied language for this text.
				//   3. Select those texts which have same set of translations to known language.
				var otherSynonymsInStudiedLanguage = translationsFromKnownLanguage[studiedTranslation.TextInKnownLanguage.Id]
					.Where(x => x.Id != textInStudiedLanguage.Id)
					.Where(x => TextCollectionsAreEqual(translationsFromStudiedLanguage[x.Id], synonymsInKnownLanguage))
					.ToList();

				yield return new StudiedText(studiedTranslation.CheckResults)
				{
					TextInStudiedLanguage = textInStudiedLanguage,
					OtherSynonymsInStudiedLanguage = otherSynonymsInStudiedLanguage.ToList(),
					SynonymsInKnownLanguage = synonymsInKnownLanguage.ToList(),
				};
			}
		}

		private static bool TextCollectionsAreEqual(IEnumerable<LanguageText> texts1, IEnumerable<LanguageText> texts2)
		{
			var ids1 = texts1.Select(x => x.Id).ToList();
			var ids2 = texts2.Select(x => x.Id).ToList();

			return !ids1.Except(ids2).Any() && !ids2.Except(ids1).Any();
		}
	}
}
