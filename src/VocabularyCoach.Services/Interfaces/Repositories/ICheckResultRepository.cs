using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;

namespace VocabularyCoach.Services.Interfaces.Repositories
{
	public interface ICheckResultRepository
	{
		Task AddCheckResult(ItemId userId, ItemId languageTextId, CheckResult checkResult, CancellationToken cancellationToken);
	}
}
