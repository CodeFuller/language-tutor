using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using VocabularyCoach.Events;
using VocabularyCoach.Extensions;
using VocabularyCoach.Interfaces;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;
using VocabularyCoach.Services.Interfaces;
using VocabularyCoach.ViewModels.Extensions;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.ViewModels
{
	public class EditLanguageTextViewModel : ObservableObject, IEditLanguageTextViewModel, INotifyDataErrorInfo
	{
		private readonly IEditVocabularyService editVocabularyService;

		private readonly IPronunciationRecordSynthesizer pronunciationRecordSynthesizer;

		private readonly IPronunciationRecordPlayer pronunciationRecordPlayer;

		private readonly IWebBrowser webBrowser;

		private readonly IMessenger messenger;

		public Language Language { get; private set; }

		public bool RequireSpellCheck { get; private set; }

		public bool CreatePronunciationRecord { get; private set; }

		public ObservableCollection<LanguageTextViewModel> ExistingTexts { get; } = new();

		private bool textIsFocused;

		public bool TextIsFocused
		{
			get => textIsFocused;
			set => SetProperty(ref textIsFocused, value);
		}

		private string text;

		// This property has different values:
		//    If user types some text in edit box, then Text will contain typed text.
		//    If user picks existing text, then Text will contain formatted string of list item, including Note.
		//
		// To avoid ambiguity, do not use this property for any business logic. Use TextWithoutNote property instead.
		public string Text
		{
			get => text;
			set
			{
				SetProperty(ref text, value);

				TextWasSpellChecked = ExistingTextIsSelected;
				PronunciationRecord = null;

				OnPropertyChanged(nameof(TextIsFilled));
				OnPropertyChanged(nameof(ExistingTextIsSelected));
				OnErrorsChanged(nameof(Text));
			}
		}

		private string TextWithoutNote => ExistingTextIsSelected ? SelectedText.Text : Text;

		private bool textWasSpellChecked;

		public bool TextWasSpellChecked
		{
			get => textWasSpellChecked;
			private set
			{
				SetProperty(ref textWasSpellChecked, value);
				OnErrorsChanged(nameof(Text));
			}
		}

		public bool TextIsFilled => !String.IsNullOrEmpty(TextWithoutNote);

		private LanguageTextViewModel selectedText;

		public LanguageTextViewModel SelectedText
		{
			get => selectedText;
			set
			{
				SetProperty(ref selectedText, value);

				Note = selectedText?.Note ?? String.Empty;

				OnPropertyChanged(nameof(ExistingTextIsSelected));
				OnErrorsChanged(nameof(Text));
			}
		}

		public bool ExistingTextIsSelected => SelectedText != null && SelectedText.TextWithNote == Text;

		private string note;

		public string Note
		{
			get => note;
			set
			{
				SetProperty(ref note, value);
				OnPropertyChanged(nameof(ExistingTextIsSelected));
				OnErrorsChanged(nameof(Text));
			}
		}

		private PronunciationRecord PronunciationRecord { get; set; }

		private static IEnumerable<string> ValidationProperties
		{
			get
			{
				yield return nameof(Text);
			}
		}

		private bool validationIsEnabled;

		public bool ValidationIsEnabled
		{
			get => validationIsEnabled;
			set
			{
				validationIsEnabled = value;
				OnErrorsChanged();
			}
		}

		public bool HasErrors => ValidationProperties.Select(GetValidationError).Any(x => !String.IsNullOrEmpty(x));

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		public ICommand SpellCheckTextCommand { get; }

		public ICommand PlayPronunciationRecordCommand { get; }

		public ICommand ProcessEnterKeyCommand { get; }

		public EditLanguageTextViewModel(IEditVocabularyService editVocabularyService, IPronunciationRecordSynthesizer pronunciationRecordSynthesizer,
			IPronunciationRecordPlayer pronunciationRecordPlayer, IWebBrowser webBrowser, IMessenger messenger)
		{
			this.editVocabularyService = editVocabularyService ?? throw new ArgumentNullException(nameof(editVocabularyService));
			this.pronunciationRecordSynthesizer = pronunciationRecordSynthesizer ?? throw new ArgumentNullException(nameof(pronunciationRecordSynthesizer));
			this.pronunciationRecordPlayer = pronunciationRecordPlayer ?? throw new ArgumentNullException(nameof(pronunciationRecordPlayer));
			this.webBrowser = webBrowser ?? throw new ArgumentNullException(nameof(webBrowser));
			this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

			SpellCheckTextCommand = new AsyncRelayCommand(SpellCheckText);
			PlayPronunciationRecordCommand = new AsyncRelayCommand(LoadAndPlayPronunciationRecord);
			ProcessEnterKeyCommand = new RelayCommand(() => messenger.Send(new EnterKeyPressedEventArgs()));
		}

		public async Task Load(Language language, bool requireSpellCheck, bool createPronunciationRecord, CancellationToken cancellationToken)
		{
			Language = language;
			RequireSpellCheck = requireSpellCheck;
			CreatePronunciationRecord = createPronunciationRecord;

			var existingTexts = await editVocabularyService.GetLanguageTexts(language, cancellationToken);
			ExistingTexts.Clear();
			ExistingTexts.AddRange(existingTexts.OrderBy(x => x.Text).Select(x => new LanguageTextViewModel(x)));

			ClearFilledData();
		}

		private async Task SpellCheckText(CancellationToken cancellationToken)
		{
			if (!RequireSpellCheck || String.IsNullOrWhiteSpace(TextWithoutNote))
			{
				return;
			}

			var languageText = new LanguageText
			{
				Language = Language,
				Text = TextWithoutNote,
			};

			var url = await editVocabularyService.GetUrlForSpellCheck(languageText, cancellationToken);

			webBrowser.OpenPage(url);

			if (CreatePronunciationRecord && PronunciationRecord == null)
			{
				PronunciationRecord = await SynthesizePronunciationRecord(cancellationToken);

				await pronunciationRecordPlayer.PlayPronunciationRecord(PronunciationRecord, cancellationToken);
			}

			TextWasSpellChecked = true;

			messenger.Send(new EditedTextSpellCheckedEventArgs());
		}

		private async Task LoadAndPlayPronunciationRecord(CancellationToken cancellationToken)
		{
			PronunciationRecord ??= await SynthesizePronunciationRecord(cancellationToken);

			await pronunciationRecordPlayer.PlayPronunciationRecord(PronunciationRecord, cancellationToken);
		}

		private Task<PronunciationRecord> SynthesizePronunciationRecord(CancellationToken cancellationToken)
		{
			return pronunciationRecordSynthesizer.SynthesizePronunciationRecord(Language, TextWithoutNote, cancellationToken);
		}

		public async Task<LanguageText> SaveChanges(CancellationToken cancellationToken)
		{
			// For possible cases, see comment for method GetValidationErrorForText.
			if (ExistingTextIsSelected)
			{
				return SelectedText.LanguageText;
			}

			if (CreatePronunciationRecord && PronunciationRecord == null)
			{
				throw new InvalidOperationException("Pronunciation record is missing");
			}

			var textCreationData = new LanguageTextCreationData
			{
				Language = Language,
				Text = TextWithoutNote,
				Note = Note,
				PronunciationRecord = PronunciationRecord,
			};

			var newText = await editVocabularyService.AddLanguageText(textCreationData, cancellationToken);
			ExistingTexts.AddToSortedCollection(new LanguageTextViewModel(newText));

			return newText;
		}

		public void ClearFilledData()
		{
			Text = String.Empty;
			SelectedText = null;
			Note = String.Empty;

			PronunciationRecord = null;

			ValidationIsEnabled = false;
		}

		private void OnErrorsChanged()
		{
			foreach (var propertyName in ValidationProperties)
			{
				OnErrorsChanged(propertyName);
			}
		}

		private void OnErrorsChanged(string propertyName)
		{
			ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
		}

		public IEnumerable GetErrors(string propertyName)
		{
			var error = GetValidationError(propertyName);

			if (!String.IsNullOrEmpty(error))
			{
				yield return error;
			}
		}

		private string GetValidationError(string propertyName)
		{
			if (!ValidationIsEnabled)
			{
				return String.Empty;
			}

			return propertyName switch
			{
				nameof(Text) => GetValidationErrorForText(),
				_ => String.Empty,
			};
		}

		private string GetValidationErrorForText()
		{
			/* Possible cases for text:
			 *   1. SelectedText == null && Text == String.Empty: User has neither typed nor picked any text - error.
			 *   2. SelectedText == null && Text != String.Empty: User has typed new text.
			 *   3. SelectedText != null && SelectedText.Text != Text: User has picked existing text and then edited it. We treat this as adding new text.
			 *   4. SelectedText != null && SelectedText.Text == Text: User has picked existing text.
			 *
			 *   If new text is created (##2-3), we check whether the same text already exists. If so, then note becomes mandatory.
			 */

			// Case #1
			if (SelectedText == null && String.IsNullOrEmpty(TextWithoutNote))
			{
				return "Please type new or pick existing text";
			}

			if (RequireSpellCheck && !TextWasSpellChecked)
			{
				return "Please perform text spell check";
			}

			// Cases ##2-3
			if ((SelectedText == null && !String.IsNullOrEmpty(TextWithoutNote)) ||
				(SelectedText != null && !ExistingTextIsSelected))
			{
				if (String.IsNullOrEmpty(Note))
				{
					if (ExistingTexts.Any(x => String.Equals(TextWithoutNote, x.Text, LanguageTextComparison.IgnoreCase)))
					{
						return $"Same text in {Language.Name} language already exists. Either pick existing text or provide a note for new text";
					}
				}
				else
				{
					if (ExistingTexts.Any(x => String.Equals(TextWithoutNote, x.Text, LanguageTextComparison.IgnoreCase) &&
					                           String.Equals(Note, x.Note, LanguageTextComparison.IgnoreCase)))
					{
						return $"Same text with the same note in {Language.Name} language already exists. Please pick existing text";
					}
				}

				return String.Empty;
			}

			// Case #4
			if (ExistingTextIsSelected)
			{
				return String.Empty;
			}

			// We should not get here.
			return "Internal error";
		}
	}
}
