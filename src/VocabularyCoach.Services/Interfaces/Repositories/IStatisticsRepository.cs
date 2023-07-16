using System;
using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;

namespace VocabularyCoach.Services.Interfaces.Repositories
{
	public interface IStatisticsRepository
	{
		Task UpdateUserStatistics(ItemId userId, ItemId studiedLanguageId, ItemId knownLanguageId, DateOnly statisticsDate, UserStatisticsData statistics, CancellationToken cancellationToken);
	}
}
