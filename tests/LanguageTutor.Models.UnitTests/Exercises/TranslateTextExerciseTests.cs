using System;
using FluentAssertions;
using LanguageTutor.Models.Exercises;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LanguageTutor.Models.UnitTests.Exercises
{
	[TestClass]
	public class TranslateTextExerciseTests
	{
		[TestMethod]
		public void Check_ForCorrectlyTypedText_AddsSuccessfulResultType()
		{
			// Arrange

			var target = new TranslateTextExercise([])
			{
				TextInStudiedLanguage = new LanguageText
				{
					Text = "something",
				},
			};

			// Act

			var result = target.Check("something", new DateTimeOffset(2024, 08, 13, 17, 05, 48, TimeSpan.Zero));

			// Assert

			var expectedResult = new TranslateTextExerciseResult
			{
				ResultType = ExerciseResultType.Successful,
				DateTime = new DateTimeOffset(2024, 08, 13, 17, 05, 48, TimeSpan.Zero),
			};

			result.Should().BeEquivalentTo(expectedResult);
			target.SortedResults.Should().BeEquivalentTo(new[] { expectedResult });
		}

		[TestMethod]
		public void Check_ForMisspelledText_AddsFailedResultType()
		{
			// Arrange

			var target = new TranslateTextExercise([])
			{
				TextInStudiedLanguage = new LanguageText
				{
					Text = "something",
				},
			};

			// Act

			var result = target.Check("another thing", new DateTimeOffset(2024, 08, 13, 17, 05, 48, TimeSpan.Zero));

			// Assert

			var expectedResult = new TranslateTextExerciseResult
			{
				ResultType = ExerciseResultType.Failed,
				DateTime = new DateTimeOffset(2024, 08, 13, 17, 05, 48, TimeSpan.Zero),
				TypedText = "another thing",
			};

			result.Should().BeEquivalentTo(expectedResult);
			target.SortedResults.Should().BeEquivalentTo(new[] { expectedResult });
		}

		[TestMethod]
		public void Check_IfTypedTextDiffersInCase_AddsFailedResultType()
		{
			// Arrange

			var target = new TranslateTextExercise([])
			{
				TextInStudiedLanguage = new LanguageText
				{
					Text = "Something",
				},
			};

			// Act

			var result = target.Check("something", new DateTimeOffset(2024, 08, 13, 17, 05, 48, TimeSpan.Zero));

			// Assert

			var expectedResult = new TranslateTextExerciseResult
			{
				ResultType = ExerciseResultType.Failed,
				DateTime = new DateTimeOffset(2024, 08, 13, 17, 05, 48, TimeSpan.Zero),
				TypedText = "something",
			};

			result.Should().BeEquivalentTo(expectedResult);
			target.SortedResults.Should().BeEquivalentTo(new[] { expectedResult });
		}

		[TestMethod]
		public void Check_IfTypedTextDiffersInDiacritics_AddsFailedResultType()
		{
			// Arrange

			var target = new TranslateTextExercise([])
			{
				TextInStudiedLanguage = new LanguageText
				{
					Text = "góra",
				},
			};

			// Act

			var result = target.Check("gora", new DateTimeOffset(2024, 08, 13, 17, 05, 48, TimeSpan.Zero));

			// Assert

			var expectedResult = new TranslateTextExerciseResult
			{
				ResultType = ExerciseResultType.Failed,
				DateTime = new DateTimeOffset(2024, 08, 13, 17, 05, 48, TimeSpan.Zero),
				TypedText = "gora",
			};

			result.Should().BeEquivalentTo(expectedResult);
			target.SortedResults.Should().BeEquivalentTo(new[] { expectedResult });
		}

		[TestMethod]
		public void Check_ForSkippedText_AddsSkippedResultType()
		{
			// Arrange

			var target = new TranslateTextExercise([])
			{
				TextInStudiedLanguage = new LanguageText
				{
					Text = "something",
				},
			};

			// Act

			var result = target.Check(String.Empty, new DateTimeOffset(2024, 08, 13, 17, 05, 48, TimeSpan.Zero));

			// Assert

			var expectedResult = new TranslateTextExerciseResult
			{
				ResultType = ExerciseResultType.Skipped,
				DateTime = new DateTimeOffset(2024, 08, 13, 17, 05, 48, TimeSpan.Zero),
				TypedText = null,
			};

			result.Should().BeEquivalentTo(expectedResult);
			target.SortedResults.Should().BeEquivalentTo(new[] { expectedResult });
		}
	}
}