using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using VocabularyCoach.Interfaces;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;
using VocabularyCoach.Services.Interfaces;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.ViewModels
{
	public class EditExistingTextViewModel : BasicEditTextViewModel, IEditExistingTextViewModel
	{
		private readonly IVocabularyService vocabularyService;

		private LanguageText EditedLanguageText { get; set; }

		private PronunciationRecord OriginalPronunciationRecord { get; set; }

		public override bool AllowTextEdit => true;

		public override bool AllowNoteEdit => true;

		public EditExistingTextViewModel(IVocabularyService vocabularyService, IEditVocabularyService editVocabularyService,
			IPronunciationRecordSynthesizer pronunciationRecordSynthesizer, IPronunciationRecordPlayer pronunciationRecordPlayer,
			IWebBrowser webBrowser, IMessenger messenger)
			: base(editVocabularyService, pronunciationRecordSynthesizer, pronunciationRecordPlayer, webBrowser, messenger)
		{
			this.vocabularyService = vocabularyService ?? throw new ArgumentNullException(nameof(vocabularyService));
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
				PronunciationRecord = await vocabularyService.GetPronunciationRecord(editedLanguageText.Id, cancellationToken);
			}

			OriginalPronunciationRecord = PronunciationRecord;
		}

		public override async Task<LanguageText> SaveChanges(CancellationToken cancellationToken)
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

			var updatedText = await EditVocabularyService.UpdateLanguageText(EditedLanguageText, textData, cancellationToken);

			return updatedText;
		}

		protected override void OnTextPropertyChanged()
		{
		}
	}
}
