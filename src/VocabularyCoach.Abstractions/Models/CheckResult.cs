using System;

namespace VocabularyCoach.Abstractions.Models
{
	public sealed class CheckResult
	{
		public DateTimeOffset DateTime { get; init; }

		public CheckResultType CheckResultType { get; init; }
	}
}
