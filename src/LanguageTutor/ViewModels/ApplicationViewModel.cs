using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LanguageTutor.Events;
using LanguageTutor.Models;
using LanguageTutor.Settings;
using LanguageTutor.ViewModels.Data;
using LanguageTutor.ViewModels.Interfaces;
using Microsoft.Extensions.Options;

namespace LanguageTutor.ViewModels
{
	public class ApplicationViewModel : ObservableObject
	{
		private User CurrentUser { get; }

		public IStartPageViewModel StartPageViewModel { get; }

		public IPracticeLanguageViewModel PracticeLanguageViewModel { get; }

		public IPracticeResultsViewModel PracticeResultsViewModel { get; }

		public IEditDictionaryViewModel EditDictionaryViewModel { get; }

		public IProblematicTextsViewModel ProblematicTextsViewModel { get; }

		public IStatisticsChartViewModel StatisticsChartViewModel { get; }

		private IPageViewModel currentPage;

		public IPageViewModel CurrentPage
		{
			get => currentPage;
			private set => SetProperty(ref currentPage, value);
		}

		public ICommand LoadCommand { get; }

		public ApplicationViewModel(IStartPageViewModel startPageViewModel, IPracticeLanguageViewModel practiceLanguageViewModel,
			IPracticeResultsViewModel practiceResultsViewModel, IEditDictionaryViewModel editDictionaryViewModel, IProblematicTextsViewModel problematicTextsViewModel,
			IStatisticsChartViewModel statisticsChartViewModel, IMessenger messenger, IOptions<ApplicationSettings> options)
		{
			StartPageViewModel = startPageViewModel ?? throw new ArgumentNullException(nameof(startPageViewModel));
			PracticeLanguageViewModel = practiceLanguageViewModel ?? throw new ArgumentNullException(nameof(practiceLanguageViewModel));
			PracticeResultsViewModel = practiceResultsViewModel ?? throw new ArgumentNullException(nameof(practiceResultsViewModel));
			EditDictionaryViewModel = editDictionaryViewModel ?? throw new ArgumentNullException(nameof(editDictionaryViewModel));
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
			messenger.Register<SwitchToPracticeLanguagePageEventArgs>(this, (_, e) => SwitchToPracticeLanguagePage(e.StudiedLanguage, e.KnownLanguage, CancellationToken.None));
			messenger.Register<SwitchToPracticeResultsPageEventArgs>(this, (_, e) => SwitchToPracticeResultsPage(e.StudiedLanguage, e.KnownLanguage, e.PracticeResults, CancellationToken.None));
			messenger.Register<SwitchToEditDictionaryPageEventArgs>(this, (_, e) => SwitchToEditDictionaryPage(e.StudiedLanguage, e.KnownLanguage, CancellationToken.None));
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

		private async void SwitchToPracticeLanguagePage(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			await PracticeLanguageViewModel.Load(CurrentUser, studiedLanguage, knownLanguage, cancellationToken);

			CurrentPage = PracticeLanguageViewModel;
		}

		private async void SwitchToPracticeResultsPage(Language studiedLanguage, Language knownLanguage, PracticeResults practiceResults, CancellationToken cancellationToken)
		{
			await PracticeResultsViewModel.Load(CurrentUser, studiedLanguage, knownLanguage, practiceResults, cancellationToken);

			CurrentPage = PracticeResultsViewModel;
		}

		private async void SwitchToEditDictionaryPage(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			await EditDictionaryViewModel.Load(studiedLanguage, knownLanguage, cancellationToken);

			CurrentPage = EditDictionaryViewModel;
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
