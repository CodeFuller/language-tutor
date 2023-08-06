using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using VocabularyCoach.Extensions;
using VocabularyCoach.Interfaces;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;
using VocabularyCoach.Services.Interfaces;
using VocabularyCoach.ViewModels.Extensions;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.ViewModels
{
	public class CreateOrPickTextViewModel : BasicEditTextViewModel, ICreateOrPickTextViewModel
	{
		public ObservableCollection<LanguageTextViewModel> ExistingTexts { get; } = new();

		private LanguageTextViewModel selectedText;

		public LanguageTextViewModel SelectedText
		{
			get => selectedText;
			set
			{
				SetProperty(ref selectedText, value);

				Text = selectedText?.Text ?? String.Empty;
				Note = selectedText?.Note ?? String.Empty;

				OnPropertyChanged(nameof(AllowTextEdit));
				OnPropertyChanged(nameof(AllowNoteEdit));
			}
		}

		private bool ExistingTextIsSelected => SelectedText != null;

		public override bool AllowTextEdit => !ExistingTextIsSelected;

		public override bool AllowNoteEdit => !ExistingTextIsSelected;

		public CreateOrPickTextViewModel(IVocabularyService vocabularyService, IEditVocabularyService editVocabularyService, ISpellCheckService spellCheckService,
			IPronunciationRecordSynthesizer pronunciationRecordSynthesizer, IPronunciationRecordPlayer pronunciationRecordPlayer, IMessenger messenger)
			: base(vocabularyService, editVocabularyService, spellCheckService, pronunciationRecordSynthesizer, pronunciationRecordPlayer, messenger)
		{
		}

		public async Task Load(Language language, bool requireSpellCheck, bool createPronunciationRecord, CancellationToken cancellationToken)
		{
			await LoadData(language, requireSpellCheck, createPronunciationRecord, cancellationToken);

			SelectedText = null;

			ExistingTexts.Clear();
			var existingTexts = await EditVocabularyService.GetLanguageTexts(language, cancellationToken);
			ExistingTexts.AddRange(existingTexts.OrderBy(x => x.Text).Select(x => new LanguageTextViewModel(x)));
		}

		protected override Task<PronunciationRecord> GetPronunciationRecordForCurrentText(CancellationToken cancellationToken)
		{
			return ExistingTextIsSelected
				? VocabularyService.GetPronunciationRecord(SelectedText.LanguageText.Id, cancellationToken)
				: base.GetPronunciationRecordForCurrentText(cancellationToken);
		}

		protected override async Task<LanguageText> SaveLanguageText(CancellationToken cancellationToken)
		{
			if (ExistingTextIsSelected)
			{
				return SelectedText.LanguageText;
			}

			if (CreatePronunciationRecord && PronunciationRecord == null)
			{
				throw new InvalidOperationException("Pronunciation record is missing");
			}

			var textData = new LanguageTextData
			{
				Language = Language,
				Text = Text,
				Note = Note,
				PronunciationRecord = PronunciationRecord,
			};

			var newText = await EditVocabularyService.AddLanguageText(textData, cancellationToken);
			ExistingTexts.AddToSortedCollection(new LanguageTextViewModel(newText));

			return newText;
		}

		public override void ClearFilledData()
		{
			base.ClearFilledData();

			SelectedText = null;
		}

		protected override string GetValidationErrorForText()
		{
			if (ExistingTextIsSelected)
			{
				return String.Empty;
			}

			return base.GetValidationErrorForText();
		}

		protected override void OnTextPropertyChanged()
		{
			TextWasSpellChecked = ExistingTextIsSelected;

			// When existing text is selected, Text property is set to combo box string, i.e. "Text (Note)".
			// We reset its value to Text only.
			if (ExistingTextIsSelected && Text != SelectedText.Text)
			{
				Text = SelectedText.Text;
			}
		}
	}
}
