using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using VocabularyCoach.Interfaces;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;
using VocabularyCoach.Services.Interfaces;
using VocabularyCoach.UnitTests.Helpers;
using VocabularyCoach.ViewModels;

namespace VocabularyCoach.UnitTests.ViewModels
{
	[TestClass]
	public class CreateOrPickTextViewModelTests
	{
		private Language TestLanguage { get; } = new()
		{
			Id = new ItemId("Test Language"),
			Name = "Test Language",
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

		private ObservableCollection<LanguageTextViewModel> TestExistingLanguageTextViewModels => new(TestExistingLanguageTexts.Select(x => new LanguageTextViewModel(x)));

		[TestMethod]
		public async Task SelectedTextSetter_ForLanguageTextWithNote_UpdatesPropertiesCorrectly()
		{
			// Arrange

			var languageTextViewModel = new LanguageTextViewModel(new LanguageText
			{
				Id = new ItemId("test text id"),
				Language = TestLanguage,
				Text = "test text",
				Note = "test note",
			});

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<CreateOrPickTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			PickExistingText(target, languageTextViewModel);

			// Assert

			target.SelectedText.Should().Be(languageTextViewModel);
			target.Text.Should().Be("test text");
			target.Note.Should().Be("test note");
			target.TextWasSpellChecked.Should().BeTrue();
			target.AllowTextEdit.Should().BeFalse();
			target.AllowNoteEdit.Should().BeFalse();
		}

		[TestMethod]
		public async Task SelectedTextSetter_ForLanguageTextWithoutNote_UpdatesPropertiesCorrectly()
		{
			// Arrange

			var languageTextViewModel = new LanguageTextViewModel(new LanguageText
			{
				Id = new ItemId("test text id"),
				Language = TestLanguage,
				Text = "test text",
				Note = null,
			});

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<CreateOrPickTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			PickExistingText(target, languageTextViewModel);

			// Assert

			target.SelectedText.Should().Be(languageTextViewModel);
			target.Text.Should().Be("test text");
			target.Note.Should().Be(String.Empty);
			target.TextWasSpellChecked.Should().BeTrue();
			target.AllowTextEdit.Should().BeFalse();
			target.AllowNoteEdit.Should().BeFalse();
		}

		[TestMethod]
		public async Task SelectedTextSetter_ForNullValue_UpdatesPropertiesCorrectly()
		{
			// Arrange

			var languageTextViewModel = new LanguageTextViewModel(new LanguageText
			{
				Id = new ItemId("test text id"),
				Language = TestLanguage,
				Text = "test text",
				Note = null,
			});

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<CreateOrPickTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			PickExistingText(target, languageTextViewModel);
			PickExistingText(target, null);

			// Assert

			target.SelectedText.Should().BeNull();
			target.Text.Should().Be(String.Empty);
			target.Note.Should().Be(String.Empty);
			target.TextWasSpellChecked.Should().BeFalse();
			target.AllowTextEdit.Should().BeTrue();
			target.AllowNoteEdit.Should().BeTrue();
		}

		[TestMethod]
		public async Task SelectedTextSetter_WhenCalled_SendsCorrectUpdateEvents()
		{
			// Arrange

			var languageTextViewModel = new LanguageTextViewModel(new LanguageText
			{
				Id = new ItemId("test text id"),
				Language = TestLanguage,
				Text = "test text",
				Note = "test note",
			});

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<CreateOrPickTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			var propertyChangedEvents = new List<PropertyChangedEventArgs>();
			target.PropertyChanged += (_, e) => propertyChangedEvents.Add(e);

			// Act

			PickExistingText(target, languageTextViewModel);

			// Assert

			var expectedProperties = new[]
			{
				nameof(CreateOrPickTextViewModel.SelectedText),
				nameof(CreateOrPickTextViewModel.Text),
				nameof(CreateOrPickTextViewModel.TextWasSpellChecked),
				nameof(CreateOrPickTextViewModel.TextIsFilled),
				nameof(CreateOrPickTextViewModel.Note),
				nameof(CreateOrPickTextViewModel.AllowTextEdit),
				nameof(CreateOrPickTextViewModel.AllowNoteEdit),
			};

			propertyChangedEvents.Select(e => e.PropertyName).Distinct().Should().BeEquivalentTo(expectedProperties);
		}

		[TestMethod]
		public async Task Load_ForFirstCall_InitializesViewModelCorrectly()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<CreateOrPickTextViewModel>();

			// Act

			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Assert

			var expectedViewModelData = new CreateOrPickTextViewModelData
			{
				Language = TestLanguage,
				RequireSpellCheck = true,
				CreatePronunciationRecord = true,
				Text = String.Empty,
				TextIsFocused = false,
				TextWasSpellChecked = false,
				TextIsFilled = false,
				Note = String.Empty,
				AllowTextEdit = true,
				AllowNoteEdit = true,
				ValidationIsEnabled = false,
				HasErrors = false,
				ExistingTexts = TestExistingLanguageTextViewModels,
			};

			target.Should().BeEquivalentTo(expectedViewModelData, CreateOrPickTextViewModelData.ExcludingCommands);
		}

		[TestMethod]
		public async Task Load_ForSubsequentCall_InitializesViewModelCorrectly()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<CreateOrPickTextViewModel>();

			// Act

			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Emulating edit of text and note, and then selection of existing text, so that as many properties as possible are changed.
			target.Text = "new text";
			target.Note = "new note";
			PickExistingText(target, TestExistingLanguageTextViewModels.First());

			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Assert

			var expectedViewModelData = new CreateOrPickTextViewModelData
			{
				Language = TestLanguage,
				RequireSpellCheck = true,
				CreatePronunciationRecord = true,
				Text = String.Empty,
				TextIsFocused = false,
				TextWasSpellChecked = false,
				TextIsFilled = false,
				Note = String.Empty,
				AllowTextEdit = true,
				AllowNoteEdit = true,
				ValidationIsEnabled = false,
				HasErrors = false,
				ExistingTexts = TestExistingLanguageTextViewModels,
			};

			target.Should().BeEquivalentTo(expectedViewModelData, CreateOrPickTextViewModelData.ExcludingCommands);
		}

		[TestMethod]
		public async Task PlayPronunciationRecordCommand_IfExistingTextIsPicked_UsesExistingPronunciationRecord()
		{
			// Arrange

			var mocker = new AutoMocker();

			var pickedText = TestExistingLanguageTextViewModels.First();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var pronunciationRecord = new PronunciationRecord();
			mocker.GetMock<IVocabularyService>()
				.Setup(x => x.GetPronunciationRecord(pickedText.LanguageText.Id, It.IsAny<CancellationToken>())).ReturnsAsync(pronunciationRecord);

			var target = mocker.CreateInstance<CreateOrPickTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			PickExistingText(target, pickedText);
			await target.PlayPronunciationRecordCommand.ExecuteAsync(null);

			// Assert

			mocker.GetMock<IPronunciationRecordSynthesizer>().Verify(x => x.SynthesizePronunciationRecord(It.IsAny<Language>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
			mocker.GetMock<IPronunciationRecordPlayer>().Verify(x => x.PlayPronunciationRecord(pronunciationRecord, It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task PlayPronunciationRecordCommand_ForNewText_SynthesizesNewPronunciationRecord()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var synthesizedPronunciationRecord = new PronunciationRecord();
			var pronunciationRecordSynthesizerMock = mocker.GetMock<IPronunciationRecordSynthesizer>();
			pronunciationRecordSynthesizerMock.Setup(x => x.SynthesizePronunciationRecord(TestLanguage, "new text", It.IsAny<CancellationToken>())).ReturnsAsync(synthesizedPronunciationRecord);

			var target = mocker.CreateInstance<CreateOrPickTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = "new text";
			await target.PlayPronunciationRecordCommand.ExecuteAsync(null);

			// Assert

			pronunciationRecordSynthesizerMock.Verify(x => x.SynthesizePronunciationRecord(TestLanguage, "new text", It.IsAny<CancellationToken>()), Times.Once);
			mocker.GetMock<IPronunciationRecordPlayer>().Verify(x => x.PlayPronunciationRecord(synthesizedPronunciationRecord, It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task SaveChanges_IfExistingTextIsPicked_DoesNotCreateNewLanguageText()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<CreateOrPickTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			// Emulating edit of text and note, and then selection of existing text.
			target.Text = "new text";
			PickExistingText(target, TestExistingLanguageTextViewModels.First());

			target.ValidationIsEnabled = true;
			target.HasErrors.Should().BeFalse();
			var languageText = await target.SaveChanges(CancellationToken.None);

			// Assert

			mocker.GetMock<IEditVocabularyService>()
				.Verify(x => x.AddLanguageText(It.IsAny<LanguageTextData>(), It.IsAny<CancellationToken>()), Times.Never);

			languageText.Should().BeEquivalentTo(TestExistingLanguageTextViewModels.First().LanguageText);
		}

		[TestMethod]
		public async Task SaveChanges_ForNewTextWhenCreatePronunciationRecordIsTrue_CreatesNewLanguageTextWithPronunciationRecord()
		{
			// Arrange

			var mocker = new AutoMocker();

			var editVocabularyServiceMock = mocker.GetMock<IEditVocabularyService>();

			var newLanguageText = new LanguageText
			{
				Id = new("New Language Text"),
			};

			editVocabularyServiceMock.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);
			editVocabularyServiceMock.Setup(x => x.AddLanguageText(It.IsAny<LanguageTextData>(), It.IsAny<CancellationToken>())).ReturnsAsync(newLanguageText);

			mocker.GetMock<ISpellCheckService>().Setup(x => x.PerformSpellCheck(It.IsAny<LanguageText>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

			var synthesizedPronunciationRecord = new PronunciationRecord();

			mocker.GetMock<IPronunciationRecordSynthesizer>()
				.Setup(x => x.SynthesizePronunciationRecord(TestLanguage, "new text", It.IsAny<CancellationToken>()))
				.ReturnsAsync(synthesizedPronunciationRecord);

			var target = mocker.CreateInstance<CreateOrPickTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

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

			editVocabularyServiceMock.Verify(x => x.AddLanguageText(It.Is<LanguageTextData>(data => verifyLanguageTextData(data)), It.IsAny<CancellationToken>()), Times.Once);

			returnedLanguageText.Should().Be(newLanguageText);

			target.ExistingTexts.Should().Contain(x => x.LanguageText.Id == new ItemId("New Language Text"));
		}

		[TestMethod]
		public async Task SaveChanges_ForNewTextWhenCreatePronunciationRecordIsTrueAndPronunciationRecordIsMissing_Throws()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			mocker.GetMock<ISpellCheckService>().Setup(x => x.PerformSpellCheck(It.IsAny<LanguageText>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

			var target = mocker.CreateInstance<CreateOrPickTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

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
		public async Task SaveChanges_ForNewTextWhenCreatePronunciationRecordIsFalse_CreatesNewLanguageTextWithoutPronunciationRecord()
		{
			// Arrange

			var mocker = new AutoMocker();

			var editVocabularyServiceMock = mocker.GetMock<IEditVocabularyService>();

			var newLanguageText = new LanguageText
			{
				Id = new("New Language Text"),
			};

			editVocabularyServiceMock.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);
			editVocabularyServiceMock.Setup(x => x.AddLanguageText(It.IsAny<LanguageTextData>(), It.IsAny<CancellationToken>())).ReturnsAsync(newLanguageText);

			var target = mocker.CreateInstance<CreateOrPickTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: false, createPronunciationRecord: false, CancellationToken.None);

			// Act

			target.Text = "new text";
			target.Note = "new note";

			target.ValidationIsEnabled = true;
			target.HasErrors.Should().BeFalse();
			var languageText = await target.SaveChanges(CancellationToken.None);

			// Assert

			Func<LanguageTextData, bool> verifyLanguageTextData = languageTextData =>
				languageTextData.Language == TestLanguage && languageTextData.Text == "new text" &&
				languageTextData.Note == "new note" &&
				languageTextData.PronunciationRecord == null;

			editVocabularyServiceMock.Verify(x => x.AddLanguageText(It.Is<LanguageTextData>(data => verifyLanguageTextData(data)), It.IsAny<CancellationToken>()), Times.Once);

			languageText.Should().BeEquivalentTo(newLanguageText);

			target.ExistingTexts.Should().Contain(x => x.LanguageText.Id == new ItemId("New Language Text"));
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

			// Emulating edit of text and note, and then selection of existing text, so that as many properties as possible are changed.
			target.Text = "new text";
			target.Note = "new note";
			PickExistingText(target, TestExistingLanguageTextViewModels.First());

			// Act

			target.ClearFilledData();

			// Assert

			var expectedViewModelData = new CreateOrPickTextViewModelData
			{
				Language = TestLanguage,
				RequireSpellCheck = true,
				CreatePronunciationRecord = true,
				Text = String.Empty,
				TextIsFocused = false,
				TextWasSpellChecked = false,
				TextIsFilled = false,
				Note = String.Empty,
				AllowTextEdit = true,
				AllowNoteEdit = true,
				ValidationIsEnabled = false,
				HasErrors = false,
				ExistingTexts = new ObservableCollection<LanguageTextViewModel>(TestExistingLanguageTextViewModels),
			};

			target.Should().BeEquivalentTo(expectedViewModelData, CreateOrPickTextViewModelData.ExcludingCommands);
		}

		[TestMethod]
		public async Task GetErrors_ForTextPropertyWhenExistingTextIsPicked_ReturnsNoError()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<CreateOrPickTextViewModel>();

			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);
			PickExistingText(target, TestExistingLanguageTextViewModels.First());

			// Act

			target.ValidationIsEnabled = true;
			var errors = target.GetErrors(nameof(target.Text));

			// Assert

			errors.Cast<string>().Should().BeEmpty();
		}

		private static void PickExistingText(CreateOrPickTextViewModel target, LanguageTextViewModel languageText)
		{
			// Emulating same sequence of setters as used by ComboBox.
			target.SelectedText = languageText;

			if (languageText != null)
			{
				target.Text = languageText.ToString();
			}
		}
	}
}
