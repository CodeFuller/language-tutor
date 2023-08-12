using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using VocabularyCoach.Models;
using VocabularyCoach.ViewModels.ContextMenu;

namespace VocabularyCoach.ViewModels.Interfaces
{
	public interface IEditVocabularyViewModel : IPageViewModel
	{
		IBasicEditTextViewModel CurrentTextInStudiedLanguageViewModel { get; }

		IBasicEditTextViewModel CurrentTextInKnownLanguageViewModel { get; }

		bool EditTextInStudiedLanguageIsEnabled { get; }

		bool EditTextInKnownLanguageIsEnabled { get; }

		string TranslationFilter { get; set; }

		IReadOnlyCollection<TranslationViewModel> FilteredTranslations { get; }

		TranslationViewModel SelectedTranslation { get; set; }

		IAsyncRelayCommand SaveChangesCommand { get; }

		ICommand ClearChangesCommand { get; }

		ICommand GoToStartPageCommand { get; }

		Task Load(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);

		IEnumerable<ContextMenuItem> GetContextMenuItemsForSelectedTranslation();
	}
}
