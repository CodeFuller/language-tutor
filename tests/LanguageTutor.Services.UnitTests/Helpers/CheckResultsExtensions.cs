using System.Collections.Generic;
using LanguageTutor.Models;

namespace LanguageTutor.Services.UnitTests.Helpers
{
	internal static class CheckResultsExtensions
	{
		public static StudiedText ToStudiedText(this IEnumerable<CheckResult> checkResults)
		{
			return new StudiedText(checkResults)
			{
				TextInStudiedLanguage = new LanguageText
				{
					Id = new ItemId("test"),
				},
			};
		}
	}
}
