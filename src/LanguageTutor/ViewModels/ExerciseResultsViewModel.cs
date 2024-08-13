using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LanguageTutor.Events;
using LanguageTutor.Models;
using LanguageTutor.Services.Interfaces;
using LanguageTutor.ViewModels.Data;
using LanguageTutor.ViewModels.Interfaces;

namespace LanguageTutor.ViewModels
{
	public class ExerciseResultsViewModel : ObservableObject, IExerciseResultsViewModel
	{
		private readonly ITutorService tutorService;

		private ExerciseResults exerciseResults;

		public ExerciseResults ExerciseResults
		{
			get => exerciseResults;
			private set
			{
				exerciseResults = value;

				OnPropertyChanged(nameof(TotalExercisesStatistics));
				OnPropertyChanged(nameof(SuccessfulExercisesStatistics));
				OnPropertyChanged(nameof(FailedExercisesStatistics));
				OnPropertyChanged(nameof(SkippedExercisesStatistics));
			}
		}

		public string TotalExercisesStatistics => $"{exerciseResults.NumberOfPerformedExercises:N0}";

		public string SuccessfulExercisesStatistics => GetStatistics(exerciseResults.NumberOfSuccessfulExercises, exerciseResults.NumberOfPerformedExercises);

		public string FailedExercisesStatistics => GetStatistics(exerciseResults.NumberOfFailedExercises, exerciseResults.NumberOfPerformedExercises);

		public string SkippedExercisesStatistics => GetStatistics(exerciseResults.NumberOfSkippedExercises, exerciseResults.NumberOfPerformedExercises);

		public ICommand GoToStartPageCommand { get; }

		public ExerciseResultsViewModel(ITutorService tutorService, IMessenger messenger)
		{
			this.tutorService = tutorService ?? throw new ArgumentNullException(nameof(tutorService));
			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));

			GoToStartPageCommand = new RelayCommand(() => messenger.Send(new SwitchToStartPageEventArgs()));
		}

		public async Task Load(User user, Language studiedLanguage, Language knownLanguage, ExerciseResults results, CancellationToken cancellationToken)
		{
			ExerciseResults = results;

			await tutorService.UpdateTodayUserStatistics(user, studiedLanguage, knownLanguage, cancellationToken);
		}

		private static string GetStatistics(int statisticsCount, int totalCount)
		{
			if (totalCount == 0)
			{
				return "0";
			}

			var percentage = 100 * (statisticsCount / (double)totalCount);

			return $"{statisticsCount:N0} ({percentage:N1}%)";
		}
	}
}
