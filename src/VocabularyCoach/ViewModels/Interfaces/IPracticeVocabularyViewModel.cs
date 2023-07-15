using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VocabularyCoach.Models;

namespace VocabularyCoach.ViewModels.Interfaces
{
	public interface IPracticeVocabularyViewModel : IPageViewModel
	{
		int NumberOfTextsForCheck { get; }

		int CurrentTextForCheckNumber { get; }

		string ProgressInfo { get; }

		StudiedText CurrentTextForCheck { get; }

		string DisplayedTextInKnownLanguage { get; }

		bool PronunciationRecordExists { get; }

		bool TypedTextIsFocused { get; }

		string TypedText { get; set; }

		bool CheckResultIsShown { get; }

		bool TextIsTypedCorrectly { get; }

		bool TextIsTypedIncorrectly { get; }

		public bool CanSwitchToNextText { get; }

		ICommand CheckTypedTextCommand { get; }

		ICommand SwitchToNextTextCommand { get; }

		ICommand CheckOrSwitchToNextTextCommand { get; }

		ICommand PlayPronunciationRecordCommand { get; }

		ICommand FinishPracticeCommand { get; }

		Task Load(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);
	}
}
