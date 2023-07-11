using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VocabularyCoach.Models;

namespace VocabularyCoach.ViewModels.Interfaces
{
	public interface IStudyVocabularyViewModel : IPageViewModel
	{
		StudiedText CurrentTextForCheck { get; }

		string DisplayedTextInKnownLanguage { get; }

		bool PronunciationRecordExists { get; }

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

		Task Load(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);
	}
}
