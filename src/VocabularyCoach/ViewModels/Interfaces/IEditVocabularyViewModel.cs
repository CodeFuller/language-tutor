using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VocabularyCoach.Models;

namespace VocabularyCoach.ViewModels.Interfaces
{
	public interface IEditVocabularyViewModel : IPageViewModel
	{
		Language StudiedLanguage { get; }

		Language KnownLanguage { get; }

		ObservableCollection<LanguageTextViewModel> TextsInStudiedLanguage { get; }

		ObservableCollection<LanguageTextViewModel> TextsInKnownLanguage { get; }

		bool TextInStudiedLanguageIsFocused { get; }

		string TextInStudiedLanguage { get; set; }

		bool TextInStudiedLanguageWasChecked { get; }

		bool TextInStudiedLanguageIsFilled { get; }

		bool TextInKnownLanguageIsFocused { get; }

		string TextInKnownLanguage { get; set; }

		LanguageTextViewModel SelectedTextInKnownLanguage { get; set; }

		bool ExistingTextInKnownLanguageIsSelected { get; }

		string NoteInKnownLanguage { get; set; }

		ICommand CheckTextCommand { get; }

		ICommand PlayPronunciationRecordCommand { get; }

		ICommand SaveChangesCommand { get; }

		ICommand ClearChangesCommand { get; }

		ICommand GoToStartPageCommand { get; }

		Task Load(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);
	}
}
