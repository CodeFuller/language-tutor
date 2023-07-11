using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VocabularyCoach.Models.UnitTests
{
	[TestClass]
	public class StudiedTextTests
	{
		[TestMethod]
		public void CheckResults_ReturnsChecksFromLatestToEarliest()
		{
			// Arrange

			var checkResults = new[]
			{
				new CheckResult { DateTime = new DateTime(2023, 07, 08) },
				new CheckResult { DateTime = new DateTime(2023, 07, 10) },
				new CheckResult { DateTime = new DateTime(2023, 07, 09) },
			};

			var target = new StudiedText(checkResults);

			// Act

			var result = target.CheckResults;

			// Assert

			var expectedCheckResults = new[]
			{
				checkResults[1],
				checkResults[2],
				checkResults[0],
			};

			result.Should().BeEquivalentTo(expectedCheckResults, x => x.WithStrictOrdering());
		}
	}
}
