using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;
using LanguageTutor.Services.Data;

namespace LanguageTutor.Services.Interfaces.Repositories
{
	public interface IUserRepository
	{
		Task<UserSettingsData> GetUserSettings(ItemId userId, CancellationToken cancellationToken);

		Task UpdateUserSettings(ItemId userId, UserSettingsData settings, CancellationToken cancellationToken);
	}
}
