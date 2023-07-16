using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VocabularyCoach.Infrastructure.Sqlite.Entities;
using VocabularyCoach.Infrastructure.Sqlite.Extensions;
using VocabularyCoach.Infrastructure.Sqlite.Internal;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;
using VocabularyCoach.Services.Interfaces.Repositories;

namespace VocabularyCoach.Infrastructure.Sqlite.Repositories
{
	internal sealed class StatisticsRepository : IStatisticsRepository
	{
		private readonly IDbContextFactory<VocabularyCoachDbContext> contextFactory;

		public StatisticsRepository(IDbContextFactory<VocabularyCoachDbContext> contextFactory)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
		}

		public async Task UpdateUserStatistics(ItemId userId, ItemId studiedLanguageId, ItemId knownLanguageId, DateOnly statisticsDate, UserStatisticsData statistics, CancellationToken cancellationToken)
		{
			await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

			var existingStatisticsEntity = await context.UserStatistics
				.SingleOrDefaultAsync(
					x => x.UserId == userId.ToInt32() && x.StudiedLanguageId == studiedLanguageId.ToInt32() &&
					     x.KnownLanguageId == knownLanguageId.ToInt32() && x.Date == statisticsDate, cancellationToken);

			if (existingStatisticsEntity == null)
			{
				var newStatisticsEntity = new UserStatisticsEntity
				{
					UserId = userId.ToInt32(),
					StudiedLanguageId = studiedLanguageId.ToInt32(),
					KnownLanguageId = knownLanguageId.ToInt32(),
					Date = statisticsDate,
				};

				CopyStatistics(statistics, newStatisticsEntity);

				await context.UserStatistics.AddAsync(newStatisticsEntity, cancellationToken);
			}
			else
			{
				CopyStatistics(statistics, existingStatisticsEntity);
			}

			await context.SaveChangesAsync(cancellationToken);
		}

		private static void CopyStatistics(UserStatisticsData source, UserStatisticsEntity target)
		{
			target.TotalTextsNumber = source.TotalNumberOfTexts;
			target.TotalLearnedTextsNumber = source.TotalNumberOfLearnedTexts;
			target.RestNumberOfTextsToPractice = source.RestNumberOfTextsToPracticeToday;
			target.NumberOfPracticedTexts = source.NumberOfTextsPracticedToday;
		}
	}
}
