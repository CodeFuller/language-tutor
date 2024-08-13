using System.Windows.Input;
using LanguageTutor.ViewModels.Interfaces;

namespace LanguageTutor.Views.DesignInstances
{
	internal sealed class ApplicationDesignData : IApplicationViewModel
	{
		public IStartPageViewModel StartPageViewModel { get; } = new StartPageDesignData();

		public IPerformExercisesViewModel PerformExercisesViewModel { get; } = new PerformExercisesDesignData();

		public IExerciseResultsViewModel ExerciseResultsViewModel { get; } = new ExerciseResultsDesignData();

		public IEditDictionaryViewModel EditDictionaryViewModel { get; } = new EditDictionaryDesignData();

		public IPageViewModel CurrentPage => StartPageViewModel;

		public ICommand LoadCommand => null;
	}
}
