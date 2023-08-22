using System.Collections.Generic;
using VocabularyCoach.Models;

namespace VocabularyCoach.Services.Internal
{
	internal interface IProblematicTextsSelector
	{
		IReadOnlyCollection<StudiedText> GetProblematicTexts(IEnumerable<StudiedText> studiedTexts);
	}
}
