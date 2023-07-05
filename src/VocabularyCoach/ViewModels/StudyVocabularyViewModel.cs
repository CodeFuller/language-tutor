using System;
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

		private WordOrPhrase currentKnownWord;

		public WordOrPhrase CurrentKnownWord
		{
			get => currentKnownWord;
			set => SetProperty(ref currentKnownWord, value);
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

		public ICommand CheckTypedWordOrPhraseCommand { get; }

		public ICommand FinishStudyCommand { get; }

		public StudyVocabularyViewModel(IVocabularyService vocabularyService, IMessenger messenger)
		{
			this.vocabularyService = vocabularyService ?? throw new ArgumentNullException(nameof(vocabularyService));
			_ = messenger ?? throw new ArgumentNullException(nameof(messenger));

			CheckTypedWordOrPhraseCommand = new AsyncRelayCommand(CheckTypedWordOrPhrase);
			FinishStudyCommand = new RelayCommand(() => messenger.Send(new SwitchToStartPageEventArgs()));
		}

		public async Task Load(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken)
		{
			var studiedWords = await vocabularyService.GetStudiedWords(studiedLanguage, knownLanguage, cancellationToken);

			SetFocusToTypedWordOrPhrase();
			TypedWordOrPhrase = String.Empty;

			// TODO: Remove.
			var firstWord = studiedWords.First();

			CurrentKnownWord = firstWord.WordOrPhraseInKnownLanguage;
		}

		private Task CheckTypedWordOrPhrase(CancellationToken cancellationToken)
		{
			SetFocusToTypedWordOrPhrase();
			TypedWordOrPhrase = String.Empty;

			return Task.CompletedTask;
		}

		private void SetFocusToTypedWordOrPhrase()
		{
			// We set property to false and true, so that PropertyChanged event is triggered.
			IsTypedWordOrPhraseFocused = false;
			IsTypedWordOrPhraseFocused = true;
		}
	}
}
