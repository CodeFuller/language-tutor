using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;

namespace VocabularyCoach.Services.Interfaces.Repositories
{
	public interface ILanguageTextRepository
	{
		Task<IReadOnlyCollection<LanguageText>> GetLanguageTexts(ItemId languageId, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<Translation>> GetTranslations(ItemId language1Id, ItemId language2Id, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<StudiedTranslationData>> GetStudiedTranslations(ItemId userId, ItemId studiedLanguageId, ItemId knownLanguageId, CancellationToken cancellationToken);

		Task AddLanguageText(LanguageText languageText, CancellationToken cancellationToken);

		Task AddTranslation(Translation translation, CancellationToken cancellationToken);
	}
}
