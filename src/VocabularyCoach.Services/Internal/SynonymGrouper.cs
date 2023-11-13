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
				var synonymIdsInKnownLanguage = synonymsInKnownLanguage.Select(x => x.Id).ToList();

				// For synonyms in studied language, we pick all texts, which translations are a superset of translations for current text.
				// To achieve this we do the following:
				//   1. Take current text in known language.
				//   2. Get all translations to studied language for this text.
				//   3. Select those texts, which translations are a superset of translations for current text.
				var otherSynonymsInStudiedLanguage = translationsFromKnownLanguage[studiedTranslation.TextInKnownLanguage.Id]
					.Where(x => x.Id != textInStudiedLanguage.Id)
					.Where(x => !synonymIdsInKnownLanguage.Except(translationsFromStudiedLanguage[x.Id].Select(y => y.Id)).Any())
					.ToList();

				yield return new StudiedText(studiedTranslation.CheckResults)
				{
					TextInStudiedLanguage = textInStudiedLanguage,
					OtherSynonymsInStudiedLanguage = otherSynonymsInStudiedLanguage.ToList(),
					SynonymsInKnownLanguage = synonymsInKnownLanguage.ToList(),
				};
			}
		}
	}
}
