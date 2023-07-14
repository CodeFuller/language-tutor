using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VocabularyCoach.Models;

namespace VocabularyCoach.ViewModels.Interfaces
{
	public interface IEditVocabularyViewModel : IPageViewModel
	{
		ObservableCollection<LanguageText> TextsInStudiedLanguage { get; }

		Language StudiedLanguage { get; }

		Language KnownLanguage { get; }

		bool TextInStudiedLanguageIsFocused { get; }

		string TextInStudiedLanguage { get; set; }

		bool TextInStudiedLanguageWasChecked { get; }

		bool TextInStudiedLanguageIsFilled { get; }

		bool TextInKnownLanguageIsFocused { get; }

		string TextInKnownLanguage { get; set; }

		string NoteInKnownLanguage { get; set; }

		ICommand CheckTextCommand { get; }

		ICommand PlayPronunciationRecordCommand { get; }

		ICommand SaveChangesCommand { get; }

		ICommand ClearChangesCommand { get; }

		ICommand GoToStartPageCommand { get; }

		Task Load(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);
	}
}
