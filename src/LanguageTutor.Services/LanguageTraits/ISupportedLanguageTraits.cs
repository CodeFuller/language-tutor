using LanguageTutor.Models;

namespace LanguageTutor.Services.LanguageTraits
{
	internal interface ISupportedLanguageTraits
	{
		ILanguageTraits GetLanguageTraits(ItemId languageId);
	}
}
