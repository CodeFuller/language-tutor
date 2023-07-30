using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using VocabularyCoach.Events;
using VocabularyCoach.Interfaces;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Interfaces;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.ViewModels
{
	public abstract class BasicEditTextViewModel : ObservableObject, IBasicEditTextViewModel
	{
		private readonly IPronunciationRecordSynthesizer pronunciationRecordSynthesizer;

		private readonly IPronunciationRecordPlayer pronunciationRecordPlayer;

		private readonly IWebBrowser webBrowser;

		private readonly IMessenger messenger;

		protected IEditVocabularyService EditVocabularyService { get; }

		protected IReadOnlyCollection<LanguageText> ExistingLanguageTexts { get; set; }

		public Language Language { get; private set; }

		public bool RequireSpellCheck { get; private set; }

		public bool CreatePronunciationRecord { get; private set; }

		private string text;

		public string Text
		{
			get => text;
			set
			{
				SetProperty(ref text, value);
				OnPropertyChanged(nameof(TextIsFilled));
				OnErrorsChanged(nameof(Text));

				TextWasSpellChecked = false;
				PronunciationRecord = null;

				OnTextPropertyChanged();
			}
		}

		private bool textIsFocused;

		public bool TextIsFocused
		{
			get => textIsFocused;
			set => SetProperty(ref textIsFocused, value);
		}

		private bool textWasSpellChecked;

		public bool TextWasSpellChecked
		{
			get => textWasSpellChecked;
			protected set
			{
				SetProperty(ref textWasSpellChecked, value);
				OnErrorsChanged(nameof(Text));
			}
		}

		public bool TextIsFilled => !String.IsNullOrEmpty(Text);

		private string note;

		public string Note
		{
			get => note;
			set
			{
				SetProperty(ref note, value);
				OnErrorsChanged(nameof(Text));
			}
		}

		protected PronunciationRecord PronunciationRecord { get; set; }

		private static IEnumerable<string> ValidationProperties
		{
			get
			{
				yield return nameof(Text);
				yield return nameof(Note);
			}
		}

		private bool validationIsEnabled;

		public abstract bool AllowTextEdit { get; }

		public abstract bool AllowNoteEdit { get; }

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

		protected BasicEditTextViewModel(IEditVocabularyService editVocabularyService, IPronunciationRecordSynthesizer pronunciationRecordSynthesizer,
			IPronunciationRecordPlayer pronunciationRecordPlayer, IWebBrowser webBrowser, IMessenger messenger)
		{
			EditVocabularyService = editVocabularyService ?? throw new ArgumentNullException(nameof(editVocabularyService));
			this.pronunciationRecordSynthesizer = pronunciationRecordSynthesizer ?? throw new ArgumentNullException(nameof(pronunciationRecordSynthesizer));
			this.pronunciationRecordPlayer = pronunciationRecordPlayer ?? throw new ArgumentNullException(nameof(pronunciationRecordPlayer));
			this.webBrowser = webBrowser ?? throw new ArgumentNullException(nameof(webBrowser));
			this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

			SpellCheckTextCommand = new AsyncRelayCommand(SpellCheckText);
			PlayPronunciationRecordCommand = new AsyncRelayCommand(LoadAndPlayPronunciationRecord);
			ProcessEnterKeyCommand = new RelayCommand(() => messenger.Send(new EnterKeyPressedEventArgs()));
		}

		protected async Task LoadData(Language language, bool requireSpellCheck, bool createPronunciationRecord, CancellationToken cancellationToken)
		{
			ClearFilledData();

			Language = language;
			RequireSpellCheck = requireSpellCheck;
			CreatePronunciationRecord = createPronunciationRecord;

			ExistingLanguageTexts = await EditVocabularyService.GetLanguageTexts(language, cancellationToken);
		}

		public abstract Task<LanguageText> SaveChanges(CancellationToken cancellationToken);

		public virtual void ClearFilledData()
		{
			Text = String.Empty;
			Note = String.Empty;

			PronunciationRecord = null;

			ValidationIsEnabled = false;
		}

		protected abstract void OnTextPropertyChanged();

		private async Task SpellCheckText(CancellationToken cancellationToken)
		{
			if (!RequireSpellCheck || String.IsNullOrWhiteSpace(Text))
			{
				return;
			}

			var languageText = new LanguageText
			{
				Language = Language,
				Text = Text,
			};

			var url = await EditVocabularyService.GetUrlForSpellCheck(languageText, cancellationToken);

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
			return pronunciationRecordSynthesizer.SynthesizePronunciationRecord(Language, Text, cancellationToken);
		}

		private void OnErrorsChanged()
		{
			foreach (var propertyName in ValidationProperties)
			{
				OnErrorsChanged(propertyName);
			}
		}

		protected void OnErrorsChanged(string propertyName)
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
				nameof(Note) => GetValidationErrorForNote(),
				_ => String.Empty,
			};
		}

		protected virtual string GetValidationErrorForText()
		{
			if (String.IsNullOrEmpty(Text))
			{
				return "Please type text";
			}

			var validationErrorForTextContent = GetValidationErrorForTextContent(Text);
			if (!String.IsNullOrEmpty(validationErrorForTextContent))
			{
				return validationErrorForTextContent;
			}

			if (RequireSpellCheck && !TextWasSpellChecked)
			{
				return "Please perform text spell check";
			}

			if (String.IsNullOrEmpty(Note))
			{
				if (ExistingLanguageTexts.Any(x => String.Equals(Text, x.Text, LanguageTextComparison.IgnoreCase)))
				{
					return $"Same text in {Language.Name} language already exists. Either use existing text or provide a note to distinguish the texts";
				}
			}
			else
			{
				if (ExistingLanguageTexts.Any(x => String.Equals(Text, x.Text, LanguageTextComparison.IgnoreCase) &&
				                                   String.Equals(Note, x.Note, LanguageTextComparison.IgnoreCase)))
				{
					return $"Same text with the same note in {Language.Name} language already exists. Either use existing text or adjust a note to distinguish the texts";
				}
			}

			return String.Empty;
		}

		private string GetValidationErrorForNote()
		{
			if (!String.IsNullOrEmpty(Note))
			{
				var validationErrorForNoteContent = GetValidationErrorForTextContent(Note);
				if (!String.IsNullOrEmpty(validationErrorForNoteContent))
				{
					return validationErrorForNoteContent;
				}
			}

			return String.Empty;
		}

		protected static string GetValidationErrorForTextContent(string content)
		{
			var leadingWhitespacesRegex = new Regex(@"^\s+");
			if (leadingWhitespacesRegex.IsMatch(content))
			{
				return "Please remove leading whitespaces";
			}

			var trailingWhitespacesRegex = new Regex(@"\s+$");
			if (trailingWhitespacesRegex.IsMatch(content))
			{
				return "Please remove trailing whitespaces";
			}

			var duplicatedWhitespacesRegex = new Regex(@"\s{2,}");
			if (duplicatedWhitespacesRegex.IsMatch(content))
			{
				return "Please remove duplicated whitespaces";
			}

			return String.Empty;
		}
	}
}
