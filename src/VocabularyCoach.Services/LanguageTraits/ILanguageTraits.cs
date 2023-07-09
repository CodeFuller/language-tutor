using System;
using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;

namespace VocabularyCoach.Services.LanguageTraits
{
	internal interface ILanguageTraits
	{
		Language Language { get; }

		Task<Uri> GetUrlForTextCheck(string text, CancellationToken cancellationToken);
	}
}
