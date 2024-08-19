using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LanguageTutor.Models;
using LanguageTutor.ViewModels.Exercises;

namespace LanguageTutor.ViewModels.Interfaces
{
	public interface IEditExercisesViewModel : IPageViewModel
	{
		IMessenger Messenger { get; }

		ObservableCollection<InflectWordExerciseTypeViewModel> ExerciseTypes { get; }

		InflectWordExerciseTypeViewModel SelectedExerciseType { get; set; }

		string BaseForm { get; set; }

		string Description { get; set; }

		ObservableCollection<IEditInflectWordFormViewModel> WordFormViewModels { get; }

		IAsyncRelayCommand SaveChangesCommand { get; }

		ICommand ClearChangesCommand { get; }

		ICommand GoToStartPageCommand { get; }

		Task Load(Language studiedLanguage, CancellationToken cancellationToken);
	}
}
