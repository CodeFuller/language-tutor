using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace VocabularyCoach.ViewModels
{
	public class EditVocabularyViewModel : ObservableObject, IEditVocabularyViewModel
	{
		private readonly IEditVocabularyService editVocabularyService;

		private readonly IWebBrowser webBrowser;

		private readonly IContentDownloader contentDownloader;

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

				if (Uri.TryCreate(pronunciationRecordSource, UriKind.Absolute, out var pronunciationRecordUrl))
				{
					Task.Run(() => LoadAndPlayPronunciationRecord(pronunciationRecordUrl, CancellationToken.None));
				}
			}
		}

		private PronunciationRecord pronunciationRecord;

		private PronunciationRecord PronunciationRecord
		{
			get => pronunciationRecord;
			set
			{
				pronunciationRecord = value;

				OnPropertyChanged(nameof(PronunciationRecordIsLoaded));
			}
		}

		public bool PronunciationRecordIsLoaded => PronunciationRecord != null;

		private string textInKnownLanguage;

		public string TextInKnownLanguage
		{
			get => textInKnownLanguage;
			set => SetProperty(ref textInKnownLanguage, value);
		}

		private string noteInKnownLanguage;

		public string NoteInKnownLanguage
		{
			get => noteInKnownLanguage;
			set => SetProperty(ref noteInKnownLanguage, value);
		}

		public ICommand CheckTextCommand { get; }

		public ICommand PlayPronunciationRecordCommand { get; }

		public ICommand SaveChangesCommand { get; }

		public ICommand ClearChangesCommand { get; }

		public ICommand GoToStartPageCommand { get; }

		public EditVocabularyViewModel(IEditVocabularyService editVocabularyService, IWebBrowser webBrowser,
			IContentDownloader contentDownloader, IPronunciationRecordPlayer pronunciationRecordPlayer, IMessenger messenger)
		{
			this.editVocabularyService = editVocabularyService ?? throw new ArgumentNullException(nameof(editVocabularyService));
			this.webBrowser = webBrowser ?? throw new ArgumentNullException(nameof(webBrowser));
			this.contentDownloader = contentDownloader ?? throw new ArgumentNullException(nameof(contentDownloader));
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

		private async Task LoadAndPlayPronunciationRecord(Uri pronunciationRecordUrl, CancellationToken cancellationToken)
		{
			var recordData = await contentDownloader.Download(pronunciationRecordUrl, cancellationToken);

			PronunciationRecord = new PronunciationRecord
			{
				Data = recordData,
				Format = GetPronunciationRecordFormat(pronunciationRecordUrl),
				Source = pronunciationRecordUrl.OriginalString,
			};

			await pronunciationRecordPlayer.PlayPronunciationRecord(PronunciationRecord, cancellationToken);
		}

		private static RecordFormat GetPronunciationRecordFormat(Uri pronunciationRecordUrl)
		{
			var extension = Path.GetExtension(pronunciationRecordUrl.OriginalString);

			var recordFormats = new Dictionary<string, RecordFormat>(StringComparer.OrdinalIgnoreCase)
			{
				{ ".oga", RecordFormat.Oga },
				{ ".ogg", RecordFormat.Oga },
				{ ".mp3", RecordFormat.Mp3 },
			};

			if (recordFormats.TryGetValue(extension, out var recordFormat))
			{
				return recordFormat;
			}

			throw new InvalidOperationException($"Cannot detect record format for URL {pronunciationRecordUrl}");
		}

		private async Task SaveChanges(CancellationToken cancellationToken)
		{
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
		}
	}
}
