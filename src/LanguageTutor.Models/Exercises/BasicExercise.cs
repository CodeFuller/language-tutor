using System;
using System.Collections.Generic;
using System.Linq;
using LanguageTutor.Models.Extensions;

namespace LanguageTutor.Models.Exercises
{
	public abstract class BasicExercise
	{
		public abstract DateTimeOffset CreationTimestamp { get; }

		protected abstract IEnumerable<BasicExerciseResult> Results { get; }

		// The results order is from the latest (most significant) to the earliest (less significant).
		public IReadOnlyList<BasicExerciseResult> SortedResults => Results.OrderByDescending(x => x.DateTime).ToList();

		public BasicExercise WithLimitedResults(int maxResultsCount)
		{
			return WithLimitedResults(DateOnly.MaxValue, maxResultsCount);
		}

		public BasicExercise WithLimitedResults(DateOnly latestDate)
		{
			return WithLimitedResults(latestDate, Int32.MaxValue);
		}

		public abstract BasicExercise WithLimitedResults(DateOnly latestDate, int maxResultsCount);

		protected static IEnumerable<TExerciseResult> GetLimitedResults<TExerciseResult>(IEnumerable<TExerciseResult> results, DateOnly latestDate, int maxResultsCount)
			where TExerciseResult : BasicExerciseResult
		{
			return results
				.Where(x => x.DateTime.ToDateOnly() <= latestDate)
				.OrderByDescending(x => x.DateTime)
				.Take(maxResultsCount)
				.ToList();
		}

		public abstract void Accept(IExerciseVisitor visitor);
	}
}
