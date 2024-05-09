using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;

namespace LanguageTutor.Services.Interfaces
{
	public interface ISpellCheckService
	{
		Task<bool> PerformSpellCheck(LanguageText languageText, CancellationToken cancellationToken);
	}
}
