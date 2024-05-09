using System;
using System.Linq;
using FluentAssertions;
using LanguageTutor.Models;
using LanguageTutor.ViewModels.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LanguageTutor.UnitTests.ViewModels.Extensions
{
	[TestClass]
	public class StudiedTextExtensionsTests
	{
		[TestMethod]
		public void GetTranslationsInKnownLanguage_ForSingleTranslationWithoutNote_ReturnsCorrectString()
		{
			// Arrange

			var target = new StudiedText(Enumerable.Empty<CheckResult>())
			{
				SynonymsInKnownLanguage = new[]
				{
					new LanguageText
					{
						Text = "some text",
						Note = null,
					},
				},
			};

			// Act

			var result = target.GetTranslationsInKnownLanguage();

			// Assert

			result.Should().Be("some text");
		}

		[TestMethod]
		public void GetTranslationsInKnownLanguage_ForSingleTranslationWithNote_ReturnsCorrectString()
		{
			// Arrange

			var target = new StudiedText(Enumerable.Empty<CheckResult>())
			{
				SynonymsInKnownLanguage = new[]
				{
					new LanguageText
					{
						Text = "some text",
						Note = "some note",
					},
				},
			};

			// Act

			var result = target.GetTranslationsInKnownLanguage();

			// Assert

			result.Should().Be("some text (some note)");
		}

		[TestMethod]
		public void GetTranslationsInKnownLanguage_ForMultipleTranslations_ReturnsAllTranslationsOrderedAlphabetically()
		{
			// Arrange

			var target = new StudiedText(Enumerable.Empty<CheckResult>())
			{
				SynonymsInKnownLanguage = new[]
				{
					new LanguageText
					{
						Text = "some text 3",
						Note = "some note 3",
					},

					new LanguageText
					{
						Text = "some text 1",
						Note = "some note 1",
					},

					new LanguageText
					{
						Text = "some text 2",
						Note = null,
					},
				},
			};

			// Act

			var result = target.GetTranslationsInKnownLanguage();

			// Assert

			result.Should().Be("some text 1 (some note 1), some text 2, some text 3 (some note 3)");
		}

		[TestMethod]
		public void GetTranslationsInKnownLanguage_IfNoteForSomeTranslationMatchesAnotherTranslation_OmitsThisNote()
		{
			// Arrange

			var target = new StudiedText(Enumerable.Empty<CheckResult>())
			{
				SynonymsInKnownLanguage = new[]
				{
					new LanguageText
					{
						Text = "some text 1",
						Note = "some note 1",
					},

					new LanguageText
					{
						Text = "some note 1",
						Note = "some note 2",
					},
				},
			};

			// Act

			var result = target.GetTranslationsInKnownLanguage();

			// Assert

			result.Should().Be("some note 1 (some note 2), some text 1");
		}

		[TestMethod]
		public void GetTranslationsInKnownLanguage_IfDuplicatedNotesExist_IncludesNoteOnceForLastTranslation()
		{
			// Arrange

			var target = new StudiedText(Enumerable.Empty<CheckResult>())
			{
				SynonymsInKnownLanguage = new[]
				{
					new LanguageText
					{
						Text = "some text 3",
						Note = "some note",
					},

					new LanguageText
					{
						Text = "some text 1",
						Note = "some note",
					},

					new LanguageText
					{
						Text = "some text 2",
						Note = "some note",
					},
				},
			};

			// Act

			var result = target.GetTranslationsInKnownLanguage();

			// Assert

			result.Should().Be("some text 1, some text 2, some text 3 (some note)");
		}

		[TestMethod]
		public void GetHintForOtherSynonyms_ForStudiedTextWithoutSynonyms_ReturnsEmptyString()
		{
			// Arrange

			var target = new StudiedText(Enumerable.Empty<CheckResult>())
			{
				OtherSynonymsInStudiedLanguage = Array.Empty<LanguageText>(),
			};

			// Act

			var result = target.GetHintForOtherSynonyms();

			// Assert

			result.Should().BeEmpty();
		}

		[TestMethod]
		public void GetHintForOtherSynonyms_ForStudiedTextWithSynonymsWithoutMatchingWords_ReturnsCommaSeparatedListOfSynonyms()
		{
			// Arrange

			var target = new StudiedText(Enumerable.Empty<CheckResult>())
			{
				TextInStudiedLanguage = new LanguageText { Text = "lemon" },

				OtherSynonymsInStudiedLanguage = new[]
				{
					new LanguageText { Text = "kiwi" },
					new LanguageText { Text = "apple" },
					new LanguageText { Text = "orange" },
				},
			};

			// Act

			var result = target.GetHintForOtherSynonyms();

			// Assert

			result.Should().Be("synonyms: apple, kiwi, orange");
		}

		[TestMethod]
		public void GetHintForOtherSynonyms_ForStudiedTextWithSynonymsWithMatchingWords_MasksMatchingWords()
		{
			// Arrange

			var target = new StudiedText(Enumerable.Empty<CheckResult>())
			{
				TextInStudiedLanguage = new LanguageText { Text = "very beautiful woman" },

				OtherSynonymsInStudiedLanguage = new[]
				{
					new LanguageText { Text = "very gorgeous woman" },
					new LanguageText { Text = "hot girl" },
				},
			};

			// Act

			var result = target.GetHintForOtherSynonyms();

			// Assert

			result.Should().Be("synonyms: hot girl, *** gorgeous ***");
		}
	}
}
