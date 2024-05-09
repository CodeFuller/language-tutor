using System.Collections.Generic;
using LanguageTutor.Models;

namespace LanguageTutor.Services.Internal
{
	internal interface IProblematicTextsSelector
	{
		IReadOnlyCollection<StudiedText> GetProblematicTexts(IEnumerable<StudiedText> studiedTexts);
	}
}
