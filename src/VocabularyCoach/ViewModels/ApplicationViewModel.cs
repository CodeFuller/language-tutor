using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Options;
using VocabularyCoach.Events;
using VocabularyCoach.Models;
using VocabularyCoach.Settings;
using VocabularyCoach.ViewModels.Data;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.ViewModels
{
	public class ApplicationViewModel : ObservableObject
	{
		private User CurrentUser { get; }

		public IStartPageViewModel StartPageViewModel { get; }

		public IPracticeVocabularyViewModel PracticeVocabularyViewModel { get; }

		public IPracticeResultsViewModel PracticeResultsViewModel { get; }

		public IEditVocabularyViewModel EditVocabularyViewModel { get; }

		public IProblematicTextsViewModel ProblematicTextsViewModel { get; }

		public IStatisticsChartViewModel StatisticsChartViewModel { get; }

		private IPageViewModel currentPage;

		public IPageViewModel CurrentPage
		{
			get => currentPage;
			private set => SetProperty(ref currentPage, value);
		}

		public ICommand LoadCommand { get; }

		public ApplicationViewModel(IStartPageViewModel startPageViewModel, IPracticeVocabularyViewModel practiceVocabularyViewModel,
			IPracticeResultsViewModel practiceResultsViewModel, IEditVocabularyViewModel editVocabularyViewModel, IProblematicTextsViewModel problematicTextsViewModel,
			IStatisticsChartViewModel statisticsChartViewModel, IMessenger messenger, IOptions<ApplicationSettings> options)
		{
			StartPageViewModel = startPageViewModel ?? throw new ArgumentNullException(nameof(startPageViewModel));
			PracticeVocabularyViewModel = practiceVocabularyViewModel ?? throw new ArgumentNullException(nameof(practiceVocabularyViewModel));
			PracticeResultsViewModel = practiceResultsViewModel ?? throw new ArgumentNullException(nameof(practiceResultsViewModel));
			EditVocabularyViewModel = editVocabularyViewModel ?? throw new ArgumentNullException(nameof(editVocabularyViewModel));
			ProblematicTextsViewModel = problematicTextsViewModel ?? throw new ArgumentNullException(nameof(problematicTextsViewModel));
			StatisticsChartViewModel = statisticsChartViewModel ?? throw new ArgumentNullException(nameof(statisticsChartViewModel));

			var settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
			if (String.IsNullOrEmpty(settings.UserId))
			{
				throw new InvalidOperationException("User id is not configured");
			}

			CurrentUser = new User
			{
				Id = new ItemId(settings.UserId),
			};

			LoadCommand = new AsyncRelayCommand(Load);

			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));
			messenger.Register<SwitchToStartPageEventArgs>(this, (_, _) => SwitchToStartPage(CancellationToken.None));
			messenger.Register<SwitchToPracticeVocabularyPageEventArgs>(this, (_, e) => SwitchToPracticeVocabularyPage(e.StudiedLanguage, e.KnownLanguage, CancellationToken.None));
			messenger.Register<SwitchToPracticeResultsPageEventArgs>(this, (_, e) => SwitchToPracticeResultsPage(e.StudiedLanguage, e.KnownLanguage, e.PracticeResults, CancellationToken.None));
			messenger.Register<SwitchToEditVocabularyPageEventArgs>(this, (_, e) => SwitchToEditVocabularyPage(e.StudiedLanguage, e.KnownLanguage, CancellationToken.None));
			messenger.Register<SwitchToProblematicTextsPageEventArgs>(this, (_, e) => SwitchToProblematicTextsPage(e.StudiedLanguage, e.KnownLanguage, CancellationToken.None));
			messenger.Register<SwitchToStatisticsChartPageEventArgs>(this, (_, e) => SwitchToStatisticsChartPage(e.StudiedLanguage, e.KnownLanguage, CancellationToken.None));
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

		private async void SwitchToPracticeResultsPage(Language studiedLanguage, Language knownLanguage, PracticeResults practiceResults, CancellationToken cancellationToken)
		{
			await PracticeResultsViewModel.Load(CurrentUser, studiedLanguage, knownLanguage, practiceResults, cancellationToken);

			CurrentPage = PracticeResultsViewModel;
		}

		private async void SwitchToEditVocabularyPage(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			await EditVocabularyViewModel.Load(studiedLanguage, knownLanguage, cancellationToken);

			CurrentPage = EditVocabularyViewModel;
		}

		private async void SwitchToProblematicTextsPage(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			await ProblematicTextsViewModel.Load(CurrentUser, studiedLanguage, knownLanguage, cancellationToken);

			CurrentPage = ProblematicTextsViewModel;
		}

		private async void SwitchToStatisticsChartPage(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			await StatisticsChartViewModel.Load(CurrentUser, studiedLanguage, knownLanguage, cancellationToken);

			CurrentPage = StatisticsChartViewModel;
		}
	}
}
