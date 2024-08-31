using System;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;
using LanguageTutor.Services.Internal;
using LanguageTutor.Services.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace LanguageTutor.Services.UnitTests.Internal
{
	[TestClass]
	public class ExercisesSelectorTests
	{
		[TestMethod]
		public void SelectExercisesToPerform_ForMissingExercises_ReturnsEmptyCollection()
		{
			// Arrange

			var exercises = Array.Empty<TranslateTextExercise>();

			var mocker = new AutoMocker();
			mocker.GetMock<INextExerciseDateProvider>()
				.Setup(x => x.GetNextExerciseDate(It.IsAny<TranslateTextExercise>())).Returns(new DateOnly(2023, 07, 11));

			var target = mocker.CreateInstance<ExercisesSelector>();

			// Act

			var result = target.SelectExercisesToPerform(new DateOnly(2023, 07, 11), exercises, 100);

			// Assert

			result.Should().BeEmpty();
		}

		[TestMethod]
		public void SelectExercisesToPerform_ForExercisesWithNextDateNotYetReached_DoesNotReturnSuchExercises()
		{
			// Arrange

			var mocker = new AutoMocker();

			var exercises = new[]
			{
				CreateTranslateTextExercise("Exercise 1", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
				CreateTranslateTextExercise("Exercise 2", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 12)),
				CreateTranslateTextExercise("Exercise 3", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
				CreateTranslateTextExercise("Exercise 4", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 12)),
			};

			var target = mocker.CreateInstance<ExercisesSelector>();

			// Act

			var result = target.SelectExercisesToPerform(new DateOnly(2023, 07, 11), exercises, 100);

			// Assert

			var expectedResult = new[]
			{
				exercises.Get("Exercise 1"),
				exercises.Get("Exercise 3"),
			};

			result.Should().BeEquivalentTo(expectedResult, x => x.WithoutStrictOrdering());
		}

		[TestMethod]
		public void SelectExercisesToPerform_IfDailyLimitIsReached_ReturnsEmptyCollection()
		{
			// Arrange

			var mocker = new AutoMocker();

			var exercises = new[]
			{
				CreateTranslateTextExercise("Exercise 1", mocker, lastExerciseDate: new DateTime(2023, 07, 11), nextExerciseDate: new DateOnly(2023, 07, 11)),
				CreateTranslateTextExercise("Exercise 2", mocker, lastExerciseDate: new DateTime(2023, 07, 11), nextExerciseDate: new DateOnly(2023, 07, 11)),
				CreateTranslateTextExercise("Exercise 3", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
			};

			var target = mocker.CreateInstance<ExercisesSelector>();

			// Act

			var result = target.SelectExercisesToPerform(new DateOnly(2023, 07, 11), exercises, 2);

			// Assert

			result.Should().BeEmpty();
		}

		[TestMethod]
		public void SelectExercisesToPerform_IfSomeExercisesWerePerformedToday_TakesSuchExercisesIntoAccountForDailyLimit()
		{
			// Arrange

			var mocker = new AutoMocker();

			var exercises = new[]
			{
				CreateTranslateTextExercise("Exercise 1", mocker, lastExerciseDate: new DateTime(2023, 07, 11), nextExerciseDate: new DateOnly(2023, 07, 12)),
				CreateTranslateTextExercise("Exercise 2", mocker, lastExerciseDate: new DateTime(2023, 07, 11), nextExerciseDate: new DateOnly(2023, 07, 12)),
				CreateTranslateTextExercise("Exercise 3", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
				CreateTranslateTextExercise("Exercise 4", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
				CreateTranslateTextExercise("Exercise 5", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
				CreateTranslateTextExercise("Exercise 6", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
				CreateTranslateTextExercise("Exercise 7", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
			};

			var target = mocker.CreateInstance<ExercisesSelector>();

			// Act

			var result = target.SelectExercisesToPerform(new DateOnly(2023, 07, 11), exercises, 5);

			// Assert

			result.Count.Should().Be(3);

			result.Should().NotContain(exercises.Get("Exercise 1"));
			result.Should().NotContain(exercises.Get("Exercise 2"));
		}

		[TestMethod]
		public void SelectExercisesToPerform_IfNumberOfSuitableTouchedExercisesExceedsDailyLimit_ReturnsExerciseWithMostOverdueDate()
		{
			// Arrange

			var mocker = new AutoMocker();

			var exercises = new[]
			{
				CreateTranslateTextExercise("Group 3 - Exercise 1", mocker, lastExerciseDate: new DateTime(2023, 07, 11), nextExerciseDate: new DateOnly(2023, 07, 14)),
				CreateTranslateTextExercise("Group 3 - Exercise 2", mocker, lastExerciseDate: new DateTime(2023, 07, 11), nextExerciseDate: new DateOnly(2023, 07, 14)),
				CreateTranslateTextExercise("Group 3 - Exercise 3", mocker, lastExerciseDate: new DateTime(2023, 07, 11), nextExerciseDate: new DateOnly(2023, 07, 14)),

				CreateTranslateTextExercise("Group 1 - Exercise 1", mocker, lastExerciseDate: new DateTime(2023, 07, 11), nextExerciseDate: new DateOnly(2023, 07, 12)),
				CreateTranslateTextExercise("Group 1 - Exercise 2", mocker, lastExerciseDate: new DateTime(2023, 07, 11), nextExerciseDate: new DateOnly(2023, 07, 12)),
				CreateTranslateTextExercise("Group 1 - Exercise 3", mocker, lastExerciseDate: new DateTime(2023, 07, 11), nextExerciseDate: new DateOnly(2023, 07, 12)),

				CreateTranslateTextExercise("Group 2 - Exercise 1", mocker, lastExerciseDate: new DateTime(2023, 07, 11), nextExerciseDate: new DateOnly(2023, 07, 13)),
				CreateTranslateTextExercise("Group 2 - Exercise 2", mocker, lastExerciseDate: new DateTime(2023, 07, 11), nextExerciseDate: new DateOnly(2023, 07, 13)),
				CreateTranslateTextExercise("Group 2 - Exercise 3", mocker, lastExerciseDate: new DateTime(2023, 07, 11), nextExerciseDate: new DateOnly(2023, 07, 13)),
			};

			var target = mocker.CreateInstance<ExercisesSelector>();

			// Act

			var result1 = target.SelectExercisesToPerform(new DateOnly(2023, 07, 14), exercises, 3);
			var result2 = target.SelectExercisesToPerform(new DateOnly(2023, 07, 14), exercises, 6);

			// Assert

			var expectedResult1 = new[]
			{
				exercises.Get("Group 1 - Exercise 1"),
				exercises.Get("Group 1 - Exercise 2"),
				exercises.Get("Group 1 - Exercise 3"),
			};

			var expectedResult2 = new[]
			{
				exercises.Get("Group 1 - Exercise 1"),
				exercises.Get("Group 1 - Exercise 2"),
				exercises.Get("Group 1 - Exercise 3"),
				exercises.Get("Group 2 - Exercise 1"),
				exercises.Get("Group 2 - Exercise 2"),
				exercises.Get("Group 2 - Exercise 3"),
			};

			result1.Should().BeEquivalentTo(expectedResult1, x => x.WithoutStrictOrdering());
			result2.Should().BeEquivalentTo(expectedResult2, x => x.WithoutStrictOrdering());
		}

		[TestMethod]
		public void SelectExercisesToPerform_IfNumberOfSuitableTouchedExercisesForLatestExerciseDateExceedsDailyLimit_ReturnsRandomSubsetOfSuchExercises()
		{
			// Arrange

			var mocker = new AutoMocker();

			// Since exercises are returned in random order, we take quite a big number of such exercises,
			// so that we can check that items are actually randomized, i.e. exercises are returned from both halves.
			var exercises1 = Enumerable.Range(1, 100)
				.Select(n => CreateTranslateTextExercise($"Touched Exercise - 1st Half - {n}", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)))
				.ToList();

			var exercises2 = Enumerable.Range(1, 100)
				.Select(n => CreateTranslateTextExercise($"Touched Exercise - 2nd Half - {n}", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)))
				.ToList();

			var target = mocker.CreateInstance<ExercisesSelector>();

			// Act

			var result = target.SelectExercisesToPerform(new DateOnly(2023, 07, 11), exercises1.Concat(exercises2), 100);

			// Assert

			result.Should().Contain(x => exercises1.Any(y => ReferenceEquals(x, y)));
			result.Should().Contain(x => exercises2.Any(y => ReferenceEquals(x, y)));
		}

		[TestMethod]
		public void SelectExercisesToPerform_IfNumberOfUntouchedTranslateTextExercisesExceedsDailyLimitAndSuitableTouchedExercisesIsEnough_ReturnsOnlyTouchedExercises()
		{
			// Arrange

			var mocker = new AutoMocker();

			var touchedExercises = new[]
			{
				CreateTranslateTextExercise("Touched Exercise 1", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
				CreateTranslateTextExercise("Touched Exercise 2", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
				CreateTranslateTextExercise("Touched Exercise 3", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
			};

			var untouchedExercises = Enumerable.Range(1, 100)
				.Select(n => CreateTranslateTextExerciseWithNoResults($"Untouched Exercise {n}", mocker, createDate: new DateTime(2023, 07, 11).AddDays(n)))
				.ToList();

			var target = mocker.CreateInstance<ExercisesSelector>();

			// Act

			var result = target.SelectExercisesToPerform(new DateOnly(2024, 07, 11), touchedExercises.Concat(untouchedExercises), 3);

			// Assert

			result.Should().BeEquivalentTo(touchedExercises, x => x.WithoutStrictOrdering());
		}

		[TestMethod]
		public void SelectExercisesToPerform_IfNumberOfUntouchedInflectWordExercisesExceedsDailyLimitAndSuitableTouchedExercisesIsEnough_ReturnsSomeUntouchedInflectWordExercises()
		{
			// Arrange

			var mocker = new AutoMocker();

			var touchedTranslateTextExercises = Enumerable.Range(1, 100)
				.Select(n => CreateTranslateTextExercise($"Touched Translate Text Exercise - {n}", mocker, lastExerciseDate: new DateTime(2024, 08, 30), nextExerciseDate: new DateOnly(2024, 08, 31)))
				.ToList();

			var untouchedTranslateTextExercises = Enumerable.Range(1, 100)
				.Select(n => CreateTranslateTextExerciseWithNoResults($"Untouched Translate Text Exercise - {n}", mocker, createDate: new DateTime(2024, 01, 01).AddDays(n)))
				.ToList();

			var untouchedInflectWordExercises = Enumerable.Range(1, 100)
				.Select(n => CreateInflectWordExerciseWithNoResults($"Untouched Inflect Word Exercise {n}", mocker, createDate: new DateTime(2024, 01, 01).AddDays(n)))
				.ToList();

			var target = mocker.CreateInstance<ExercisesSelector>();

			// Act

			var allExercises = touchedTranslateTextExercises
				.Concat(untouchedTranslateTextExercises)
				.Concat<BasicExercise>(untouchedInflectWordExercises);

			var result = target.SelectExercisesToPerform(new DateOnly(2024, 08, 31), allExercises, 50);

			// Assert

			result.Should().Contain(x => untouchedInflectWordExercises.Any(y => ReferenceEquals(x, y)));
		}

		[TestMethod]
		public void SelectExercisesToPerform_IfNumberOfUntouchedTranslateTextExercisesExceedsDailyLimitAndSuitableTouchedExercisesIsNotEnough_ReturnsAllTouchedExercisesAndFirstUntouchedExercises()
		{
			// Arrange

			var mocker = new AutoMocker();

			var touchedExercises = new[]
			{
				CreateTranslateTextExercise("Touched Exercise 1", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
				CreateTranslateTextExercise("Touched Exercise 2", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
				CreateTranslateTextExercise("Touched Exercise 3", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
			};

			var untouchedExercises = Enumerable.Range(1, 100)
				.Select(n => CreateTranslateTextExerciseWithNoResults($"Untouched Exercise {n}", mocker, createDate: new DateTime(2023, 07, 11).AddDays(n)))
				.ToList();

			var target = mocker.CreateInstance<ExercisesSelector>();

			// Act

			var result = target.SelectExercisesToPerform(new DateOnly(2024, 07, 11), touchedExercises.Concat(untouchedExercises), 5);

			// Assert

			var expectedResult = new[]
			{
				touchedExercises.Get("Touched Exercise 1"),
				touchedExercises.Get("Touched Exercise 2"),
				touchedExercises.Get("Touched Exercise 3"),
				untouchedExercises.Get("Untouched Exercise 1"),
				untouchedExercises.Get("Untouched Exercise 2"),
			};

			result.Should().BeEquivalentTo(expectedResult, x => x.WithoutStrictOrdering());
		}

		[TestMethod]
		public void SelectExercisesToPerform_IfNumberOfUntouchedTranslateTextExercisesDoesNotExceedDailyLimit_ReturnsAllSuchExercises()
		{
			// Arrange

			var mocker = new AutoMocker();

			var touchedExercises = new[]
			{
				CreateTranslateTextExercise("Touched Exercise 1", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
				CreateTranslateTextExercise("Touched Exercise 2", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
				CreateTranslateTextExercise("Touched Exercise 3", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
				CreateTranslateTextExercise("Touched Exercise 4", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
				CreateTranslateTextExercise("Touched Exercise 5", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
				CreateTranslateTextExercise("Touched Exercise 6", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
				CreateTranslateTextExercise("Touched Exercise 7", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
				CreateTranslateTextExercise("Touched Exercise 8", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
				CreateTranslateTextExercise("Touched Exercise 9", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
				CreateTranslateTextExercise("Touched Exercise 10", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11)),
			};

			var untouchedExercises = new[]
			{
				CreateTranslateTextExerciseWithNoResults("Untouched Exercise 1", mocker, createDate: new DateTime(2023, 07, 03)),
				CreateTranslateTextExerciseWithNoResults("Untouched Exercise 2", mocker, createDate: new DateTime(2023, 07, 03)),
				CreateTranslateTextExerciseWithNoResults("Untouched Exercise 3", mocker, createDate: new DateTime(2023, 07, 03)),
				CreateTranslateTextExerciseWithNoResults("Untouched Exercise 4", mocker, createDate: new DateTime(2023, 07, 03)),
				CreateTranslateTextExerciseWithNoResults("Untouched Exercise 5", mocker, createDate: new DateTime(2023, 07, 03)),
				CreateTranslateTextExerciseWithNoResults("Untouched Exercise 6", mocker, createDate: new DateTime(2023, 07, 03)),
				CreateTranslateTextExerciseWithNoResults("Untouched Exercise 7", mocker, createDate: new DateTime(2023, 07, 03)),
				CreateTranslateTextExerciseWithNoResults("Untouched Exercise 8", mocker, createDate: new DateTime(2023, 07, 03)),
				CreateTranslateTextExerciseWithNoResults("Untouched Exercise 9", mocker, createDate: new DateTime(2023, 07, 03)),
				CreateTranslateTextExerciseWithNoResults("Untouched Exercise 10", mocker, createDate: new DateTime(2023, 07, 03)),
			};

			var target = mocker.CreateInstance<ExercisesSelector>();

			// Act

			var result = target.SelectExercisesToPerform(new DateOnly(2024, 07, 11), touchedExercises.Concat(untouchedExercises), 15);

			// Assert

			result.Count.Should().Be(15);

			result.Should().Contain(untouchedExercises);
		}

		[TestMethod]
		public void SelectExercisesToPerform_IfOnlyTouchedExercisesAreReturned_ReturnsExercisesInRandomOrder()
		{
			// Arrange

			var mocker = new AutoMocker();

			var exercises = Enumerable.Range(1, 100)
				.Select(n => CreateTranslateTextExercise($"{n}", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11).AddDays(n)))
				.ToList();

			var target = mocker.CreateInstance<ExercisesSelector>();

			// Act

			var result = target.SelectExercisesToPerform(new DateOnly(2024, 07, 11), exercises, 100);

			// Assert

			var resultIds = result.Cast<TranslateTextExercise>().Select(x => Int32.Parse(x.TextInStudiedLanguage.Id.Value, NumberStyles.None, CultureInfo.InvariantCulture));

			resultIds.Should().NotBeInAscendingOrder();
		}

		[TestMethod]
		public void SelectExercisesToPerform_IfOnlyUntouchedExercisesAreReturned_ReturnsExercisesInRandomOrder()
		{
			// Arrange

			var mocker = new AutoMocker();

			var exercises = Enumerable.Range(1, 100)
				.Select(n => CreateTranslateTextExerciseWithNoResults($"{n}", mocker, createDate: new DateTime(2023, 07, 11).AddDays(n)))
				.ToList();

			var target = mocker.CreateInstance<ExercisesSelector>();

			// Act

			var result = target.SelectExercisesToPerform(new DateOnly(2024, 07, 11), exercises, 100);

			// Assert

			var resultIds = result.Cast<TranslateTextExercise>().Select(x => Int32.Parse(x.TextInStudiedLanguage.Id.Value, NumberStyles.None, CultureInfo.InvariantCulture));

			resultIds.Should().NotBeInAscendingOrder();
		}

		[TestMethod]
		public void SelectExercisesToPerform_IfTouchedAndUntouchedExercisesAreReturned_ReturnsExercisesInRandomOrder()
		{
			// Arrange

			var mocker = new AutoMocker();

			var touchedExercises = Enumerable.Range(1, 100)
				.Select(n => CreateTranslateTextExercise($"{n}", mocker, lastExerciseDate: new DateTime(2023, 07, 10), nextExerciseDate: new DateOnly(2023, 07, 11).AddDays(n)))
				.ToList();

			var untouchedExercises = Enumerable.Range(101, 100)
				.Select(n => CreateTranslateTextExerciseWithNoResults($"{n}", mocker, createDate: new DateTime(2023, 07, 11).AddDays(n)))
				.ToList();

			var target = mocker.CreateInstance<ExercisesSelector>();

			// Act

			var result = target.SelectExercisesToPerform(new DateOnly(2024, 07, 11), touchedExercises.Concat(untouchedExercises), 200);

			// Assert

			var resultIds = result.Cast<TranslateTextExercise>().Select(x => Int32.Parse(x.TextInStudiedLanguage.Id.Value, NumberStyles.None, CultureInfo.InvariantCulture)).ToList();

			resultIds.Should().NotBeInAscendingOrder();
			resultIds.Take(100).Should().NotBeInAscendingOrder();
			resultIds.Skip(100).Take(100).Should().NotBeInAscendingOrder();
		}

		private static TranslateTextExercise CreateTranslateTextExercise(string id, AutoMocker mocker, DateTimeOffset lastExerciseDate, DateOnly nextExerciseDate)
		{
			var exercise = new TranslateTextExercise(new[] { lastExerciseDate.DateTime.ToSuccessfulExerciseResult() })
			{
				TextInStudiedLanguage = new LanguageText
				{
					Id = new ItemId(id),
				},
			};

			mocker.GetMock<INextExerciseDateProvider>()
				.Setup(x => x.GetNextExerciseDate(exercise)).Returns(nextExerciseDate);

			return exercise;
		}

		private static TranslateTextExercise CreateTranslateTextExerciseWithNoResults(string id, AutoMocker mocker, DateTimeOffset createDate)
		{
			var exercise = new TranslateTextExercise([])
			{
				TextInStudiedLanguage = new LanguageText
				{
					Id = new ItemId(id),
					CreationTimestamp = createDate,
				},
			};

			mocker.GetMock<INextExerciseDateProvider>()
				.Setup(x => x.GetNextExerciseDate(exercise)).Returns(DateOnly.MinValue);

			return exercise;
		}

		private static InflectWordExercise CreateInflectWordExerciseWithNoResults(string id, AutoMocker mocker, DateTimeOffset createDate)
		{
			var exercise = new InflectWordExercise(new ItemId(id), createDate, "test description", "test base form", [], []);

			mocker.GetMock<INextExerciseDateProvider>()
				.Setup(x => x.GetNextExerciseDate(exercise)).Returns(DateOnly.MinValue);

			return exercise;
		}
	}
}
