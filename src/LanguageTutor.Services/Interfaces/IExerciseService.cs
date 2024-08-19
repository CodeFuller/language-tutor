using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;
using LanguageTutor.Services.Data;

namespace LanguageTutor.Services.Interfaces
{
	public interface IExerciseService
	{
		bool InflectWordExercisesAreSupported(Language studiedLanguage);

		Task<IReadOnlyCollection<InflectWordExerciseTypeDescriptor>> GetInflectWordExerciseTypes(Language studiedLanguage, CancellationToken cancellationToken);

		Task AddInflectWordExercise(CreateInflectWordExerciseData createExerciseData, CancellationToken cancellationToken);
	}
}
