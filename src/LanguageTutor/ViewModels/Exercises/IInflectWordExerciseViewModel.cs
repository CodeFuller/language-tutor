using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using LanguageTutor.Models.Exercises;
using LanguageTutor.ViewModels.Interfaces;

namespace LanguageTutor.ViewModels.Exercises
{
	public interface IInflectWordExerciseViewModel : IExerciseViewModel
	{
		IMessenger Messenger { get; }

		InflectWordExercise Exercise { get; }

		string Description { get; }

		ObservableCollection<IInflectWordFormViewModel> WordFormViewModels { get; }
	}
}
