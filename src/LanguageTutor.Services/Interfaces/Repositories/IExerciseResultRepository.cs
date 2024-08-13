using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;

namespace LanguageTutor.Services.Interfaces.Repositories
{
	public interface IExerciseResultRepository
	{
		Task AddTranslateTextExerciseResult(ItemId userId, TranslateTextExercise exercise, TranslateTextExerciseResult exerciseResult, CancellationToken cancellationToken);
	}
}
