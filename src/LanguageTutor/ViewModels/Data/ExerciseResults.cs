using System;
using LanguageTutor.Models;

namespace LanguageTutor.ViewModels.Data
{
	public class ExerciseResults
	{
		public int NumberOfPerformedExercises { get; private set; }

		public int NumberOfSuccessfulExercises { get; private set; }

		public int NumberOfFailedExercises { get; private set; }

		public int NumberOfSkippedExercises { get; private set; }

		public void AddResult(ExerciseResultType exerciseResultType)
		{
			++NumberOfPerformedExercises;

			switch (exerciseResultType)
			{
				case ExerciseResultType.Successful:
					++NumberOfSuccessfulExercises;
					break;

				case ExerciseResultType.Failed:
					++NumberOfFailedExercises;
					break;

				case ExerciseResultType.Skipped:
					++NumberOfSkippedExercises;
					break;

				default:
					throw new NotSupportedException($"Exercise result type is not supported: {exerciseResultType}");
			}
		}
	}
}
