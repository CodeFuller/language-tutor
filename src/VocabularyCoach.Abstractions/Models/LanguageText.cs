namespace VocabularyCoach.Abstractions.Models
{
	public sealed class LanguageText
	{
		public string Id { get; init; }

		public Language Language { get; init; }

		public string Text { get; init; }

		public string Note { get; init; }
	}
}
