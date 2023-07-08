using VocabularyCoach.Abstractions.Models;

namespace VocabularyCoach.Abstractions.LanguageTraits
{
	internal static class SupportedLanguages
	{
		public static Language Polish { get; } = new()
		{
			Id = "f7013078-e287-4dd1-8f09-4e16a9d2f4d4",
			Name = "Polish",
		};

		public static Language Russian { get; } = new()
		{
			Id = "454c0e62-0cc8-4e33-9977-00b06ca309d2",
			Name = "Russian",
		};
	}
}
