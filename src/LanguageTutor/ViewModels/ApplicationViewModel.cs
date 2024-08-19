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

		public IPerformExercisesViewModel PerformExercisesViewModel { get; }

		public IExerciseResultsViewModel ExerciseResultsViewModel { get; }

		public IEditDictionaryViewModel EditDictionaryViewModel { get; }

		public IEditExercisesViewModel EditExercisesViewModel { get; }

		public IProblematicExercisesViewModel ProblematicExercisesViewModel { get; }

		public IStatisticsChartViewModel StatisticsChartViewModel { get; }

		private IPageViewModel currentPage;

		public IPageViewModel CurrentPage
		{
			get => currentPage;
			private set => SetProperty(ref currentPage, value);
		}

		public ICommand LoadCommand { get; }

		public ApplicationViewModel(IStartPageViewModel startPageViewModel, IPerformExercisesViewModel performExercisesViewModel,
			IExerciseResultsViewModel exerciseResultsViewModel, IEditDictionaryViewModel editDictionaryViewModel, IEditExercisesViewModel editExercisesViewModel,
			IProblematicExercisesViewModel problematicExercisesViewModel, IStatisticsChartViewModel statisticsChartViewModel, IMessenger messenger, IOptions<ApplicationSettings> options)
		{
			StartPageViewModel = startPageViewModel ?? throw new ArgumentNullException(nameof(startPageViewModel));
			PerformExercisesViewModel = performExercisesViewModel ?? throw new ArgumentNullException(nameof(performExercisesViewModel));
			ExerciseResultsViewModel = exerciseResultsViewModel ?? throw new ArgumentNullException(nameof(exerciseResultsViewModel));
			EditDictionaryViewModel = editDictionaryViewModel ?? throw new ArgumentNullException(nameof(editDictionaryViewModel));
			EditExercisesViewModel = editExercisesViewModel ?? throw new ArgumentNullException(nameof(editExercisesViewModel));
			ProblematicExercisesViewModel = problematicExercisesViewModel ?? throw new ArgumentNullException(nameof(problematicExercisesViewModel));
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
			messenger.Register<SwitchToPerformExercisesPageEventArgs>(this, (_, e) => SwitchToPerformExercisesPage(e.StudiedLanguage, e.KnownLanguage, CancellationToken.None));
			messenger.Register<SwitchToExerciseResultsPageEventArgs>(this, (_, e) => SwitchToExerciseResultsPage(e.StudiedLanguage, e.KnownLanguage, e.ExerciseResults, CancellationToken.None));
			messenger.Register<SwitchToEditDictionaryPageEventArgs>(this, (_, e) => SwitchToEditDictionaryPage(e.StudiedLanguage, e.KnownLanguage, CancellationToken.None));
			messenger.Register<SwitchToEditExercisesPageEventArgs>(this, (_, e) => SwitchToEditExercisesPage(e.StudiedLanguage, CancellationToken.None));
			messenger.Register<SwitchToProblematicExercisesPageEventArgs>(this, (_, e) => SwitchToProblematicExercisesPage(e.StudiedLanguage, e.KnownLanguage, CancellationToken.None));
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

		private async void SwitchToPerformExercisesPage(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			await PerformExercisesViewModel.Load(CurrentUser, studiedLanguage, knownLanguage, cancellationToken);

			CurrentPage = PerformExercisesViewModel;
		}

		private async void SwitchToExerciseResultsPage(Language studiedLanguage, Language knownLanguage, ExerciseResults exerciseResults, CancellationToken cancellationToken)
		{
			await ExerciseResultsViewModel.Load(CurrentUser, studiedLanguage, knownLanguage, exerciseResults, cancellationToken);

			CurrentPage = ExerciseResultsViewModel;
		}

		private async void SwitchToEditDictionaryPage(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			await EditDictionaryViewModel.Load(studiedLanguage, knownLanguage, cancellationToken);

			CurrentPage = EditDictionaryViewModel;
		}

		private async void SwitchToEditExercisesPage(Language studiedLanguage, CancellationToken cancellationToken)
		{
			await EditExercisesViewModel.Load(studiedLanguage, cancellationToken);

			CurrentPage = EditExercisesViewModel;
		}

		private async void SwitchToProblematicExercisesPage(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			await ProblematicExercisesViewModel.Load(CurrentUser, studiedLanguage, knownLanguage, cancellationToken);

			CurrentPage = ProblematicExercisesViewModel;
		}

		private async void SwitchToStatisticsChartPage(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			await StatisticsChartViewModel.Load(CurrentUser, studiedLanguage, knownLanguage, cancellationToken);

			CurrentPage = StatisticsChartViewModel;
		}
	}
}
