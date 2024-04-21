using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Settings;

namespace VocabularyCoach.Services.UnitTests
{
	[TestClass]
	public class VocabularyServiceTests
	{
		private User TestUser { get; } = new();

		private PracticeSettings TestSettings { get; } = new()
		{
			DailyLimit = 100,
		};

		[TestMethod]
		public async Task CheckTypedText_ForCorrectlyTypedText_AddsOkCheckResultType()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.Use(Options.Create(TestSettings));
			var target = mocker.CreateInstance<VocabularyService>();

			var studiedText = new StudiedText(Enumerable.Empty<CheckResult>())
			{
				TextInStudiedLanguage = new LanguageText
				{
					Text = "some text",
				},
			};

			// Act

			await target.CheckTypedText(TestUser, studiedText, "some text", CancellationToken.None);

			// Assert

			var expectedCheckResult = new CheckResult
			{
				CheckResultType = CheckResultType.Ok,
			};

			studiedText.CheckResults.Single().Should().BeEquivalentTo(expectedCheckResult, x => x.Excluding(y => y.DateTime));
		}

		[TestMethod]
		public async Task CheckTypedText_ForMisspelledText_AddsMisspelledCheckResultType()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.Use(Options.Create(TestSettings));
			var target = mocker.CreateInstance<VocabularyService>();

			var studiedText = new StudiedText(Enumerable.Empty<CheckResult>())
			{
				TextInStudiedLanguage = new LanguageText
				{
					Text = "some text",
				},
			};

			// Act

			await target.CheckTypedText(TestUser, studiedText, "another text", CancellationToken.None);

			// Assert

			var expectedCheckResult = new CheckResult
			{
				CheckResultType = CheckResultType.Misspelled,
				TypedText = "another text",
			};

			studiedText.CheckResults.Single().Should().BeEquivalentTo(expectedCheckResult, x => x.Excluding(y => y.DateTime));
		}

		[TestMethod]
		public async Task CheckTypedText_IfTypedTextDiffersInCase_AddsMisspelledCheckResultType()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.Use(Options.Create(TestSettings));
			var target = mocker.CreateInstance<VocabularyService>();

			var studiedText = new StudiedText(Enumerable.Empty<CheckResult>())
			{
				TextInStudiedLanguage = new LanguageText
				{
					Text = "Some Text",
				},
			};

			// Act

			await target.CheckTypedText(TestUser, studiedText, "some text", CancellationToken.None);

			// Assert

			var expectedCheckResult = new CheckResult
			{
				CheckResultType = CheckResultType.Misspelled,
				TypedText = "some text",
			};

			studiedText.CheckResults.Single().Should().BeEquivalentTo(expectedCheckResult, x => x.Excluding(y => y.DateTime));
		}

		[TestMethod]
		public async Task CheckTypedText_IfTypedTextDiffersInDiacritics_AddsMisspelledCheckResultType()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.Use(Options.Create(TestSettings));
			var target = mocker.CreateInstance<VocabularyService>();

			var studiedText = new StudiedText(Enumerable.Empty<CheckResult>())
			{
				TextInStudiedLanguage = new LanguageText
				{
					Text = "góra",
				},
			};

			// Act

			await target.CheckTypedText(TestUser, studiedText, "gora", CancellationToken.None);

			// Assert

			var expectedCheckResult = new CheckResult
			{
				CheckResultType = CheckResultType.Misspelled,
				TypedText = "gora",
			};

			studiedText.CheckResults.Single().Should().BeEquivalentTo(expectedCheckResult, x => x.Excluding(y => y.DateTime));
		}

		[TestMethod]
		public async Task CheckTypedText_ForSkippedText_AddsSkippedCheckResultType()
		{
			// Arrange

			var mocker = new AutoMocker();
			mocker.Use(Options.Create(TestSettings));
			var target = mocker.CreateInstance<VocabularyService>();

			var studiedText = new StudiedText(Enumerable.Empty<CheckResult>())
			{
				TextInStudiedLanguage = new LanguageText
				{
					Text = "some text",
				},
			};

			// Act

			await target.CheckTypedText(TestUser, studiedText, String.Empty, CancellationToken.None);

			// Assert

			var expectedCheckResult = new CheckResult
			{
				CheckResultType = CheckResultType.Skipped,
			};

			studiedText.CheckResults.Single().Should().BeEquivalentTo(expectedCheckResult, x => x.Excluding(y => y.DateTime));
		}
	}
}
