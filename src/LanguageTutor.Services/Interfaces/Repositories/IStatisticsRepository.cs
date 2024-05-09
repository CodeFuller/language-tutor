using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;
using LanguageTutor.Services.Data;

namespace LanguageTutor.Services.Interfaces.Repositories
{
	public interface IStatisticsRepository
	{
		Task<IReadOnlyCollection<UserStatisticsData>> GetUserStatistics(ItemId userId, ItemId studiedLanguageId, ItemId knownLanguageId, CancellationToken cancellationToken);

		Task UpdateUserStatistics(ItemId userId, ItemId studiedLanguageId, ItemId knownLanguageId, UserStatisticsData statistics, CancellationToken cancellationToken);
	}
}
