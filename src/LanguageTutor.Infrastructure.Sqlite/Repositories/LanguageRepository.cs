using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Infrastructure.Sqlite.Extensions;
using LanguageTutor.Infrastructure.Sqlite.Internal;
using LanguageTutor.Models;
using LanguageTutor.Services.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LanguageTutor.Infrastructure.Sqlite.Repositories
{
	internal sealed class LanguageRepository : ILanguageRepository
	{
		private readonly IDbContextFactory<LanguageTutorDbContext> contextFactory;

		public LanguageRepository(IDbContextFactory<LanguageTutorDbContext> contextFactory)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
		}

		public async Task<IReadOnlyCollection<Language>> GetLanguages(CancellationToken cancellationToken)
		{
			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			var languages = await dbContext.Languages.ToListAsync(cancellationToken);

			return languages.Select(x => x.ToModel()).ToList();
		}
	}
}
