using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;

namespace VocabularyCoach.Services.Interfaces.Repositories
{
	public interface ILanguageRepository
	{
		Task<IReadOnlyCollection<Language>> GetLanguages(CancellationToken cancellationToken);
	}
}
