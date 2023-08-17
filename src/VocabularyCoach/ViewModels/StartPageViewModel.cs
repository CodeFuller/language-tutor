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
using VocabularyCoach.Extensions;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;
using VocabularyCoach.Services.Interfaces;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.ViewModels
{
	public class StartPageViewModel : ObservableObject, IStartPageViewModel
	{
		private readonly IVocabularyService vocabularyService;

		public ObservableCollection<Language> AvailableLanguages { get; } = new();

		private Language selectedStudiedLanguage;

		public Language SelectedStudiedLanguage
		{
			get => selectedStudiedLanguage;
			set => SetProperty(ref selectedStudiedLanguage, value);
		}

		private Language selectedKnownLanguage;

		public Language SelectedKnownLanguage
		{
			get => selectedKnownLanguage;
			set => SetProperty(ref selectedKnownLanguage, value);
		}

		private UserStatisticsData userStatistics;

		public UserStatisticsData UserStatistics
		{
			get => userStatistics;
			private set
			{
				SetProperty(ref userStatistics, value);

				OnPropertyChanged(nameof(HasTextsForPractice));
			}
		}

		public bool HasTextsForPractice => UserStatistics?.RestNumberOfTextsToPracticeToday > 0;

		public ICommand PracticeVocabularyCommand { get; }

		public ICommand EditVocabularyCommand { get; }

		public StartPageViewModel(IVocabularyService vocabularyService, IMessenger messenger)
		{
			this.vocabularyService = vocabularyService ?? throw new ArgumentNullException(nameof(vocabularyService));
			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));

			PracticeVocabularyCommand = new RelayCommand(() => messenger.Send(new SwitchToPracticeVocabularyPageEventArgs(SelectedStudiedLanguage, SelectedKnownLanguage)));
			EditVocabularyCommand = new RelayCommand(() => messenger.Send(new SwitchToEditVocabularyPageEventArgs(SelectedStudiedLanguage, SelectedKnownLanguage)));
		}

		public async Task Load(User user, CancellationToken cancellationToken)
		{
			var languages = await vocabularyService.GetLanguages(cancellationToken);

			AvailableLanguages.Clear();
			AvailableLanguages.AddRange(languages);

			// TODO: Load last languages from application settings.
			SelectedStudiedLanguage = AvailableLanguages.First();
			SelectedKnownLanguage = AvailableLanguages.Last();

			// TODO: Handle change in language selection.
			UserStatistics = await vocabularyService.GetTodayUserStatistics(user, SelectedStudiedLanguage, SelectedKnownLanguage, cancellationToken);
		}
	}
}
