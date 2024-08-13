using System.Windows.Input;

namespace LanguageTutor.ViewModels.Interfaces
{
	internal interface IApplicationViewModel
	{
		IStartPageViewModel StartPageViewModel { get; }

		IPerformExercisesViewModel PerformExercisesViewModel { get; }

		IExerciseResultsViewModel ExerciseResultsViewModel { get; }

		IEditDictionaryViewModel EditDictionaryViewModel { get; }

		IPageViewModel CurrentPage { get; }

		ICommand LoadCommand { get; }
	}
}
