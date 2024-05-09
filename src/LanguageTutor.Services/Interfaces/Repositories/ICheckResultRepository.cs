using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;

namespace LanguageTutor.Services.Interfaces.Repositories
{
	public interface ICheckResultRepository
	{
		Task AddCheckResult(ItemId userId, ItemId languageTextId, CheckResult checkResult, CancellationToken cancellationToken);
	}
}
