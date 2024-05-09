using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LanguageTutor.Models;
using LanguageTutor.ViewModels.ContextMenu;

namespace LanguageTutor.ViewModels.Interfaces
{
	public interface IEditDictionaryViewModel : IPageViewModel
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
