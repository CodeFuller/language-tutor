using VocabularyCoach.Models;

namespace VocabularyCoach.Services.UnitTests.Helpers
{
	internal static class StringExtensions
	{
		public static LanguageText ToLanguageText(this string text)
		{
			return new LanguageText
			{
				Id = new ItemId(text),
				Text = text,
			};
		}
	}
}
