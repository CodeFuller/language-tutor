using System.Windows.Input;

namespace LanguageTutor.ViewModels.Interfaces
{
	internal interface IApplicationViewModel
	{
		IStartPageViewModel StartPageViewModel { get; }

		IPracticeLanguageViewModel PracticeLanguageViewModel { get; }

		IPracticeResultsViewModel PracticeResultsViewModel { get; }

		IEditDictionaryViewModel EditDictionaryViewModel { get; }

		IPageViewModel CurrentPage { get; }

		ICommand LoadCommand { get; }
	}
}
