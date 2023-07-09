using System;
using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;

namespace VocabularyCoach.Services.LanguageTraits
{
	internal sealed class PolishLanguageTraits : ILanguageTraits
	{
		public Language Language => SupportedLanguages.Polish;

		public Task<Uri> GetUrlForTextCheck(string text, CancellationToken cancellationToken)
		{
			var uri = new Uri($"https://pl.wiktionary.org/wiki/{text}");
			return Task.FromResult(uri);
		}
	}
}
