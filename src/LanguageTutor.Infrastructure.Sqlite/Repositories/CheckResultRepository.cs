using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Infrastructure.Sqlite.Extensions;
using LanguageTutor.Infrastructure.Sqlite.Internal;
using LanguageTutor.Models;
using LanguageTutor.Services.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LanguageTutor.Infrastructure.Sqlite.Repositories
{
	internal sealed class CheckResultRepository : ICheckResultRepository
	{
		private readonly IDbContextFactory<LanguageTutorDbContext> contextFactory;

		public CheckResultRepository(IDbContextFactory<LanguageTutorDbContext> contextFactory)
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
