using System.Collections.Generic;
using VocabularyCoach.Models;

namespace VocabularyCoach.Services.Internal
{
	internal interface ITextsForPracticeSelector
	{
		IReadOnlyCollection<StudiedText> SelectTextsForTodayPractice(IEnumerable<StudiedText> studiedTexts);
	}
}
