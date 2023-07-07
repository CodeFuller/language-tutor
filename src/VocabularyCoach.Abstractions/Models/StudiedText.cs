using System.Collections.Generic;

namespace VocabularyCoach.Abstractions.Models
{
	public sealed class StudiedText
	{
		private readonly List<CheckResult> checkResults = new();

		public LanguageText LanguageText { get; init; }

		public IReadOnlyCollection<CheckResult> CheckResults => checkResults;

		public void AddCheckResult(CheckResult checkResult)
		{
			checkResults.Add(checkResult);
		}
	}
}
