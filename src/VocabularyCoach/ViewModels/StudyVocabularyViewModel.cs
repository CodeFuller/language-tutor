using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using VocabularyCoach.Abstractions.Interfaces;
using VocabularyCoach.Abstractions.Models;
using VocabularyCoach.Events;
using VocabularyCoach.ViewModels.Interfaces;

namespace VocabularyCoach.ViewModels
{
	public class StudyVocabularyViewModel : ObservableObject, IStudyVocabularyViewModel
	{
		private readonly IVocabularyService vocabularyService;

		private IReadOnlyList<StudiedTextWithTranslation> StudiedText { get; set; }

		private int CurrentTextIndex { get; set; }

		private StudiedTextWithTranslation currentStudiedTextWithTranslation;

		public StudiedTextWithTranslation CurrentStudiedTextWithTranslation
		{
			get => currentStudiedTextWithTranslation;
			private set => SetProperty(ref currentStudiedTextWithTranslation, value);
		}

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
			private set => SetProperty(ref checkResultIsShown, value);
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

		public ICommand CheckTypedTextCommand { get; }

		public ICommand SwitchToNextTextCommand { get; }

		public ICommand CheckOrSwitchToNextTextCommand { get; }

		public ICommand FinishStudyCommand { get; }

		public StudyVocabularyViewModel(IVocabularyService vocabularyService, IMessenger messenger)
		{
			this.vocabularyService = vocabularyService ?? throw new ArgumentNullException(nameof(vocabularyService));
			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));

			CheckTypedTextCommand = new AsyncRelayCommand(CheckTypedText);
			SwitchToNextTextCommand = new RelayCommand(SwitchToNextText);
			CheckOrSwitchToNextTextCommand = new AsyncRelayCommand(CheckOrSwitchToNextText);
			FinishStudyCommand = new RelayCommand(() => messenger.Send(new SwitchToStartPageEventArgs()));
		}

		public async Task Load(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			StudiedText = (await vocabularyService.GetStudiedTexts(studiedLanguage, knownLanguage, cancellationToken)).ToList();
			CurrentTextIndex = -1;

			SwitchToNextText();
		}

		private async Task CheckTypedText(CancellationToken cancellationToken)
		{
			var checkResult = await vocabularyService.CheckTypedText(CurrentStudiedTextWithTranslation.StudiedText, TypedText, cancellationToken);

			CheckResultIsShown = true;
			TextIsTypedCorrectly = checkResult == CheckResultType.Ok;
			TextIsTypedIncorrectly = !TextIsTypedCorrectly;
		}

		private void SwitchToNextText()
		{
			++CurrentTextIndex;
			if (CurrentTextIndex >= StudiedText.Count)
			{
				// TODO: Complete the lesson if all texts are checked.
				CurrentTextIndex = 0;
			}

			CurrentStudiedTextWithTranslation = StudiedText[CurrentTextIndex];

			// We set property to false and true, so that PropertyChanged event is triggered.
			IsTypedTextFocused = false;
			IsTypedTextFocused = true;

			TypedText = String.Empty;

			CheckResultIsShown = false;
			TextIsTypedCorrectly = false;
			TextIsTypedIncorrectly = false;
		}

		private async Task CheckOrSwitchToNextText(CancellationToken cancellationToken)
		{
			if (!CheckResultIsShown)
			{
				await CheckTypedText(cancellationToken);
			}
			else
			{
				SwitchToNextText();
			}
		}
	}
}
