using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VocabularyCoach.Infrastructure.Sqlite.Extensions;
using VocabularyCoach.Infrastructure.Sqlite.Internal;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Interfaces.Repositories;

namespace VocabularyCoach.Infrastructure.Sqlite.Repositories
{
	internal sealed class LanguageRepository : ILanguageRepository
	{
		private readonly IDbContextFactory<VocabularyCoachDbContext> contextFactory;

		public LanguageRepository(IDbContextFactory<VocabularyCoachDbContext> contextFactory)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
		}

		public async Task<IReadOnlyCollection<Language>> GetLanguages(CancellationToken cancellationToken)
		{
			await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

			var languages = await context.Languages.ToListAsync(cancellationToken);

			return languages.Select(x => x.ToModel()).ToList();
		}
	}
}
