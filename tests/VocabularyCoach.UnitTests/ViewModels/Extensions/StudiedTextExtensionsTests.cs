using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocabularyCoach.Models;
using VocabularyCoach.ViewModels.Extensions;

namespace VocabularyCoach.UnitTests.ViewModels.Extensions
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
	}
}
