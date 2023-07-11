using System.Collections.Generic;
using System.Linq;

namespace VocabularyCoach.Models
{
	public sealed class StudiedText
	{
		private readonly List<CheckResult> checkResults = new();

		public LanguageText LanguageText { get; init; }

		// The check results order is from the latest (most significant) to the earliest (less significant).
		public IReadOnlyList<CheckResult> CheckResults => checkResults.OrderByDescending(x => x.DateTime).ToList();

		public void AddCheckResult(CheckResult checkResult)
		{
			checkResults.Add(checkResult);
		}
	}
}
