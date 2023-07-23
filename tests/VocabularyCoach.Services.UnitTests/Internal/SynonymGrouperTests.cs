using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;
using VocabularyCoach.Services.Internal;
using VocabularyCoach.Services.UnitTests.Helpers;

namespace VocabularyCoach.Services.UnitTests.Internal
{
	[TestClass]
	public class SynonymGrouperTests
	{
		[TestMethod]
		public void GroupStudiedTranslationsBySynonyms_ForTranslationsWithoutSynonyms_DoesNotGroupTranslations()
		{
			// Arrange

			var studiedTranslations = new StudiedTranslationData[]
			{
				new()
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					TextInKnownLanguage = "Text in known language #1".ToLanguageText(),
					CheckResults = Array.Empty<CheckResult>(),
				},

				new()
				{
					TextInStudiedLanguage = "Text in studied language #2".ToLanguageText(),
					TextInKnownLanguage = "Text in known language #2".ToLanguageText(),
					CheckResults = Array.Empty<CheckResult>(),
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<SynonymGrouper>();

			// Act

			var groupedTexts = target.GroupStudiedTranslationsBySynonyms(studiedTranslations);

			// Assert

			var expectedResults = new StudiedText[]
			{
				new(Enumerable.Empty<CheckResult>())
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					OtherSynonymsInStudiedLanguage = Array.Empty<LanguageText>(),
					SynonymsInKnownLanguage = new[] { "Text in known language #1".ToLanguageText() },
				},

				new(Enumerable.Empty<CheckResult>())
				{
					TextInStudiedLanguage = "Text in studied language #2".ToLanguageText(),
					OtherSynonymsInStudiedLanguage = Array.Empty<LanguageText>(),
					SynonymsInKnownLanguage = new[] { "Text in known language #2".ToLanguageText() },
				},
			};

			groupedTexts.Should().BeEquivalentTo(expectedResults, x => x.WithoutStrictOrdering());
		}

		[TestMethod]
		public void GroupStudiedTranslationsBySynonyms_ForSynonymsIsStudiedLanguageWithSingleTranslation_GroupsSynonymsCorrectly()
		{
			// Arrange

			var studiedTranslations = new StudiedTranslationData[]
			{
				new()
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					TextInKnownLanguage = "Text in known language #1".ToLanguageText(),
					CheckResults = Array.Empty<CheckResult>(),
				},

				new()
				{
					TextInStudiedLanguage = "Text in studied language #2".ToLanguageText(),
					TextInKnownLanguage = "Text in known language #1".ToLanguageText(),
					CheckResults = Array.Empty<CheckResult>(),
				},

				new()
				{
					TextInStudiedLanguage = "Text in studied language #3".ToLanguageText(),
					TextInKnownLanguage = "Text in known language #1".ToLanguageText(),
					CheckResults = Array.Empty<CheckResult>(),
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<SynonymGrouper>();

			// Act

			var groupedTexts = target.GroupStudiedTranslationsBySynonyms(studiedTranslations);

			// Assert

			var expectedResults = new StudiedText[]
			{
				new(Enumerable.Empty<CheckResult>())
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					OtherSynonymsInStudiedLanguage = new[]
					{
						"Text in studied language #2".ToLanguageText(),
						"Text in studied language #3".ToLanguageText(),
					},
					SynonymsInKnownLanguage = new[] { "Text in known language #1".ToLanguageText() },
				},

				new(Enumerable.Empty<CheckResult>())
				{
					TextInStudiedLanguage = "Text in studied language #2".ToLanguageText(),
					OtherSynonymsInStudiedLanguage = new[]
					{
						"Text in studied language #1".ToLanguageText(),
						"Text in studied language #3".ToLanguageText(),
					},
					SynonymsInKnownLanguage = new[] { "Text in known language #1".ToLanguageText() },
				},

				new(Enumerable.Empty<CheckResult>())
				{
					TextInStudiedLanguage = "Text in studied language #3".ToLanguageText(),
					OtherSynonymsInStudiedLanguage = new[]
					{
						"Text in studied language #1".ToLanguageText(),
						"Text in studied language #2".ToLanguageText(),
					},
					SynonymsInKnownLanguage = new[] { "Text in known language #1".ToLanguageText() },
				},
			};

			groupedTexts.Should().BeEquivalentTo(expectedResults, x => x.WithoutStrictOrdering());
		}

		[TestMethod]
		public void GroupStudiedTranslationsBySynonyms_ForSynonymsIsStudiedLanguageWithMultipleTranslations_GroupsSynonymsCorrectly()
		{
			// Arrange

			var studiedTranslations = new StudiedTranslationData[]
			{
				new()
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					TextInKnownLanguage = "Text in known language #1".ToLanguageText(),
					CheckResults = Array.Empty<CheckResult>(),
				},

				new()
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					TextInKnownLanguage = "Text in known language #2".ToLanguageText(),
					CheckResults = Array.Empty<CheckResult>(),
				},

				new()
				{
					TextInStudiedLanguage = "Text in studied language #2".ToLanguageText(),
					TextInKnownLanguage = "Text in known language #1".ToLanguageText(),
					CheckResults = Array.Empty<CheckResult>(),
				},

				new()
				{
					TextInStudiedLanguage = "Text in studied language #2".ToLanguageText(),
					TextInKnownLanguage = "Text in known language #2".ToLanguageText(),
					CheckResults = Array.Empty<CheckResult>(),
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<SynonymGrouper>();

			// Act

			var groupedTexts = target.GroupStudiedTranslationsBySynonyms(studiedTranslations);

			// Assert

			var expectedResults = new StudiedText[]
			{
				new(Enumerable.Empty<CheckResult>())
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					OtherSynonymsInStudiedLanguage = new[]
					{
						"Text in studied language #2".ToLanguageText(),
					},
					SynonymsInKnownLanguage = new[]
					{
						"Text in known language #1".ToLanguageText(),
						"Text in known language #2".ToLanguageText(),
					},
				},

				new(Enumerable.Empty<CheckResult>())
				{
					TextInStudiedLanguage = "Text in studied language #2".ToLanguageText(),
					OtherSynonymsInStudiedLanguage = new[]
					{
						"Text in studied language #1".ToLanguageText(),
					},
					SynonymsInKnownLanguage = new[]
					{
						"Text in known language #1".ToLanguageText(),
						"Text in known language #2".ToLanguageText(),
					},
				},
			};

			groupedTexts.Should().BeEquivalentTo(expectedResults, x => x.WithoutStrictOrdering());
		}

		[TestMethod]
		public void GroupStudiedTranslationsBySynonyms_ForSynonymsIsStudiedLanguageWithPartialTranslationsMatching_DoesNotGroupSychSynonyms()
		{
			// Arrange

			var studiedTranslations = new StudiedTranslationData[]
			{
				new()
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					TextInKnownLanguage = "Text in known language #1".ToLanguageText(),
					CheckResults = Array.Empty<CheckResult>(),
				},

				new()
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					TextInKnownLanguage = "Text in known language #2".ToLanguageText(),
					CheckResults = Array.Empty<CheckResult>(),
				},

				new()
				{
					TextInStudiedLanguage = "Text in studied language #2".ToLanguageText(),
					TextInKnownLanguage = "Text in known language #1".ToLanguageText(),
					CheckResults = Array.Empty<CheckResult>(),
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<SynonymGrouper>();

			// Act

			var groupedTexts = target.GroupStudiedTranslationsBySynonyms(studiedTranslations);

			// Assert

			var expectedResults = new StudiedText[]
			{
				new(Enumerable.Empty<CheckResult>())
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					OtherSynonymsInStudiedLanguage = Array.Empty<LanguageText>(),
					SynonymsInKnownLanguage = new[]
					{
						"Text in known language #1".ToLanguageText(),
						"Text in known language #2".ToLanguageText(),
					},
				},

				new(Enumerable.Empty<CheckResult>())
				{
					TextInStudiedLanguage = "Text in studied language #2".ToLanguageText(),
					OtherSynonymsInStudiedLanguage = Array.Empty<LanguageText>(),
					SynonymsInKnownLanguage = new[]
					{
						"Text in known language #1".ToLanguageText(),
					},
				},
			};

			groupedTexts.Should().BeEquivalentTo(expectedResults, x => x.WithoutStrictOrdering());
		}

		[TestMethod]
		public void GroupStudiedTranslationsBySynonyms_ForSynonymsIsKnownLanguage_GroupsSynonymsCorrectly()
		{
			// Arrange

			var studiedTranslations = new StudiedTranslationData[]
			{
				new()
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					TextInKnownLanguage = "Text in known language #1".ToLanguageText(),
					CheckResults = Array.Empty<CheckResult>(),
				},

				new()
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					TextInKnownLanguage = "Text in known language #2".ToLanguageText(),
					CheckResults = Array.Empty<CheckResult>(),
				},

				new()
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					TextInKnownLanguage = "Text in known language #3".ToLanguageText(),
					CheckResults = Array.Empty<CheckResult>(),
				},
			};

			var mocker = new AutoMocker();
			var target = mocker.CreateInstance<SynonymGrouper>();

			// Act

			var groupedTexts = target.GroupStudiedTranslationsBySynonyms(studiedTranslations);

			// Assert

			var expectedResults = new StudiedText[]
			{
				new(Enumerable.Empty<CheckResult>())
				{
					TextInStudiedLanguage = "Text in studied language #1".ToLanguageText(),
					OtherSynonymsInStudiedLanguage = Array.Empty<LanguageText>(),
					SynonymsInKnownLanguage = new[]
					{
						"Text in known language #1".ToLanguageText(),
						"Text in known language #2".ToLanguageText(),
						"Text in known language #3".ToLanguageText(),
					},
				},
			};

			groupedTexts.Should().BeEquivalentTo(expectedResults, x => x.WithoutStrictOrdering());
		}
	}
}
