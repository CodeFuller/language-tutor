using System;
using System.Collections.Generic;
using System.Linq;
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

		public async Task<IReadOnlyCollection<UserStatisticsData>> GetUserStatistics(ItemId userId, ItemId studiedLanguageId, ItemId knownLanguageId, CancellationToken cancellationToken)
		{
			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			var userStatistics = await dbContext.UserStatistics
				.Where(x => x.UserId == userId.ToInt32() && x.StudiedLanguageId == studiedLanguageId.ToInt32() && x.KnownLanguageId == knownLanguageId.ToInt32())
				.ToListAsync(cancellationToken);

			return userStatistics.Select(x => new UserStatisticsData
				{
					Date = x.Date,
					TotalNumberOfTexts = x.TotalNumberOfTexts,
					TotalNumberOfLearnedTexts = x.TotalNumberOfLearnedTexts,
					NumberOfProblematicTexts = x.NumberOfProblematicTexts,
					RestNumberOfTextsToPracticeToday = x.RestNumberOfTextsToPracticeToday,
					RestNumberOfTextsToPracticeTodayIfNoLimit = x.RestNumberOfTextsToPracticeTodayIfNoLimit,
					NumberOfTextsPracticedToday = x.NumberOfTextsPracticedToday,
					NumberOfTextsLearnedToday = x.NumberOfTextsLearnedToday,
				})
				.ToList();
		}

		public async Task UpdateUserStatistics(ItemId userId, ItemId studiedLanguageId, ItemId knownLanguageId, UserStatisticsData statistics, CancellationToken cancellationToken)
		{
			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			var existingStatisticsEntity = await dbContext.UserStatistics
				.SingleOrDefaultAsync(
					x => x.UserId == userId.ToInt32() && x.StudiedLanguageId == studiedLanguageId.ToInt32() &&
					     x.KnownLanguageId == knownLanguageId.ToInt32() && x.Date == statistics.Date, cancellationToken);

			if (existingStatisticsEntity == null)
			{
				var newStatisticsEntity = new UserStatisticsEntity
				{
					UserId = userId.ToInt32(),
					StudiedLanguageId = studiedLanguageId.ToInt32(),
					KnownLanguageId = knownLanguageId.ToInt32(),
					Date = statistics.Date,
				};

				CopyStatistics(statistics, newStatisticsEntity);

				await dbContext.UserStatistics.AddAsync(newStatisticsEntity, cancellationToken);
			}
			else
			{
				CopyStatistics(statistics, existingStatisticsEntity);
			}

			await dbContext.SaveChangesAsync(cancellationToken);
		}

		private static void CopyStatistics(UserStatisticsData source, UserStatisticsEntity target)
		{
			target.TotalNumberOfTexts = source.TotalNumberOfTexts;
			target.TotalNumberOfLearnedTexts = source.TotalNumberOfLearnedTexts;
			target.NumberOfProblematicTexts = source.NumberOfProblematicTexts;
			target.RestNumberOfTextsToPracticeToday = source.RestNumberOfTextsToPracticeToday;
			target.RestNumberOfTextsToPracticeTodayIfNoLimit = source.RestNumberOfTextsToPracticeTodayIfNoLimit;
			target.NumberOfTextsPracticedToday = source.NumberOfTextsPracticedToday;
			target.NumberOfTextsLearnedToday = source.NumberOfTextsLearnedToday;
		}
	}
}
