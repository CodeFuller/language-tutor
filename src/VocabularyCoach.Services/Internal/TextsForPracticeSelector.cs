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

		private readonly ISystemClock systemClock;

		public TextsForPracticeSelector(ISystemClock systemClock)
		{
			this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
		}

		public IReadOnlyCollection<StudiedText> SelectTextsForTodayPractice(IEnumerable<StudiedText> studiedTexts)
		{
			return studiedTexts
				.Select(x => new
				{
					StudiedText = x,
					NextCheckDateTime = GetNextCheckDateTimeForStudiedText(x.CheckResults),
				})
				.Where(x => x.NextCheckDateTime <= systemClock.Today)
				.GroupBy(x => x.NextCheckDateTime, x => x.StudiedText)
				.OrderBy(x => x.Key)
				.SelectMany(x => x.Randomize())
				.ToList();
		}

		private static DateOnly GetNextCheckDateTimeForStudiedText(IReadOnlyList<CheckResult> checkResults)
		{
			if (!checkResults.Any())
			{
				// If text was not yet checked, this is the highest priority for practice.
				return DateOnly.MinValue;
			}

			var lastCheckDate = DateOnly.FromDateTime(checkResults[0].DateTime.Date);

			for (var i = 0; i < Math.Max(checkResults.Count, CheckIntervals.Count); ++i)
			{
				// If all text checks are successful, however they are not enough - we add interval for first missing check.
				if (i >= checkResults.Count)
				{
					return lastCheckDate.AddDays(CheckIntervals[i]);
				}

				// If some check in the past is failed, we add interval for latest failed check.
				if (checkResults[i].CheckResultType != CheckResultType.Ok)
				{
					return lastCheckDate.AddDays(CheckIntervals[i]);
				}
			}

			// If all checks are successful, we add the last interval.
			return lastCheckDate.AddDays(CheckIntervals[^1]);
		}
	}
}
