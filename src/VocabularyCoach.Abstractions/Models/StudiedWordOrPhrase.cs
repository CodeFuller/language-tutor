using System.Collections.Generic;

namespace VocabularyCoach.Abstractions.Models
{
	public sealed class StudiedWordOrPhrase
	{
		private List<CheckResult> checkResults = new();

		public WordOrPhrase WordOrPhrase { get; init; }

		// TODO: Remove initializer.
		public IReadOnlyCollection<CheckResult> CheckResults { get; init; }

		public void AddCheckResult(CheckResult checkResult)
		{
			checkResults.Add(checkResult);
		}
	}
}
