using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VocabularyCoach.Infrastructure.Sqlite.Internal;
using VocabularyCoach.Infrastructure.Sqlite.Repositories;
using VocabularyCoach.Services.Interfaces.Repositories;

namespace VocabularyCoach.Infrastructure.Sqlite
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddVocabularyCoachSqliteDal(this IServiceCollection services, Action<VocabularyDatabaseSettings> setupSettings)
		{
			services.Configure(setupSettings);

			services.AddVocabularyCoachDbContext(setupSettings);

			services.AddSingleton<IUserRepository, UserRepository>();
			services.AddSingleton<ILanguageRepository, LanguageRepository>();
			services.AddSingleton<ILanguageTextRepository, LanguageTextRepository>();
			services.AddSingleton<IPronunciationRecordRepository, PronunciationRecordRepository>();
			services.AddSingleton<ICheckResultRepository, CheckResultRepository>();
			services.AddSingleton<IStatisticsRepository, StatisticsRepository>();

			services.AddSingleton<IChecksumCalculator, Crc32Calculator>();

			return services;
		}

		private static IServiceCollection AddVocabularyCoachDbContext(this IServiceCollection services, Action<VocabularyDatabaseSettings> setupSettings)
		{
			var settings = new VocabularyDatabaseSettings();
			setupSettings(settings);

			if (String.IsNullOrWhiteSpace(settings.ConnectionString))
			{
				throw new InvalidOperationException("The connection string for Vocabulary DB is not configured");
			}

			// QuerySplittingBehavior: https://docs.microsoft.com/en-us/ef/core/querying/single-split-queries
			services.AddDbContext<VocabularyCoachDbContext>(
				options => options
					.UseSqlite(settings.ConnectionString, sqLiteOptions => sqLiteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)));

			services.AddSingleton<IDbContextFactory<VocabularyCoachDbContext>, VocabularyCoachDbContextFactory>();

			return services;
		}
	}
}
