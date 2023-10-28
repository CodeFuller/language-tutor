namespace VocabularyCoach.Infrastructure.Sqlite.Entities
{
	internal sealed class UserSettingsEntity
	{
		public int UserId { get; set; }

		public int? LastStudiedLanguageId { get; set; }

		public LanguageEntity LastStudiedLanguage { get; set; }

		public int? LastKnownLanguageId { get; set; }

		public LanguageEntity LastKnownLanguage { get; set; }
	}
}
