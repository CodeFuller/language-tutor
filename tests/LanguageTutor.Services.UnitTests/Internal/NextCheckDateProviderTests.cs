using System;
using FluentAssertions;
using LanguageTutor.Models;
using LanguageTutor.Services.Internal;
using LanguageTutor.Services.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace LanguageTutor.Services.UnitTests.Internal
{
	[TestClass]
	public class NextCheckDateProviderTests
	{
		[TestMethod]
		public void GetNextCheckDate_ForTextWithoutChecks_ReturnsMinDate()
		{
			// Arrange

			var checkResults = Array.Empty<CheckResult>();

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextCheckDateProvider>();

			// Act

			var result = target.GetNextCheckDate(checkResults.ToStudiedText());

			// Assert

			result.Should().Be(DateOnly.MinValue);
		}

		[TestMethod]
		public void GetNextCheckDate_ForTextWithLastFailedCheck_ReturnsCorrectDate()
		{
			// Arrange

			var checkResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 04).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 05).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 06).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 07).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 08).ToFailedCheckResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextCheckDateProvider>();

			// Act

			var result = target.GetNextCheckDate(checkResults.ToStudiedText());

			// Assert

			result.Should().Be(new DateOnly(2024, 04, 09));
		}

		[TestMethod]
		public void GetNextCheckDate_ForTextWithOneSuccessfulCheckOnly_ReturnsCorrectDate()
		{
			// Arrange

			var checkResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulCheckResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextCheckDateProvider>();

			// Act

			var result = target.GetNextCheckDate(checkResults.ToStudiedText());

			// Assert

			result.Should().Be(new DateOnly(2024, 04, 05));
		}

		[TestMethod]
		public void GetNextCheckDate_ForTextWithOneSuccessfulCheckAfterFailedCheck_ReturnsCorrectDate()
		{
			// Arrange

			var checkResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 04).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 05).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 06).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 07).ToFailedCheckResult(),
				new DateTime(2024, 04, 08).ToSuccessfulCheckResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextCheckDateProvider>();

			// Act

			var result = target.GetNextCheckDate(checkResults.ToStudiedText());

			// Assert

			result.Should().Be(new DateOnly(2024, 04, 10));
		}

		[TestMethod]
		public void GetNextCheckDate_ForTextWithTwoSuccessfulChecksOnly_ReturnsCorrectDate()
		{
			// Arrange

			var checkResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 04).ToSuccessfulCheckResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextCheckDateProvider>();

			// Act

			var result = target.GetNextCheckDate(checkResults.ToStudiedText());

			// Assert

			result.Should().Be(new DateOnly(2024, 04, 07));
		}

		[TestMethod]
		public void GetNextCheckDate_ForTextWithTwoSuccessfulChecksAfterFailedCheck_ReturnsCorrectDate()
		{
			// Arrange

			var checkResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 04).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 05).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 06).ToFailedCheckResult(),
				new DateTime(2024, 04, 07).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 08).ToSuccessfulCheckResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextCheckDateProvider>();

			// Act

			var result = target.GetNextCheckDate(checkResults.ToStudiedText());

			// Assert

			result.Should().Be(new DateOnly(2024, 04, 11));
		}

		[TestMethod]
		public void GetNextCheckDate_ForTextWithThreeSuccessfulChecksOnly_ReturnsCorrectDate()
		{
			// Arrange

			var checkResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 04).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 05).ToSuccessfulCheckResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextCheckDateProvider>();

			// Act

			var result = target.GetNextCheckDate(checkResults.ToStudiedText());

			// Assert

			result.Should().Be(new DateOnly(2024, 04, 12));
		}

		[TestMethod]
		public void GetNextCheckDate_ForTextWithThreeSuccessfulChecksAfterFailedCheck_ReturnsCorrectDate()
		{
			// Arrange

			var checkResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 04).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 05).ToFailedCheckResult(),
				new DateTime(2024, 04, 06).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 07).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 08).ToSuccessfulCheckResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextCheckDateProvider>();

			// Act

			var result = target.GetNextCheckDate(checkResults.ToStudiedText());

			// Assert

			result.Should().Be(new DateOnly(2024, 04, 15));
		}

		[TestMethod]
		public void GetNextCheckDate_ForTextWithFourSuccessfulChecksOnly_ReturnsCorrectDate()
		{
			// Arrange

			var checkResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 04).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 05).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 06).ToSuccessfulCheckResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextCheckDateProvider>();

			// Act

			var result = target.GetNextCheckDate(checkResults.ToStudiedText());

			// Assert

			result.Should().Be(new DateOnly(2024, 04, 20));
		}

		[TestMethod]
		public void GetNextCheckDate_ForTextWithFourSuccessfulChecksAfterFailedCheck_ReturnsCorrectDate()
		{
			// Arrange

			var checkResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 04).ToFailedCheckResult(),
				new DateTime(2024, 04, 05).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 06).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 07).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 08).ToSuccessfulCheckResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextCheckDateProvider>();

			// Act

			var result = target.GetNextCheckDate(checkResults.ToStudiedText());

			// Assert

			result.Should().Be(new DateOnly(2024, 04, 22));
		}

		[TestMethod]
		public void GetNextCheckDate_ForTextWithFiveSuccessfulChecksOnly_ReturnsCorrectDate()
		{
			// Arrange

			var checkResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 04).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 05).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 06).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 07).ToSuccessfulCheckResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextCheckDateProvider>();

			// Act

			var result = target.GetNextCheckDate(checkResults.ToStudiedText());

			// Assert

			result.Should().Be(new DateOnly(2024, 04, 28));
		}

		[TestMethod]
		public void GetNextCheckDate_ForTextWithFiveSuccessfulChecksAfterFailedCheck_ReturnsCorrectDate()
		{
			// Arrange

			var checkResults = new[]
			{
				new DateTime(2024, 04, 03).ToFailedCheckResult(),
				new DateTime(2024, 04, 04).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 05).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 06).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 07).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 08).ToSuccessfulCheckResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextCheckDateProvider>();

			// Act

			var result = target.GetNextCheckDate(checkResults.ToStudiedText());

			// Assert

			result.Should().Be(new DateOnly(2024, 04, 29));
		}

		[TestMethod]
		public void GetNextCheckDate_ForTextWithSixSuccessfulChecksOnly_ReturnsCorrectDate()
		{
			// Arrange

			var checkResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 04).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 05).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 06).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 07).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 08).ToSuccessfulCheckResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextCheckDateProvider>();

			// Act

			var result = target.GetNextCheckDate(checkResults.ToStudiedText());

			// Assert

			result.Should().Be(new DateOnly(2024, 05, 08));
		}

		[TestMethod]
		public void GetNextCheckDate_ForTextWithSixSuccessfulChecksAfterFailedCheck_ReturnsCorrectDate()
		{
			// Arrange

			var checkResults = new[]
			{
				new DateTime(2024, 04, 03).ToFailedCheckResult(),
				new DateTime(2024, 04, 04).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 05).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 06).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 07).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 08).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 09).ToSuccessfulCheckResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextCheckDateProvider>();

			// Act

			var result = target.GetNextCheckDate(checkResults.ToStudiedText());

			// Assert

			result.Should().Be(new DateOnly(2024, 05, 09));
		}

		[TestMethod]
		public void GetNextCheckDate_ForTextWithSevenSuccessfulChecksOnly_ReturnsCorrectDate()
		{
			// Arrange

			var checkResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 04).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 05).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 06).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 07).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 08).ToSuccessfulCheckResult(),
				new DateTime(2024, 04, 09).ToSuccessfulCheckResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextCheckDateProvider>();

			// Act

			var result = target.GetNextCheckDate(checkResults.ToStudiedText());

			// Assert

			result.Should().Be(new DateOnly(2024, 05, 09));
		}
	}
}
