using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using VocabularyCoach.Events;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;
using VocabularyCoach.Services.Interfaces;
using VocabularyCoach.ViewModels.Extensions;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.ViewModels
{
	public class StartPageViewModel : ObservableObject, IStartPageViewModel
	{
		private readonly IUserService userService;

		private readonly IVocabularyService vocabularyService;

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

				OnPropertyChanged(nameof(HasTextsForPractice));
				OnPropertyChanged(nameof(HasProblematicTexts));
			}
		}

		public bool LanguagesAreSelected => SelectedStudiedLanguage != null && SelectedKnownLanguage != null;

		public bool HasTextsForPractice => UserStatistics?.RestNumberOfTextsToPracticeToday > 0;

		public bool HasProblematicTexts => UserStatistics?.NumberOfProblematicTexts > 0;

		public ICommand PracticeVocabularyCommand { get; }

		public ICommand EditVocabularyCommand { get; }

		public ICommand GoToProblematicTextsCommand { get; }

		public ICommand ShowStatisticsChartCommand { get; }

		public StartPageViewModel(IUserService userService, IVocabularyService vocabularyService, IMessenger messenger)
		{
			this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
			this.vocabularyService = vocabularyService ?? throw new ArgumentNullException(nameof(vocabularyService));
			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));

			PracticeVocabularyCommand = new RelayCommand(() => messenger.Send(new SwitchToPracticeVocabularyPageEventArgs(SelectedStudiedLanguage, SelectedKnownLanguage)));
			EditVocabularyCommand = new RelayCommand(() => messenger.Send(new SwitchToEditVocabularyPageEventArgs(SelectedStudiedLanguage, SelectedKnownLanguage)));
			GoToProblematicTextsCommand = new RelayCommand(() => messenger.Send(new SwitchToProblematicTextsPageEventArgs(SelectedStudiedLanguage, SelectedKnownLanguage)));
			ShowStatisticsChartCommand = new RelayCommand(() => messenger.Send(new SwitchToStatisticsChartPageEventArgs(SelectedStudiedLanguage, SelectedKnownLanguage)));
		}

		public async Task Load(User user, CancellationToken cancellationToken)
		{
			CurrentUser = user;
			CurrentUserSettings = await userService.GetUserSettings(user, cancellationToken);

			var languages = await vocabularyService.GetLanguages(cancellationToken);

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
				UserStatistics = await vocabularyService.GetTodayUserStatistics(CurrentUser, SelectedStudiedLanguage, SelectedKnownLanguage, cancellationToken);
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
