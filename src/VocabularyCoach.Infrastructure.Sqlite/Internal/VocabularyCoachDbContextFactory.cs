using System;
using Microsoft.EntityFrameworkCore;

namespace VocabularyCoach.Infrastructure.Sqlite.Internal
{
	internal sealed class VocabularyCoachDbContextFactory : IDbContextFactory<VocabularyCoachDbContext>
	{
		private readonly DbContextOptions<VocabularyCoachDbContext> options;

		public VocabularyCoachDbContextFactory(DbContextOptions<VocabularyCoachDbContext> options)
		{
			this.options = options ?? throw new ArgumentNullException(nameof(options));
		}

		public VocabularyCoachDbContext CreateDbContext()
		{
			return new VocabularyCoachDbContext(options);
		}
	}
}
