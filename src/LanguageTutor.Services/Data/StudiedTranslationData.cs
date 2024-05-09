using System.Collections.Generic;
using LanguageTutor.Models;

namespace LanguageTutor.Services.Data
{
	public class StudiedTranslationData
	{
		public LanguageText TextInStudiedLanguage { get; init; }

		public LanguageText TextInKnownLanguage { get; init; }

		public IReadOnlyCollection<CheckResult> CheckResults { get; init; }
	}
}
