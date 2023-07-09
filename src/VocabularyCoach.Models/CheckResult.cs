using System;

namespace VocabularyCoach.Models
{
	public sealed class CheckResult
	{
		public ItemId Id { get; set; }

		public DateTimeOffset DateTime { get; init; }

		public CheckResultType CheckResultType { get; init; }
	}
}
