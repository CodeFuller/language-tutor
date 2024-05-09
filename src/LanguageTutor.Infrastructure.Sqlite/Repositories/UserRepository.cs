using System;
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
	internal sealed class UserRepository : IUserRepository
	{
		private readonly IDbContextFactory<LanguageTutorDbContext> contextFactory;

		public UserRepository(IDbContextFactory<LanguageTutorDbContext> contextFactory)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
		}

		public async Task<UserSettingsData> GetUserSettings(ItemId userId, CancellationToken cancellationToken)
		{
			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			var userSettings = await dbContext.UserSettings
				.Include(x => x.LastStudiedLanguage)
				.Include(x => x.LastKnownLanguage)
				.SingleOrDefaultAsync(x => x.UserId == userId.ToInt32(), cancellationToken);

			return new UserSettingsData
			{
				LastStudiedLanguage = userSettings?.LastStudiedLanguage?.ToModel(),
				LastKnownLanguage = userSettings?.LastKnownLanguage?.ToModel(),
			};
		}

		public async Task UpdateUserSettings(ItemId userId, UserSettingsData settings, CancellationToken cancellationToken)
		{
			await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

			var userSettingsEntity = await dbContext.UserSettings.SingleOrDefaultAsync(x => x.UserId == userId.ToInt32(), cancellationToken);

			if (userSettingsEntity == null)
			{
				userSettingsEntity = new UserSettingsEntity
				{
					UserId = userId.ToInt32(),
				};

				CopyUserSettings(settings, userSettingsEntity);

				await dbContext.UserSettings.AddAsync(userSettingsEntity, cancellationToken);
			}
			else
			{
				CopyUserSettings(settings, userSettingsEntity);
			}

			await dbContext.SaveChangesAsync(cancellationToken);
		}

		private static void CopyUserSettings(UserSettingsData source, UserSettingsEntity target)
		{
			target.LastStudiedLanguageId = source.LastStudiedLanguage.Id.ToInt32();
			target.LastKnownLanguageId = source.LastKnownLanguage.Id.ToInt32();
		}
	}
}
