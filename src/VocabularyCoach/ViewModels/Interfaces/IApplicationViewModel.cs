using System.Windows.Input;

namespace VocabularyCoach.ViewModels.Interfaces
{
	internal interface IApplicationViewModel
	{
		IStartPageViewModel StartPageViewModel { get; }

		IPracticeVocabularyViewModel PracticeVocabularyViewModel { get; }

		IPracticeResultsViewModel PracticeResultsViewModel { get; }

		IEditVocabularyViewModel EditVocabularyViewModel { get; }

		IPageViewModel CurrentPage { get; }

		ICommand LoadCommand { get; }
	}
}
