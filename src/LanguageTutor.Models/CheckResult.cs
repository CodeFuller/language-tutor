using System;

namespace LanguageTutor.Models
{
	public sealed class CheckResult
	{
		public ItemId Id { get; set; }

		public DateTimeOffset DateTime { get; init; }

		public CheckResultType CheckResultType { get; init; }

		public bool IsSuccessful => CheckResultType == CheckResultType.Ok;

		public bool IsFailed => !IsSuccessful;

		// This property is filled only for CheckResultType.Misspelled.
		// Also, it could be missing for checks saved before introducing TypedText property.
		public string TypedText { get; init; }
	}
}
