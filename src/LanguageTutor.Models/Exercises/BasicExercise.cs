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

		protected abstract BasicExercise WithLimitedResults(IEnumerable<BasicExerciseResult> limitedResults);

		public BasicExercise WithLimitedResults(int maxResultsCount)
		{
			return WithLimitedResults(DateOnly.MaxValue, maxResultsCount);
		}

		public BasicExercise WithLimitedResults(DateOnly latestDate)
		{
			return WithLimitedResults(latestDate, Int32.MaxValue);
		}

		public BasicExercise WithLimitedResults(DateOnly latestDate, int maxResultsCount)
		{
			var limitedResults = SortedResults
				.Where(x => x.DateTime.ToDateOnly() <= latestDate)
				.Take(maxResultsCount)
				.ToList();

			if (limitedResults.Count == SortedResults.Count)
			{
				return this;
			}

			return WithLimitedResults(limitedResults);
		}

		public abstract void Accept(IExerciseVisitor visitor);
	}
}
