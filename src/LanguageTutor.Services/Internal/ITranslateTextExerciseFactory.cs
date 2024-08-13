using System.Collections.Generic;
using LanguageTutor.Models.Exercises;
using LanguageTutor.Services.Data;

namespace LanguageTutor.Services.Internal
{
	internal interface ITranslateTextExerciseFactory
	{
		IEnumerable<TranslateTextExercise> CreateTranslateTextExercises(IReadOnlyCollection<TranslateTextExerciseData> exercisesData);
	}
}
