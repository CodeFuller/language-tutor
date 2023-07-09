using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VocabularyCoach.Models;

namespace VocabularyCoach.ViewModels.Interfaces
{
	public interface IStartPageViewModel : IPageViewModel
	{
		ObservableCollection<Language> AvailableLanguages { get; }

		Language SelectedStudiedLanguage { get; set; }

		Language SelectedKnownLanguage { get; set; }

		ICommand StudyVocabularyCommand { get; }

		ICommand EditVocabularyCommand { get; }

		Task Load(CancellationToken cancellationToken);
	}
}
