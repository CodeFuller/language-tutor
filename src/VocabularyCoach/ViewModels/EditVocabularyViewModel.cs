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
using VocabularyCoach.Exceptions;
using VocabularyCoach.Extensions;
using VocabularyCoach.Interfaces;
using VocabularyCoach.Models;
using VocabularyCoach.Services.Data;
using VocabularyCoach.Services.Interfaces;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.ViewModels
{
	public class EditVocabularyViewModel : ObservableObject, IEditVocabularyViewModel, INotifyDataErrorInfo
	{
		private readonly IEditVocabularyService editVocabularyService;

		private readonly IWebBrowser webBrowser;

		private readonly IPronunciationRecordLoader pronunciationRecordLoader;

		private readonly IPronunciationRecordPlayer pronunciationRecordPlayer;

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

		public ObservableCollection<LanguageText> TextsInStudiedLanguage { get; } = new();

		private string textInStudiedLanguage;

		public string TextInStudiedLanguage
		{
			get => textInStudiedLanguage;
			set
			{
				SetProperty(ref textInStudiedLanguage, value);
				OnPropertyChanged(nameof(TextInStudiedLanguageIsFilled));
				OnErrorsChanged(nameof(TextInStudiedLanguage));
			}
		}

		public bool TextInStudiedLanguageIsFilled => !String.IsNullOrEmpty(TextInStudiedLanguage);

		private string pronunciationRecordSource;

		public string PronunciationRecordSource
		{
			get => pronunciationRecordSource;
			set
			{
				SetProperty(ref pronunciationRecordSource, value);

				PronunciationRecord = null;

				Task.Run(() => LoadAndPlayPronunciationRecord(CancellationToken.None));
			}
		}

		private string PronunciationRecordSourceError { get; set; }

		private PronunciationRecord pronunciationRecord;

		private PronunciationRecord PronunciationRecord
		{
			get => pronunciationRecord;
			set
			{
				pronunciationRecord = value;

				OnPropertyChanged(nameof(PronunciationRecordIsLoaded));
				OnErrorsChanged(nameof(PronunciationRecordSource));
			}
		}

		public bool PronunciationRecordIsLoaded => PronunciationRecord != null;

		private string textInKnownLanguage;

		public string TextInKnownLanguage
		{
			get => textInKnownLanguage;
			set
			{
				SetProperty(ref textInKnownLanguage, value);
				OnErrorsChanged(nameof(TextInKnownLanguage));
			}
		}

		private string noteInKnownLanguage;

		public string NoteInKnownLanguage
		{
			get => noteInKnownLanguage;
			set => SetProperty(ref noteInKnownLanguage, value);
		}

		private static IEnumerable<string> ValidationProperties
		{
			get
			{
				yield return nameof(TextInStudiedLanguage);
				yield return nameof(TextInKnownLanguage);
				yield return nameof(PronunciationRecordSource);
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

		public EditVocabularyViewModel(IEditVocabularyService editVocabularyService, IWebBrowser webBrowser,
			IPronunciationRecordLoader pronunciationRecordLoader, IPronunciationRecordPlayer pronunciationRecordPlayer, IMessenger messenger)
		{
			this.editVocabularyService = editVocabularyService ?? throw new ArgumentNullException(nameof(editVocabularyService));
			this.webBrowser = webBrowser ?? throw new ArgumentNullException(nameof(webBrowser));
			this.pronunciationRecordLoader = pronunciationRecordLoader ?? throw new ArgumentNullException(nameof(pronunciationRecordLoader));
			this.pronunciationRecordPlayer = pronunciationRecordPlayer ?? throw new ArgumentNullException(nameof(pronunciationRecordPlayer));
			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));

			CheckTextCommand = new AsyncRelayCommand(CheckText);
			PlayPronunciationRecordCommand = new AsyncRelayCommand(PlayPronunciationRecord);
			SaveChangesCommand = new AsyncRelayCommand(SaveChanges);
			ClearChangesCommand = new RelayCommand(ClearFilledData);
			GoToStartPageCommand = new RelayCommand(() => messenger.Send(new SwitchToStartPageEventArgs()));
		}

		public async Task Load(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			var textsInStudiedLanguage = await editVocabularyService.GetLanguageTexts(studiedLanguage, cancellationToken);

			TextsInStudiedLanguage.Clear();
			TextsInStudiedLanguage.AddRange(textsInStudiedLanguage.OrderBy(x => x.Text));

			StudiedLanguage = studiedLanguage;
			KnownLanguage = knownLanguage;

			ClearFilledData();
		}

		private async Task CheckText(CancellationToken cancellationToken)
		{
			var languageText = new LanguageText
			{
				Language = StudiedLanguage,
				Text = TextInStudiedLanguage,
			};

			var url = await editVocabularyService.GetUrlForLanguageTextCheck(languageText, cancellationToken);

			webBrowser.OpenPage(url);
		}

		private async Task PlayPronunciationRecord(CancellationToken cancellationToken)
		{
			await pronunciationRecordPlayer.PlayPronunciationRecord(PronunciationRecord, cancellationToken);
		}

		private async Task LoadAndPlayPronunciationRecord(CancellationToken cancellationToken)
		{
			PronunciationRecordSourceError = null;

			if (String.IsNullOrWhiteSpace(PronunciationRecordSource))
			{
				PronunciationRecordSourceError = "Please fill URL for pronunciation record";
				return;
			}

			try
			{
				PronunciationRecord = await pronunciationRecordLoader.LoadPronunciationRecord(PronunciationRecordSource, cancellationToken);
			}
			catch (PronunciationRecordLoadException e)
			{
				PronunciationRecordSourceError = e.Message;
				return;
			}

			await pronunciationRecordPlayer.PlayPronunciationRecord(PronunciationRecord, cancellationToken);
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

			var textDataInKnownLanguage = new LanguageTextCreationData
			{
				Language = KnownLanguage,
				Text = TextInKnownLanguage,
				Note = NoteInKnownLanguage,
			};

			var addedText = await editVocabularyService.AddLanguageTextWithTranslation(textDataInStudiedLanguage, textDataInKnownLanguage, cancellationToken);

			// Adding new text keeping sorting.
			for (var i = 0; i < TextsInStudiedLanguage.Count + 1; ++i)
			{
				if (i == TextsInStudiedLanguage.Count || String.Compare(addedText.Text, TextsInStudiedLanguage[i].Text, StringComparison.Ordinal) < 0)
				{
					TextsInStudiedLanguage.Insert(i, addedText);
					break;
				}
			}

			ClearFilledData();
		}

		private void ClearFilledData()
		{
			TextInStudiedLanguage = String.Empty;
			TextInKnownLanguage = String.Empty;
			NoteInKnownLanguage = String.Empty;
			PronunciationRecordSource = String.Empty;

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
				nameof(TextInStudiedLanguage) when String.IsNullOrWhiteSpace(TextInStudiedLanguage) => $"Please fill text in {StudiedLanguage.Name} language",
				nameof(TextInKnownLanguage) when String.IsNullOrWhiteSpace(TextInKnownLanguage) => $"Please fill text in {KnownLanguage.Name} language",
				nameof(PronunciationRecordSource) when PronunciationRecord == null => String.IsNullOrEmpty(PronunciationRecordSourceError) ? "Error" : PronunciationRecordSourceError,
				_ => String.Empty,
			};
		}
	}
}
