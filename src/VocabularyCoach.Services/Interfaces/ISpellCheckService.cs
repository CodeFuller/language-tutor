using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;

namespace VocabularyCoach.Services.Interfaces
{
	public interface ISpellCheckService
	{
		Task<bool> PerformSpellCheck(LanguageText languageText, CancellationToken cancellationToken);
	}
}
