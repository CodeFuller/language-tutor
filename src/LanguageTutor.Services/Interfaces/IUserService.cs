using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;
using LanguageTutor.Services.Data;

namespace LanguageTutor.Services.Interfaces
{
	public interface IUserService
	{
		Task<UserSettingsData> GetUserSettings(User user, CancellationToken cancellationToken);

		Task UpdateUserSettings(User user, UserSettingsData settings, CancellationToken cancellationToken);
	}
}
