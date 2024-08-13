using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Infrastructure.Sqlite.Entities;
using LanguageTutor.Infrastructure.Sqlite.Extensions;
using LanguageTutor.Infrastructure.Sqlite.Internal;
using LanguageTutor.Models;
using LanguageTutor.Services.Data;
using LanguageTutor.Services.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LanguageTutor.Infrastructure.Sqlite.Repositories
{
	internal sealed class StatisticsRepository : IStatisticsRepository
	{
		private readonly IDbContextFactory<LanguageTutorDbContext> contextFactory;

		public StatisticsRepository(IDbContextFactory<LanguageTutorDbContext> contextFactory)
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
					TotalNumberOfExercises = x.TotalNumberOfExercises,
					TotalNumberOfLearnedExercises = x.TotalNumberOfLearnedExercises,
					NumberOfProblematicExercises = x.NumberOfProblematicExercises,
					RestNumberOfExercisesToPerformToday = x.RestNumberOfExercisesToPerformToday,
					RestNumberOfExercisesToPerformTodayIfNoLimit = x.RestNumberOfExercisesToPerformTodayIfNoLimit,
					NumberOfExercisesPerformedToday = x.NumberOfExercisesPerformedToday,
					NumberOfExercisesLearnedToday = x.NumberOfExercisesLearnedToday,
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
			target.TotalNumberOfExercises = source.TotalNumberOfExercises;
			target.TotalNumberOfLearnedExercises = source.TotalNumberOfLearnedExercises;
			target.NumberOfProblematicExercises = source.NumberOfProblematicExercises;
			target.RestNumberOfExercisesToPerformToday = source.RestNumberOfExercisesToPerformToday;
			target.RestNumberOfExercisesToPerformTodayIfNoLimit = source.RestNumberOfExercisesToPerformTodayIfNoLimit;
			target.NumberOfExercisesPerformedToday = source.NumberOfExercisesPerformedToday;
			target.NumberOfExercisesLearnedToday = source.NumberOfExercisesLearnedToday;
		}
	}
}
