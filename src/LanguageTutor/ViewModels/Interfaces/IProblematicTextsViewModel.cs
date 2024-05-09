using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageTutor.Models;

namespace LanguageTutor.ViewModels.Interfaces
{
	public interface IProblematicTextsViewModel : IPageViewModel
	{
		ObservableCollection<ProblematicTextViewModel> ProblematicTexts { get; }

		ProblematicTextViewModel SelectedText { get; set; }

		ICommand GoToStartPageCommand { get; }

		Task Load(User user, Language studiedLanguage, Language knownLanguage, CancellationToken cancellationToken);
	}
}
