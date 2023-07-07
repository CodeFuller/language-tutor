using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using VocabularyCoach.Events;
using VocabularyCoach.ViewModels.Data;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.ViewModels
{
	public class CheckResultsViewModel : ObservableObject, ICheckResultsViewModel
	{
		private CheckResults checkResults;

		public CheckResults CheckResults
		{
			get => checkResults;
			private set
			{
				checkResults = value;

				OnPropertyChanged(nameof(CheckedTextsStatistics));
				OnPropertyChanged(nameof(CorrectTextStatistics));
				OnPropertyChanged(nameof(IncorrectTextStatistics));
				OnPropertyChanged(nameof(SkippedTextStatistics));
			}
		}

		public string CheckedTextsStatistics => $"{checkResults.CheckedTextsCount:N0}";

		public string CorrectTextStatistics => GetStatistics(checkResults.CorrectTextsCount, checkResults.CheckedTextsCount);

		public string IncorrectTextStatistics => GetStatistics(checkResults.IncorrectTextsCount, checkResults.CheckedTextsCount);

		public string SkippedTextStatistics => GetStatistics(checkResults.SkippedTextsCount, checkResults.CheckedTextsCount);

		public ICommand GoToStartPageCommand { get; }

		public CheckResultsViewModel(IMessenger messenger)
		{
			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));

			GoToStartPageCommand = new RelayCommand(() => messenger.Send(new SwitchToStartPageEventArgs()));
		}

		public void Load(CheckResults checkResults)
		{
			CheckResults = checkResults;
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
