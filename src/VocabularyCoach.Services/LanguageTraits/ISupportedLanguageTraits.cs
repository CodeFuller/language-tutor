using VocabularyCoach.Models;

namespace VocabularyCoach.Services.LanguageTraits
{
	internal interface ISupportedLanguageTraits
	{
		ILanguageTraits GetLanguageTraits(Language language);
	}
}
