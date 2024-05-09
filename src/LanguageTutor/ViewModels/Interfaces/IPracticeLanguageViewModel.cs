using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageTutor.Models;

namespace LanguageTutor.ViewModels.Interfaces
{
	public interface IPracticeLanguageViewModel : IPageViewModel
	{
		int NumberOfTextsForCheck { get; }

		int CurrentTextForCheckNumber { get; }

		string ProgressInfo { get; }

		StudiedText CurrentTextForCheck { get; }

		string DisplayedTextInKnownLanguage { get; }

		string HintForOtherSynonyms { get; }

		bool PronunciationRecordExists { get; }

		bool TypedTextIsFocused { get; }

		string TypedText { get; set; }

		bool CheckResultIsShown { get; }

		bool TextIsTypedCorrectly { get; }

		bool TextIsTypedIncorrectly { get; }

		bool CanSwitchToNextText { get; }

		ICommand CheckTypedTextCommand { get; }

		ICommand SwitchToNextTextCommand { get; }

		ICommand CheckOrSwitchToNextTextCommand { get; }

		ICommand PlayPronunciationRecordCommand { get; }

		ICommand FinishPracticeCommand { get; }

		Task Load(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);
	}
}
