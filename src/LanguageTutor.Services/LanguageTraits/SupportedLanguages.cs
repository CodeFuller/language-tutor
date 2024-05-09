using LanguageTutor.Models;

namespace LanguageTutor.Services.LanguageTraits
{
	internal static class SupportedLanguages
	{
		public static Language Polish { get; } = new()
		{
			Id = new ItemId("2"),
			Name = "Polish",
		};
	}
}
