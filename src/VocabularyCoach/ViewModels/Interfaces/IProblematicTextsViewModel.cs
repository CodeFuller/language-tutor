using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VocabularyCoach.Models;

namespace VocabularyCoach.ViewModels.Interfaces
{
	public interface IProblematicTextsViewModel : IPageViewModel
	{
		ObservableCollection<ProblematicTextViewModel> ProblematicTexts { get; }

		ProblematicTextViewModel SelectedText { get; set; }

		ICommand GoToStartPageCommand { get; }

		Task Load(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);
	}
}
