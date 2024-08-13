using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using LanguageTutor.Models.Exercises;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LanguageTutor.Models.UnitTests.Exercises
{
	[TestClass]
	public class BasicExerciseTests
	{
		private sealed class TestExerciseResult : BasicExerciseResult
		{
		}

		private sealed class TestExercise : BasicExercise
		{
			private readonly List<TestExerciseResult> results;

			public override DateTimeOffset CreationTimestamp => new(2024, 08, 13, 07, 38, 15, TimeSpan.FromHours(2));

			protected override IEnumerable<BasicExerciseResult> Results => results;

			public TestExercise(IEnumerable<TestExerciseResult> results)
			{
				this.results = results?.ToList() ?? throw new ArgumentNullException(nameof(results));
			}

			protected override BasicExercise WithLimitedResults(IEnumerable<BasicExerciseResult> limitedResults)
			{
				return new TestExercise(limitedResults.Cast<TestExerciseResult>());
			}

			public override void Accept(IExerciseVisitor visitor)
			{
			}
		}

		[TestMethod]
		public void SortedResults_ReturnsResultsFromLatestToEarliest()
		{
			// Arrange

			var results = new[]
			{
				new TestExerciseResult { DateTime = new DateTime(2023, 07, 08) },
				new TestExerciseResult { DateTime = new DateTime(2023, 07, 10) },
				new TestExerciseResult { DateTime = new DateTime(2023, 07, 09) },
			};

			var target = new TestExercise(results);

			// Act

			var result = target.SortedResults;

			// Assert

			var expectedResults = new[]
			{
				results[1],
				results[2],
				results[0],
			};

			result.Should().BeEquivalentTo(expectedResults, x => x.WithStrictOrdering());
		}
	}
}
