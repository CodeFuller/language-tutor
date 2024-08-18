using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;

namespace LanguageTutor.Services.Interfaces
{
	public interface IExerciseResultService
	{
		Task AddTranslateTextExerciseResult(User user, TranslateTextExercise exercise, TranslateTextExerciseResult exerciseResult, CancellationToken cancellationToken);

		Task AddInflectWordExerciseResult(User user, InflectWordExercise exercise, InflectWordExerciseResult exerciseResult, CancellationToken cancellationToken);
	}
}
