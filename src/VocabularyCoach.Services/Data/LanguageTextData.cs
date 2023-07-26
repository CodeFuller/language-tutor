using VocabularyCoach.Models;

namespace VocabularyCoach.Services.Data
{
	public class LanguageTextData
	{
		public Language Language { get; init; }

		public string Text { get; init; }

		public string Note { get; init; }

		public PronunciationRecord PronunciationRecord { get; init; }
	}
}
