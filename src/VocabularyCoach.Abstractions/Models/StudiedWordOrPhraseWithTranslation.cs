namespace VocabularyCoach.Abstractions.Models
{
	public sealed class StudiedWordOrPhraseWithTranslation
	{
		public StudiedWordOrPhrase StudiedWordOrPhrase { get; init; }

		public WordOrPhrase WordOrPhraseInKnownLanguage { get; init; }
	}
}
