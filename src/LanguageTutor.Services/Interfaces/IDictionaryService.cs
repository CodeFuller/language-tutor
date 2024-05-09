using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;
using LanguageTutor.Services.Data;

namespace LanguageTutor.Services.Interfaces
{
	public interface IDictionaryService
	{
		Task<IReadOnlyCollection<LanguageText>> GetLanguageTexts(Language language, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<Translation>> GetTranslations(Language language1, Language language2, CancellationToken cancellationToken);

		Task<LanguageText> AddLanguageText(LanguageTextData languageTextData, CancellationToken cancellationToken);

		Task<Translation> AddTranslation(LanguageText languageText1, LanguageText languageText2, CancellationToken cancellationToken);

		Task<LanguageText> UpdateLanguageText(LanguageText languageText, LanguageTextData newLanguageTextData, CancellationToken cancellationToken);

		Task DeleteLanguageText(LanguageText languageText, CancellationToken cancellationToken);

		Task DeleteTranslation(Translation translation, CancellationToken cancellationToken);
	}
}
