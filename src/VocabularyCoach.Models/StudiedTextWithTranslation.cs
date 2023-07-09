namespace VocabularyCoach.Models
{
	public sealed class StudiedTextWithTranslation
	{
		public StudiedText StudiedText { get; init; }

		public LanguageText TextInKnownLanguage { get; init; }
	}
}
