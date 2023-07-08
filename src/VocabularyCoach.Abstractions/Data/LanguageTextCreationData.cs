using VocabularyCoach.Abstractions.Models;

namespace VocabularyCoach.Abstractions.Data
{
	public class LanguageTextCreationData
	{
		public Language Language { get; init; }

		public string Text { get; init; }

		public string Note { get; init; }

		public PronunciationRecord PronunciationRecord { get; init; }
	}
}
