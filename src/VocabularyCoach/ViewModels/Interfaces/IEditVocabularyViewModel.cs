using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VocabularyCoach.Models;

namespace VocabularyCoach.ViewModels.Interfaces
{
	public interface IEditVocabularyViewModel : IPageViewModel
	{
		IEditLanguageTextViewModel EditTextInStudiedLanguageViewModel { get; }

		IEditLanguageTextViewModel EditTextInKnownLanguageViewModel { get; }

		// TODO: Replace with list of translations (text in studied language - text in known language).
		ObservableCollection<LanguageTextViewModel> ExistingTextsInStudiedLanguage { get; }

		public ICommand SaveChangesCommand { get; }

		public ICommand ClearChangesCommand { get; }

		ICommand GoToStartPageCommand { get; }

		Task Load(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);
	}
}
