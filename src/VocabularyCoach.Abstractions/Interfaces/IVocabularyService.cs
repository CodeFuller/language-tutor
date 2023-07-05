using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Abstractions.Models;

namespace VocabularyCoach.Abstractions.Interfaces
{
	public interface IVocabularyService
	{
		Task<IReadOnlyCollection<Language>> GetLanguages(CancellationToken cancellationToken);

		Task<IReadOnlyCollection<StudiedWordOrPhraseWithTranslation>> GetStudiedWords(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);
	}
}
