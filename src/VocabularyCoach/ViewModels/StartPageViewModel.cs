using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using VocabularyCoach.Abstractions.Interfaces;
using VocabularyCoach.Abstractions.Models;
using VocabularyCoach.Events;
using VocabularyCoach.Extensions;
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

		public ICommand StudyVocabularyCommand { get; }

		public ICommand EditVocabularyCommand { get; }

		public StartPageViewModel(IVocabularyService vocabularyService, IMessenger messenger)
		{
			this.vocabularyService = vocabularyService ?? throw new ArgumentNullException(nameof(vocabularyService));
			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));

			StudyVocabularyCommand = new RelayCommand(() => messenger.Send(new SwitchToStudyVocabularyPageEventArgs(SelectedStudiedLanguage, SelectedKnownLanguage)));
			EditVocabularyCommand = new RelayCommand(() => messenger.Send(new SwitchToEditVocabularyPageEventArgs(SelectedStudiedLanguage, SelectedKnownLanguage)));
		}

		public async Task Load(CancellationToken cancellationToken)
		{
			var languages = await vocabularyService.GetLanguages(cancellationToken);

			AvailableLanguages.Clear();
			AvailableLanguages.AddRange(languages);

			// TODO: Load last studied language from settings.
			SelectedStudiedLanguage = AvailableLanguages.First();

			// TODO: Load last known language from settings.
			SelectedKnownLanguage = AvailableLanguages.Last();
		}
	}
}
