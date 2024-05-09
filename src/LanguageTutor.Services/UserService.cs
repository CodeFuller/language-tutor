using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;
using LanguageTutor.Services.Data;
using LanguageTutor.Services.Interfaces;
using LanguageTutor.Services.Interfaces.Repositories;

namespace LanguageTutor.Services
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
