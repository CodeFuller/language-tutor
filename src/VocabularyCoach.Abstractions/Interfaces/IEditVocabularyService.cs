using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Abstractions.Data;
using VocabularyCoach.Abstractions.Models;

namespace VocabularyCoach.Abstractions.Interfaces
{
	public interface IEditVocabularyService
	{
		Task<IReadOnlyCollection<LanguageText>> GetLanguageTexts(Language language, CancellationToken cancellationToken);

		Task<Uri> GetUrlForLanguageTextCheck(LanguageText languageText, CancellationToken cancellationToken);

		Task<LanguageText> AddOrUpdateLanguageTextWithTranslation(LanguageTextCreationData languageTextData1, LanguageTextCreationData languageTextData2, CancellationToken cancellationToken);
	}
}
