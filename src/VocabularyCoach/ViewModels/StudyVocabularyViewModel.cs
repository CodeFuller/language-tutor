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

		private IReadOnlyList<StudiedWordOrPhraseWithTranslation> StudiedWords { get; set; }

		private int CurrentWordIndex { get; set; }

		private StudiedWordOrPhraseWithTranslation currentStudiedWordOrPhraseWithTranslation;

		public StudiedWordOrPhraseWithTranslation CurrentStudiedWordOrPhraseWithTranslation
		{
			get => currentStudiedWordOrPhraseWithTranslation;
			private set => SetProperty(ref currentStudiedWordOrPhraseWithTranslation, value);
		}

		private bool isTypedWordOrPhraseFocused;

		public bool IsTypedWordOrPhraseFocused
		{
			get => isTypedWordOrPhraseFocused;
			set => SetProperty(ref isTypedWordOrPhraseFocused, value);
		}

		private string typedWordOrPhrase;

		public string TypedWordOrPhrase
		{
			get => typedWordOrPhrase;
			set => SetProperty(ref typedWordOrPhrase, value);
		}

		private bool checkResultIsShown;

		public bool CheckResultIsShown
		{
			get => checkResultIsShown;
			private set => SetProperty(ref checkResultIsShown, value);
		}

		// We use a pair of properties - WordIsTypedCorrectly and WordIsTypedIncorrectly, because they are no actually inverted.
		// When word was not yet checked, both properties are set to false. This state could be expressed as null value of bool? property,
		// however this requires custom visibility value converter which converts null value to collapsed result.
		private bool wordIsTypedCorrectly;

		public bool WordIsTypedCorrectly
		{
			get => wordIsTypedCorrectly;
			private set => SetProperty(ref wordIsTypedCorrectly, value);
		}

		private bool wordIsTypedIncorrectly;

		public bool WordIsTypedIncorrectly
		{
			get => wordIsTypedIncorrectly;
			private set => SetProperty(ref wordIsTypedIncorrectly, value);
		}

		public ICommand CheckTypedWordOrPhraseCommand { get; }

		public ICommand SwitchToNextWordOrPhraseCommand { get; }

		public ICommand FinishStudyCommand { get; }

		public StudyVocabularyViewModel(IVocabularyService vocabularyService, IMessenger messenger)
		{
			this.vocabularyService = vocabularyService ?? throw new ArgumentNullException(nameof(vocabularyService));
			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));

			CheckTypedWordOrPhraseCommand = new AsyncRelayCommand(CheckTypedWordOrPhrase);
			SwitchToNextWordOrPhraseCommand = new RelayCommand(SwitchToNextWordOrPhrase);
			FinishStudyCommand = new RelayCommand(() => messenger.Send(new SwitchToStartPageEventArgs()));
		}

		public async Task Load(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			StudiedWords = (await vocabularyService.GetStudiedWords(studiedLanguage, knownLanguage, cancellationToken)).ToList();
			CurrentWordIndex = -1;

			SwitchToNextWord();
		}

		private async Task CheckTypedWordOrPhrase(CancellationToken cancellationToken)
		{
			var checkResult = await vocabularyService.CheckTypedWordOrPhrase(CurrentStudiedWordOrPhraseWithTranslation.StudiedWordOrPhrase, TypedWordOrPhrase, cancellationToken);

			CheckResultIsShown = true;
			WordIsTypedCorrectly = checkResult == CheckResultType.Ok;
			WordIsTypedIncorrectly = !WordIsTypedCorrectly;
		}

		private void SwitchToNextWordOrPhrase()
		{
			SwitchToNextWord();
		}

		private void SwitchToNextWord()
		{
			++CurrentWordIndex;
			if (CurrentWordIndex >= StudiedWords.Count)
			{
				// TODO: Complete the lesson if all words are checked.
				CurrentWordIndex = 0;
			}

			CurrentStudiedWordOrPhraseWithTranslation = StudiedWords[CurrentWordIndex];

			// We set property to false and true, so that PropertyChanged event is triggered.
			IsTypedWordOrPhraseFocused = false;
			IsTypedWordOrPhraseFocused = true;

			TypedWordOrPhrase = String.Empty;

			CheckResultIsShown = false;
			WordIsTypedCorrectly = false;
			WordIsTypedIncorrectly = false;
		}
	}
}
