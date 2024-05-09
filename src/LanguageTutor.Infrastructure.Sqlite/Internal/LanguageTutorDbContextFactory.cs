using System;
using Microsoft.EntityFrameworkCore;

namespace LanguageTutor.Infrastructure.Sqlite.Internal
{
	internal sealed class LanguageTutorDbContextFactory : IDbContextFactory<LanguageTutorDbContext>
	{
		private readonly DbContextOptions<LanguageTutorDbContext> options;

		public LanguageTutorDbContextFactory(DbContextOptions<LanguageTutorDbContext> options)
		{
			this.options = options ?? throw new ArgumentNullException(nameof(options));
		}

		public LanguageTutorDbContext CreateDbContext()
		{
			return new LanguageTutorDbContext(options);
		}
	}
}
