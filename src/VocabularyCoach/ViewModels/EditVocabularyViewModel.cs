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
using VocabularyCoach.ViewModels.Interfaces;
using static VocabularyCoach.ViewModels.Extensions.FocusHelpers;

namespace VocabularyCoach.ViewModels
{
	public class EditVocabularyViewModel : ObservableObject, IEditVocabularyViewModel, INotifyDataErrorInfo
	{
		private readonly IEditVocabularyService editVocabularyService;

		private readonly IPronunciationRecordSynthesizer pronunciationRecordSynthesizer;

		private readonly IPronunciationRecordPlayer pronunciationRecordPlayer;

		private readonly IWebBrowser webBrowser;

		private Language studiedLanguage;

		public Language StudiedLanguage
		{
			get => studiedLanguage;
			private set => SetProperty(ref studiedLanguage, value);
		}

		private Language knownLanguage;

		public Language KnownLanguage
		{
			get => knownLanguage;
			private set => SetProperty(ref knownLanguage, value);
		}

		public ObservableCollection<LanguageTextViewModel> TextsInStudiedLanguage { get; } = new();

		public ObservableCollection<LanguageTextViewModel> TextsInKnownLanguage { get; } = new();

		private bool textInStudiedLanguageIsFocused;

		public bool TextInStudiedLanguageIsFocused
		{
			get => textInStudiedLanguageIsFocused;
			set => SetProperty(ref textInStudiedLanguageIsFocused, value);
		}

		private string textInStudiedLanguage;

		public string TextInStudiedLanguage
		{
			get => textInStudiedLanguage;
			set
			{
				TextInStudiedLanguageWasChecked = false;
				PronunciationRecord = null;

				SetProperty(ref textInStudiedLanguage, value);
				OnPropertyChanged(nameof(TextInStudiedLanguageIsFilled));
				OnErrorsChanged(nameof(TextInStudiedLanguage));
			}
		}

		private bool textInStudiedLanguageWasChecked;

		public bool TextInStudiedLanguageWasChecked
		{
			get => textInStudiedLanguageWasChecked;
			private set
			{
				SetProperty(ref textInStudiedLanguageWasChecked, value);
				OnErrorsChanged(nameof(TextInStudiedLanguage));
			}
		}

		public bool TextInStudiedLanguageIsFilled => !String.IsNullOrEmpty(TextInStudiedLanguage);

		private PronunciationRecord PronunciationRecord { get; set; }

		private bool textInKnownLanguageIsFocused;

		public bool TextInKnownLanguageIsFocused
		{
			get => textInKnownLanguageIsFocused;
			set => SetProperty(ref textInKnownLanguageIsFocused, value);
		}

		private string textInKnownLanguage;

		public string TextInKnownLanguage
		{
			get => textInKnownLanguage;
			set
			{
				SetProperty(ref textInKnownLanguage, value);
				OnPropertyChanged(nameof(ExistingTextInKnownLanguageIsSelected));
				OnErrorsChanged(nameof(TextInKnownLanguage));
			}
		}

		private LanguageTextViewModel selectedTextInKnownLanguage;

		public LanguageTextViewModel SelectedTextInKnownLanguage
		{
			get => selectedTextInKnownLanguage;
			set
			{
				SetProperty(ref selectedTextInKnownLanguage, value);

				NoteInKnownLanguage = selectedTextInKnownLanguage?.Note ?? String.Empty;

				OnPropertyChanged(nameof(ExistingTextInKnownLanguageIsSelected));
				OnErrorsChanged(nameof(TextInKnownLanguage));
			}
		}

		public bool ExistingTextInKnownLanguageIsSelected => SelectedTextInKnownLanguage != null && SelectedTextInKnownLanguage.TextWithNote == TextInKnownLanguage;

		private string noteInKnownLanguage;

		public string NoteInKnownLanguage
		{
			get => noteInKnownLanguage;
			set
			{
				SetProperty(ref noteInKnownLanguage, value);
				OnErrorsChanged(nameof(TextInKnownLanguage));
			}
		}

		private static IEnumerable<string> ValidationProperties
		{
			get
			{
				yield return nameof(TextInStudiedLanguage);
				yield return nameof(TextInKnownLanguage);
			}
		}

		private bool validationIsEnabled;

		private bool ValidationIsEnabled
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

		public ICommand CheckTextCommand { get; }

		public ICommand PlayPronunciationRecordCommand { get; }

		public ICommand SaveChangesCommand { get; }

		public ICommand ClearChangesCommand { get; }

		public ICommand GoToStartPageCommand { get; }

		public EditVocabularyViewModel(IEditVocabularyService editVocabularyService, IPronunciationRecordSynthesizer pronunciationRecordSynthesizer,
			IPronunciationRecordPlayer pronunciationRecordPlayer, IWebBrowser webBrowser, IMessenger messenger)
		{
			this.editVocabularyService = editVocabularyService ?? throw new ArgumentNullException(nameof(editVocabularyService));
			this.pronunciationRecordSynthesizer = pronunciationRecordSynthesizer ?? throw new ArgumentNullException(nameof(pronunciationRecordSynthesizer));
			this.pronunciationRecordPlayer = pronunciationRecordPlayer ?? throw new ArgumentNullException(nameof(pronunciationRecordPlayer));
			this.webBrowser = webBrowser ?? throw new ArgumentNullException(nameof(webBrowser));
			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));

			CheckTextCommand = new AsyncRelayCommand(CheckText);
			PlayPronunciationRecordCommand = new AsyncRelayCommand(LoadAndPlayPronunciationRecord);
			SaveChangesCommand = new AsyncRelayCommand(SaveChanges);
			ClearChangesCommand = new RelayCommand(ClearFilledData);
			GoToStartPageCommand = new RelayCommand(() => messenger.Send(new SwitchToStartPageEventArgs()));
		}

		public async Task Load(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			var textsInStudiedLanguage = await editVocabularyService.GetLanguageTexts(studiedLanguage, cancellationToken);
			TextsInStudiedLanguage.Clear();
			TextsInStudiedLanguage.AddRange(textsInStudiedLanguage.OrderBy(x => x.Text).Select(x => new LanguageTextViewModel(x)));

			var textsInKnownLanguage = await editVocabularyService.GetLanguageTexts(knownLanguage, cancellationToken);
			TextsInKnownLanguage.Clear();
			TextsInKnownLanguage.AddRange(textsInKnownLanguage.OrderBy(x => x.Text).Select(x => new LanguageTextViewModel(x)));

			StudiedLanguage = studiedLanguage;
			KnownLanguage = knownLanguage;

			ClearFilledData();
		}

		private async Task CheckText(CancellationToken cancellationToken)
		{
			if (String.IsNullOrWhiteSpace(TextInStudiedLanguage))
			{
				return;
			}

			var languageText = new LanguageText
			{
				Language = StudiedLanguage,
				Text = TextInStudiedLanguage,
			};

			var url = await editVocabularyService.GetUrlForLanguageTextCheck(languageText, cancellationToken);

			webBrowser.OpenPage(url);

			if (PronunciationRecord == null)
			{
				PronunciationRecord = await SynthesizePronunciationRecord(cancellationToken);

				await pronunciationRecordPlayer.PlayPronunciationRecord(PronunciationRecord, cancellationToken);
			}

			TextInStudiedLanguageWasChecked = true;

			SetFocus(() => TextInKnownLanguageIsFocused);
		}

		private async Task LoadAndPlayPronunciationRecord(CancellationToken cancellationToken)
		{
			PronunciationRecord ??= await SynthesizePronunciationRecord(cancellationToken);

			await pronunciationRecordPlayer.PlayPronunciationRecord(PronunciationRecord, cancellationToken);
		}

		private Task<PronunciationRecord> SynthesizePronunciationRecord(CancellationToken cancellationToken)
		{
			return pronunciationRecordSynthesizer.SynthesizePronunciationRecord(StudiedLanguage, TextInStudiedLanguage, cancellationToken);
		}

		private async Task SaveChanges(CancellationToken cancellationToken)
		{
			ValidationIsEnabled = true;

			if (HasErrors)
			{
				return;
			}

			var textDataInStudiedLanguage = new LanguageTextCreationData
			{
				Language = StudiedLanguage,
				Text = TextInStudiedLanguage,
				PronunciationRecord = PronunciationRecord,
			};

			var createdTextInStudiedLanguage = await editVocabularyService.AddLanguageText(textDataInStudiedLanguage, cancellationToken);
			AddTextToSortedList(TextsInStudiedLanguage, createdTextInStudiedLanguage);

			LanguageText newOrExistingTextInKnownLanguage;

			// For possible cases, see comment for method GetValidationErrorForTextInKnownLanguage.
			if (SelectedTextInKnownLanguage == null || (SelectedTextInKnownLanguage != null && SelectedTextInKnownLanguage.Text != TextInKnownLanguage))
			{
				var textDataInKnownLanguage = new LanguageTextCreationData
				{
					Language = KnownLanguage,
					Text = TextInKnownLanguage,
					Note = NoteInKnownLanguage,
				};

				newOrExistingTextInKnownLanguage = await editVocabularyService.AddLanguageText(textDataInKnownLanguage, cancellationToken);
				AddTextToSortedList(TextsInKnownLanguage, newOrExistingTextInKnownLanguage);
			}
			else
			{
				newOrExistingTextInKnownLanguage = SelectedTextInKnownLanguage.LanguageText;
			}

			await editVocabularyService.AddTranslation(createdTextInStudiedLanguage, newOrExistingTextInKnownLanguage, cancellationToken);

			ClearFilledData();
		}

		private static void AddTextToSortedList(ObservableCollection<LanguageTextViewModel> textsCollection, LanguageText languageText)
		{
			for (var i = 0; i < textsCollection.Count + 1; ++i)
			{
				if (i == textsCollection.Count || String.Compare(languageText.Text, textsCollection[i].Text, LanguageTextComparison.IgnoreCase) < 0)
				{
					textsCollection.Insert(i, new LanguageTextViewModel(languageText));
					break;
				}
			}
		}

		private void ClearFilledData()
		{
			TextInStudiedLanguage = String.Empty;
			TextInKnownLanguage = String.Empty;
			SelectedTextInKnownLanguage = null;
			NoteInKnownLanguage = String.Empty;

			PronunciationRecord = null;

			ValidationIsEnabled = false;

			SetFocus(() => TextInStudiedLanguageIsFocused);
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
				nameof(TextInStudiedLanguage) when String.IsNullOrWhiteSpace(TextInStudiedLanguage) => $"Please fill text in {StudiedLanguage.Name} language",
				nameof(TextInStudiedLanguage) when !TextInStudiedLanguageWasChecked => $"Please check text in {StudiedLanguage.Name} language",
				nameof(TextInKnownLanguage) => GetValidationErrorForTextInKnownLanguage(),
				_ => String.Empty,
			};
		}

		private string GetValidationErrorForTextInKnownLanguage()
		{
			/* Possible cases for text in known languages:
			 *   1. SelectedTextInKnownLanguage == null && TextInKnownLanguage == String.Empty: User has neither typed nor picked any text - error.
			 *   2. SelectedTextInKnownLanguage == null && TextInKnownLanguage != String.Empty: User has typed new text.
			 *   3. SelectedTextInKnownLanguage != null && SelectedTextInKnownLanguage.Text != TextInKnownLanguage: User has picked existing text and then edited it. We treat this as adding new text.
			 *   4. SelectedTextInKnownLanguage != null && SelectedTextInKnownLanguage.Text == TextInKnownLanguage: User has picked existing text.
			 *
			 *   If new text is created (##2-3), we check whether the same text already exists. If so, then note becomes mandatory.
			 */

			// Case #1
			if (SelectedTextInKnownLanguage == null && String.IsNullOrEmpty(TextInKnownLanguage))
			{
				return $"Please type new or pick existing text in {KnownLanguage.Name} language";
			}

			// Cases ##2-3
			if ((SelectedTextInKnownLanguage == null && !String.IsNullOrEmpty(TextInKnownLanguage)) ||
			    (SelectedTextInKnownLanguage != null && SelectedTextInKnownLanguage.TextWithNote != TextInKnownLanguage))
			{
				if (String.IsNullOrEmpty(NoteInKnownLanguage))
				{
					if (TextsInKnownLanguage.Any(x => String.Equals(TextInKnownLanguage, x.Text, LanguageTextComparison.IgnoreCase)))
					{
						return $"Same text in {KnownLanguage.Name} language already exists. Either pick existing text or provide a note for new text";
					}
				}
				else
				{
					if (TextsInKnownLanguage.Any(x => String.Equals(TextInKnownLanguage, x.Text, LanguageTextComparison.IgnoreCase) &&
					                                  String.Equals(NoteInKnownLanguage, x.Note, LanguageTextComparison.IgnoreCase)))
					{
						return $"Same text with the same note in {KnownLanguage.Name} language already exists. Please pick existing text";
					}
				}

				return String.Empty;
			}

			// Case #4
			if (SelectedTextInKnownLanguage != null && SelectedTextInKnownLanguage.TextWithNote == TextInKnownLanguage)
			{
				return String.Empty;
			}

			// We should not get here.
			return "Internal error";
		}
	}
}
