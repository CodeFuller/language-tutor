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

			var check1 = new CheckResult { DateTime = new DateTime(2023, 07, 08) };
			var check2 = new CheckResult { DateTime = new DateTime(2023, 07, 10) };
			var check3 = new CheckResult { DateTime = new DateTime(2023, 07, 09) };

			var target = new StudiedText();

			// Act

			target.AddCheckResult(check1);
			target.AddCheckResult(check2);
			target.AddCheckResult(check3);

			// Assert

			var expectedCheckResults = new[]
			{
				check2,
				check3,
				check1,
			};

			target.CheckResults.Should().BeEquivalentTo(expectedCheckResults, x => x.WithStrictOrdering());
		}
	}
}
