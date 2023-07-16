using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using VocabularyCoach.Events;
using VocabularyCoach.Models;
using VocabularyCoach.ViewModels.Data;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.ViewModels
{
	public class ApplicationViewModel : ObservableObject
	{
		// Currently we do not support multi-user mode on application level (although it is supported on service level).
		private User CurrentUser { get; } = new()
		{
			Id = new ItemId("1"),
			Name = "Default User",
		};

		public IStartPageViewModel StartPageViewModel { get; }

		public IPracticeVocabularyViewModel PracticeVocabularyViewModel { get; }

		public ICheckResultsViewModel CheckResultsViewModel { get; }

		public IEditVocabularyViewModel EditVocabularyViewModel { get; }

		private IPageViewModel currentPage;

		public IPageViewModel CurrentPage
		{
			get => currentPage;
			private set => SetProperty(ref currentPage, value);
		}

		public ICommand LoadCommand { get; }

		public ApplicationViewModel(IStartPageViewModel startPageViewModel, IPracticeVocabularyViewModel practiceVocabularyViewModel,
			ICheckResultsViewModel checkResultsViewModel, IEditVocabularyViewModel editVocabularyViewModel, IMessenger messenger)
		{
			StartPageViewModel = startPageViewModel ?? throw new ArgumentNullException(nameof(startPageViewModel));
			PracticeVocabularyViewModel = practiceVocabularyViewModel ?? throw new ArgumentNullException(nameof(practiceVocabularyViewModel));
			CheckResultsViewModel = checkResultsViewModel ?? throw new ArgumentNullException(nameof(checkResultsViewModel));
			EditVocabularyViewModel = editVocabularyViewModel ?? throw new ArgumentNullException(nameof(editVocabularyViewModel));

			LoadCommand = new AsyncRelayCommand(Load);

			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));
			messenger.Register<SwitchToStartPageEventArgs>(this, (_, _) => SwitchToStartPage(CancellationToken.None));
			messenger.Register<SwitchToPracticeVocabularyPageEventArgs>(this, (_, e) => SwitchToPracticeVocabularyPage(e.StudiedLanguage, e.KnownLanguage, CancellationToken.None));
			messenger.Register<SwitchToCheckResultsPageEventArgs>(this, (_, e) => SwitchToCheckResultsPage(e.CheckResults));
			messenger.Register<SwitchToEditVocabularyPageEventArgs>(this, (_, e) => SwitchToEditVocabularyPage(e.StudiedLanguage, e.KnownLanguage, CancellationToken.None));
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
			await StartPageViewModel.Load(CurrentUser, cancellationToken);

			CurrentPage = StartPageViewModel;
		}

		private async void SwitchToPracticeVocabularyPage(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			await PracticeVocabularyViewModel.Load(CurrentUser, studiedLanguage, knownLanguage, cancellationToken);

			CurrentPage = PracticeVocabularyViewModel;
		}

		private void SwitchToCheckResultsPage(CheckResults checkResults)
		{
			CheckResultsViewModel.Load(checkResults);

			CurrentPage = CheckResultsViewModel;
		}

		private async void SwitchToEditVocabularyPage(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			await EditVocabularyViewModel.Load(studiedLanguage, knownLanguage, cancellationToken);

			CurrentPage = EditVocabularyViewModel;
		}
	}
}
