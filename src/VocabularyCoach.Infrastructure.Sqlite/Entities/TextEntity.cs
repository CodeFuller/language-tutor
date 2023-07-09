namespace VocabularyCoach.Infrastructure.Sqlite.Entities
{
	internal sealed class TextEntity
	{
		public int Id { get; set; }

		public int LanguageId { get; set; }

		public LanguageEntity Language { get; set; }

		public string Text { get; set; }

		public string Note { get; set; }
	}
}
