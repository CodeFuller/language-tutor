using System;
using System.Collections.Generic;
using System.Linq;
using LanguageTutor.Models.Exercises;
using LanguageTutor.Models.Extensions;

namespace LanguageTutor.Services.Internal
{
	internal class NextExerciseDateProvider : INextExerciseDateProvider
	{
		private static readonly List<int> ExerciseIntervals = new()
		{
			// If the last (^1) exercise was failed, we add 1 day to the last exercise date.
			+1,

			// If ^1 exercise was successful, but ^2 exercise was failed, we add 2 days to the last exercise date.
			+2,

			// ...
			+3,
			+7,
			+14,
			+21,
			+30,
			+90,
			+180,

			// The last interval is added for exercises with all successful results.
			+365,
		};

		public DateOnly GetNextExerciseDate(BasicExercise exercise)
		{
			var exerciseResults = exercise.SortedResults;

			if (!exerciseResults.Any())
			{
				// If exercise was not yet performed, this is the highest priority for exercising.
				return DateOnly.MinValue;
			}

			var lastExerciseDate = exerciseResults[0].DateTime.ToDateOnly();

			for (var i = 0; i <= Math.Max(exerciseResults.Count, ExerciseIntervals.Count); ++i)
			{
				// If all results up to last interval are successful, we add the last interval.
				if (i >= ExerciseIntervals.Count)
				{
					return lastExerciseDate.AddDays(ExerciseIntervals[^1]);
				}

				// If all results are successful, however they are not enough - we add interval for first missing result.
				if (i >= exerciseResults.Count)
				{
					return lastExerciseDate.AddDays(ExerciseIntervals[i]);
				}

				// If some result in the past is failed, we add interval for latest failed result.
				if (exerciseResults[i].IsFailed)
				{
					return lastExerciseDate.AddDays(ExerciseIntervals[i]);
				}
			}

			// We should not get here, because we return via condition (i >= ExerciseIntervals.Count) or (i >= exerciseResults.Count).
			throw new InvalidOperationException("Unexpected loop finish");
		}
	}
}
