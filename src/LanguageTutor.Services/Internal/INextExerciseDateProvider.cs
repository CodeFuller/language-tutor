using System;
using LanguageTutor.Models.Exercises;

namespace LanguageTutor.Services.Internal
{
	internal interface INextExerciseDateProvider
	{
		DateOnly GetNextExerciseDate(BasicExercise exercise);
	}
}
