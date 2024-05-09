using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;

namespace LanguageTutor.Services.Interfaces.Repositories
{
	public interface ILanguageRepository
	{
		Task<IReadOnlyCollection<Language>> GetLanguages(CancellationToken cancellationToken);
	}
}
