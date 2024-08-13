using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models.Exercises;

namespace LanguageTutor.ViewModels.Interfaces
{
	public interface IExerciseViewModel
	{
		bool ExerciseWasChecked { get; }

		Task<BasicExerciseResult> CheckExercise(CancellationToken cancellationToken);
	}
}
