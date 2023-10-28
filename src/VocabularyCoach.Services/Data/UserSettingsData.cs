using VocabularyCoach.Models;

namespace VocabularyCoach.Services.Data
{
	public class UserSettingsData
	{
		public Language LastStudiedLanguage { get; init; }

		public Language LastKnownLanguage { get; init; }
	}
}
