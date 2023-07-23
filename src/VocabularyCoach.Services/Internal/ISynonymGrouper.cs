using System.Collections.Generic;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;

namespace VocabularyCoach.Services.Internal
{
	internal interface ISynonymGrouper
	{
		IEnumerable<StudiedText> GroupStudiedTranslationsBySynonyms(IReadOnlyCollection<StudiedTranslationData> studiedTranslations);
	}
}
