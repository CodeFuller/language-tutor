using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VocabularyCoach.Models;
using VocabularyCoach.ViewModels.ContextMenu;

namespace VocabularyCoach.ViewModels.Interfaces
{
	public interface IEditVocabularyViewModel : IPageViewModel
	{
		IEditLanguageTextViewModel EditTextInStudiedLanguageViewModel { get; }

		IEditLanguageTextViewModel EditTextInKnownLanguageViewModel { get; }

		bool EditTextInStudiedLanguageIsEnabled { get; }

		bool EditTextInKnownLanguageIsEnabled { get; }

		ObservableCollection<TranslationViewModel> Translations { get; }

		TranslationViewModel SelectedTranslation { get; set; }

		public ICommand SaveChangesCommand { get; }

		public ICommand ClearChangesCommand { get; }

		ICommand GoToStartPageCommand { get; }

		IEnumerable<ContextMenuItem> ContextMenuItems { get; }

		Task Load(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);
	}
}
