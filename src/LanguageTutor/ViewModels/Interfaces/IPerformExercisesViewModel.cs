using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageTutor.Models;

namespace LanguageTutor.ViewModels.Interfaces
{
	public interface IPerformExercisesViewModel : IPageViewModel
	{
		int NumberOfExercisesToPerform { get; }

		int NumberOfPerformedExercises { get; }

		string ProgressInfo { get; }

		IExerciseViewModel CurrentExerciseViewModel { get; }

		bool ExerciseWasChecked { get; }

		bool CanSwitchToNextExercise { get; }

		ICommand CheckExerciseCommand { get; }

		ICommand SwitchToNextExerciseCommand { get; }

		ICommand FinishExercisesCommand { get; }

		Task Load(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);
	}
}
