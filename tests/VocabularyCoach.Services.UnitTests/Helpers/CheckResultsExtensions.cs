using System.Collections.Generic;
using System.Globalization;
using VocabularyCoach.Models;

namespace VocabularyCoach.Services.UnitTests.Helpers
{
	internal static class CheckResultsExtensions
	{
		public static StudiedTextWithTranslation ToStudiedText(this IEnumerable<CheckResult> checkResults, string itemId)
		{
			var studiedText = new StudiedText
			{
				LanguageText = new LanguageText
				{
					Id = new ItemId(itemId),
				},
			};

			foreach (var checkResult in checkResults)
			{
				studiedText.AddCheckResult(checkResult);
			}

			return new StudiedTextWithTranslation
			{
				StudiedText = studiedText,
			};
		}

		private static ItemId CreateItemId(int id)
		{
			return new ItemId(id.ToString(CultureInfo.InvariantCulture));
		}
	}
}
