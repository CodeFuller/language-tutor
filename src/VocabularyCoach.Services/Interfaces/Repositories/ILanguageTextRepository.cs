using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;

namespace VocabularyCoach.Services.Interfaces.Repositories
{
	public interface ILanguageTextRepository
	{
		Task<IReadOnlyCollection<LanguageText>> GetLanguageText(ItemId languageId, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<StudiedTextWithTranslation>> GetStudiedTexts(ItemId userId, ItemId studiedLanguageId, ItemId knownLanguageId, CancellationToken cancellationToken);

		Task AddLanguageText(LanguageText languageText, CancellationToken cancellationToken);

		Task AddTranslation(LanguageText languageText1, LanguageText languageText2, CancellationToken cancellationToken);
	}
}
