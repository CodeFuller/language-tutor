using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VocabularyCoach.Infrastructure.Sqlite.Extensions;
using VocabularyCoach.Infrastructure.Sqlite.Internal;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Interfaces.Repositories;

namespace VocabularyCoach.Infrastructure.Sqlite.Repositories
{
	internal sealed class CheckResultRepository : ICheckResultRepository
	{
		private readonly IDbContextFactory<VocabularyCoachDbContext> contextFactory;

		public CheckResultRepository(IDbContextFactory<VocabularyCoachDbContext> contextFactory)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
		}

		public async Task AddCheckResult(ItemId userId, ItemId languageTextId, CheckResult checkResult, CancellationToken cancellationToken)
		{
			var checkResultEntity = checkResult.ToEntity(userId, languageTextId);

			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			await dbContext.CheckResults.AddAsync(checkResultEntity, cancellationToken);
			await dbContext.SaveChangesAsync(cancellationToken);

			checkResult.Id = checkResultEntity.Id.ToItemId();
		}
	}
}
