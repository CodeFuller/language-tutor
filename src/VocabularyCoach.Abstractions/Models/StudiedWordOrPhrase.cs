using System.Collections.Generic;

namespace VocabularyCoach.Abstractions.Models
{
	public sealed class StudiedWordOrPhrase
	{
		public WordOrPhrase WordOrPhrase { get; init; }

		public IReadOnlyCollection<CheckResult> CheckResults { get; init; }
	}
}
