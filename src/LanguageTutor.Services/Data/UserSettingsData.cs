using LanguageTutor.Models;

namespace LanguageTutor.Services.Data
{
	public class UserSettingsData
	{
		public Language LastStudiedLanguage { get; init; }

		public Language LastKnownLanguage { get; init; }
	}
}
