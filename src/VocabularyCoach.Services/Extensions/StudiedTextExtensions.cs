using System;
using System.Linq;
using VocabularyCoach.Models;

namespace VocabularyCoach.Services.Extensions
{
	internal static class StudiedTextExtensions
	{
		public static StudiedText WithLimitedCheckResults(this StudiedText studiedText, int checkResultsCount)
		{
			return studiedText.WithLimitedCheckResults(DateOnly.MaxValue, checkResultsCount);
		}

		public static StudiedText WithLimitedCheckResults(this StudiedText studiedText, DateOnly date)
		{
			return studiedText.WithLimitedCheckResults(date, Int32.MaxValue);
		}

		public static StudiedText WithLimitedCheckResults(this StudiedText studiedText, DateOnly date, int checkResultsCount)
		{
			var limitedCheckResults = studiedText.CheckResults
				.Where(x => x.DateTime.ToDateOnly() <= date)
				.Take(checkResultsCount)
				.ToList();

			if (limitedCheckResults.Count == studiedText.CheckResults.Count)
			{
				return studiedText;
			}

			return new StudiedText(limitedCheckResults)
			{
				TextInStudiedLanguage = studiedText.TextInStudiedLanguage,
				OtherSynonymsInStudiedLanguage = studiedText.OtherSynonymsInStudiedLanguage,
				SynonymsInKnownLanguage = studiedText.SynonymsInKnownLanguage,
			};
		}
	}
}
