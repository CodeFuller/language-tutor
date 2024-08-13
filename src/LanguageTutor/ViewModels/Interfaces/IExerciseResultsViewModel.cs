using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageTutor.Models;
using LanguageTutor.ViewModels.Data;

namespace LanguageTutor.ViewModels.Interfaces
{
	public interface IExerciseResultsViewModel : IPageViewModel
	{
		string TotalExercisesStatistics { get; }

		string SuccessfulExercisesStatistics { get; }

		string FailedExercisesStatistics { get; }

		string SkippedExercisesStatistics { get; }

		ICommand GoToStartPageCommand { get; }

		Task Load(User user, Language studiedLanguage, Language knownLanguage, ExerciseResults results, CancellationToken cancellationToken);
	}
}
