using System.Collections.Generic;
using LanguageTutor.Models.Exercises;

namespace LanguageTutor.Services.Internal
{
	internal interface IProblematicExercisesProvider
	{
		IReadOnlyCollection<BasicExercise> GetProblematicExercises(IEnumerable<BasicExercise> exercises);
	}
}
