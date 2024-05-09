using System;

namespace LanguageTutor.Models
{
	public sealed class LanguageText
	{
		public ItemId Id { get; set; }

		public Language Language { get; init; }

		public string Text { get; init; }

		public string Note { get; init; }

		public DateTimeOffset CreationTimestamp { get; init; }
	}
}
