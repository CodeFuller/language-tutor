using System;
using System.Collections.Generic;
using System.Linq;
using LanguageTutor.Models;

namespace LanguageTutor.Services.LanguageTraits
{
	internal class SupportedLanguageTraits : ISupportedLanguageTraits
	{
		private readonly Dictionary<ItemId, ILanguageTraits> languagesTraits;

		public SupportedLanguageTraits(IEnumerable<ILanguageTraits> languagesTraits)
		{
			this.languagesTraits = languagesTraits?.ToDictionary(x => x.Language.Id, x => x) ?? throw new ArgumentNullException(nameof(languagesTraits));
		}

		public ILanguageTraits GetLanguageTraits(ItemId languageId)
		{
			if (!languagesTraits.TryGetValue(languageId, out var languageTraits))
			{
				throw new NotSupportedException($"Language with id {languageId} is not supported");
			}

			return languageTraits;
		}
	}
}
