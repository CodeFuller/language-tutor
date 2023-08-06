using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;

namespace VocabularyCoach.Services.Interfaces
{
	public interface IEditVocabularyService
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
