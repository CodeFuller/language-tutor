using System;
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

		Task<Uri> GetUrlForSpellCheck(LanguageText languageText, CancellationToken cancellationToken);

		Task<LanguageText> AddLanguageText(LanguageTextCreationData languageTextData, CancellationToken cancellationToken);

		Task AddTranslation(LanguageText languageText1, LanguageText languageText2, CancellationToken cancellationToken);
	}
}
