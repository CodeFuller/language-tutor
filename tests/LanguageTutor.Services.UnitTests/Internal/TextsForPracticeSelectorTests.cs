using System;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using LanguageTutor.Models;
using LanguageTutor.Services.Internal;
using LanguageTutor.Services.UnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace LanguageTutor.Services.UnitTests.Internal
{
	[TestClass]
	public class TextsForPracticeSelectorTests
	{
		[TestMethod]
		public void GetTextsForPractice_ForMissingStudiedTexts_ReturnsEmptyCollection()
		{
			// Arrange

			var studiedTexts = Array.Empty<StudiedText>();

			var mocker = new AutoMocker();
			mocker.GetMock<INextCheckDateProvider>()
				.Setup(x => x.GetNextCheckDate(It.IsAny<StudiedText>())).Returns(new DateOnly(2023, 07, 11));

			var target = mocker.CreateInstance<TextsForPracticeSelector>();

			// Act

			var result = target.GetTextsForPractice(new DateOnly(2023, 07, 11), studiedTexts, 100);

			// Assert

			result.Should().BeEmpty();
		}

		[TestMethod]
		public void GetTextsForPractice_ForTextsWithNextCheckDateNotYetReached_DoesNotReturnSuchTexts()
		{
			// Arrange

			var mocker = new AutoMocker();

			var studiedTexts = new[]
			{
				CreateStudiedText("Text 1", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
				CreateStudiedText("Text 2", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 12)),
				CreateStudiedText("Text 3", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
				CreateStudiedText("Text 4", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 12)),
			};

			var target = mocker.CreateInstance<TextsForPracticeSelector>();

			// Act

			var textsForPractice = target.GetTextsForPractice(new DateOnly(2023, 07, 11), studiedTexts, 100);

			// Assert

			var expectedTextsForCheck = new[]
			{
				studiedTexts.Get("Text 1"),
				studiedTexts.Get("Text 3"),
			};

			textsForPractice.Should().BeEquivalentTo(expectedTextsForCheck, x => x.WithoutStrictOrdering());
		}

		[TestMethod]
		public void GetTextsForPractice_IfDailyLimitIsReached_ReturnsEmptyCollection()
		{
			// Arrange

			var mocker = new AutoMocker();

			var studiedTexts = new[]
			{
				CreateStudiedText("Text 1", mocker, lastCheckDate: new DateTime(2023, 07, 11), nextCheckDate: new DateOnly(2023, 07, 11)),
				CreateStudiedText("Text 2", mocker, lastCheckDate: new DateTime(2023, 07, 11), nextCheckDate: new DateOnly(2023, 07, 11)),
				CreateStudiedText("Text 3", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
			};

			var target = mocker.CreateInstance<TextsForPracticeSelector>();

			// Act

			var textsForPractice = target.GetTextsForPractice(new DateOnly(2023, 07, 11), studiedTexts, 2);

			// Assert

			textsForPractice.Should().BeEmpty();
		}

		[TestMethod]
		public void GetTextsForPractice_IfSomeTextsWerePracticedToday_TakesSuchTextsIntoAccountForDailyLimit()
		{
			// Arrange

			var mocker = new AutoMocker();

			var studiedTexts = new[]
			{
				CreateStudiedText("Text 1", mocker, lastCheckDate: new DateTime(2023, 07, 11), nextCheckDate: new DateOnly(2023, 07, 12)),
				CreateStudiedText("Text 2", mocker, lastCheckDate: new DateTime(2023, 07, 11), nextCheckDate: new DateOnly(2023, 07, 12)),
				CreateStudiedText("Text 3", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
				CreateStudiedText("Text 4", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
				CreateStudiedText("Text 5", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
				CreateStudiedText("Text 6", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
				CreateStudiedText("Text 7", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
			};

			var target = mocker.CreateInstance<TextsForPracticeSelector>();

			// Act

			var textsForPractice = target.GetTextsForPractice(new DateOnly(2023, 07, 11), studiedTexts, 5);

			// Assert

			textsForPractice.Count.Should().Be(3);

			textsForPractice.Should().NotContain(studiedTexts.Get("Text 1"));
			textsForPractice.Should().NotContain(studiedTexts.Get("Text 2"));
		}

		[TestMethod]
		public void GetTextsForPractice_IfNumberOfSuitablePreviouslyPracticedTextsExceedsDailyLimit_ReturnsTextsWithMostOverduePracticeDate()
		{
			// Arrange

			var mocker = new AutoMocker();

			var studiedTexts = new[]
			{
				CreateStudiedText("Group 3 - Text 1", mocker, lastCheckDate: new DateTime(2023, 07, 11), nextCheckDate: new DateOnly(2023, 07, 14)),
				CreateStudiedText("Group 3 - Text 2", mocker, lastCheckDate: new DateTime(2023, 07, 11), nextCheckDate: new DateOnly(2023, 07, 14)),
				CreateStudiedText("Group 3 - Text 3", mocker, lastCheckDate: new DateTime(2023, 07, 11), nextCheckDate: new DateOnly(2023, 07, 14)),

				CreateStudiedText("Group 1 - Text 1", mocker, lastCheckDate: new DateTime(2023, 07, 11), nextCheckDate: new DateOnly(2023, 07, 12)),
				CreateStudiedText("Group 1 - Text 2", mocker, lastCheckDate: new DateTime(2023, 07, 11), nextCheckDate: new DateOnly(2023, 07, 12)),
				CreateStudiedText("Group 1 - Text 3", mocker, lastCheckDate: new DateTime(2023, 07, 11), nextCheckDate: new DateOnly(2023, 07, 12)),

				CreateStudiedText("Group 2 - Text 1", mocker, lastCheckDate: new DateTime(2023, 07, 11), nextCheckDate: new DateOnly(2023, 07, 13)),
				CreateStudiedText("Group 2 - Text 2", mocker, lastCheckDate: new DateTime(2023, 07, 11), nextCheckDate: new DateOnly(2023, 07, 13)),
				CreateStudiedText("Group 2 - Text 3", mocker, lastCheckDate: new DateTime(2023, 07, 11), nextCheckDate: new DateOnly(2023, 07, 13)),
			};

			var target = mocker.CreateInstance<TextsForPracticeSelector>();

			// Act

			var textsForPractice1 = target.GetTextsForPractice(new DateOnly(2023, 07, 14), studiedTexts, 3);
			var textsForPractice2 = target.GetTextsForPractice(new DateOnly(2023, 07, 14), studiedTexts, 6);

			// Assert

			var expectedTextsForCheck1 = new[]
			{
				studiedTexts.Get("Group 1 - Text 1"),
				studiedTexts.Get("Group 1 - Text 2"),
				studiedTexts.Get("Group 1 - Text 3"),
			};

			var expectedTextsForCheck2 = new[]
			{
				studiedTexts.Get("Group 1 - Text 1"),
				studiedTexts.Get("Group 1 - Text 2"),
				studiedTexts.Get("Group 1 - Text 3"),
				studiedTexts.Get("Group 2 - Text 1"),
				studiedTexts.Get("Group 2 - Text 2"),
				studiedTexts.Get("Group 2 - Text 3"),
			};

			textsForPractice1.Should().BeEquivalentTo(expectedTextsForCheck1, x => x.WithoutStrictOrdering());
			textsForPractice2.Should().BeEquivalentTo(expectedTextsForCheck2, x => x.WithoutStrictOrdering());
		}

		[TestMethod]
		public void GetTextsForPractice_IfNumberOfSuitablePreviouslyPracticedTextsForLatestCheckDateExceedsDailyLimit_ReturnsRandomSubsetOfSuchTexts()
		{
			// Arrange

			var mocker = new AutoMocker();

			// Since texts are picked in random order, we take quite a big number of such texts,
			// so that we can check that items are actually randomized, i.e. texts are returned from both halves.
			var studiedTexts1 = Enumerable.Range(1, 100)
				.Select(n => CreateStudiedText($"Practiced Text - 1st Half - {n}", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)))
				.ToList();

			var studiedTexts2 = Enumerable.Range(1, 100)
				.Select(n => CreateStudiedText($"Practiced Text - 2nd Half - {n}", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)))
				.ToList();

			var target = mocker.CreateInstance<TextsForPracticeSelector>();

			// Act

			var textsForPractice = target.GetTextsForPractice(new DateOnly(2023, 07, 11), studiedTexts1.Concat(studiedTexts2), 100);

			// Assert

			textsForPractice.Should().Contain(x => studiedTexts1.Any(y => ReferenceEquals(x, y)));
			textsForPractice.Should().Contain(x => studiedTexts2.Any(y => ReferenceEquals(x, y)));
		}

		[TestMethod]
		public void GetTextsForPractice_IfNumberOfPreviouslyUnpracticedTextsExceedsDailyLimitAndSuitablePreviouslyPracticedTextsIsEnough_ReturnsOnlyPreviouslyPracticedTexts()
		{
			// Arrange

			var mocker = new AutoMocker();

			var practicedTexts = new[]
			{
				CreateStudiedText("Practiced Text 1", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
				CreateStudiedText("Practiced Text 2", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
				CreateStudiedText("Practiced Text 3", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
			};

			var unpracticedTexts = Enumerable.Range(1, 100)
				.Select(n => CreateStudiedTextWithNoChecks($"Unpracticed Text {n}", mocker, createDate: new DateTime(2023, 07, 11).AddDays(n)))
				.ToList();

			var target = mocker.CreateInstance<TextsForPracticeSelector>();

			// Act

			var textsForPractice = target.GetTextsForPractice(new DateOnly(2024, 07, 11), practicedTexts.Concat(unpracticedTexts), 3);

			// Assert

			textsForPractice.Should().BeEquivalentTo(practicedTexts, x => x.WithoutStrictOrdering());
		}

		[TestMethod]
		public void GetTextsForPractice_IfNumberOfPreviouslyUnpracticedTextsExceedsDailyLimitAndSuitablePreviouslyPracticedTextsIsNotEnough_ReturnsAllPreviouslyPracticedTextsAndFirstUnpracticedTexts()
		{
			// Arrange

			var mocker = new AutoMocker();

			var practicedTexts = new[]
			{
				CreateStudiedText("Practiced Text 1", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
				CreateStudiedText("Practiced Text 2", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
				CreateStudiedText("Practiced Text 3", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
			};

			var unpracticedTexts = Enumerable.Range(1, 100)
				.Select(n => CreateStudiedTextWithNoChecks($"Unpracticed Text {n}", mocker, createDate: new DateTime(2023, 07, 11).AddDays(n)))
				.ToList();

			var target = mocker.CreateInstance<TextsForPracticeSelector>();

			// Act

			var textsForPractice = target.GetTextsForPractice(new DateOnly(2024, 07, 11), practicedTexts.Concat(unpracticedTexts), 5);

			// Assert

			var expectedTextsForCheck = new[]
			{
				practicedTexts.Get("Practiced Text 1"),
				practicedTexts.Get("Practiced Text 2"),
				practicedTexts.Get("Practiced Text 3"),
				unpracticedTexts.Get("Unpracticed Text 1"),
				unpracticedTexts.Get("Unpracticed Text 2"),
			};

			textsForPractice.Should().BeEquivalentTo(expectedTextsForCheck, x => x.WithoutStrictOrdering());
		}

		[TestMethod]
		public void GetTextsForPractice_IfNumberOfPreviouslyUnpracticedTextsDoesNotExceedDailyLimit_ReturnsAllSuchTexts()
		{
			// Arrange

			var mocker = new AutoMocker();

			var practicedTexts = new[]
			{
				CreateStudiedText("Practiced Text 1", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
				CreateStudiedText("Practiced Text 2", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
				CreateStudiedText("Practiced Text 3", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
				CreateStudiedText("Practiced Text 4", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
				CreateStudiedText("Practiced Text 5", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
				CreateStudiedText("Practiced Text 6", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
				CreateStudiedText("Practiced Text 7", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
				CreateStudiedText("Practiced Text 8", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
				CreateStudiedText("Practiced Text 9", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
				CreateStudiedText("Practiced Text 10", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11)),
			};

			var unpracticedTexts = new[]
			{
				CreateStudiedTextWithNoChecks("Unpracticed Text 1", mocker, createDate: new DateTime(2023, 07, 03)),
				CreateStudiedTextWithNoChecks("Unpracticed Text 2", mocker, createDate: new DateTime(2023, 07, 03)),
				CreateStudiedTextWithNoChecks("Unpracticed Text 3", mocker, createDate: new DateTime(2023, 07, 03)),
				CreateStudiedTextWithNoChecks("Unpracticed Text 4", mocker, createDate: new DateTime(2023, 07, 03)),
				CreateStudiedTextWithNoChecks("Unpracticed Text 5", mocker, createDate: new DateTime(2023, 07, 03)),
				CreateStudiedTextWithNoChecks("Unpracticed Text 6", mocker, createDate: new DateTime(2023, 07, 03)),
				CreateStudiedTextWithNoChecks("Unpracticed Text 7", mocker, createDate: new DateTime(2023, 07, 03)),
				CreateStudiedTextWithNoChecks("Unpracticed Text 8", mocker, createDate: new DateTime(2023, 07, 03)),
				CreateStudiedTextWithNoChecks("Unpracticed Text 9", mocker, createDate: new DateTime(2023, 07, 03)),
				CreateStudiedTextWithNoChecks("Unpracticed Text 10", mocker, createDate: new DateTime(2023, 07, 03)),
			};

			var target = mocker.CreateInstance<TextsForPracticeSelector>();

			// Act

			var textsForPractice = target.GetTextsForPractice(new DateOnly(2024, 07, 11), practicedTexts.Concat(unpracticedTexts), 15);

			// Assert

			textsForPractice.Count.Should().Be(15);

			textsForPractice.Should().Contain(unpracticedTexts);
		}

		[TestMethod]
		public void GetTextsForPractice_IfOnlyTextsPracticedBeforeAreReturned_ReturnsTextsInRandomOrder()
		{
			// Arrange

			var mocker = new AutoMocker();

			var studiedTexts = Enumerable.Range(1, 100)
				.Select(n => CreateStudiedText($"{n}", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11).AddDays(n)))
				.ToList();

			var target = mocker.CreateInstance<TextsForPracticeSelector>();

			// Act

			var textsForPractice = target.GetTextsForPractice(new DateOnly(2024, 07, 11), studiedTexts, 100);

			// Assert

			var resultIds = textsForPractice.Select(x => Int32.Parse(x.TextInStudiedLanguage.Id.Value, NumberStyles.None, CultureInfo.InvariantCulture));

			resultIds.Should().NotBeInAscendingOrder();
		}

		[TestMethod]
		public void GetTextsForPractice_IfOnlyTextsUnpracticedBeforeAreReturned_ReturnsTextsInRandomOrder()
		{
			// Arrange

			var mocker = new AutoMocker();

			var studiedTexts = Enumerable.Range(1, 100)
				.Select(n => CreateStudiedTextWithNoChecks($"{n}", mocker, createDate: new DateTime(2023, 07, 11).AddDays(n)))
				.ToList();

			var target = mocker.CreateInstance<TextsForPracticeSelector>();

			// Act

			var textsForPractice = target.GetTextsForPractice(new DateOnly(2024, 07, 11), studiedTexts, 100);

			// Assert

			var resultIds = textsForPractice.Select(x => Int32.Parse(x.TextInStudiedLanguage.Id.Value, NumberStyles.None, CultureInfo.InvariantCulture));

			resultIds.Should().NotBeInAscendingOrder();
		}

		[TestMethod]
		public void GetTextsForPractice_IfPracticedAndUnpracticedTextsAreReturned_ReturnsTextsInRandomOrder()
		{
			// Arrange

			var mocker = new AutoMocker();

			var practicedTexts = Enumerable.Range(1, 100)
				.Select(n => CreateStudiedText($"{n}", mocker, lastCheckDate: new DateTime(2023, 07, 10), nextCheckDate: new DateOnly(2023, 07, 11).AddDays(n)))
				.ToList();

			var unpracticedTexts = Enumerable.Range(101, 100)
				.Select(n => CreateStudiedTextWithNoChecks($"{n}", mocker, createDate: new DateTime(2023, 07, 11).AddDays(n)))
				.ToList();

			var target = mocker.CreateInstance<TextsForPracticeSelector>();

			// Act

			var textsForPractice = target.GetTextsForPractice(new DateOnly(2024, 07, 11), practicedTexts.Concat(unpracticedTexts), 200);

			// Assert

			var resultIds = textsForPractice.Select(x => Int32.Parse(x.TextInStudiedLanguage.Id.Value, NumberStyles.None, CultureInfo.InvariantCulture)).ToList();

			resultIds.Should().NotBeInAscendingOrder();
			resultIds.Take(100).Should().NotBeInAscendingOrder();
			resultIds.Skip(100).Take(100).Should().NotBeInAscendingOrder();
		}

		private static StudiedText CreateStudiedText(string id, AutoMocker mocker, DateTimeOffset lastCheckDate, DateOnly nextCheckDate)
		{
			var studiedText = new StudiedText(new[] { GetSuccessfulCheck(lastCheckDate) })
			{
				TextInStudiedLanguage = new LanguageText
				{
					Id = new ItemId(id),
				},
			};

			mocker.GetMock<INextCheckDateProvider>()
				.Setup(x => x.GetNextCheckDate(studiedText)).Returns(nextCheckDate);

			return studiedText;
		}

		private static CheckResult GetSuccessfulCheck(DateTimeOffset dateTime)
		{
			return new CheckResult
			{
				DateTime = dateTime,
				CheckResultType = CheckResultType.Ok,
			};
		}

		private static StudiedText CreateStudiedTextWithNoChecks(string id, AutoMocker mocker, DateTimeOffset createDate)
		{
			var studiedText = new StudiedText(Enumerable.Empty<CheckResult>())
			{
				TextInStudiedLanguage = new LanguageText
				{
					Id = new ItemId(id),
					CreationTimestamp = createDate,
				},
			};

			mocker.GetMock<INextCheckDateProvider>()
				.Setup(x => x.GetNextCheckDate(studiedText)).Returns(DateOnly.MinValue);

			return studiedText;
		}
	}
}
