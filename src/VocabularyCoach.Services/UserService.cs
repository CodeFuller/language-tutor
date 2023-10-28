using System;
using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;
using VocabularyCoach.Services.Interfaces;
using VocabularyCoach.Services.Interfaces.Repositories;

namespace VocabularyCoach.Services
{
	internal class UserService : IUserService
	{
		private readonly IUserRepository userRepository;

		public UserService(IUserRepository userRepository)
		{
			this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
		}

		public Task<UserSettingsData> GetUserSettings(User user, CancellationToken cancellationToken)
		{
			return userRepository.GetUserSettings(user.Id, cancellationToken);
		}

		public Task UpdateUserSettings(User user, UserSettingsData settings, CancellationToken cancellationToken)
		{
			return userRepository.UpdateUserSettings(user.Id, settings, cancellationToken);
		}
	}
}
