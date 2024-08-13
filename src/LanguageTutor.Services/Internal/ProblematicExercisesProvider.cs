using System;
using System.Collections.Generic;
using System.Linq;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;

namespace LanguageTutor.Services.Internal
{
	internal class ProblematicExercisesProvider : IProblematicExercisesProvider
	{
		private class ExerciseResultTypeComparer : IComparer<ExerciseResultType>
		{
			public int Compare(ExerciseResultType x, ExerciseResultType y)
			{
				return GetResultTypeValue(x).CompareTo(GetResultTypeValue(y));
			}

			private static int GetResultTypeValue(ExerciseResultType exerciseResultType)
			{
				return exerciseResultType switch
				{
					ExerciseResultType.Successful => 0,
					ExerciseResultType.Failed => -1,
					ExerciseResultType.Skipped => -2,
					_ => throw new NotSupportedException($"Exercise result type is not supported: {exerciseResultType}"),
				};
			}
		}

		private class ProblematicExerciseComparer : IComparer<BasicExercise>
		{
			private readonly ExerciseResultTypeComparer exerciseResultTypeComparer = new();

			public int Compare(BasicExercise x, BasicExercise y)
			{
				foreach (var (first, second) in x.SortedResults.Zip(y.SortedResults))
				{
					var exerciseResultTypeCmp = exerciseResultTypeComparer.Compare(first.ResultType, second.ResultType);
					if (exerciseResultTypeCmp != 0)
					{
						return exerciseResultTypeCmp;
					}
				}

				var failedCount1 = x.SortedResults.Count(c => c.IsFailed);
				var failedCount2 = y.SortedResults.Count(c => c.IsFailed);
				var failedCountCmp = failedCount1.CompareTo(failedCount2);
				if (failedCountCmp != 0)
				{
					return -failedCountCmp;
				}

				var successfulCount1 = x.SortedResults.Count(c => c.IsSuccessful);
				var successfulCount2 = y.SortedResults.Count(c => c.IsSuccessful);

				return successfulCount1.CompareTo(successfulCount2);
			}
		}

		public IReadOnlyCollection<BasicExercise> GetProblematicExercises(IEnumerable<BasicExercise> exercises)
		{
			// We consider exercise as problematic, if 3 or more of the last 5 results were failed.
			return exercises
				.Select(x => x.WithLimitedResults(5))
				.Where(x => x.SortedResults.Count(y => y.IsFailed) >= 3)
				.Order(new ProblematicExerciseComparer())
				.ToList();
		}
	}
}
