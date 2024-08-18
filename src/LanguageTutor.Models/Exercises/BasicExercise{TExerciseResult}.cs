using System;
using System.Collections.Generic;
using System.Linq;

namespace LanguageTutor.Models.Exercises
{
	public abstract class BasicExercise<TExerciseResult> : BasicExercise
		where TExerciseResult : BasicExerciseResult
	{
		private readonly List<TExerciseResult> results;

		protected override IEnumerable<BasicExerciseResult> Results => results;

		protected BasicExercise(IEnumerable<TExerciseResult> results)
		{
			this.results = results?.ToList() ?? throw new ArgumentNullException(nameof(results));
		}

		protected void AddResult(TExerciseResult result)
		{
			results.Add(result);
		}

		protected abstract BasicExercise<TExerciseResult> WithLimitedResults(IEnumerable<TExerciseResult> limitedResults);

		public override BasicExercise WithLimitedResults(DateOnly latestDate, int maxResultsCount)
		{
			return WithLimitedResults(GetLimitedResults(results, latestDate, maxResultsCount));
		}
	}
}
