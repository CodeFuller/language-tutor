using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;

namespace VocabularyCoach.Services.Interfaces
{
	public interface IUserService
	{
		Task<UserSettingsData> GetUserSettings(User user, CancellationToken cancellationToken);

		Task UpdateUserSettings(User user, UserSettingsData settings, CancellationToken cancellationToken);
	}
}
