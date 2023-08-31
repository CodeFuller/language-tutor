using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;
using VocabularyCoach.Services.Interfaces;
using VocabularyCoach.UnitTests.Helpers;
using VocabularyCoach.ViewModels;

namespace VocabularyCoach.UnitTests.ViewModels
{
	[TestClass]
	public class EditExistingTextViewModelTests
	{
		private Language TestLanguage { get; } = new()
		{
			Id = new ItemId("Test Language"),
			Name = "Test Language",
		};

		private LanguageText TestEditedLanguageText => new()
		{
			Id = new ItemId("test text id"),
			Language = TestLanguage,
			Text = "test text",
			Note = "test note",
		};

		private IReadOnlyCollection<LanguageText> TestExistingLanguageTexts => new[]
		{
			new LanguageText
			{
				Id = new ItemId("test text id 1"),
				Language = TestLanguage,
				Text = "test text 1",
				Note = "test note 1",
			},

			new LanguageText
			{
				Id = new ItemId("test text id 2"),
				Language = TestLanguage,
				Text = "test text 2",
				Note = "test note 2",
			},
		};

		[TestMethod]
		public async Task Load_ForFirstCall_InitializesViewModelCorrectly()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<EditExistingTextViewModel>();

			// Act

			await target.Load(TestEditedLanguageText, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Assert

			var expectedViewModelData = new EditExistingTextViewModelData
			{
				Language = TestLanguage,
				RequireSpellCheck = true,
				CreatePronunciationRecord = true,
				Text = "test text",
				TextIsFocused = false,
				TextWasSpellChecked = true,
				TextIsFilled = true,
				Note = "test note",
				AllowNoteEdit = true,
				ValidationIsEnabled = false,
				HasErrors = false,
			};

			target.Should().BeEquivalentTo(expectedViewModelData, EditExistingTextViewModelData.ExcludingCommands);
		}

		[TestMethod]
		public async Task Load_ForSubsequentCall_InitializesViewModelCorrectly()
		{
			// Arrange

			var languageText2 = new LanguageText
			{
				Id = new ItemId("test text 2 id"),
				Language = TestLanguage,
				Text = "test text 2",
				Note = "test note 2",
			};

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<EditExistingTextViewModel>();

			await target.Load(TestEditedLanguageText, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			await target.Load(languageText2, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Assert

			var expectedViewModelData = new EditExistingTextViewModelData
			{
				Language = TestLanguage,
				RequireSpellCheck = true,
				CreatePronunciationRecord = true,
				Text = "test text 2",
				TextIsFocused = false,
				TextWasSpellChecked = true,
				TextIsFilled = true,
				Note = "test note 2",
				AllowNoteEdit = true,
				ValidationIsEnabled = false,
				HasErrors = false,
			};

			target.Should().BeEquivalentTo(expectedViewModelData, EditExistingTextViewModelData.ExcludingCommands);
		}

		[TestMethod]
		public async Task SaveChanges_WhenCreatePronunciationRecordIsTrueAndPronunciationRecordWasChanged_UpdateLanguageTextWithNewPronunciationRecord()
		{
			// Arrange

			var mocker = new AutoMocker();

			var editVocabularyServiceMock = mocker.GetMock<IEditVocabularyService>();

			// Saving TestEditedLanguageText to local variable, because new instance is returned on each call, which fails Mock.Verify().
			var originalLanguageText = TestEditedLanguageText;

			var updatedLanguageText = new LanguageText
			{
				Id = new("Updated Language Text"),
			};

			editVocabularyServiceMock.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);
			editVocabularyServiceMock.Setup(x => x.UpdateLanguageText(It.IsAny<LanguageText>(), It.IsAny<LanguageTextData>(), It.IsAny<CancellationToken>())).ReturnsAsync(updatedLanguageText);

			mocker.GetMock<ISpellCheckService>().Setup(x => x.PerformSpellCheck(It.IsAny<LanguageText>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

			var synthesizedPronunciationRecord = new PronunciationRecord();

			mocker.GetMock<IPronunciationRecordSynthesizer>()
				.Setup(x => x.SynthesizePronunciationRecord(TestLanguage, "new text", It.IsAny<CancellationToken>()))
				.ReturnsAsync(synthesizedPronunciationRecord);

			var target = mocker.CreateInstance<EditExistingTextViewModel>();
			await target.Load(originalLanguageText, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = "new text";
			target.Note = "new note";
			await target.SpellCheckTextCommand.ExecuteAsync(null);

			target.ValidationIsEnabled = true;
			target.HasErrors.Should().BeFalse();
			var returnedLanguageText = await target.SaveChanges(CancellationToken.None);

			// Assert

			Func<LanguageTextData, bool> verifyLanguageTextData = languageTextData =>
				languageTextData.Language == TestLanguage && languageTextData.Text == "new text" &&
				languageTextData.Note == "new note" &&
				languageTextData.PronunciationRecord == synthesizedPronunciationRecord;

			editVocabularyServiceMock.Verify(x => x.UpdateLanguageText(originalLanguageText, It.Is<LanguageTextData>(data => verifyLanguageTextData(data)), It.IsAny<CancellationToken>()), Times.Once);

			returnedLanguageText.Should().Be(updatedLanguageText);
		}

		[TestMethod]
		public async Task SaveChanges_WhenCreatePronunciationRecordIsTrueAndPronunciationRecordWasNotChanged_UpdateLanguageTextWithoutPronunciationRecord()
		{
			// Arrange

			var mocker = new AutoMocker();

			var editVocabularyServiceMock = mocker.GetMock<IEditVocabularyService>();

			// Saving TestEditedLanguageText to local variable, because new instance is returned on each call, which fails Mock.Verify().
			var originalLanguageText = TestEditedLanguageText;
			var originalPronunciationRecord = new PronunciationRecord();

			var updatedLanguageText = new LanguageText
			{
				Id = new("Updated Language Text"),
			};

			editVocabularyServiceMock.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);
			editVocabularyServiceMock.Setup(x => x.UpdateLanguageText(It.IsAny<LanguageText>(), It.IsAny<LanguageTextData>(), It.IsAny<CancellationToken>())).ReturnsAsync(updatedLanguageText);

			mocker.GetMock<IVocabularyService>()
				.Setup(x => x.GetPronunciationRecord(originalLanguageText.Id, It.IsAny<CancellationToken>())).ReturnsAsync(originalPronunciationRecord);

			editVocabularyServiceMock.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<EditExistingTextViewModel>();
			await target.Load(originalLanguageText, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Note = "new note";

			target.ValidationIsEnabled = true;
			target.HasErrors.Should().BeFalse();
			var returnedLanguageText = await target.SaveChanges(CancellationToken.None);

			// Assert

			Func<LanguageTextData, bool> verifyLanguageTextData = languageTextData =>
				languageTextData.Language == TestLanguage && languageTextData.Text == originalLanguageText.Text &&
				languageTextData.Note == "new note" &&
				languageTextData.PronunciationRecord == null;

			editVocabularyServiceMock.Verify(x => x.UpdateLanguageText(originalLanguageText, It.Is<LanguageTextData>(data => verifyLanguageTextData(data)), It.IsAny<CancellationToken>()), Times.Once);

			returnedLanguageText.Should().Be(updatedLanguageText);
		}

		[TestMethod]
		public async Task SaveChanges_WhenCreatePronunciationRecordIsTrueAndPronunciationRecordIsMissing_Throws()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			mocker.GetMock<ISpellCheckService>().Setup(x => x.PerformSpellCheck(It.IsAny<LanguageText>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

			var target = mocker.CreateInstance<EditExistingTextViewModel>();
			await target.Load(TestEditedLanguageText, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = "new text";
			target.Note = "new note";
			await target.SpellCheckTextCommand.ExecuteAsync(null);

			target.ValidationIsEnabled = true;
			target.HasErrors.Should().BeFalse();
			var call = () => target.SaveChanges(CancellationToken.None);

			// Assert

			await call.Should().ThrowAsync<InvalidOperationException>().WithMessage("Pronunciation record is missing");
		}

		[TestMethod]
		public async Task SaveChanges_WhenNoteForTextWithoutDuplicatesIsCleared_SavesChangesCorrectly()
		{
			// Arrange

			var testEditedLanguageText = new LanguageText
			{
				Id = new ItemId("test text id"),
				Language = TestLanguage,
				Text = "test text",
				Note = "test note",
			};

			var mocker = new AutoMocker();

			var editVocabularyServiceMock = mocker.GetMock<IEditVocabularyService>();
			editVocabularyServiceMock.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(new[] { testEditedLanguageText });

			var target = mocker.CreateInstance<EditExistingTextViewModel>();
			await target.Load(testEditedLanguageText, requireSpellCheck: false, createPronunciationRecord: false, CancellationToken.None);

			// Act

			target.Note = String.Empty;

			target.ValidationIsEnabled = true;
			await target.SaveChanges(CancellationToken.None);

			// Assert

			Func<LanguageTextData, bool> verifyLanguageTextData = languageTextData =>
				languageTextData.Language == TestLanguage && languageTextData.Text == testEditedLanguageText.Text &&
				languageTextData.Note.Length == 0 &&
				languageTextData.PronunciationRecord == null;

			editVocabularyServiceMock.Verify(x => x.UpdateLanguageText(testEditedLanguageText, It.Is<LanguageTextData>(data => verifyLanguageTextData(data)), It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task SaveChanges_WhenLetterCaseForTextWithoutDuplicatesIsChanged_SavesChangesCorrectly()
		{
			// Arrange

			var testEditedLanguageText = new LanguageText
			{
				Id = new ItemId("test text id"),
				Language = TestLanguage,
				Text = "test text",
				Note = "test note",
			};

			var mocker = new AutoMocker();

			var editVocabularyServiceMock = mocker.GetMock<IEditVocabularyService>();
			editVocabularyServiceMock.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(new[] { testEditedLanguageText });

			var target = mocker.CreateInstance<EditExistingTextViewModel>();
			await target.Load(testEditedLanguageText, requireSpellCheck: false, createPronunciationRecord: false, CancellationToken.None);

			// Act

			target.Text = "Test Text";

			target.ValidationIsEnabled = true;
			await target.SaveChanges(CancellationToken.None);

			// Assert

			Func<LanguageTextData, bool> verifyLanguageTextData = languageTextData =>
				languageTextData.Language == TestLanguage && languageTextData.Text == "Test Text" &&
				languageTextData.Note == testEditedLanguageText.Note &&
				languageTextData.PronunciationRecord == null;

			editVocabularyServiceMock.Verify(x => x.UpdateLanguageText(testEditedLanguageText, It.Is<LanguageTextData>(data => verifyLanguageTextData(data)), It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task ClearFilledData_ClearsViewModelProperties()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<CreateOrPickTextViewModel>();

			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			target.Text = "new text";
			target.Note = "new note";

			// Act

			target.ClearFilledData();

			// Assert

			var expectedViewModelData = new EditExistingTextViewModelData
			{
				Language = TestLanguage,
				RequireSpellCheck = true,
				CreatePronunciationRecord = true,
				Text = String.Empty,
				TextIsFocused = false,
				TextWasSpellChecked = false,
				TextIsFilled = false,
				Note = String.Empty,
				AllowNoteEdit = true,
				ValidationIsEnabled = false,
				HasErrors = false,
			};

			target.Should().BeEquivalentTo(expectedViewModelData, EditExistingTextViewModelData.ExcludingCommands);
		}
	}
}
