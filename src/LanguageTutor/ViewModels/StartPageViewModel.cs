using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LanguageTutor.Events;
using LanguageTutor.Models;
using LanguageTutor.Services.Data;
using LanguageTutor.Services.Interfaces;
using LanguageTutor.ViewModels.Extensions;
using LanguageTutor.ViewModels.Interfaces;

namespace LanguageTutor.ViewModels
{
	public class StartPageViewModel : ObservableObject, IStartPageViewModel
	{
		private readonly IUserService userService;

		private readonly ITutorService tutorService;

		private User CurrentUser { get; set; }

		private UserSettingsData CurrentUserSettings { get; set; }

		public ObservableCollection<Language> AvailableLanguages { get; } = new();

		private Language selectedStudiedLanguage;

		public Language SelectedStudiedLanguage
		{
			get => selectedStudiedLanguage;
			set
			{
				if (value?.Id == SelectedKnownLanguage?.Id)
				{
					// We do not call setter of SelectedKnownLanguage, to avoid statistics re-loading while selectedStudiedLanguage is not yet updated.
					selectedKnownLanguage = null;
					OnPropertyChanged(nameof(SelectedKnownLanguage));
				}

				SetProperty(ref selectedStudiedLanguage, value);

				OnLanguagesUpdated(CancellationToken.None).GetAwaiter().GetResult();

				OnPropertyChanged(nameof(LanguagesAreSelected));
			}
		}

		private Language selectedKnownLanguage;

		public Language SelectedKnownLanguage
		{
			get => selectedKnownLanguage;
			set
			{
				if (value?.Id == SelectedStudiedLanguage?.Id)
				{
					// We do not call setter of SelectedStudiedLanguage, to avoid statistics re-loading while selectedKnownLanguage is not yet updated.
					selectedStudiedLanguage = null;
					OnPropertyChanged(nameof(SelectedStudiedLanguage));
				}

				SetProperty(ref selectedKnownLanguage, value);

				OnLanguagesUpdated(CancellationToken.None).GetAwaiter().GetResult();

				OnPropertyChanged(nameof(LanguagesAreSelected));
			}
		}

		private UserStatisticsData userStatistics;

		public UserStatisticsData UserStatistics
		{
			get => userStatistics;
			private set
			{
				SetProperty(ref userStatistics, value);

				OnPropertyChanged(nameof(HasExercisesToPerform));
				OnPropertyChanged(nameof(HasProblematicExercises));
			}
		}

		public string RestNumberOfExercisesToPerformToday =>
			UserStatistics.RestNumberOfExercisesToPerformTodayIfNoLimit == UserStatistics.RestNumberOfExercisesToPerformToday
				? UserStatistics.RestNumberOfExercisesToPerformToday.ToString(CultureInfo.InvariantCulture)
				: $"{UserStatistics.RestNumberOfExercisesToPerformToday} ({UserStatistics.RestNumberOfExercisesToPerformTodayIfNoLimit})";

		public bool LanguagesAreSelected => SelectedStudiedLanguage != null && SelectedKnownLanguage != null;

		public bool HasExercisesToPerform => UserStatistics?.RestNumberOfExercisesToPerformToday > 0;

		public bool HasProblematicExercises => UserStatistics?.NumberOfProblematicExercises > 0;

		public ICommand PerformExercisesCommand { get; }

		public ICommand EditDictionaryCommand { get; }

		public ICommand GoToProblematicExercisesCommand { get; }

		public ICommand ShowStatisticsChartCommand { get; }

		public StartPageViewModel(IUserService userService, ITutorService tutorService, IMessenger messenger)
		{
			this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
			this.tutorService = tutorService ?? throw new ArgumentNullException(nameof(tutorService));
			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));

			PerformExercisesCommand = new RelayCommand(() => messenger.Send(new SwitchToPerformExercisesPageEventArgs(SelectedStudiedLanguage, SelectedKnownLanguage)));
			EditDictionaryCommand = new RelayCommand(() => messenger.Send(new SwitchToEditDictionaryPageEventArgs(SelectedStudiedLanguage, SelectedKnownLanguage)));
			GoToProblematicExercisesCommand = new RelayCommand(() => messenger.Send(new SwitchToProblematicExercisesPageEventArgs(SelectedStudiedLanguage, SelectedKnownLanguage)));
			ShowStatisticsChartCommand = new RelayCommand(() => messenger.Send(new SwitchToStatisticsChartPageEventArgs(SelectedStudiedLanguage, SelectedKnownLanguage)));
		}

		public async Task Load(User user, CancellationToken cancellationToken)
		{
			// We clear selected languages to prevent duplicated load of user statistics from OnLanguagesUpdated method.
			// This will happen if SelectedStudiedLanguage and SelectedKnownLanguage are set from the previous Load.
			// Now the user statistics will be loaded once after both SelectedStudiedLanguage and SelectedKnownLanguage are set by current method.
			SelectedStudiedLanguage = null;
			SelectedKnownLanguage = null;

			CurrentUser = user;
			CurrentUserSettings = await userService.GetUserSettings(user, cancellationToken);

			var languages = await tutorService.GetLanguages(cancellationToken);

			AvailableLanguages.Clear();
			AvailableLanguages.AddRange(languages);

			SelectedStudiedLanguage = AvailableLanguages.FirstOrDefault(x => x.Id == CurrentUserSettings.LastStudiedLanguage?.Id);
			SelectedKnownLanguage = AvailableLanguages.FirstOrDefault(x => x.Id == CurrentUserSettings.LastKnownLanguage?.Id);
		}

		private async Task OnLanguagesUpdated(CancellationToken cancellationToken)
		{
			await LoadUserStatistics(cancellationToken);

			await SaveUserSettingsIfNecessary(cancellationToken);
		}

		private async Task LoadUserStatistics(CancellationToken cancellationToken)
		{
			if (LanguagesAreSelected)
			{
				UserStatistics = await tutorService.GetTodayUserStatistics(CurrentUser, SelectedStudiedLanguage, SelectedKnownLanguage, cancellationToken);
			}
			else
			{
				UserStatistics = null;
			}
		}

		private async Task SaveUserSettingsIfNecessary(CancellationToken cancellationToken)
		{
			if (SelectedStudiedLanguage == null || SelectedKnownLanguage == null)
			{
				return;
			}

			if (SelectedStudiedLanguage.Id == CurrentUserSettings.LastStudiedLanguage?.Id &&
			    SelectedKnownLanguage.Id == CurrentUserSettings.LastKnownLanguage?.Id)
			{
				return;
			}

			var userSettings = new UserSettingsData
			{
				LastStudiedLanguage = SelectedStudiedLanguage,
				LastKnownLanguage = SelectedKnownLanguage,
			};

			await userService.UpdateUserSettings(CurrentUser, userSettings, cancellationToken);
		}
	}
}
