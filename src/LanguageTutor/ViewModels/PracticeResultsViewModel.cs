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
	public class PracticeResultsViewModel : ObservableObject, IPracticeResultsViewModel
	{
		private readonly ITutorService tutorService;

		private PracticeResults practiceResults;

		public PracticeResults PracticeResults
		{
			get => practiceResults;
			private set
			{
				practiceResults = value;

				OnPropertyChanged(nameof(PracticedTextsStatistics));
				OnPropertyChanged(nameof(CorrectTextStatistics));
				OnPropertyChanged(nameof(IncorrectTextStatistics));
				OnPropertyChanged(nameof(SkippedTextStatistics));
			}
		}

		public string PracticedTextsStatistics => $"{practiceResults.CheckedTextsCount:N0}";

		public string CorrectTextStatistics => GetStatistics(practiceResults.CorrectTextsCount, practiceResults.CheckedTextsCount);

		public string IncorrectTextStatistics => GetStatistics(practiceResults.IncorrectTextsCount, practiceResults.CheckedTextsCount);

		public string SkippedTextStatistics => GetStatistics(practiceResults.SkippedTextsCount, practiceResults.CheckedTextsCount);

		public ICommand GoToStartPageCommand { get; }

		public PracticeResultsViewModel(ITutorService tutorService, IMessenger messenger)
		{
			this.tutorService = tutorService ?? throw new ArgumentNullException(nameof(tutorService));
			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));

			GoToStartPageCommand = new RelayCommand(() => messenger.Send(new SwitchToStartPageEventArgs()));
		}

		public async Task Load(User user, Language studiedLanguage, Language knownLanguage, PracticeResults results, CancellationToken cancellationToken)
		{
			PracticeResults = results;

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
