using System;
using System.Collections.Generic;
using System.Linq;
using VocabularyCoach.Models;

namespace VocabularyCoach.Services.LanguageTraits
{
	internal class SupportedLanguageTraits : ISupportedLanguageTraits
	{
		private readonly IReadOnlyDictionary<ItemId, ILanguageTraits> languagesTraits;

		public SupportedLanguageTraits(IEnumerable<ILanguageTraits> languagesTraits)
		{
			this.languagesTraits = languagesTraits?.ToDictionary(x => x.Language.Id, x => x) ?? throw new ArgumentNullException(nameof(languagesTraits));
		}

		public ILanguageTraits GetLanguageTraits(Language language)
		{
			if (!languagesTraits.TryGetValue(language.Id, out var languageTraits))
			{
				throw new NotSupportedException($"Language is not supported: {language}");
			}

			return languageTraits;
		}
	}
}
