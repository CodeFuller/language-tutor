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

		Task<Uri> GetUrlForLanguageTextCheck(LanguageText languageText, CancellationToken cancellationToken);

		Task<LanguageText> AddLanguageTextWithTranslation(LanguageTextCreationData languageTextData1, LanguageTextCreationData languageTextData2, CancellationToken cancellationToken);
	}
}
