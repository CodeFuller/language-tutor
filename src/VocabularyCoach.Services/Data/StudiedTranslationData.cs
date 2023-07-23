using System.Collections.Generic;
using VocabularyCoach.Models;

namespace VocabularyCoach.Services.Data
{
	public class StudiedTranslationData
	{
		public LanguageText TextInStudiedLanguage { get; init; }

		public LanguageText TextInKnownLanguage { get; init; }

		public IReadOnlyCollection<CheckResult> CheckResults { get; init; }
	}
}
