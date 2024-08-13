using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageTutor.Models;
using LanguageTutor.ViewModels.Interfaces;
using LanguageTutor.Views.DesignInstances.Exercises;

namespace LanguageTutor.Views.DesignInstances
{
	internal sealed class PerformExercisesDesignData : IPerformExercisesViewModel
	{
		public int NumberOfExercisesToPerform => 100;

		public int NumberOfPerformedExercises => 25;

		public string ProgressInfo => "25 / 100";

		private readonly TranslateTextExerciseDesignData currentExerciseViewModel = new();

		public IExerciseViewModel CurrentExerciseViewModel => currentExerciseViewModel;

		public bool ExerciseWasChecked => true;

		public bool CanSwitchToNextExercise => true;

		public ICommand CheckExerciseCommand => null;

		public ICommand SwitchToNextExerciseCommand => null;

		public ICommand FinishExercisesCommand => null;

		public Task Load(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
