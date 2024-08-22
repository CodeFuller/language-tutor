using System.Collections.ObjectModel;
using LanguageTutor.Models.Exercises;
using LanguageTutor.ViewModels.Interfaces;

namespace LanguageTutor.ViewModels.Exercises
{
	public interface IInflectWordExerciseViewModel : IExerciseViewModel
	{
		InflectWordExercise Exercise { get; }

		string Description { get; }

		ObservableCollection<IInflectWordFormViewModel> WordFormViewModels { get; }
	}
}
