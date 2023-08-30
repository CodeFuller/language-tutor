using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
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
		public ObservableCollection<LanguageTextViewModel> ExistingTexts => ExistingLanguageTexts;

		private LanguageTextViewModel selectedText;

		public LanguageTextViewModel SelectedText
		{
			get => selectedText;
			set
			{
				SetProperty(ref selectedText, value);

				// We set only Note property here and do not touch Text property.
				// Two cases are possible:
				// 1. Current setter is called by ComboBox control. In this case ComboBox will call Text setter just after SelectedText setter.
				//    The Text will be adjusted by OnTextPropertyChanged method (Note is removed).
				// 2. Current setter is called by OnTextPropertyChanged method. In this case Text was already set to the correct value (i.e. edited by user) and we should not reset it.
				Note = selectedText?.Note ?? String.Empty;

				OnPropertyChanged(nameof(AllowNoteEdit));
			}
		}

		private bool ExistingTextIsSelected => SelectedText != null;

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
			ExistingLanguageTexts.AddToSortedCollection(new LanguageTextViewModel(newText));

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
			if (ExistingTextIsSelected && Text != SelectedText.Text)
			{
				if (Text == SelectedText.TextWithNote)
				{
					// When existing text is selected, Text property is set to combo box string, i.e. "Text (Note)".
					// We reset its value to Text only.
					Text = SelectedText.Text;
				}
				else
				{
					// Existing text was selected and then edited.
					// We treat this as new text.
					SelectedText = null;
				}
			}

			TextWasSpellChecked = ExistingTextIsSelected;
		}
	}
}
