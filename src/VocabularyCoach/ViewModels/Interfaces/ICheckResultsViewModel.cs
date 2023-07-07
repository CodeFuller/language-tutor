using System.Windows.Input;
using VocabularyCoach.ViewModels.Data;

namespace VocabularyCoach.ViewModels.Interfaces
{
	public interface ICheckResultsViewModel : IPageViewModel
	{
		string CheckedTextsStatistics { get; }

		string CorrectTextStatistics { get; }

		string IncorrectTextStatistics { get; }

		string SkippedTextStatistics { get; }

		ICommand GoToStartPageCommand { get; }

		void Load(CheckResults checkResults);
	}
}
