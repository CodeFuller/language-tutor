using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LanguageTutor.Events;
using LanguageTutor.Interfaces;
using LanguageTutor.Models;
using LanguageTutor.Services.Interfaces;
using LanguageTutor.ViewModels.Data;
using LanguageTutor.ViewModels.Extensions;
using LanguageTutor.ViewModels.Interfaces;
using static LanguageTutor.ViewModels.Extensions.FocusHelpers;

namespace LanguageTutor.ViewModels
{
	public class PracticeLanguageViewModel : ObservableObject, IPracticeLanguageViewModel
	{
		private readonly ITutorService tutorService;

		private readonly IPronunciationRecordPlayer pronunciationRecordPlayer;

		private readonly IMessenger messenger;

		private User User { get; set; }

		private Language StudiedLanguage { get; set; }

		private Language KnownLanguage { get; set; }

		private IReadOnlyList<StudiedText> TextsForCheck { get; set; }

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

		public int NumberOfTextsForCheck => TextsForCheck.Count;

		private int currentTextForCheckNumber;

		public int CurrentTextForCheckNumber
		{
			get => currentTextForCheckNumber;
			private set
			{
				SetProperty(ref currentTextForCheckNumber, value);
				OnPropertyChanged(nameof(ProgressInfo));
			}
		}

		public string ProgressInfo => $"{CurrentTextForCheckNumber} / {NumberOfTextsForCheck}";

		private StudiedText currentTextForCheck;

		public StudiedText CurrentTextForCheck
		{
			get => currentTextForCheck;
			private set
			{
				SetProperty(ref currentTextForCheck, value);
				OnPropertyChanged(nameof(DisplayedTextInKnownLanguage));
				OnPropertyChanged(nameof(HintForOtherSynonyms));
			}
		}

		public string DisplayedTextInKnownLanguage => CurrentTextForCheck.GetTranslationsInKnownLanguage();

		public string HintForOtherSynonyms => CurrentTextForCheck.GetHintForOtherSynonyms();

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

		private bool typedTextIsFocused;

		public bool TypedTextIsFocused
		{
			get => typedTextIsFocused;
			set => SetProperty(ref typedTextIsFocused, value);
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

		public bool CanSwitchToNextText => CheckResultIsShown && CurrentTextIndex + 1 < TextsForCheck.Count;

		private PracticeResults PracticeResults { get; set; }

		public ICommand CheckTypedTextCommand { get; }

		public ICommand SwitchToNextTextCommand { get; }

		public ICommand CheckOrSwitchToNextTextCommand { get; }

		public ICommand PlayPronunciationRecordCommand { get; }

		public ICommand FinishPracticeCommand { get; }

		public PracticeLanguageViewModel(ITutorService tutorService, IPronunciationRecordPlayer pronunciationRecordPlayer, IMessenger messenger)
		{
			this.tutorService = tutorService ?? throw new ArgumentNullException(nameof(tutorService));
			this.pronunciationRecordPlayer = pronunciationRecordPlayer ?? throw new ArgumentNullException(nameof(pronunciationRecordPlayer));
			this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

			CheckTypedTextCommand = new AsyncRelayCommand(CheckTypedText);
			SwitchToNextTextCommand = new AsyncRelayCommand(SwitchToNextText);
			CheckOrSwitchToNextTextCommand = new AsyncRelayCommand(CheckOrSwitchToNextText);
			PlayPronunciationRecordCommand = new AsyncRelayCommand(PlayPronunciationRecord);
			FinishPracticeCommand = new RelayCommand(FinishPractice);
		}

		public async Task Load(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			User = user;
			StudiedLanguage = studiedLanguage;
			KnownLanguage = knownLanguage;

			TextsForCheck = (await tutorService.GetTextsForPractice(User, studiedLanguage, knownLanguage, cancellationToken)).ToList();
			CurrentTextIndex = -1;
			CurrentTextForCheckNumber = 0;

			PracticeResults = new PracticeResults();

			await SwitchToNextText(cancellationToken);
		}

		private async Task CheckTypedText(CancellationToken cancellationToken)
		{
			var checkResult = await tutorService.CheckTypedText(User, CurrentTextForCheck, TypedText, cancellationToken);

			CheckResultIsShown = true;
			TextIsTypedCorrectly = checkResult == CheckResultType.Ok;
			TextIsTypedIncorrectly = !TextIsTypedCorrectly;

			await PlayPronunciationRecord(cancellationToken);

			PracticeResults.AddResult(checkResult);

			++CurrentTextForCheckNumber;
		}

		private async Task SwitchToNextText(CancellationToken cancellationToken)
		{
			++CurrentTextIndex;
			if (CurrentTextIndex >= TextsForCheck.Count)
			{
				FinishPractice();
				return;
			}

			CurrentTextForCheck = TextsForCheck[CurrentTextIndex];

			SetFocus(() => TypedTextIsFocused);

			TypedText = String.Empty;

			CheckResultIsShown = false;
			TextIsTypedCorrectly = false;
			TextIsTypedIncorrectly = false;

			CurrentPronunciationRecord = await tutorService.GetPronunciationRecord(CurrentTextForCheck.TextInStudiedLanguage.Id, cancellationToken);
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

		private void FinishPractice()
		{
			messenger.Send(new SwitchToPracticeResultsPageEventArgs(StudiedLanguage, KnownLanguage, PracticeResults));
		}
	}
}
