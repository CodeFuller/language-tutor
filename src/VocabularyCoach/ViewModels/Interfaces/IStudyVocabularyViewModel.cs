using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VocabularyCoach.Abstractions.Models;

namespace VocabularyCoach.ViewModels.Interfaces
{
	public interface IStudyVocabularyViewModel : IPageViewModel
	{
		StudiedWordOrPhraseWithTranslation CurrentStudiedWordOrPhraseWithTranslation { get; }

		bool IsTypedWordOrPhraseFocused { get; }

		string TypedWordOrPhrase { get; set; }

		bool CheckResultIsShown { get; }

		bool WordIsTypedCorrectly { get; }

		bool WordIsTypedIncorrectly { get; }

		ICommand CheckTypedWordOrPhraseCommand { get; }

		ICommand SwitchToNextWordOrPhraseCommand { get; }

		ICommand FinishStudyCommand { get; }

		Task Load(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);
	}
}
