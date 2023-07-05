using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using VocabularyCoach.Abstractions.Interfaces;
using VocabularyCoach.Abstractions.Models;
using VocabularyCoach.Events;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.ViewModels
{
	public class ApplicationViewModel : ObservableObject
	{
		private readonly IMessenger messenger;

		public IStartPageViewModel StartPageViewModel { get; }

		public IStudyVocabularyViewModel StudyVocabularyViewModel { get; }

		public IEditVocabularyViewModel EditVocabularyViewModel { get; }

		private IPageViewModel currentPage;

		public IPageViewModel CurrentPage
		{
			get => currentPage;
			private set => SetProperty(ref currentPage, value);
		}

		public ICommand LoadCommand { get; }

		public ApplicationViewModel(IStartPageViewModel startPageViewModel, IStudyVocabularyViewModel studyVocabularyViewModel,
			IEditVocabularyViewModel editVocabularyViewModel, IMessenger messenger)
		{
			StartPageViewModel = startPageViewModel ?? throw new ArgumentNullException(nameof(startPageViewModel));
			StudyVocabularyViewModel = studyVocabularyViewModel ?? throw new ArgumentNullException(nameof(studyVocabularyViewModel));
			EditVocabularyViewModel = editVocabularyViewModel ?? throw new ArgumentNullException(nameof(editVocabularyViewModel));

			this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

			LoadCommand = new AsyncRelayCommand(Load);

			this.messenger.Register<SwitchToStartPageEventArgs>(this, (_, _) => SwitchToStartPage(CancellationToken.None));
			this.messenger.Register<SwitchToStudyVocabularyPageEventArgs>(this, (_, e) => SwitchToStudyVocabularyPage(e.StudiedLanguage, e.KnownLanguage, CancellationToken.None));
			this.messenger.Register<SwitchToEditVocabularyPageEventArgs>(this, (_, _) => CurrentPage = EditVocabularyViewModel);
		}

		private async Task Load(CancellationToken cancellationToken)
		{
			await LoadAndSetStartPage(cancellationToken);
		}

		private async void SwitchToStartPage(CancellationToken cancellationToken)
		{
			await LoadAndSetStartPage(cancellationToken);
		}

		private async Task LoadAndSetStartPage(CancellationToken cancellationToken)
		{
			await StartPageViewModel.Load(cancellationToken);

			CurrentPage = StartPageViewModel;
		}

		private async void SwitchToStudyVocabularyPage(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			await StudyVocabularyViewModel.Load(studiedLanguage, knownLanguage, cancellationToken);

			CurrentPage = StudyVocabularyViewModel;
		}
	}
}
