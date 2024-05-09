using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using LanguageTutor.Interfaces;
using LanguageTutor.Models;
using LanguageTutor.Services.Data;
using LanguageTutor.Services.Interfaces;
using LanguageTutor.ViewModels.Interfaces;

namespace LanguageTutor.ViewModels
{
	public class EditExistingTextViewModel : BasicEditTextViewModel, IEditExistingTextViewModel
	{
		private LanguageText EditedLanguageText { get; set; }

		private PronunciationRecord OriginalPronunciationRecord { get; set; }

		public override bool AllowNoteEdit => true;

		public EditExistingTextViewModel(ITutorService tutorService, IDictionaryService dictionaryService, ISpellCheckService spellCheckService,
			IPronunciationRecordSynthesizer pronunciationRecordSynthesizer, IPronunciationRecordPlayer pronunciationRecordPlayer, IMessenger messenger)
			: base(tutorService, dictionaryService, spellCheckService, pronunciationRecordSynthesizer, pronunciationRecordPlayer, messenger)
		{
		}

		public async Task Load(LanguageText editedLanguageText, bool requireSpellCheck, bool createPronunciationRecord, CancellationToken cancellationToken)
		{
			await LoadData(editedLanguageText.Language, requireSpellCheck, createPronunciationRecord, cancellationToken);

			EditedLanguageText = editedLanguageText;

			Text = editedLanguageText.Text;
			TextWasSpellChecked = true;

			Note = editedLanguageText.Note;

			if (createPronunciationRecord)
			{
				PronunciationRecord = await TutorService.GetPronunciationRecord(editedLanguageText.Id, cancellationToken);
			}

			OriginalPronunciationRecord = PronunciationRecord;
		}

		protected override async Task<LanguageText> SaveLanguageText(CancellationToken cancellationToken)
		{
			if (CreatePronunciationRecord && PronunciationRecord == null)
			{
				throw new InvalidOperationException("Pronunciation record is missing");
			}

			var textData = new LanguageTextData
			{
				Language = Language,
				Text = Text,
				Note = Note,
				PronunciationRecord = ReferenceEquals(PronunciationRecord, OriginalPronunciationRecord) ? null : PronunciationRecord,
			};

			var updatedText = await DictionaryService.UpdateLanguageText(EditedLanguageText, textData, cancellationToken);

			return updatedText;
		}

		protected override void OnTextPropertyChanged()
		{
		}

		protected override bool IsEditedText(ItemId textId)
		{
			return textId == EditedLanguageText.Id;
		}
	}
}
