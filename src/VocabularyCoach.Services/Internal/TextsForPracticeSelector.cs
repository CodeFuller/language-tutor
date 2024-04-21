using System;
using System.Collections.Generic;
using System.Linq;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Extensions;

namespace VocabularyCoach.Services.Internal
{
	internal class TextsForPracticeSelector : ITextsForPracticeSelector
	{
		private static readonly IReadOnlyList<int> CheckIntervals = new List<int>
		{
			// If the last (^1) check was failed, we add 1 day to the last check date.
			+1,

			// If ^1 check was successful, but ^2 check was failed, we add 2 days to the last check date.
			+2,

			// ...
			+3,
			+7,
			+14,

			// The last interval is added for texts with all successful checks.
			+30,
		};

		public IReadOnlyCollection<StudiedText> GetTextsForPractice(DateOnly date, IEnumerable<StudiedText> studiedTexts, int dailyLimit)
		{
			var studiedTextsList = studiedTexts.ToList();

			var numberOfAlreadyPracticedTextsForDate = studiedTextsList.Count(x => x.CheckResults.Any(y => y.DateTime.ToDateOnly() == date));
			var numberOfRestTextsForPractice = dailyLimit - numberOfAlreadyPracticedTextsForDate;

			if (numberOfRestTextsForPractice < 0)
			{
				return Array.Empty<StudiedText>();
			}

			var selectedTexts = new List<StudiedText>();

			// Taking all texts that were practiced before.
			var selectedPreviouslyPracticedTexts = studiedTextsList
				.Where(x => x.CheckResults.Any())
				.Where(x => GetNextCheckDateTimeForStudiedText(x.CheckResults) <= date);

			selectedTexts.AddRange(selectedPreviouslyPracticedTexts);

			if (selectedTexts.Count < numberOfRestTextsForPractice)
			{
				// Taking first texts that were not practiced before.
				var selectedPreviouslyUnpracticedTexts = studiedTextsList
					.Where(x => !x.CheckResults.Any())
					.OrderBy(x => x.TextInStudiedLanguage.CreationTimestamp)
					.Take(numberOfRestTextsForPractice - selectedTexts.Count);

				selectedTexts.AddRange(selectedPreviouslyUnpracticedTexts);
			}

			return selectedTexts
				.Randomize()
				.Take(numberOfRestTextsForPractice)
				.ToList();
		}

		private static DateOnly GetNextCheckDateTimeForStudiedText(IReadOnlyList<CheckResult> checkResults)
		{
			if (!checkResults.Any())
			{
				// If text was not yet checked, this is the highest priority for practice.
				return DateOnly.MinValue;
			}

			var lastCheckDate = checkResults[0].DateTime.ToDateOnly();

			for (var i = 0; i <= Math.Max(checkResults.Count, CheckIntervals.Count); ++i)
			{
				// If all checks up to last interval are successful, we add the last interval.
				if (i >= CheckIntervals.Count)
				{
					return lastCheckDate.AddDays(CheckIntervals[^1]);
				}

				// If all checks are successful, however they are not enough - we add interval for first missing check.
				if (i >= checkResults.Count)
				{
					return lastCheckDate.AddDays(CheckIntervals[i]);
				}

				// If some check in the past is failed, we add interval for latest failed check.
				if (checkResults[i].IsFailed)
				{
					return lastCheckDate.AddDays(CheckIntervals[i]);
				}
			}

			// We should not get here, because we return via condition (i >= CheckIntervals.Count) or (i >= checkResults.Count).
			throw new InvalidOperationException("Unexpected loop finish");
		}
	}
}
