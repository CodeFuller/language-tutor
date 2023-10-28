using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;

namespace VocabularyCoach.Services.Interfaces.Repositories
{
	public interface IUserRepository
	{
		Task<UserSettingsData> GetUserSettings(ItemId userId, CancellationToken cancellationToken);

		Task UpdateUserSettings(ItemId userId, UserSettingsData settings, CancellationToken cancellationToken);
	}
}
