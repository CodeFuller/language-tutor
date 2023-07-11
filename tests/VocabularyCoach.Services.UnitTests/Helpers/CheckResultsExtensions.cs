using System.Collections.Generic;
using VocabularyCoach.Models;

namespace VocabularyCoach.Services.UnitTests.Helpers
{
	internal static class CheckResultsExtensions
	{
		public static StudiedText ToStudiedText(this IEnumerable<CheckResult> checkResults, string itemId)
		{
			return new StudiedText(checkResults)
			{
				TextInStudiedLanguage = new LanguageText
				{
					Id = new ItemId(itemId),
				},
			};
		}
	}
}
