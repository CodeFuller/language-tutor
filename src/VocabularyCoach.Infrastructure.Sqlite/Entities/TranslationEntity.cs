namespace VocabularyCoach.Infrastructure.Sqlite.Entities
{
	internal sealed class TranslationEntity
	{
		public int Text1Id { get; set; }

		public int Text2Id { get; set; }

		public TextEntity Text1 { get; set; }

		public TextEntity Text2 { get; set; }
	}
}
