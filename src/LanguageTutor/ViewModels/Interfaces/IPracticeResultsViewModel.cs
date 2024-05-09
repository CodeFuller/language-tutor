using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageTutor.Models;
using LanguageTutor.ViewModels.Data;

namespace LanguageTutor.ViewModels.Interfaces
{
	public interface IPracticeResultsViewModel : IPageViewModel
	{
		string PracticedTextsStatistics { get; }

		string CorrectTextStatistics { get; }

		string IncorrectTextStatistics { get; }

		string SkippedTextStatistics { get; }

		ICommand GoToStartPageCommand { get; }

		Task Load(User user, Language studiedLanguage, Language knownLanguage, PracticeResults results, CancellationToken cancellationToken);
	}
}
