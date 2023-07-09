using Microsoft.EntityFrameworkCore;
using VocabularyCoach.Infrastructure.Sqlite.Entities;

namespace VocabularyCoach.Infrastructure.Sqlite.Internal
{
	internal sealed class VocabularyCoachDbContext : DbContext
	{
		public DbSet<LanguageEntity> Languages { get; set; }

		public DbSet<TextEntity> Texts { get; set; }

		public DbSet<TranslationEntity> Translations { get; set; }

		public DbSet<PronunciationRecordEntity> PronunciationRecords { get; set; }

		public DbSet<CheckResultEntity> CheckResults { get; set; }

		public VocabularyCoachDbContext(DbContextOptions options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<TranslationEntity>(builder =>
			{
				builder.ToTable("Translations");

				builder.HasKey(x => new { x.TextId1, x.TextId2 });
			});
		}
	}
}
