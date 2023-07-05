using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VocabularyCoach.Abstractions.Models;

namespace VocabularyCoach.ViewModels.Interfaces
{
	public interface IStudyVocabularyViewModel : IPageViewModel
	{
		WordOrPhrase CurrentKnownWord { get; }

		bool IsTypedWordOrPhraseFocused { get; }

		string TypedWordOrPhrase { get; set; }

		ICommand CheckTypedWordOrPhraseCommand { get; }

		ICommand FinishStudyCommand { get; }

		Task Load(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);
	}
}
