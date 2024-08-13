using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;

namespace LanguageTutor.Services.Interfaces.Repositories
{
	public interface ILanguageTextRepository
	{
		Task<IReadOnlyCollection<LanguageText>> GetLanguageTexts(ItemId languageId, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<Translation>> GetTranslations(ItemId language1Id, ItemId language2Id, CancellationToken cancellationToken);

		Task AddLanguageText(LanguageText languageText, CancellationToken cancellationToken);

		Task AddTranslation(Translation translation, CancellationToken cancellationToken);

		Task UpdateLanguageText(LanguageText languageText, CancellationToken cancellationToken);

		Task DeleteLanguageText(LanguageText languageText, CancellationToken cancellationToken);

		Task DeleteTranslation(Translation translation, CancellationToken cancellationToken);
	}
}
