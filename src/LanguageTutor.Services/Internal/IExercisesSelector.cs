using System;
using System.Collections.Generic;
using LanguageTutor.Models.Exercises;

namespace LanguageTutor.Services.Internal
{
	internal interface IExercisesSelector
	{
		IReadOnlyCollection<BasicExercise> SelectExercisesToPerform(DateOnly date, IEnumerable<BasicExercise> exercises, int dailyLimit);
	}
}
