using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using VocabularyCoach.Events;
using VocabularyCoach.Interfaces;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Interfaces;
using VocabularyCoach.UnitTests.Helpers;
using VocabularyCoach.ViewModels;

namespace VocabularyCoach.UnitTests.ViewModels
{
	[TestClass]
	public class BasicEditTextViewModelTests
	{
		private sealed class TestEditTextViewModel : BasicEditTextViewModel
		{
			public override bool AllowTextEdit => true;

			public override bool AllowNoteEdit => true;

			public PronunciationRecord PronunciationRecordAccessor => PronunciationRecord;

			public TestEditTextViewModel(IVocabularyService vocabularyService, IEditVocabularyService editVocabularyService, ISpellCheckService spellCheckService,
				IPronunciationRecordSynthesizer pronunciationRecordSynthesizer, IPronunciationRecordPlayer pronunciationRecordPlayer, IMessenger messenger)
				: base(vocabularyService, editVocabularyService, spellCheckService, pronunciationRecordSynthesizer, pronunciationRecordPlayer, messenger)
			{
			}

			public Task Load(Language language, bool requireSpellCheck, bool createPronunciationRecord, CancellationToken cancellationToken)
			{
				return LoadData(language, requireSpellCheck, createPronunciationRecord, cancellationToken);
			}

			protected override Task<LanguageText> SaveLanguageText(CancellationToken cancellationToken)
			{
				throw new NotImplementedException();
			}

			protected override void OnTextPropertyChanged()
			{
			}
		}

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

		[TestMethod]
		public async Task TextSetter_ResetsTextWasSpellCheckedProperty()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			mocker.GetMock<ISpellCheckService>()
				.Setup(x => x.PerformSpellCheck(It.IsAny<LanguageText>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			target.Text = "new";
			await target.SpellCheckTextCommand.ExecuteAsync(null);

			// Sanity check.
			target.TextWasSpellChecked.Should().BeTrue();

			// Act

			target.Text += " text";

			// Assert

			target.TextWasSpellChecked.Should().BeFalse();
		}

		[TestMethod]
		public async Task TextSetter_ResetsPronunciationRecordProperty()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var synthesizedPronunciationRecord = new PronunciationRecord();
			var pronunciationRecordSynthesizerMock = mocker.GetMock<IPronunciationRecordSynthesizer>();
			pronunciationRecordSynthesizerMock.Setup(x => x.SynthesizePronunciationRecord(It.IsAny<Language>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(synthesizedPronunciationRecord);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			target.Text = "new";
			await target.PlayPronunciationRecordCommand.ExecuteAsync(null);

			// Sanity check.
			target.PronunciationRecordAccessor.Should().NotBeNull();

			// Act

			target.Text += " text";

			// Assert

			target.PronunciationRecordAccessor.Should().BeNull();
		}

		[TestMethod]
		public async Task SpellCheckTextCommand_IfRequireSpellCheckIsFalse_ThrowsWithoutSpellCheck()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: false, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = "new text";
			var call = () => target.SpellCheckTextCommand.ExecuteAsync(null);

			// Assert

			await call.Should().ThrowAsync<InvalidOperationException>().WithMessage("Spell check is disabled for current ViewModel");

			mocker.GetMock<ISpellCheckService>().Verify(x => x.PerformSpellCheck(It.IsAny<LanguageText>(), It.IsAny<CancellationToken>()), Times.Never);

			mocker.GetMock<IMessenger>().Verify(x => x.Send(It.IsAny<EditedTextSpellCheckedEventArgs>(), It.IsAny<IsAnyToken>()), Times.Never);
		}

		[TestMethod]
		public async Task SpellCheckTextCommand_IfSpellCheckFails_ReturnsWithoutSpellCheckAndSynthesisOfPronunciationRecord()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			mocker.GetMock<ISpellCheckService>()
				.Setup(x => x.PerformSpellCheck(It.IsAny<LanguageText>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = "new text";
			await target.SpellCheckTextCommand.ExecuteAsync(null);

			// Assert

			target.TextWasSpellChecked.Should().BeFalse();

			mocker.GetMock<IPronunciationRecordSynthesizer>().Verify(x => x.SynthesizePronunciationRecord(It.IsAny<Language>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
			mocker.GetMock<IPronunciationRecordPlayer>().Verify(x => x.PlayPronunciationRecord(It.IsAny<PronunciationRecord>(), It.IsAny<CancellationToken>()), Times.Never);

			mocker.GetMock<IMessenger>().Verify(x => x.Send(It.IsAny<EditedTextSpellCheckedEventArgs>(), It.IsAny<IsAnyToken>()), Times.Never);
		}

		[TestMethod]
		public async Task SpellCheckTextCommand_IfCreatePronunciationRecordIsFalse_DoesNotSynthesizePronunciationRecord()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			mocker.GetMock<ISpellCheckService>()
				.Setup(x => x.PerformSpellCheck(It.IsAny<LanguageText>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: false, CancellationToken.None);

			// Act

			target.Text = "new text";
			await target.SpellCheckTextCommand.ExecuteAsync(null);

			// Assert

			target.TextWasSpellChecked.Should().BeTrue();

			mocker.GetMock<IPronunciationRecordSynthesizer>().Verify(x => x.SynthesizePronunciationRecord(It.IsAny<Language>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
			mocker.GetMock<IPronunciationRecordPlayer>().Verify(x => x.PlayPronunciationRecord(It.IsAny<PronunciationRecord>(), It.IsAny<CancellationToken>()), Times.Never);

			mocker.GetMock<IMessenger>().Verify(x => x.Send(It.IsAny<EditedTextSpellCheckedEventArgs>(), It.IsAny<IsAnyToken>()), Times.Once);
		}

		[TestMethod]
		public async Task SpellCheckTextCommand_IfCreatePronunciationRecordIsTrue_SynthesizesAndPlaysPronunciationRecord()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			mocker.GetMock<ISpellCheckService>()
				.Setup(x => x.PerformSpellCheck(It.IsAny<LanguageText>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

			var synthesizedPronunciationRecord = new PronunciationRecord();
			var pronunciationRecordSynthesizerMock = mocker.GetMock<IPronunciationRecordSynthesizer>();
			pronunciationRecordSynthesizerMock.Setup(x => x.SynthesizePronunciationRecord(TestLanguage, "new text", It.IsAny<CancellationToken>())).ReturnsAsync(synthesizedPronunciationRecord);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = "new text";
			await target.SpellCheckTextCommand.ExecuteAsync(null);

			// Assert

			target.TextWasSpellChecked.Should().BeTrue();

			pronunciationRecordSynthesizerMock.Verify(x => x.SynthesizePronunciationRecord(TestLanguage, "new text", It.IsAny<CancellationToken>()), Times.Once);
			mocker.GetMock<IPronunciationRecordPlayer>().Verify(x => x.PlayPronunciationRecord(synthesizedPronunciationRecord, It.IsAny<CancellationToken>()), Times.Once);

			mocker.GetMock<IMessenger>().Verify(x => x.Send(It.IsAny<EditedTextSpellCheckedEventArgs>(), It.IsAny<IsAnyToken>()), Times.Once);
		}

		[TestMethod]
		public async Task ProcessPastedTextCommand_IfRequireSpellCheckIsFalse_ReturnsWithoutSpellCheck()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: false, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = "new text";
			await target.ProcessPastedTextCommand.ExecuteAsync(null);

			// Assert

			mocker.GetMock<ISpellCheckService>().Verify(x => x.PerformSpellCheck(It.IsAny<LanguageText>(), It.IsAny<CancellationToken>()), Times.Never);

			mocker.GetMock<IMessenger>().Verify(x => x.Send(It.IsAny<EditedTextSpellCheckedEventArgs>(), It.IsAny<IsAnyToken>()), Times.Never);
		}

		[TestMethod]
		public async Task ProcessPastedTextCommand_IfSpellCheckFails_ReturnsWithoutSpellCheckAndSynthesisOfPronunciationRecord()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			mocker.GetMock<ISpellCheckService>()
				.Setup(x => x.PerformSpellCheck(It.IsAny<LanguageText>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = "new text";
			await target.ProcessPastedTextCommand.ExecuteAsync(null);

			// Assert

			target.TextWasSpellChecked.Should().BeFalse();

			mocker.GetMock<IPronunciationRecordSynthesizer>().Verify(x => x.SynthesizePronunciationRecord(It.IsAny<Language>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
			mocker.GetMock<IPronunciationRecordPlayer>().Verify(x => x.PlayPronunciationRecord(It.IsAny<PronunciationRecord>(), It.IsAny<CancellationToken>()), Times.Never);

			mocker.GetMock<IMessenger>().Verify(x => x.Send(It.IsAny<EditedTextSpellCheckedEventArgs>(), It.IsAny<IsAnyToken>()), Times.Never);
		}

		[TestMethod]
		public async Task ProcessPastedTextCommand_IfCreatePronunciationRecordIsFalse_DoesNotSynthesizePronunciationRecord()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			mocker.GetMock<ISpellCheckService>()
				.Setup(x => x.PerformSpellCheck(It.IsAny<LanguageText>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: false, CancellationToken.None);

			// Act

			target.Text = "new text";
			await target.ProcessPastedTextCommand.ExecuteAsync(null);

			// Assert

			target.TextWasSpellChecked.Should().BeTrue();

			mocker.GetMock<IPronunciationRecordSynthesizer>().Verify(x => x.SynthesizePronunciationRecord(It.IsAny<Language>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
			mocker.GetMock<IPronunciationRecordPlayer>().Verify(x => x.PlayPronunciationRecord(It.IsAny<PronunciationRecord>(), It.IsAny<CancellationToken>()), Times.Never);

			mocker.GetMock<IMessenger>().Verify(x => x.Send(It.IsAny<EditedTextSpellCheckedEventArgs>(), It.IsAny<IsAnyToken>()), Times.Once);
		}

		[TestMethod]
		public async Task ProcessPastedTextCommand_IfCreatePronunciationRecordIsTrue_SynthesizesAndPlaysPronunciationRecord()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			mocker.GetMock<ISpellCheckService>()
				.Setup(x => x.PerformSpellCheck(It.IsAny<LanguageText>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

			var synthesizedPronunciationRecord = new PronunciationRecord();
			var pronunciationRecordSynthesizerMock = mocker.GetMock<IPronunciationRecordSynthesizer>();
			pronunciationRecordSynthesizerMock.Setup(x => x.SynthesizePronunciationRecord(TestLanguage, "new text", It.IsAny<CancellationToken>())).ReturnsAsync(synthesizedPronunciationRecord);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = "new text";
			await target.ProcessPastedTextCommand.ExecuteAsync(null);

			// Assert

			target.TextWasSpellChecked.Should().BeTrue();

			pronunciationRecordSynthesizerMock.Verify(x => x.SynthesizePronunciationRecord(TestLanguage, "new text", It.IsAny<CancellationToken>()), Times.Once);
			mocker.GetMock<IPronunciationRecordPlayer>().Verify(x => x.PlayPronunciationRecord(synthesizedPronunciationRecord, It.IsAny<CancellationToken>()), Times.Once);

			mocker.GetMock<IMessenger>().Verify(x => x.Send(It.IsAny<EditedTextSpellCheckedEventArgs>(), It.IsAny<IsAnyToken>()), Times.Once);
		}

		[TestMethod]
		public async Task PlayPronunciationRecordCommand_IfCreatePronunciationRecordIsFalse_ThrowsWithoutSynthesisAndPlayOfPronunciationRecord()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: false, CancellationToken.None);

			// Act

			target.Text = "new text";
			var call = () => target.PlayPronunciationRecordCommand.ExecuteAsync(null);

			// Assert

			await call.Should().ThrowAsync<InvalidOperationException>().WithMessage("Pronunciation record is disabled for current ViewModel");

			mocker.GetMock<IPronunciationRecordSynthesizer>().Verify(x => x.SynthesizePronunciationRecord(It.IsAny<Language>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
			mocker.GetMock<IPronunciationRecordPlayer>().Verify(x => x.PlayPronunciationRecord(It.IsAny<PronunciationRecord>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task PlayPronunciationRecordCommand_ForFirstCall_SynthesizesAndPlaysPronunciationRecord()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var synthesizedPronunciationRecord = new PronunciationRecord();
			var pronunciationRecordSynthesizerMock = mocker.GetMock<IPronunciationRecordSynthesizer>();
			pronunciationRecordSynthesizerMock.Setup(x => x.SynthesizePronunciationRecord(TestLanguage, "new text", It.IsAny<CancellationToken>())).ReturnsAsync(synthesizedPronunciationRecord);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = "new text";
			await target.PlayPronunciationRecordCommand.ExecuteAsync(null);

			// Assert

			pronunciationRecordSynthesizerMock.Verify(x => x.SynthesizePronunciationRecord(TestLanguage, "new text", It.IsAny<CancellationToken>()), Times.Once);
			mocker.GetMock<IPronunciationRecordPlayer>().Verify(x => x.PlayPronunciationRecord(synthesizedPronunciationRecord, It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task PlayPronunciationRecordCommand_ForSubsequentCall_UsesSynthesizedPronunciationRecord()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var synthesizedPronunciationRecord = new PronunciationRecord();
			var pronunciationRecordSynthesizerMock = mocker.GetMock<IPronunciationRecordSynthesizer>();
			pronunciationRecordSynthesizerMock.Setup(x => x.SynthesizePronunciationRecord(TestLanguage, "new text", It.IsAny<CancellationToken>())).ReturnsAsync(synthesizedPronunciationRecord);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = "new text";
			await target.PlayPronunciationRecordCommand.ExecuteAsync(null);
			await target.PlayPronunciationRecordCommand.ExecuteAsync(null);

			// Assert

			pronunciationRecordSynthesizerMock.Verify(x => x.SynthesizePronunciationRecord(TestLanguage, "new text", It.IsAny<CancellationToken>()), Times.Once);
			mocker.GetMock<IPronunciationRecordPlayer>().Verify(x => x.PlayPronunciationRecord(synthesizedPronunciationRecord, It.IsAny<CancellationToken>()), Times.Exactly(2));
		}

		[TestMethod]
		public async Task PlayPronunciationRecordCommand_IfCalledAfterSpellCheck_UsesSynthesizedPronunciationRecord()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			mocker.GetMock<ISpellCheckService>()
				.Setup(x => x.PerformSpellCheck(It.IsAny<LanguageText>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

			var synthesizedPronunciationRecord = new PronunciationRecord();
			var pronunciationRecordSynthesizerMock = mocker.GetMock<IPronunciationRecordSynthesizer>();
			pronunciationRecordSynthesizerMock.Setup(x => x.SynthesizePronunciationRecord(TestLanguage, "new text", It.IsAny<CancellationToken>())).ReturnsAsync(synthesizedPronunciationRecord);

			var pronunciationRecordPlayerMock = mocker.GetMock<IPronunciationRecordPlayer>();

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = "new text";
			await target.SpellCheckTextCommand.ExecuteAsync(null);

			pronunciationRecordSynthesizerMock.Invocations.Clear();
			pronunciationRecordPlayerMock.Invocations.Clear();

			await target.PlayPronunciationRecordCommand.ExecuteAsync(null);

			// Assert

			pronunciationRecordSynthesizerMock.Verify(x => x.SynthesizePronunciationRecord(It.IsAny<Language>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
			pronunciationRecordPlayerMock.Verify(x => x.PlayPronunciationRecord(synthesizedPronunciationRecord, It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task SaveChanges_IfValidationIsNotEnabled_Throws()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = "new text";

			var call = () => target.SaveChanges(CancellationToken.None);

			// Assert

			await call.Should().ThrowAsync<InvalidOperationException>().WithMessage("Validation should be enabled before saving changes");
		}

		[TestMethod]
		public async Task SaveChanges_IfThereAreValidationErrors_Throws()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			// Setting text without spell check will trigger validation error.
			target.Text = "new text";

			target.ValidationIsEnabled = true;
			target.HasErrors.Should().BeTrue();
			var call = () => target.SaveChanges(CancellationToken.None);

			// Assert

			await call.Should().ThrowAsync<InvalidOperationException>().WithMessage("ViewModel has validation errors");
		}

		[TestMethod]
		public async Task GetErrors_ForTextPropertyIfTextIsEmpty_ReturnsCorrectError()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.ValidationIsEnabled = true;
			var validationErrors = target.GetErrors(nameof(target.Text)).Cast<string>();

			// Assert

			validationErrors.Single().Should().Be("Please type text");
		}

		[DataTestMethod]
		[DataRow(" new text", "Please remove leading whitespaces")]
		[DataRow("\tnew text", "Please remove leading whitespaces")]
		[DataRow("new text ", "Please remove trailing whitespaces")]
		[DataRow("new text\t", "Please remove trailing whitespaces")]
		[DataRow("new  text", "Please remove duplicated whitespaces")]
		[DataRow("new\t\ttext", "Please remove duplicated whitespaces")]
		public async Task GetErrors_ForTextPropertyIfTextContentIsInvalid_ReturnsCorrectError(string textValue, string expectedValidationError)
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = textValue;

			target.ValidationIsEnabled = true;
			var validationErrors = target.GetErrors(nameof(target.Text)).Cast<string>();

			// Assert

			validationErrors.Single().Should().Be(expectedValidationError);
		}

		[TestMethod]
		public async Task GetErrors_ForTextPropertyWhenRequireSpellCheckIsTrueAndTextWasNotSpellChecked_ReturnsCorrectError()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = "new text";

			target.ValidationIsEnabled = true;
			var validationErrors = target.GetErrors(nameof(target.Text)).Cast<string>();

			// Assert

			validationErrors.Single().Should().Be("Please perform text spell check");
		}

		[TestMethod]
		public async Task GetErrors_ForTextPropertyWhenRequireSpellCheckIsTrueAndTextWasSpellChecked_ReturnsNoError()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			mocker.GetMock<ISpellCheckService>()
				.Setup(x => x.PerformSpellCheck(It.IsAny<LanguageText>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = "new text";
			await target.SpellCheckTextCommand.ExecuteAsync(null);

			target.ValidationIsEnabled = true;
			var validationErrors = target.GetErrors(nameof(target.Text)).Cast<string>();

			// Assert

			validationErrors.Should().BeEmpty();
		}

		[TestMethod]
		public async Task GetErrors_ForTextPropertyWhenRequireSpellCheckIsFalseAndTextWasNotSpellChecked_ReturnsNoError()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: false, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = "new text";

			target.ValidationIsEnabled = true;
			var validationErrors = target.GetErrors(nameof(target.Text)).Cast<string>();

			// Assert

			validationErrors.Should().BeEmpty();
		}

		[TestMethod]
		public async Task GetErrors_ForTextPropertyWhenNoteIsNotFilledAndSameTextExists_ReturnsCorrectError()
		{
			// Arrange

			var mocker = new AutoMocker();

			var existingText = TestExistingLanguageTexts.Last();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			mocker.GetMock<ISpellCheckService>()
				.Setup(x => x.PerformSpellCheck(It.IsAny<LanguageText>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = existingText.Text;
			await target.SpellCheckTextCommand.ExecuteAsync(null);

			target.ValidationIsEnabled = true;
			var validationErrors = target.GetErrors(nameof(target.Text)).Cast<string>();

			// Assert

			validationErrors.Single().Should().Be("Same text in Test Language language already exists. Either use existing text or provide a note to distinguish the texts");
		}

		[TestMethod]
		public async Task GetErrors_ForTextPropertyWhenNoteIsFilledAndSameTextWithSameNoteExists_ReturnsCorrectError()
		{
			// Arrange

			var mocker = new AutoMocker();

			var existingText = TestExistingLanguageTexts.Last();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			mocker.GetMock<ISpellCheckService>()
				.Setup(x => x.PerformSpellCheck(It.IsAny<LanguageText>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = existingText.Text;
			target.Note = existingText.Note;
			await target.SpellCheckTextCommand.ExecuteAsync(null);

			target.ValidationIsEnabled = true;
			var validationErrors = target.GetErrors(nameof(target.Text)).Cast<string>();

			// Assert

			validationErrors.Single().Should().Be("Same text with the same note in Test Language language already exists. Either use existing text or adjust a note to distinguish the texts");
		}

		[TestMethod]
		public async Task GetErrors_ForTextPropertyWhenNoteIsFilledAndSameTextWithDifferentNoteExists_ReturnsNoError()
		{
			// Arrange

			var mocker = new AutoMocker();

			var existingText = TestExistingLanguageTexts.Last();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			mocker.GetMock<ISpellCheckService>()
				.Setup(x => x.PerformSpellCheck(It.IsAny<LanguageText>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = existingText.Text;
			target.Note = "new note";
			await target.SpellCheckTextCommand.ExecuteAsync(null);

			target.ValidationIsEnabled = true;
			var validationErrors = target.GetErrors(nameof(target.Text)).Cast<string>();

			// Assert

			validationErrors.Should().BeEmpty();
		}

		[DataTestMethod]
		[DataRow(" new note", "Please remove leading whitespaces")]
		[DataRow("\tnew note", "Please remove leading whitespaces")]
		[DataRow("new note ", "Please remove trailing whitespaces")]
		[DataRow("new note\t", "Please remove trailing whitespaces")]
		[DataRow("new  note", "Please remove duplicated whitespaces")]
		[DataRow("new\t\tnote", "Please remove duplicated whitespaces")]
		public async Task GetErrors_ForNotePropertyIfNoteContentIsInvalid_ReturnsCorrectError(string noteValue, string expectedValidationError)
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = "new text";
			target.Note = noteValue;

			target.ValidationIsEnabled = true;
			var validationErrors = target.GetErrors(nameof(target.Note)).Cast<string>();

			// Assert

			validationErrors.Single().Should().Be(expectedValidationError);
		}

		[TestMethod]
		public async Task GetErrors_ForNotePropertyIfNoteContentIsValid_ReturnsNoErrors()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = "new text";
			target.Note = "new note";

			target.ValidationIsEnabled = true;
			var validationErrors = target.GetErrors(nameof(target.Note)).Cast<string>();

			// Assert

			validationErrors.Should().BeEmpty();
		}

		[TestMethod]
		public async Task GetErrors_ForNotePropertyIfNoteIsNotFilled_ReturnsNoErrors()
		{
			// Arrange

			var mocker = new AutoMocker();

			mocker.GetMock<IEditVocabularyService>()
				.Setup(x => x.GetLanguageTexts(TestLanguage, It.IsAny<CancellationToken>())).ReturnsAsync(TestExistingLanguageTexts);

			var target = mocker.CreateInstance<TestEditTextViewModel>();
			await target.Load(TestLanguage, requireSpellCheck: true, createPronunciationRecord: true, CancellationToken.None);

			// Act

			target.Text = "new text";
			target.Note = String.Empty;

			target.ValidationIsEnabled = true;
			var validationErrors = target.GetErrors(nameof(target.Note)).Cast<string>();

			// Assert

			validationErrors.Should().BeEmpty();
		}
	}
}
