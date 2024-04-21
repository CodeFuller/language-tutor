using System;
using System.Collections.Generic;
using VocabularyCoach.Models;

namespace VocabularyCoach.Services.Internal
{
	internal interface ITextsForPracticeSelector
	{
		IReadOnlyCollection<StudiedText> GetTextsForPractice(DateOnly date, IEnumerable<StudiedText> studiedTexts, int dailyLimit);
	}
}
