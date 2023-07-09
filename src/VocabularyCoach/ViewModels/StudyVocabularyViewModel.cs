using System;
using System.Collections.Generic;
using System.Linq;
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
using VocabularyCoach.ViewModels.Data;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.ViewModels
{
	public class StudyVocabularyViewModel : ObservableObject, IStudyVocabularyViewModel
	{
		private readonly IVocabularyService vocabularyService;

		private readonly IPronunciationRecordPlayer pronunciationRecordPlayer;

		private readonly IMessenger messenger;

		private User User { get; set; }

		private IReadOnlyList<StudiedTextWithTranslation> StudiedTexts { get; set; }

		private int currentTextIndex;

		private int CurrentTextIndex
		{
			get => currentTextIndex;
			set
			{
				currentTextIndex = value;
				OnPropertyChanged(nameof(CanSwitchToNextText));
			}
		}

		private StudiedTextWithTranslation currentStudiedTextWithTranslation;

		public StudiedTextWithTranslation CurrentStudiedTextWithTranslation
		{
			get => currentStudiedTextWithTranslation;
			private set
			{
				SetProperty(ref currentStudiedTextWithTranslation, value);
				OnPropertyChanged(nameof(DisplayedTextInKnownLanguage));
			}
		}

		public string DisplayedTextInKnownLanguage
		{
			get
			{
				var textInKnownLanguage = CurrentStudiedTextWithTranslation.TextInKnownLanguage;

				return String.IsNullOrEmpty(textInKnownLanguage.Note) ? textInKnownLanguage.Text : $"{textInKnownLanguage.Text} ({textInKnownLanguage.Note})";
			}
		}

		private PronunciationRecord currentPronunciationRecord;

		private PronunciationRecord CurrentPronunciationRecord
		{
			get => currentPronunciationRecord;
			set
			{
				currentPronunciationRecord = value;
				OnPropertyChanged(nameof(PronunciationRecordExists));
			}
		}

		public bool PronunciationRecordExists => CurrentPronunciationRecord != null;

		private bool isTypedTextFocused;

		public bool IsTypedTextFocused
		{
			get => isTypedTextFocused;
			set => SetProperty(ref isTypedTextFocused, value);
		}

		private string typedText;

		public string TypedText
		{
			get => typedText;
			set => SetProperty(ref typedText, value);
		}

		private bool checkResultIsShown;

		public bool CheckResultIsShown
		{
			get => checkResultIsShown;
			private set
			{
				SetProperty(ref checkResultIsShown, value);

				OnPropertyChanged(nameof(CanSwitchToNextText));
			}
		}

		// We use a pair of properties - TextIsTypedCorrectly and TextIsTypedIncorrectly, because they are no actually inverted.
		// When text was not yet checked, both properties are set to false. This state could be expressed as null value of bool? property,
		// however this requires custom visibility value converter which converts null value to collapsed result.
		private bool textIsTypedCorrectly;

		public bool TextIsTypedCorrectly
		{
			get => textIsTypedCorrectly;
			private set => SetProperty(ref textIsTypedCorrectly, value);
		}

		private bool textIsTypedIncorrectly;

		public bool TextIsTypedIncorrectly
		{
			get => textIsTypedIncorrectly;
			private set => SetProperty(ref textIsTypedIncorrectly, value);
		}

		public bool CanSwitchToNextText => CheckResultIsShown && CurrentTextIndex + 1 < StudiedTexts.Count;

		private CheckResults CheckResults { get; set; }

		public ICommand CheckTypedTextCommand { get; }

		public ICommand SwitchToNextTextCommand { get; }

		public ICommand CheckOrSwitchToNextTextCommand { get; }

		public ICommand PlayPronunciationRecordCommand { get; }

		public ICommand FinishStudyCommand { get; }

		public StudyVocabularyViewModel(IVocabularyService vocabularyService, IPronunciationRecordPlayer pronunciationRecordPlayer, IMessenger messenger)
		{
			this.vocabularyService = vocabularyService ?? throw new ArgumentNullException(nameof(vocabularyService));
			this.pronunciationRecordPlayer = pronunciationRecordPlayer ?? throw new ArgumentNullException(nameof(pronunciationRecordPlayer));
			this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

			CheckTypedTextCommand = new AsyncRelayCommand(CheckTypedText);
			SwitchToNextTextCommand = new AsyncRelayCommand(SwitchToNextText);
			CheckOrSwitchToNextTextCommand = new AsyncRelayCommand(CheckOrSwitchToNextText);
			PlayPronunciationRecordCommand = new AsyncRelayCommand(PlayPronunciationRecord);
			FinishStudyCommand = new RelayCommand(FinishStudy);
		}

		public async Task Load(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			User = user;

			StudiedTexts = (await vocabularyService.GetStudiedTexts(User, studiedLanguage, knownLanguage, cancellationToken)).ToList();
			CurrentTextIndex = -1;

			CheckResults = new CheckResults();

			await SwitchToNextText(cancellationToken);
		}

		private async Task CheckTypedText(CancellationToken cancellationToken)
		{
			var checkResult = await vocabularyService.CheckTypedText(User, CurrentStudiedTextWithTranslation.StudiedText, TypedText, cancellationToken);

			CheckResultIsShown = true;
			TextIsTypedCorrectly = checkResult == CheckResultType.Ok;
			TextIsTypedIncorrectly = !TextIsTypedCorrectly;

			await PlayPronunciationRecord(cancellationToken);

			CheckResults.AddResult(checkResult);
		}

		private async Task SwitchToNextText(CancellationToken cancellationToken)
		{
			++CurrentTextIndex;
			if (CurrentTextIndex >= StudiedTexts.Count)
			{
				FinishStudy();
				return;
			}

			CurrentStudiedTextWithTranslation = StudiedTexts[CurrentTextIndex];

			// We set property to false and true, so that PropertyChanged event is triggered.
			IsTypedTextFocused = false;
			IsTypedTextFocused = true;

			TypedText = String.Empty;

			CheckResultIsShown = false;
			TextIsTypedCorrectly = false;
			TextIsTypedIncorrectly = false;

			CurrentPronunciationRecord = await vocabularyService.GetPronunciationRecord(CurrentStudiedTextWithTranslation.StudiedText.LanguageText.Id, cancellationToken);
		}

		private async Task CheckOrSwitchToNextText(CancellationToken cancellationToken)
		{
			if (!CheckResultIsShown)
			{
				await CheckTypedText(cancellationToken);
			}
			else
			{
				await SwitchToNextText(cancellationToken);
			}
		}

		private async Task PlayPronunciationRecord(CancellationToken cancellationToken)
		{
			if (CurrentPronunciationRecord != null)
			{
				await pronunciationRecordPlayer.PlayPronunciationRecord(CurrentPronunciationRecord, cancellationToken);
			}
		}

		private void FinishStudy()
		{
			messenger.Send(new SwitchToCheckResultsPageEventArgs(CheckResults));
		}
	}
}
