using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VocabularyCoach.Abstractions.Models;

namespace VocabularyCoach.ViewModels.Interfaces
{
	public interface IStudyVocabularyViewModel : IPageViewModel
	{
		StudiedTextWithTranslation CurrentStudiedTextWithTranslation { get; }

		bool IsTypedTextFocused { get; }

		string TypedText { get; set; }

		bool CheckResultIsShown { get; }

		bool TextIsTypedCorrectly { get; }

		bool TextIsTypedIncorrectly { get; }

		public bool CanSwitchToNextText { get; }

		ICommand CheckTypedTextCommand { get; }

		ICommand SwitchToNextTextCommand { get; }

		ICommand CheckOrSwitchToNextTextCommand { get; }

		ICommand PlayPronunciationRecordCommand { get; }

		ICommand FinishStudyCommand { get; }

		Task Load(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);
	}
}
