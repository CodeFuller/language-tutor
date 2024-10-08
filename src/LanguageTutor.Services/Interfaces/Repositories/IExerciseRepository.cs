using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;
using LanguageTutor.Services.Data;

namespace LanguageTutor.Services.Interfaces.Repositories
{
	public interface IExerciseRepository
	{
		Task<IReadOnlyCollection<TranslateTextExerciseData>> GetTranslateTextExercises(ItemId userId, ItemId studiedLanguageId, ItemId knownLanguageId, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<InflectWordExerciseData>> GetInflectWordExercises(ItemId userId, ItemId studiedLanguageId, CancellationToken cancellationToken);

		Task AddInflectWordExercise(SaveInflectWordExerciseData createExerciseData, CancellationToken cancellationToken);
	}
}
