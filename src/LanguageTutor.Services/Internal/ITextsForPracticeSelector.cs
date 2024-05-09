using System;
using System.Collections.Generic;
using LanguageTutor.Models;

namespace LanguageTutor.Services.Internal
{
	internal interface ITextsForPracticeSelector
	{
		IReadOnlyCollection<StudiedText> GetTextsForPractice(DateOnly date, IEnumerable<StudiedText> studiedTexts, int dailyLimit);
	}
}
