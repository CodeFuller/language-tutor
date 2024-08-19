using System;
using LanguageTutor.Infrastructure.Sqlite.Internal;
using LanguageTutor.Infrastructure.Sqlite.Repositories;
using LanguageTutor.Services.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LanguageTutor.Infrastructure.Sqlite
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddLanguageTutorSqliteDal(this IServiceCollection services, Action<LanguageTutorDatabaseSettings> setupSettings)
		{
			services.Configure(setupSettings);

			services.AddLanguageTutorDbContext(setupSettings);

			services.AddSingleton<IUserRepository, UserRepository>();
			services.AddSingleton<ILanguageRepository, LanguageRepository>();
			services.AddSingleton<ILanguageTextRepository, LanguageTextRepository>();
			services.AddSingleton<IPronunciationRecordRepository, PronunciationRecordRepository>();
			services.AddSingleton<IExerciseRepository, ExerciseRepository>();
			services.AddSingleton<IExerciseResultRepository, ExerciseResultRepository>();
			services.AddSingleton<IStatisticsRepository, StatisticsRepository>();

			services.AddSingleton<IChecksumCalculator, Crc32Calculator>();
			services.AddSingleton<IJsonSerializer, JsonSerializer>();

			return services;
		}

		private static IServiceCollection AddLanguageTutorDbContext(this IServiceCollection services, Action<LanguageTutorDatabaseSettings> setupSettings)
		{
			var settings = new LanguageTutorDatabaseSettings();
			setupSettings(settings);

			if (String.IsNullOrWhiteSpace(settings.ConnectionString))
			{
				throw new InvalidOperationException("The connection string for Language Tutor DB is not configured");
			}

			// QuerySplittingBehavior: https://docs.microsoft.com/en-us/ef/core/querying/single-split-queries
			services.AddDbContext<LanguageTutorDbContext>(
				options => options
					.UseSqlite(settings.ConnectionString, sqLiteOptions => sqLiteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)));

			services.AddSingleton<IDbContextFactory<LanguageTutorDbContext>, LanguageTutorDbContextFactory>();

			return services;
		}
	}
}
