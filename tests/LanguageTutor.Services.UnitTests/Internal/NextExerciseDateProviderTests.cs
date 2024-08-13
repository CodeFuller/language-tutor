using System;
using FluentAssertions;
using LanguageTutor.Models.Exercises;
using LanguageTutor.Services.Internal;
using LanguageTutor.Services.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace LanguageTutor.Services.UnitTests.Internal
{
	[TestClass]
	public class NextExerciseDateProviderTests
	{
		[TestMethod]
		public void GetNextExerciseDate_ForExerciseWithoutResults_ReturnsMinDate()
		{
			// Arrange

			var exerciseResults = Array.Empty<TranslateTextExerciseResult>();

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextExerciseDateProvider>();

			// Act

			var result = target.GetNextExerciseDate(exerciseResults.ToExercise());

			// Assert

			result.Should().Be(DateOnly.MinValue);
		}

		[TestMethod]
		public void GetNextExerciseDate_ForExerciseWithLastResultFailed_ReturnsCorrectDate()
		{
			// Arrange

			var exerciseResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 04).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 05).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 06).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 07).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 08).ToFailedExerciseResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextExerciseDateProvider>();

			// Act

			var result = target.GetNextExerciseDate(exerciseResults.ToExercise());

			// Assert

			result.Should().Be(new DateOnly(2024, 04, 09));
		}

		[TestMethod]
		public void GetNextExerciseDate_ForExerciseWithOneSuccessfulResultOnly_ReturnsCorrectDate()
		{
			// Arrange

			var exerciseResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulExerciseResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextExerciseDateProvider>();

			// Act

			var result = target.GetNextExerciseDate(exerciseResults.ToExercise());

			// Assert

			result.Should().Be(new DateOnly(2024, 04, 05));
		}

		[TestMethod]
		public void GetNextExerciseDate_ForExerciseWithOneSuccessfulResultAfterFailedResult_ReturnsCorrectDate()
		{
			// Arrange

			var exerciseResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 04).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 05).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 06).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 07).ToFailedExerciseResult(),
				new DateTime(2024, 04, 08).ToSuccessfulExerciseResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextExerciseDateProvider>();

			// Act

			var result = target.GetNextExerciseDate(exerciseResults.ToExercise());

			// Assert

			result.Should().Be(new DateOnly(2024, 04, 10));
		}

		[TestMethod]
		public void GetNextExerciseDate_ForExerciseWithTwoSuccessfulResultsOnly_ReturnsCorrectDate()
		{
			// Arrange

			var exerciseResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 04).ToSuccessfulExerciseResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextExerciseDateProvider>();

			// Act

			var result = target.GetNextExerciseDate(exerciseResults.ToExercise());

			// Assert

			result.Should().Be(new DateOnly(2024, 04, 07));
		}

		[TestMethod]
		public void GetNextExerciseDate_ForExerciseWithTwoSuccessfulResultsAfterFailedResult_ReturnsCorrectDate()
		{
			// Arrange

			var exerciseResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 04).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 05).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 06).ToFailedExerciseResult(),
				new DateTime(2024, 04, 07).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 08).ToSuccessfulExerciseResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextExerciseDateProvider>();

			// Act

			var result = target.GetNextExerciseDate(exerciseResults.ToExercise());

			// Assert

			result.Should().Be(new DateOnly(2024, 04, 11));
		}

		[TestMethod]
		public void GetNextExerciseDate_ForExerciseWithThreeSuccessfulResultsOnly_ReturnsCorrectDate()
		{
			// Arrange

			var exerciseResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 04).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 05).ToSuccessfulExerciseResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextExerciseDateProvider>();

			// Act

			var result = target.GetNextExerciseDate(exerciseResults.ToExercise());

			// Assert

			result.Should().Be(new DateOnly(2024, 04, 12));
		}

		[TestMethod]
		public void GetNextExerciseDate_ForExerciseWithThreeSuccessfulResultsAfterFailedResult_ReturnsCorrectDate()
		{
			// Arrange

			var exerciseResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 04).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 05).ToFailedExerciseResult(),
				new DateTime(2024, 04, 06).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 07).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 08).ToSuccessfulExerciseResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextExerciseDateProvider>();

			// Act

			var result = target.GetNextExerciseDate(exerciseResults.ToExercise());

			// Assert

			result.Should().Be(new DateOnly(2024, 04, 15));
		}

		[TestMethod]
		public void GetNextExerciseDate_ForExerciseWithFourSuccessfulResultsOnly_ReturnsCorrectDate()
		{
			// Arrange

			var exerciseResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 04).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 05).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 06).ToSuccessfulExerciseResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextExerciseDateProvider>();

			// Act

			var result = target.GetNextExerciseDate(exerciseResults.ToExercise());

			// Assert

			result.Should().Be(new DateOnly(2024, 04, 20));
		}

		[TestMethod]
		public void GetNextExerciseDate_ForExerciseWithFourSuccessfulResultsAfterFailedResult_ReturnsCorrectDate()
		{
			// Arrange

			var exerciseResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 04).ToFailedExerciseResult(),
				new DateTime(2024, 04, 05).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 06).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 07).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 08).ToSuccessfulExerciseResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextExerciseDateProvider>();

			// Act

			var result = target.GetNextExerciseDate(exerciseResults.ToExercise());

			// Assert

			result.Should().Be(new DateOnly(2024, 04, 22));
		}

		[TestMethod]
		public void GetNextExerciseDate_ForExerciseWithFiveSuccessfulResultsOnly_ReturnsCorrectDate()
		{
			// Arrange

			var exerciseResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 04).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 05).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 06).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 07).ToSuccessfulExerciseResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextExerciseDateProvider>();

			// Act

			var result = target.GetNextExerciseDate(exerciseResults.ToExercise());

			// Assert

			result.Should().Be(new DateOnly(2024, 04, 28));
		}

		[TestMethod]
		public void GetNextExerciseDate_ForExerciseWithFiveSuccessfulResultsAfterFailedResult_ReturnsCorrectDate()
		{
			// Arrange

			var exerciseResults = new[]
			{
				new DateTime(2024, 04, 03).ToFailedExerciseResult(),
				new DateTime(2024, 04, 04).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 05).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 06).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 07).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 08).ToSuccessfulExerciseResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextExerciseDateProvider>();

			// Act

			var result = target.GetNextExerciseDate(exerciseResults.ToExercise());

			// Assert

			result.Should().Be(new DateOnly(2024, 04, 29));
		}

		[TestMethod]
		public void GetNextExerciseDate_ForExerciseWithSixSuccessfulResultsOnly_ReturnsCorrectDate()
		{
			// Arrange

			var exerciseResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 04).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 05).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 06).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 07).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 08).ToSuccessfulExerciseResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextExerciseDateProvider>();

			// Act

			var result = target.GetNextExerciseDate(exerciseResults.ToExercise());

			// Assert

			result.Should().Be(new DateOnly(2024, 05, 08));
		}

		[TestMethod]
		public void GetNextExerciseDate_ForExerciseWithSixSuccessfulResultsAfterFailedResult_ReturnsCorrectDate()
		{
			// Arrange

			var exerciseResults = new[]
			{
				new DateTime(2024, 04, 03).ToFailedExerciseResult(),
				new DateTime(2024, 04, 04).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 05).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 06).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 07).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 08).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 09).ToSuccessfulExerciseResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextExerciseDateProvider>();

			// Act

			var result = target.GetNextExerciseDate(exerciseResults.ToExercise());

			// Assert

			result.Should().Be(new DateOnly(2024, 05, 09));
		}

		[TestMethod]
		public void GetNextExerciseDate_ForExerciseWithSevenSuccessfulResultsOnly_ReturnsCorrectDate()
		{
			// Arrange

			var exerciseResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 04).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 05).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 06).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 07).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 08).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 09).ToSuccessfulExerciseResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextExerciseDateProvider>();

			// Act

			var result = target.GetNextExerciseDate(exerciseResults.ToExercise());

			// Assert

			result.Should().Be(new DateOnly(2024, 07, 08));
		}

		[TestMethod]
		public void GetNextExerciseDate_ForExerciseWithSevenSuccessfulResultsAfterFailedResult_ReturnsCorrectDate()
		{
			// Arrange

			var exerciseResults = new[]
			{
				new DateTime(2024, 04, 03).ToFailedExerciseResult(),
				new DateTime(2024, 04, 04).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 05).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 06).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 07).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 08).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 09).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 10).ToSuccessfulExerciseResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextExerciseDateProvider>();

			// Act

			var result = target.GetNextExerciseDate(exerciseResults.ToExercise());

			// Assert

			result.Should().Be(new DateOnly(2024, 07, 09));
		}

		[TestMethod]
		public void GetNextExerciseDate_ForExerciseWithEightSuccessfulResultsOnly_ReturnsCorrectDate()
		{
			// Arrange

			var exerciseResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 04).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 05).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 06).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 07).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 08).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 09).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 10).ToSuccessfulExerciseResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextExerciseDateProvider>();

			// Act

			var result = target.GetNextExerciseDate(exerciseResults.ToExercise());

			// Assert

			result.Should().Be(new DateOnly(2024, 10, 07));
		}

		[TestMethod]
		public void GetNextExerciseDate_ForExerciseWithEightSuccessfulResultsAfterFailedResult_ReturnsCorrectDate()
		{
			// Arrange

			var exerciseResults = new[]
			{
				new DateTime(2024, 04, 03).ToFailedExerciseResult(),
				new DateTime(2024, 04, 04).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 05).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 06).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 07).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 08).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 09).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 10).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 11).ToSuccessfulExerciseResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextExerciseDateProvider>();

			// Act

			var result = target.GetNextExerciseDate(exerciseResults.ToExercise());

			// Assert

			result.Should().Be(new DateOnly(2024, 10, 08));
		}

		[TestMethod]
		public void GetNextExerciseDate_ForExerciseWithNineSuccessfulResultsOnly_ReturnsCorrectDate()
		{
			// Arrange

			var exerciseResults = new[]
			{
				new DateTime(2024, 04, 03).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 04).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 05).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 06).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 07).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 08).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 09).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 10).ToSuccessfulExerciseResult(),
				new DateTime(2024, 04, 11).ToSuccessfulExerciseResult(),
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<NextExerciseDateProvider>();

			// Act

			var result = target.GetNextExerciseDate(exerciseResults.ToExercise());

			// Assert

			result.Should().Be(new DateOnly(2025, 04, 11));
		}
	}
}
