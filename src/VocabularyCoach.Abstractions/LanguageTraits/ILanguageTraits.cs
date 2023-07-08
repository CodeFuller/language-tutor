using System;
using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Abstractions.Models;

namespace VocabularyCoach.Abstractions.LanguageTraits
{
	internal interface ILanguageTraits
	{
		Language Language { get; }

		Task<Uri> GetUrlForTextCheck(string text, CancellationToken cancellationToken);
	}
}
