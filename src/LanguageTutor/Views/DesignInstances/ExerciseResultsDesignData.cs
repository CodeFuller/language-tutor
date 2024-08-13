using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageTutor.Models;
using LanguageTutor.ViewModels.Data;
using LanguageTutor.ViewModels.Interfaces;

namespace LanguageTutor.Views.DesignInstances
{
	internal sealed class ExerciseResultsDesignData : IExerciseResultsViewModel
	{
		public string TotalExercisesStatistics => "100";

		public string SuccessfulExercisesStatistics => "95 (95.0%)";

		public string FailedExercisesStatistics => "3 (3.0%)";

		public string SkippedExercisesStatistics => "2 (2.0%)";

		public ICommand GoToStartPageCommand => null;

		public Task Load(User user, Language studiedLanguage, Language knownLanguage, ExerciseResults results, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
