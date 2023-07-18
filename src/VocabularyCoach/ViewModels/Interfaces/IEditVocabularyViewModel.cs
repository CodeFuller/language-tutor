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

		ObservableCollection<TranslationViewModel> Translations { get; }

		public ICommand SaveChangesCommand { get; }

		public ICommand ClearChangesCommand { get; }

		ICommand GoToStartPageCommand { get; }

		Task Load(Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);
	}
}
