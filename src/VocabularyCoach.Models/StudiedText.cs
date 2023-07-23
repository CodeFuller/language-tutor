using System;
using System.Collections.Generic;
using System.Linq;

namespace VocabularyCoach.Models
{
	public sealed class StudiedText
	{
		private readonly List<CheckResult> checkResults;

		public LanguageText TextInStudiedLanguage { get; init; }

		public IReadOnlyCollection<LanguageText> OtherSynonymsInStudiedLanguage { get; init; }

		public IReadOnlyCollection<LanguageText> SynonymsInKnownLanguage { get; init; }

		// The check results order is from the latest (most significant) to the earliest (less significant).
		public IReadOnlyList<CheckResult> CheckResults => checkResults.OrderByDescending(x => x.DateTime).ToList();

		public StudiedText(IEnumerable<CheckResult> checkResults)
		{
			this.checkResults = checkResults?.ToList() ?? throw new ArgumentNullException(nameof(checkResults));
		}

		public void AddCheckResult(CheckResult checkResult)
		{
			checkResults.Add(checkResult);
		}
	}
}
