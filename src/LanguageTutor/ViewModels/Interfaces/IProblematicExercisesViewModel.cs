using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageTutor.Models;
using LanguageTutor.ViewModels.Exercises;

namespace LanguageTutor.ViewModels.Interfaces
{
	public interface IProblematicExercisesViewModel : IPageViewModel
	{
		ObservableCollection<BasicProblematicExerciseViewModel> ProblematicExercises { get; }

		BasicProblematicExerciseViewModel SelectedExercise { get; set; }

		ICommand GoToStartPageCommand { get; }

		Task Load(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);
	}
}
