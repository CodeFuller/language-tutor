using System.Collections.Generic;
using LanguageTutor.Models;
using LanguageTutor.Services.Data;

namespace LanguageTutor.Services.Internal
{
	internal interface ISynonymGrouper
	{
		IEnumerable<StudiedText> GroupStudiedTranslationsBySynonyms(IReadOnlyCollection<StudiedTranslationData> studiedTranslations);
	}
}
