using System.Collections.Generic;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;
using LanguageTutor.Services.Data;

namespace LanguageTutor.Services.Internal
{
	internal interface IExerciseFactory
	{
		IEnumerable<TranslateTextExercise> CreateTranslateTextExercises(IReadOnlyCollection<TranslateTextExerciseData> exercisesData);

		IEnumerable<InflectWordExercise> CreateInflectWordExercises(ItemId studiedLanguageId, IReadOnlyCollection<InflectWordExerciseData> exercisesData);
	}
}
