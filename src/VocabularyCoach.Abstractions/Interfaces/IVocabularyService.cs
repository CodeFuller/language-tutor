using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Abstractions.Models;

namespace VocabularyCoach.Abstractions.Interfaces
{
	public interface IVocabularyService
	{
		Task<IReadOnlyCollection<Language>> GetLanguages(CancellationToken cancellationToken);

		Task<IReadOnlyCollection<StudiedTextWithTranslation>> GetStudiedTexts(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);

		Task<CheckResultType> CheckTypedText(StudiedText studiedText, string typedText, CancellationToken cancellationToken);
	}
}
